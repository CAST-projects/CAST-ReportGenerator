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
using CastReporting.HL.Domain;
using CastReporting.HL.Services;
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
    public class HighlightContextVM : ViewModelBase
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public HighlightContextVM() {
            LoadTagsCommand = new CommandHandler(ExecuteLoadTagsCommand, null);
            LoadSnapshotsCommand = new CommandHandler(ExecuteLoadSnapshotsCommand, null);
            LoadPreviousSnapshotsCommand = new CommandHandler(ExecuteLoadPreviousSnapshotsCommand, null);
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
        private IEnumerable<ApplicationItem<AppId>> _Applications;
        public IEnumerable<ApplicationItem<AppId>> Applications
        {
            get { return _Applications; }
            set
            {
                _Applications = value;
                OnPropertyChanged("Applications");
                Snaphosts=null;
                SelectedSnapshot = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<string> _Categories;
        public IEnumerable<string> Categories
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
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private ApplicationItem <AppId>_SelectedApplication;
        public ApplicationItem <AppId>SelectedApplication
        {
            get { return _SelectedApplication; }
            set
            {
                if (value == _SelectedApplication)
                    return;
                _SelectedApplication = value;
                OnPropertyChanged("SelectedApplication");
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
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private HLWSConnection _ActiveConnection;
        public HLWSConnection ActiveConnection
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
                ActiveConnection = Setting?.GetActiveHLConnection();

                //Get list of domains
                if (_ActiveConnection == null) return;
                try
                {
                    //using (CastDomainBLL castDomainBLL = new CastDomainBLL(ActiveConnection))
                    //{
                    //    List<Tag> _tags = castDomainBLL.GetTags(SelectedCategory);
                    //    Tags = _tags.Select(t => t.Label).ToList();
                    //}
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
                //using (ApplicationBLL applicationBLL = new ApplicationBLL(ActiveConnection, SelectedApplication.Application))
                //{
                //    applicationBLL.SetSnapshots();
                //    Snaphosts = SelectedApplication.Application.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).ToList();
                //}
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
            // Highlight connection
            ActiveConnection = Setting?.GetActiveHLConnection();
            if (ActiveConnection == null) return;

            //Get list of domains
            if (ActiveConnection.Password != null && ActiveConnection.Login != null && ActiveConnection.CompanyId != null)
            {
                try
                {
                    var accountService = new AccountService(ActiveConnection);
                    Applications = accountService.GetAvailableApplications()?.Select(app => new ApplicationItem<AppId>(app));
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
        public void ActiveCurrentWebService(HLWSConnection connection)
        {
            StatesEnum state;
            Setting = SettingsBLL.AddConnection(connection, true, out state);
            MessageManager.OnServiceAdded(state);
        }
    }
}
