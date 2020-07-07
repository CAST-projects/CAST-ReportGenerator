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
using Cast.Util.Version;
using System.Reflection;

namespace CastReporting.BLL
{

    /// <summary>
    /// 
    /// </summary>
    public class ExtendBLL : BaseBLL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public ExtendBLL()
            : base()
        {
        }

        public static bool CheckExtendValid()
        {
            string url = SettingsBLL.GetExtendUrl();
            using (var extendRepository = GetExtendRepository(url, null))
            {
                return extendRepository.IsExtendValid(url);
            }
        }

        public static bool IsRGVersionLatest()
        {
            string url = SettingsBLL.GetExtendUrl();
            using (var extendRepository = GetExtendRepository(url, null))
            {
                var ver = VersionUtil.GetRGVersion(Assembly.GetExecutingAssembly());
                string latestVersion = extendRepository.SearchForLatestVersion("com.castsoftware.aip.reportgenerator");
                string extendVer = latestVersion.Substring(0, latestVersion.IndexOf("-"));
                return new System.Version(ver).CompareTo(new System.Version(extendVer)) >= 0;
            }
        }
        public static string LatestRGversion()
        {
            string url = SettingsBLL.GetExtendUrl();
            using (var extendRepository = GetExtendRepository(url, null))
            {
                return extendRepository.SearchForLatestVersion("com.castsoftware.aip.reportgenerator");
            }
        }
    }
}
