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
using CastReporting.Reporting.Highlight.ReportingModel;
using System.IO;

namespace CastReporting.Reporting.ReportingModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportData
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
                    case ".docx":
                        return FormatType.Word;

                    case ".xlsx":
                        return FormatType.Excel;

                    case ".pptx":
                        return FormatType.PowerPoint;

                    default:
                        return 0; // WARNING(DMA): I believe this is the same as FormatType.Word since the enum definition does not assign a value to its fields.
                                  // WARNING(DMA): Is this the expected behavior?
                }
            }

        }

        public ImagingData ImagingData { get; set; }

        public HighlightData HighlightData { get; set; }
    }
}
