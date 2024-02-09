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
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Domain
{
    /// <summary>
    /// Represents a central database.
    /// </summary>
    [Serializable]
    public class Setting
    {
        /// <summary>
        /// 
        /// </summary>       
        private ReportingParameter _reportingParameter;
        public ReportingParameter ReportingParameter
        {
            get => _reportingParameter ?? (_reportingParameter = new ReportingParameter());
            set => _reportingParameter = value;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<WSImagingConnection> _wsImagingConnections;
        public List<WSImagingConnection> WSImagingConnections
        {
            get => _wsImagingConnections ?? (_wsImagingConnections = new List<WSImagingConnection>());
            set => _wsImagingConnections = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WSImagingConnection GetActiveImagingConnection()
        {
            return WSImagingConnections.FirstOrDefault(_ => _.IsActive);
        }


        /// <summary>
        /// 
        /// </summary>
        public void ChangeActiveImagingConnection(string newActiveUrl)
        {
            WSImagingConnection previousActiveconnection = WSImagingConnections.FirstOrDefault(_ => _.IsActive);
            if (previousActiveconnection != null) previousActiveconnection.IsActive = false;

            WSImagingConnection newActiveConnection = WSImagingConnections.FirstOrDefault(_ => _.Url.Equals(newActiveUrl));
            if (newActiveConnection != null) newActiveConnection.IsActive = true;
        }


        /// <summary>
        /// 
        /// </summary>
        private List<WSHighlightConnection> _wsHighlightConnections;
        public List<WSHighlightConnection> WSHighlightConnections
        {
            get => _wsHighlightConnections ?? (_wsHighlightConnections = new List<WSHighlightConnection>());
            set => _wsHighlightConnections = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WSHighlightConnection GetActiveHighlightConnection()
        {
            return WSHighlightConnections.FirstOrDefault(_ => _.IsActive);
        }


        /// <summary>
        /// 
        /// </summary>
        public void ChangeActiveHighlightConnection(string newActiveUrl)
        {
            WSHighlightConnection previousActiveconnection = WSHighlightConnections.FirstOrDefault(_ => _.IsActive);
            if (previousActiveconnection != null) previousActiveconnection.IsActive = false;

            WSHighlightConnection newActiveConnection = WSHighlightConnections.FirstOrDefault(_ => _.Url.Equals(newActiveUrl));
            if (newActiveConnection != null) newActiveConnection.IsActive = true;
        }

        public override bool Equals(object obj)
        {
            return obj is Setting setting &&
                   EqualityComparer<ReportingParameter>.Default.Equals(_reportingParameter, setting._reportingParameter) &&
                   EqualityComparer<List<WSImagingConnection>>.Default.Equals(_wsImagingConnections, setting._wsImagingConnections) &&
                   EqualityComparer<List<WSHighlightConnection>>.Default.Equals(_wsHighlightConnections, setting._wsHighlightConnections);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_reportingParameter, _wsImagingConnections, _wsHighlightConnections);
        }
    }
}
