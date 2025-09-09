/*
 *   Copyright (c) 2019 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using Cast.Util.Log;
using CastReporting.Mediation.Core;
using CastReporting.Mediation.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace CastReporting.Mediation
{



    /// <summary>
    /// WebClient Class for Cast Reporting
    /// </summary>
    public class CastProxy : ICastProxy
    {
        #region ATTRIBUTES

        /// <summary>
        /// 
        /// </summary>
        /// 
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;

        private RequestComplexity _currentComplexity = RequestComplexity.Standard;

        private HttpRequestMessage _request;

        private readonly bool _restApiKey;

        #endregion ATTRIBUTES

        #region PROPERTIES

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout => GetRequestTimeOut(_currentComplexity);

        /// <summary>
        /// HEAD method option
        /// </summary>
        public bool HeadOnly { get; set; }

        #endregion PROPERTIES

        #region CONSTRUCTORS

        /// <summary>
        /// Default Constructor
        /// <param name="cookies">The cookies. If set to <c>null</c> a container will be created.</param>
        /// <param name="autoRedirect">if set to <c>true</c> the client should handle the redirect automatically. Default value is <c>true</c></param>
        /// </summary>
        public CastProxy(string login, string password, bool apiKey, bool validateCertificate, CookieContainer cookies = null, bool autoRedirect = true)
        {
            // to debug on https with self signed certificat, disable the certificate validation in the settings
            // add line <ServerCertificateValidation>disable</ServerCertificateValidation> in the reporting parameters
            _httpClientHandler = new HttpClientHandler();
            _httpClientHandler.CookieContainer = cookies ?? new CookieContainer();
            _httpClientHandler.AllowAutoRedirect = autoRedirect;
            _httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            if (!validateCertificate)
            {
                _httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            }
            _httpClient = new HttpClient(_httpClientHandler);

            _restApiKey = apiKey;
            if (apiKey)
            {
                // With dashboards v3, using cookie JSESSIONID does not work anymore
                RemoveAuthenticationHeaders();
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", password);
                _httpClient.DefaultRequestHeaders.Add("X-API-USER", login);
            }
            else
            {
                if (cookies?.Count > 0)
                {
                    RemoveAuthenticationHeaders();
                    return;
                }
                string credentials = CreateBasicAuthenticationCredentials(login, password);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials.Substring(6));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically redirect when a 301 or 302 is returned by the request.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the client should handle the redirect automatically; otherwise, <c>false</c>.
        /// </value>
        public bool AutoRedirect { get; set; }

        /// <summary>
        /// Gets or sets the cookie container. This contains all the cookies for all the requests.
        /// </summary>
        /// <value>
        /// The cookie container.
        /// </value>
        public void SetCookieContainer(CookieContainer cookie) {
            this._httpClientHandler.CookieContainer = cookie;
        }

        public CookieContainer GetCookieContainer()
        {
            return this._httpClientHandler.CookieContainer;
        }

        public void RemoveAuthenticationHeaders()
        {
            if (_restApiKey)
            {
                _httpClient.DefaultRequestHeaders.Remove("X-API-KEY");
                _httpClient.DefaultRequestHeaders.Remove("X-API-USER");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// Gets the cookies header (Set-Cookie) of the last request.
        /// </summary>
        /// <value>
        /// The cookies or <c>null</c>.
        /// </value>
        public string Cookies => GetHeaderValue("Set-Cookie");

        /// <summary>
        /// Gets the location header for the last request.
        /// </summary>
        /// <value>
        /// The location or <c>null</c>.
        /// </value>
        public string Location => GetHeaderValue("Location");

        /// <summary>
        /// Gets the status code. When no request is present, <see cref="HttpStatusCode.Gone"/> will be returned.
        /// </summary>
        /// <value>
        /// The status code or <see cref="HttpStatusCode.Gone"/>.
        /// </value>
        public HttpStatusCode StatusCode
        {
            get
            {
                var result = HttpStatusCode.Gone;

                if (_request == null) return result;
                var response = _httpClient.SendAsync(_request).GetAwaiter().GetResult();

                if (response != null)
                {
                    result = response.StatusCode;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the setup that is called before the request is done.
        /// </summary>
        /// <value>
        /// The setup.
        /// </value>
        public Action<HttpRequestMessage> Setup { get; set; }

        /// <summary>
        /// Gets the header value.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>The value.</returns>
        public string GetHeaderValue(string headerName)
        {
            if (_request == null) return null;
            var response = _httpClient.SendAsync(_request).GetAwaiter().GetResult();
            if (response.Headers.TryGetValues(headerName, out var values))
            {
                return values.FirstOrDefault();
            }
            return null;
        }


        #endregion CONSTRUCTORS

        private string DownloadContent(string pUrl, string mimeType, RequestComplexity pComplexity)
        {

            string result;
            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, pUrl))
                {
                    request.Headers.Accept.Clear();
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mimeType));

                    var culture = Thread.CurrentThread.CurrentCulture;
                    request.Headers.AcceptLanguage.Clear();
                    request.Headers.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(culture.Name.Equals("zh-Hans") ? "zh" : "en"));

                    // for audit trail
                    request.Headers.Remove("X-Client");
                    request.Headers.Add("X-Client", "CAST-ReportGenerator");

                    RequestComplexity previousComplexity = _currentComplexity;
                    _currentComplexity = pComplexity;

                    var requestWatch = new Stopwatch();
                    requestWatch.Start();
                    var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    requestWatch.Stop();
                    _request = request;
                    _currentComplexity = previousComplexity;

                    LogHelper.LogDebugFormat
                            ("Request URL '{0}' - Time elapsed : {1} "
                            , pUrl
                            , requestWatch.Elapsed.ToString()
                            );

                }
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is HttpRequestException || ex is NotSupportedException || ex is InvalidOperationException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogErrorFormat
                       ("Request URL '{0}' - Error execution :  {1}"
                       , pUrl
                       , ex.Message
                       );

                throw;
            }

            return result;
        }
        #region METHODS

        /// <summary>
        /// Download String
        /// </summary>
        /// <param name="pUrl"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        public string DownloadString(string pUrl, RequestComplexity pComplexity)
        {
            return DownloadContent(pUrl, "application/json", pComplexity);
        }

        public string DownloadText(string pUrl, RequestComplexity pComplexity)
        {
            try
            {
                return DownloadContent(pUrl, "text/plain", pComplexity);
            }
            catch (HttpRequestException webEx)
            {
                LogHelper.LogInfo(webEx.Message);
                return null;
            }
        }


        /// <summary>
        /// Download String by URI
        /// </summary>
        /// <param name="pUri"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        public string DownloadString(Uri pUri, RequestComplexity pComplexity)
        {
            return DownloadString(pUri.ToString(), pComplexity);
        }

        /// <summary>
        /// Download Csv String
        /// </summary>
        /// <param name="pUrl"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        public string DownloadCsvString(string pUrl, RequestComplexity pComplexity)
        {
            try
            {
                return DownloadContent(pUrl, "text/csv", pComplexity);
            }
            catch (HttpRequestException webEx)
            {
                // AIP < 8 sends CSV data as application/vnd.ms-excel
                LogHelper.LogInfo(webEx.Message);
                return DownloadContent(pUrl, "application/vnd.ms-excel", pComplexity);
            }
        }

        /// <summary>
        /// Download String by URI
        /// </summary>
        /// <param name="pUri"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        public string DownloadCsvString(Uri pUri, RequestComplexity pComplexity)
        {
            return DownloadCsvString(pUri.ToString(), pComplexity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        private static int GetRequestTimeOut(RequestComplexity pComplexity)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (pComplexity)
            {
                case RequestComplexity.Long:
                    {
                        return Settings.Default.TimoutLong;
                    }
                case RequestComplexity.Soft:
                    {
                        return Settings.Default.TimeoutQuick;
                    }
                //case RequestComplexity.Standard:
                default:
                    {
                        return Settings.Default.TimeoutStandard;
                    }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string CreateBasicAuthenticationCredentials(string userName, string password)
        {
            string base64UsernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));

            var returnValue = $"Basic {base64UsernamePassword}";

            return returnValue;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClientHandler?.Dispose();
        }

        #endregion METHODS
    }
}
