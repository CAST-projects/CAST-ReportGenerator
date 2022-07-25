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

namespace CastReporting.Domain
{
    [Serializable]
    public class ReportingParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public int ApplicationSizeLimitSupSmall { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ApplicationSizeLimitSupMedium { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ApplicationSizeLimitSupLarge { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double ApplicationQualityVeryLow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double ApplicationQualityLow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double ApplicationQualityMedium { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double ApplicationQualityGood { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NbResultDefault { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GeneratedFilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PortfolioFolderNamePath { get; set; }

        public string ApplicationFolderNamePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AADURL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CultureName { get; set; }

        public string ServerCertificateValidation { get; set; }

        public string ExtendUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ReportingParameter()
        {
            ApplicationSizeLimitSupSmall = 200000;
            ApplicationSizeLimitSupMedium = 500000;
            ApplicationSizeLimitSupLarge = 1000000;

            ApplicationQualityVeryLow = 2;
            ApplicationQualityLow = 2.8;
            ApplicationQualityMedium = 3.2;
            ApplicationQualityGood = 3.5;

            NbResultDefault = 5;
            PortfolioFolderNamePath = "\\Portfolio";
            ApplicationFolderNamePath = "\\Application";

            ExtendUrl = "https://extend.castsoftware.com";
        }

        public override bool Equals(object obj)
        {
            return obj is ReportingParameter parameter &&
                   ApplicationSizeLimitSupSmall == parameter.ApplicationSizeLimitSupSmall &&
                   ApplicationSizeLimitSupMedium == parameter.ApplicationSizeLimitSupMedium &&
                   ApplicationSizeLimitSupLarge == parameter.ApplicationSizeLimitSupLarge &&
                   ApplicationQualityVeryLow == parameter.ApplicationQualityVeryLow &&
                   ApplicationQualityLow == parameter.ApplicationQualityLow &&
                   ApplicationQualityMedium == parameter.ApplicationQualityMedium &&
                   ApplicationQualityGood == parameter.ApplicationQualityGood &&
                   NbResultDefault == parameter.NbResultDefault &&
                   GeneratedFilePath == parameter.GeneratedFilePath &&
                   TemplatePath == parameter.TemplatePath &&
                   PortfolioFolderNamePath == parameter.PortfolioFolderNamePath &&
                   ApplicationFolderNamePath == parameter.ApplicationFolderNamePath &&
                   AADURL == parameter.AADURL &&
                   CultureName == parameter.CultureName &&
                   ServerCertificateValidation == parameter.ServerCertificateValidation &&
                   ExtendUrl == parameter.ExtendUrl;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(ApplicationSizeLimitSupSmall);
            hash.Add(ApplicationSizeLimitSupMedium);
            hash.Add(ApplicationSizeLimitSupLarge);
            hash.Add(ApplicationQualityVeryLow);
            hash.Add(ApplicationQualityLow);
            hash.Add(ApplicationQualityMedium);
            hash.Add(ApplicationQualityGood);
            hash.Add(NbResultDefault);
            hash.Add(GeneratedFilePath);
            hash.Add(TemplatePath);
            hash.Add(PortfolioFolderNamePath);
            hash.Add(ApplicationFolderNamePath);
            hash.Add(AADURL);
            hash.Add(CultureName);
            hash.Add(ServerCertificateValidation);
            hash.Add(ExtendUrl);
            return hash.ToHashCode();
        }
    }
}
