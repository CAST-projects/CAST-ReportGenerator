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
using Cast.Util.Security;
using CastReporting.HL.Mediation.Interfaces;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace CastReporting.Mediation
{



    /// <summary>
    /// WebClient Class for Cast Reporting
    /// </summary>
    public class HighlightProxy : WebClient, IHighlightProxy
    {
        private readonly bool _restApiKey;

        /// <summary>
        /// Default Constructor
        /// <param name="cookies">The cookies. If set to <c>null</c> a container will be created.</param>
        /// <param name="autoRedirect">if set to <c>true</c> the client should handle the redirect automatically. Default value is <c>true</c></param>
        /// </summary>
        public HighlightProxy(string login, string password, bool apiKey, bool validateCertificate)
        {
            // to debug on https with self signed certificat, disable the certificate validation in the settings
            // add line <ServerCertificateValidation>disable</ServerCertificateValidation> in the reporting parameters
            if (!validateCertificate)
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            }

            _restApiKey = apiKey;

            if (apiKey)
            {
                Headers.Add(HttpRequestHeader.Authorization, $"Bearer {password}");
            }
            else
            {
                var credentials = AuthHelper.CreateBasicAuthenticationCredentials(login, password);
                Headers.Add(HttpRequestHeader.Authorization, credentials);
            }
        }

        private string DownloadContent(string pUrl, string mimeType)
        {
            string result;
            try
            {
                Headers.Remove(HttpRequestHeader.Accept);
                Headers.Add(HttpRequestHeader.Accept, mimeType);
                var culture = Thread.CurrentThread.CurrentCulture;
                Headers.Remove(HttpRequestHeader.AcceptLanguage);
                Headers.Add(HttpRequestHeader.AcceptLanguage, culture.Name.Equals("zh-Hans") ? "zh" : "en");

                // For RestAPI audit trail 
                Headers.Remove("X-Client");
                Headers.Add("X-Client", "CAST-ReportGenerator");

                Encoding = Encoding.UTF8;

                var requestWatch = new Stopwatch();
                requestWatch.Start();
                result = DownloadString(pUrl);
                requestWatch.Stop();

                LogHelper.LogDebug($"Request URL '{pUrl}' - Time elapsed : {requestWatch.Elapsed}");
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is WebException || ex is NotSupportedException || ex is InvalidOperationException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogError($"Request URL '{pUrl}' - Error execution : {ex.Message}");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Download String
        /// </summary>
        /// <param name="pUrl"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        string IHighlightProxy.DownloadString(string pUrl)
        {
            return DownloadContent(pUrl, "application/json");
        }
    }
}
