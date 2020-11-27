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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CastReporting.BLL
{

    /// <summary>
    /// 
    /// </summary>
    public class CastDomainBLL : BaseBLL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public CastDomainBLL(WSConnection connection)
            : base(connection)
        {
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CastDomain> GetDomains()
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetDomains();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<Application> GetApplications()
        {
            List<Application> applications = new List<Application>();

            var domains = GetDomains();

            using (var castRepsitory = GetRepository())
            {
                foreach (var domain in domains)
                {
                    List<Application> domainApps = castRepsitory.GetApplicationsByDomain(domain.Href)?.ToList();
                    if (domainApps == null) continue;
                    foreach (Application application in domainApps.Where(_ => string.IsNullOrEmpty(_.Version)))
                    {
                        application.Version = domain.Version;
                    }
                    applications.AddRange(domainApps);
                }
            }

            return applications.OrderBy(_ => _.Name).ToList();
        }

        public List<Application> GetAadApplications()
        {
            List<Application> applications = new List<Application>();

            CastDomain aad = GetDomains().Where(d => d.DBType.Equals("AAD")).First();

            using (var castRepsitory = GetRepository())
            {
                List<Application> domainApps = castRepsitory.GetApplicationsByDomain(aad.Href)?.ToList();
                if (domainApps == null) return applications;
                foreach (Application application in domainApps.Where(_ => string.IsNullOrEmpty(_.Version)))
                {
                    application.Version = aad.Version;
                }
                applications.AddRange(domainApps);
            }

            return applications.OrderBy(_ => _.Name).ToList();
        }

        public List<Application> GetAdgApplications()
        {
            List<Application> applications = new List<Application>();

            var adgDomains = GetDomains().Where(d => d.DBType.Equals("ADG"));

            using (var castRepsitory = GetRepository())
            {
                foreach (var domain in adgDomains)
                {
                    List<Application> domainApps = castRepsitory.GetApplicationsByDomain(domain.Href)?.ToList();
                    if (domainApps == null) continue;
                    foreach (Application application in domainApps.Where(_ => string.IsNullOrEmpty(_.Version)))
                    {
                        application.Version = domain.Version;
                    }
                    applications.AddRange(domainApps);
                }
            }

            return applications.OrderBy(_ => _.Name).ToList();
        }

        public static List<Snapshot> GetAllSnapshots(Application[] applications)
        {
            List<Snapshot> _snapshots = new List<Snapshot>();
            foreach (Application _appl in applications)
            {
                int nbSnapshotsEachApp = _appl.Snapshots.Count();
                if (nbSnapshotsEachApp <= 0) continue;
                foreach (Snapshot snapshot in _appl.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot))
                {
                    snapshot.AdgVersion = _appl.AdgVersion;
                    _snapshots.Add(snapshot);
                }
            }
            return _snapshots;
        }


        public List<Application> GetCommonTaggedApplications(string strSelectedTag, string strSelectedCategoy)
        {
            List<Application> _commonTaggedApplications = new List<Application>();
            List<Application> applications = GetAadApplications();
            if (strSelectedTag == null && strSelectedCategoy == null)
            {
                return applications;
            }
            else
            {
                List<Tag> strTags = GetTags(strSelectedCategoy);
                using (var castRepository = GetRepository())
                {
                    List<CommonTaggedApplications> commonTaggedApplications = castRepository.GetCommonTaggedApplications().ToList();
                    if (strTags.Count() == 0 || commonTaggedApplications.Count() == 0)
                    {
                        return _commonTaggedApplications;
                    }
                    foreach (Tag _tag in strTags)
                    {
                        if (_tag.Label != strSelectedTag) continue;
                        foreach (CommonTaggedApplications taggedApplications in commonTaggedApplications)
                        {
                            foreach(Tag t in taggedApplications.Tags)
                            {
                                if (t.Key.Equals(_tag.Key))
                                {
                                   _commonTaggedApplications.Add(applications.Where(a => a.Href == taggedApplications.TaggedApplication.Href).First());
                                }
                            }
                        }
                    }
                }
            }
            return _commonTaggedApplications;
        }

        public List<Tag> GetTags(string strCategory)
        {
            List<Tag> _tags = new List<Tag>();

            using (var castRepository = GetRepository())
            {
                List<CommonCategories> _commonCategories = castRepository.GetCommonCategories().ToList();
                if (_commonCategories.Count == 0) return _tags;
                foreach(CommonCategories cat in _commonCategories)
                {
                    if (strCategory != cat.Name) continue;
                    _tags.AddRange(cat.Tags.ToList());
                }
            }
            return _tags;
        }

        public List<string> GetCategories()
        {
            try
            {
                List<string> _categories = new List<string>();

                using (var castRepository = GetRepository())
                {
                    var _categoriess = castRepository.GetCommonCategories();

                    _categories.AddRange(_categoriess.Select(category => string.IsNullOrEmpty(category.Name) ? " " : category.Name));
                }

                return _categories;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                List<string> _categories = new List<string>();
                return _categories;
            }
        }

    }
}
