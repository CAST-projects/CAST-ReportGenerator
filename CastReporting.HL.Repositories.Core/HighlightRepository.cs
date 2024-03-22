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
using CastReporting.HL.Domain;
using CastReporting.HL.Mediation.Interfaces;
using CastReporting.HL.Repositories.Interfaces;
using CastReporting.Mediation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming


namespace CastReporting.HL.Repositories
{
    /// <summary>
    /// Cast reporting Context Class
    /// </summary>
    public class HighlightRepository : IHighlightRepository
    {
        protected readonly IHighlightProxy _Client;

        public readonly string CurrentConnection;

        protected readonly string _CompanyId;

        protected readonly bool CurrentApiKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="client"></param>
        public HighlightRepository(HLWSConnection connection, IHighlightProxy? client)
        {
            _Client = client ?? new HighlightProxy(connection.Login, connection.Password, connection.ApiKey, connection.ServerCertificateValidation);
            _CompanyId = connection.CompanyId;
            CurrentConnection = connection.Url;
            if (!CurrentConnection.EndsWith('/'))
            {
                CurrentConnection += "/";
            }
            CurrentApiKey = connection.ApiKey;
        }

        public IHighlightProxy GetClient()
        {
            return _Client;
        }

        public void Dispose()
        {
            _Client?.Dispose();
            GC.SuppressFinalize(this);
        }

        string IHighlightRepository.GetServerVersion()
        {
            return "WS2";
        }

        private string GetResource(string url) => _Client.DownloadString(CurrentConnection + url);

        /// <summary>
        /// Is Service Valid
        /// </summary>
        /// <returns>True if OK</returns>
        bool IHighlightRepository.IsServiceValid()
        {
            try
            {
                var jsonString = GetResource("benchmark");
                JsonConvert.DeserializeObject(jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        Company? IHighlightRepository.GetCompany()
        {
            var json = GetResource($"companies/{_CompanyId}");
            return JsonConvert.DeserializeObject<Company>(json);
        }

        HLDomain? IHighlightRepository.GetDomain(string domainId)
        {
            var json = GetResource($"domains/{domainId}");
            return JsonConvert.DeserializeObject<HLDomain>(json);
        }

        IList<AppId> IHighlightRepository.GetDomainAppIds(string? domainId)
        {
            if (string.IsNullOrEmpty(domainId))
            {
                domainId = _CompanyId;
            }
            var json = GetResource($"domains/{domainId}/applications");
            return JsonConvert.DeserializeObject<IList<AppId>>(json) ?? [];
        }
    }
}
