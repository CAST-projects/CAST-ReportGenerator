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
using CastReporting.Domain;
using CastReporting.Domain.Highlight;
using CastReporting.Domain.Imaging;
using CastReporting.Mediation;
using CastReporting.Mediation.Interfaces;
using CastReporting.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
// ReSharper disable InconsistentNaming


namespace CastReporting.Repositories
{
    /// <summary>
    /// Cast reporting Context Class
    /// </summary>
    public class HighlightRepository : IHighlightRepository
    {
        #region CONSTANTS

        // Sometimes modules, technologies, snapshots, categories are null and the rest api 8.2 does not support it anymore for security reasons
        private const string _query_result_quality_indicators = "{0}/results?quality-indicators=({1})&select=(evolutionSummary,violationRatio,omgTechnicalDebt)";
        private const string _query_result_sizing_measures = "{0}/results?sizing-measures=({1})";
        private const string _query_result_background_facts = "{0}/results?background-facts=({1})";
        private const string _query_configuration = "{0}/configuration/snapshots/{1}";
        private const string _query_action_plan = "{0}/action-plan/summary";
        private const string _query_action_plan2 = "{0}/actionPlan/summary";
        private const string _query_result_rules_violations = "{0}/results?quality-indicators={1}{2}&select=violationRatio&modules=($all)";
        private const string _query_result_quality_distribution_complexity = "{0}/results?quality-indicators=({1})&select=(categories)&modules=$all&technologies=$all";
        private const string _query_rule_patterns = "{0}/rule-patterns/{1}";
        private const string _query_rules_details = "{0}/quality-indicators/{1}/snapshots/{2}/base-quality-indicators";
        private const string _query_grade_contributors = "{0}/quality-indicators/{1}/snapshots/{2}";
        private const string _query_transactions = "{0}/transactions/{1}?nbRows={2}";
        private const string _query_ifpug_functions = "{0}/ifpug-functions";
        private const string _query_ifpug_functions_evolutions = "{0}/ifpug-functions-evolution";
        private const string _query_omg_functions_evolutions = "{0}/omg-functions-functional-evolution";
        private const string _query_metric_top_artefact = "{0}/violations?rule-pattern={1}";
        private const string _query_components = "{0}/components/{1}?nbRows={2}";
        private const string _query_components_with_properties = "{0}/components/{1}?properties=({2},{3})&order=({4})&startRow=1&nbRows={5}";
        private const string _query_components_by_modules = "{0}/modules/{1}/snapshots/{2}/components/{3}?nbRows={4}";
        private const string _query_common_categories = "{0}/AAD/common-categories";
        private const string _query_tags = "{0}/AAD/tags";
        private const string _query_violations_list_by_rule_bcid = "{0}/violations?rule-pattern={1}&business-criterion={2}&startRow=1&nbRows={3}&technologies={4}";
        private const string _query_action_plan_issues = "{0}/action-plan/issues?nbRows={1}";
        private const string _query_result_quality_standards_rules = "{0}/results?quality-indicators=(c:{1})";
        private const string _query_findings = "{0}/components/{1}/snapshots/{2}/findings/{3}";
        private const string _query_component_source_code = "{0}/components/{1}/snapshots/{2}/source-codes";
        private const string _query_file_content = "{0}/local-sites/{1}/file-contents/{2}?start-line={3}&end-line={4}";
        private const string _query_component_type = "{0}/components/{1}/snapshots/{2}";
        private const string _query_quality_standards_evolution = "{0}/results?quality-standards=(c:{1})&select=(evolutionSummary)";
        private const string _query_quality_standards_information = "{0}/quality-standards";
        private const string _query_quality_standards_doc_applicability = "{0}/quality-standards-categories/{1}";
        private const string _query_removed_violations_by_bcid = "{0}/removed-violations?";
        private const string _query_delta_components = "{0}/components/65005?snapshot-ids=({1},{2})&status={3}";
        private const string _query_omg_technical_debt_details = "{0}/results/?quality-indicators=(c:{1})&select=(omgTechnicalDebt)";

        #endregion CONSTANTS

        #region ATTRIBUTES

        /// <summary>
        /// 
        /// </summary>
        protected IHighlightProxy _Client;

        #endregion ATTRIBUTES

        #region PROPERTIES

        /// <summary>
        /// Get/Set the current connection.
        /// </summary>
        protected string _CurrentConnection;
        public string CurrentConnection
        {
            get
            {
                if (_CurrentConnection == null) throw new TypeLoadException("Rest connection not set");
                return _CurrentConnection;
            }
        }

        protected bool _CurrentApiKey;
        public bool CurrentApiKey => _CurrentApiKey;

        #endregion PROPERTIES

        #region CONSTRUCTORS

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="client"></param>
        public HighlightRepository(WSHighlightConnection connection, IHighlightProxy client)
        {
            _Client = client ?? new HighlightProxy(connection.Login, connection.Password, connection.ApiKey, connection.ServerCertificateValidation);
            _CurrentConnection = connection.Url;
            if (_CurrentConnection.EndsWith("/"))
            {
                _CurrentConnection = _CurrentConnection.Substring(0, _CurrentConnection.Length - 1);
            }
            _CurrentApiKey = connection.ApiKey;
        }

        public IHighlightProxy GetClient()
        {
            return _Client;
        }

        #endregion CONSTRUCTORS

        /// <summary>
        /// Dispose Method
        /// </summary>
        public void Dispose()
        {
            _Client?.Dispose();
        }


        #region Databases

        /// <summary>
        /// Is Service Valid
        /// </summary>
        /// <returns>True if OK</returns>
        bool IHighlightRepository.IsServiceValid()
        {
            var requestUrl = $"{_CurrentConnection}/benchmark";

            try
            {
                var jsonString = _Client.DownloadString(requestUrl);
                JsonConvert.DeserializeObject(jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        string IHighlightRepository.GetServerVersion()
        {
            return "WS2";
        }

        #endregion

        private string GetResource(string url) => _Client.DownloadString(_CurrentConnection + url);

        Company IHighlightRepository.GetCompany(string companyId)
        {
            var json = GetResource($"/companies/{companyId}");
            return JsonConvert.DeserializeObject<Company>(json);
        }

        HLDomain IHighlightRepository.GetDomain(string domainId)
        {
            var json = GetResource($"/domains/{domainId}");
            return JsonConvert.DeserializeObject<HLDomain>(json);
        }

        IList<AppId> IHighlightRepository.GetDomainAppIds(string domainId)
        {
            var json = GetResource($"/domains/{domainId}/applications");
            return JsonConvert.DeserializeObject<IList<AppId>>(json);
        }
    }
}
