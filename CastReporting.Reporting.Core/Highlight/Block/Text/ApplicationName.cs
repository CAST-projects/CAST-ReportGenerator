﻿/*
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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Highlight.Builder.BlockProcessing;
using CastReporting.Reporting.Highlight.ReportingModel;
using System.Collections.Generic;

namespace CastReporting.Reporting.Highlight.Block.Text
{
    [Block("HL_APPLICATION_NAME")]
    public class ApplicationName : TextBlock
    {
        #region METHODS
        public override string Content(HighlightData data, Dictionary<string, string> options)
        {
            return (data != null) ? /* do something with HL reportData ??*/ Domain.Constants.No_Value : Domain.Constants.No_Value;
        }
        #endregion METHODS
    }
}