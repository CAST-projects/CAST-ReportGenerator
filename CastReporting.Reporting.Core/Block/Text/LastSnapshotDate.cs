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
using Cast.Util;
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Text
{
    [Block("LAST_SNAPSHOT_DATE"), Block("TO_SNAPSHOT_DATE")]
    public class CurrentSnapshotDate : TextBlock
    {
        #region METHODS
        public override string Content(ImagingData reportData, Dictionary<string, string> options)
        {
            if (reportData?.CurrentSnapshot == null) return FormatHelper.No_Value;

            string res = string.Empty;
            DateTime? d = reportData.CurrentSnapshot.GetSnapshotDate();
            if (d.HasValue) res = d.Value.ToString(Labels.FORMAT_LONG_DATE);

            return string.IsNullOrEmpty(res) ? FormatHelper.No_Value : res;
        }
        #endregion METHODS
    }
}
