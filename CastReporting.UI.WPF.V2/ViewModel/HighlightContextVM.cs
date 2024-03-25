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
//using CastReporting.Domain;
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
        private IEnumerable<AppId> _Applications;
        public IEnumerable<AppId> Applications
        {
            get { return _Applications; }
            set
            {
                _Applications = value;
                OnPropertyChanged("Applications");
                Snapshots=null;
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
        private IEnumerable<Snapshot> _Snapshots;
        public IEnumerable<Snapshot> Snapshots
        {
            get { return _Snapshots; }
            set
            {
                _Snapshots = value;
                OnPropertyChanged("Snapshots");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<Snapshot> _PreviousSnapshosts { get; set; }
        public IEnumerable<Snapshot> PreviousSnapshots
        {
            get { return _PreviousSnapshosts; }
            set
            {
                _PreviousSnapshosts = value;
                OnPropertyChanged("PreviousSnapshots");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private AppId _SelectedApplication;
        public AppId SelectedApplication
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
            // portfolio reports not supported yet
            Tags = null;
        }

        /// <summary>
        /// Implement Command that Load the current snapshots list
        /// </summary>
        private void ExecuteLoadSnapshotsCommand(object parameter)
        {
            if (SelectedApplication != null) {
                var accountService = new AccountService(ActiveConnection);
                Snapshots = accountService.GetAvailableSnapshots(SelectedApplication)
                    .OrderByDescending(_ => _.SnapshotDate);
            } else {
                Snapshots = null;
            }
        }


        /// <summary>
        /// Implement Command that Load previous snapshots list
        /// </summary>
        private void ExecuteLoadPreviousSnapshotsCommand(object parameter)
        {
            if (SelectedSnapshot != null && Snapshots != null) {
                PreviousSnapshots = Snapshots.Where(_ => _.SnapshotDate.CompareTo(SelectedSnapshot.SnapshotDate) < 0)
                    .OrderByDescending(_ => _.SnapshotDate);
            } else {
                PreviousSnapshots = null;
            }
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
            if (ActiveConnection.Password != null && ActiveConnection.Login != null && ActiveConnection.CompanyId != null) {
                try {
                    var accountService = new AccountService(ActiveConnection);
                    Applications = accountService.GetAvailableApplications().OrderBy(_ => _.Name);
                } catch (Exception ex) {
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
