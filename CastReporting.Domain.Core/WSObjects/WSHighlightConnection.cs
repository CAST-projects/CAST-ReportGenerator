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
using Cast.Util.Security;
using System;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace CastReporting.Domain
{
    /// <summary>
    /// Represents a webservice connection.
    /// </summary>    
    [Serializable]
    public class WSHighlightConnection
    {

        /// <summary>
        /// 
        /// </summary>
        public WSHighlightConnection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// /// <param name="name"></param>
        public WSHighlightConnection(string url, string login, string password, string name)
        {
            Url = ValidateUrl(url);
            Login = login;
            Password = password;
            Name = name;
            ApiKey = false;
            ServerCertificateValidation = true;
        }

        #region PROPERTIES

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private string _url;
        public string Url
        {
            set
            {
                _url = ValidateUrl(value);
                Uri = !string.IsNullOrEmpty(_url) ? new Uri(_url) : null;
            }
            get => _url;
        }

        private string ValidateUrl(string url)
        {
            string validatedUrl = string.IsNullOrEmpty(url) ? string.Empty : url.Trim();
            return validatedUrl.EndsWith("/WS2") || validatedUrl.EndsWith("/WS2/") ?
                validatedUrl
                : validatedUrl.EndsWith("/") ?
                validatedUrl + "WS2"
                : validatedUrl + "/WS2";
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public byte[] CryptedLogin
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the connection name
        /// </summary>    
        private string _login;
        [XmlIgnore]
        public string Login
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_login) && CryptedLogin != null)
                {
                    using (Aes myAes = Aes.Create())
                    {
                        // Decrypt the bytes to a string.
                        _login = CryptoHelper.DecryptStringFromBytes_Aes(CryptedLogin);
                    }
                }

                return _login;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    using (Aes myAes = Aes.Create())
                    {
                        CryptedLogin = CryptoHelper.EncryptStringToBytes_Aes(value);

                    }
                }
            }
        }
        /// <summary>
        /// Get/Set the connection name
        /// </summary>       


        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public byte[] CryptedPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the connection name
        /// </summary>            
        private string _password;
        [XmlIgnore]
        public string Password
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_password) && CryptedPassword != null)
                {
                    using (Aes myAes = Aes.Create())
                    {
                        // Decrypt the bytes to a string.
                        _password = CryptoHelper.DecryptStringFromBytes_Aes(CryptedPassword);
                    }
                }

                return _password;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    using (Aes myAes = Aes.Create())
                    {
                        CryptedPassword = CryptoHelper.EncryptStringToBytes_Aes(value);

                    }
                }
            }
        }

        /// <summary>
        /// Get/Set the connection name
        /// </summary>       
        public bool IsActive { get; set; }

        [XmlIgnore]
        public bool ApiKey { get; set; }

        [XmlIgnore]
        public bool ServerCertificateValidation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri Uri { get; private set; }


        #endregion PROPERTIES

        #region METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Uri.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var connection = obj as WSHighlightConnection;
            return connection != null && Uri.Equals(connection.Uri);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Uri.GetHashCode();
        }
        #endregion METHODS
    }
}
