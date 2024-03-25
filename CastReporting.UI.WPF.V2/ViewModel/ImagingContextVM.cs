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


using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.UI.WPF.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

// ReSharper disable InconsistentNaming

namespace CastReporting.UI.WPF.Core.ViewModel
{
    /// <summary>
    ///  Implement the ViewModel for the reporting page
    /// </summary>
    public class ImagingContextVM : ViewModelBase
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public ImagingContextVM() {
            LoadSnapshotsCommand = new CommandHandler(ExecuteLoadSnapshotsCommand, null);
            LoadPreviousSnapshotsCommand = new CommandHandler(ExecuteLoadPreviousSnapshotsCommand, null);
            LoadTagsCommand = new CommandHandler(ExecuteLoadTagsCommand, null);
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand LoadTagsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand LoadSnapshotsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand LoadPreviousSnapshotsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<ApplicationItem> _Applications;
        public IEnumerable<ApplicationItem> Applications
        {
            get { return _Applications; }
            set {
                _Applications = value;
                OnPropertyChanged("Applications");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IList<string> _Categories;
        public IList<string> Categories
        {
            get { return _Categories; }
            set
            {
                _Categories = value;
                OnPropertyChanged("Categories");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<string> _Tags;
        public IEnumerable<string> Tags
        {
            get { return _Tags; }
            set
            {
                _Tags = value;
                OnPropertyChanged("Tags");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<Snapshot> _Snaphosts;
        public IEnumerable<Snapshot> Snaphosts
        {
            get { return _Snaphosts; }
            set
            {
                _Snaphosts = value;
                OnPropertyChanged("Snaphosts");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<Snapshot> _PreviousSnapshosts { get; set; }
        public IEnumerable<Snapshot> PreviousSnaphosts
        {
            get { return _PreviousSnapshosts; }
            set
            {
                _PreviousSnapshosts = value;
                OnPropertyChanged("PreviousSnaphosts");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private CastDomain _SelectedDomain;
        public CastDomain SelectedDomain
        {
            get => _SelectedDomain;
            set
            {
                if (Equals(value, _SelectedDomain))
                    return;
                _SelectedDomain = value;
                OnPropertyChanged("SelectedDomain");
                OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private ApplicationItem _SelectedApplication;
        public ApplicationItem SelectedApplication
        {
            get { return _SelectedApplication; }
            set
            {
                if (value == _SelectedApplication)
                    return;
                _SelectedApplication = value;

                OnPropertyChanged("SelectedApplication");
                OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _SelectedCategory;
        public string SelectedCategory
        {
            get { return _SelectedCategory; }
            set
            {
                if (value == _SelectedCategory)
                    return;
                _SelectedCategory = value;

                OnPropertyChanged("SelectedCategory");
                OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Snapshot _SelectedSnapshot;
        public Snapshot SelectedSnapshot
        {
            get { return _SelectedSnapshot; }
            set
            {
                if (Equals(value, _SelectedSnapshot))
                    return;
                _SelectedSnapshot = value;

                OnPropertyChanged("SelectedSnapshot");
                OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _SelectedTag;
        public string SelectedTag
        {
            get { return _SelectedTag; }
            set
            {
                if (value == _SelectedTag)
                    return;
                _SelectedTag = value;

                OnPropertyChanged("SelectedTag");
                OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Snapshot _PreviousSnapshot;
        public Snapshot PreviousSnapshot
        {
            get { return _PreviousSnapshot; }
            set
            {
                if (Equals(value, _PreviousSnapshot))
                    return;
                _PreviousSnapshot = value;

                OnPropertyChanged("PreviousSnapshot");
                OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private WSConnection _ActiveConnection;
        public WSConnection ActiveConnection
        {
            get
            {
                return _ActiveConnection;
            }
            set
            {
                _ActiveConnection = value;
                OnPropertyChanged("ActiveConnection");
            }

        }

        /// <summary>
        /// Implement Command that Load the templates list
        /// </summary>
        private void ExecuteLoadTagsCommand(object parameter)
        {
            if (SelectedCategory != null)
            {
                //GetActive Connection           
                ActiveConnection = Setting?.GetActiveConnection();

                //Get list of domains
                if (_ActiveConnection == null) return;
                try
                {
                    using (CastDomainBLL castDomainBLL = new CastDomainBLL(ActiveConnection))
                    {
                        List<Tag> _tags = castDomainBLL.GetTags(SelectedCategory);
                        Tags = _tags.Select(t => t.Label).ToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageManager.OnErrorOccured(ex);
                }
            }
            else

                Tags = null;
        }

        /// <summary>
        /// Implement Command that Load the current snapshots list
        /// </summary>
        private void ExecuteLoadSnapshotsCommand(object parameter)
        {
            if (SelectedApplication != null)
            {
                using (ApplicationBLL applicationBLL = new ApplicationBLL(ActiveConnection, SelectedApplication.Application))
                {
                    applicationBLL.SetSnapshots();
                    Snaphosts = SelectedApplication.Application.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).ToList();
                }
            }
            else
                Snaphosts = null;
        }

        /// <summary>
        /// Implement Command that Load previous snapshots list
        /// </summary>
        private void ExecuteLoadPreviousSnapshotsCommand(object parameter)
        {
            if (SelectedSnapshot != null && Snaphosts != null)
                PreviousSnaphosts = Snaphosts.Where(_ => _.Annotation.Date.CompareTo(SelectedSnapshot.Annotation.Date) < 0).ToList();
            else
                PreviousSnaphosts = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeFromWS()
        {
            // Imaging connection
            ActiveConnection = Setting?.GetActiveConnection();

            //Get list of domains
            if (_ActiveConnection?.Password != null && _ActiveConnection?.Login != null)
            {
                try
                {
                    using CastDomainBLL castDomainBLL = new CastDomainBLL(ActiveConnection);
                    Applications = castDomainBLL.GetAdgApplications()?.Select(app => new ApplicationItem(app));
                    IEnumerable<CastDomain> domains = castDomainBLL.GetDomains().Where(domain => domain.DBType.Equals("AAD"));
                    Categories = domains.Any() ? castDomainBLL.GetCategories() : new List<string>();
                }
                catch (Exception ex)
                {
                    MessageManager.OnErrorOccured(ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public void ActiveCurrentWebService(WSConnection connection)
        {
            StatesEnum state;
            Setting = SettingsBLL.AddConnection(connection, true, out state);
            MessageManager.OnServiceAdded(state);
        }
    }
}
