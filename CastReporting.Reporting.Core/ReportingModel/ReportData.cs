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

using CastReporting.Domain;
using CastReporting.Domain.Imaging;
using CastReporting.Domain.Imaging.Interfaces;
using System;
using System.IO;

namespace CastReporting.Reporting.ReportingModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportData : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatType ReportType
        {
            get
            {
                switch (Path.GetExtension(FileName).ToLowerInvariant())
                {
                    case ".docx": return FormatType.Word;
                    case ".xlsx": return FormatType.Excel;
                    case ".pptx": return FormatType.PowerPoint;
                    default: return FormatType.Unknown;
                }
            }
        }

        public ImagingData ImagingData { get; set; }

        public HighlightData HighlightData { get; set; }

        public void Dispose()
        {
            ImagingData?.Dispose();
            HighlightData.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
