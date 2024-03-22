/*
 *   Copyright (c) 2024 CAST
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
using System.Xml.Serialization;

namespace CastReporting.HL.Domain;

/// <summary>
/// Represents a webservice connection.
/// </summary>    
[Serializable]
public class HLWSConnection
{

    public HLWSConnection()
    {
    }

    /// <summary>
    /// COnstructor
    /// </summary>
    /// <param name="url"></param>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// /// <param name="name"></param>
    public HLWSConnection(string url, string login, string password, string companyId, string name)
    {
        Url = ValidateUrl(url);
        Login = login;
        Password = password;
        CompanyId = companyId;
        Name = name;
        ApiKey = false;
        ServerCertificateValidation = true;
    }

    public string Name
    {
        get;
        set;
    } = string.Empty;

    public string CompanyId
    {
        get;
        set;
    } = string.Empty;

    private string _url = string.Empty;

    public string Url
    {
        set
        {
            _url = ValidateUrl(value);
            Uri = !string.IsNullOrEmpty(_url) ? new Uri(_url) : null;
        }
        get => _url;
    }

    private static string ValidateUrl(string url)
    {
        string validatedUrl = url?.Trim() ?? string.Empty;
        if (validatedUrl.EndsWith('/'))
        {
            validatedUrl = validatedUrl[..^1];
        }
        return validatedUrl.EndsWith("/WS2") ? validatedUrl : (validatedUrl + "/WS2");
    }

    [XmlIgnore]
    public byte[]? CryptedLogin
    {
        get;
        set;
    }

    /// <summary>
    /// Get/Set the connection name
    /// </summary>    
    private string? _login;

    public string Login
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_login) && CryptedLogin != null)
            {
                // Decrypt the bytes to a string.
                _login = CryptoHelper.DecryptStringFromBytes_Aes(CryptedLogin);
            }
            return _login ?? string.Empty;
        }
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                CryptedLogin = CryptoHelper.EncryptStringToBytes_Aes(value);
                _login = value;
            }
        }
    }

    [XmlIgnore]
    public byte[]? CryptedPassword
    {
        get;
        set;
    }

    private string? _password;

    [XmlIgnore]
    public string Password
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_password) && CryptedPassword != null)
            {
                // Decrypt the bytes to a string.
                _password = CryptoHelper.DecryptStringFromBytes_Aes(CryptedPassword);
            }
            return _password ?? string.Empty;
        }
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                CryptedPassword = CryptoHelper.EncryptStringToBytes_Aes(value);
                _password = value;
            }
        }
    }

    public bool IsActive { get; set; } = false;

    [XmlIgnore]
    public bool ApiKey { get; set; } = false;

    [XmlIgnore]
    public bool ServerCertificateValidation { get; set; } = false;

    [XmlIgnore]
    public Uri? Uri { get; private set; }

    public override string ToString()
    {
        return Uri?.ToString() ?? string.Empty;
    }

    public override bool Equals(object? obj)
    {
        var connection = obj as HLWSConnection;
        return (connection != null) && (Uri?.Equals(connection.Uri) ?? (connection.Uri == null)) && (Login?.Equals(connection.Login) ?? (connection.Login == null)) && (CompanyId == connection.CompanyId);
    }


    public override int GetHashCode()
    {
        return Uri?.GetHashCode() ?? 0;
    }
}

