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
using CastReporting.HL.Domain;
using CastReporting.HL.Services;
using CastReporting.Reporting.Core.ReportingModel;
using CastReporting.UI.WPF.Core.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            LoadSnapshotsCommand = new CommandHandler(ExecuteLoadSnapshotsCommand, null);
            LoadPreviousSnapshotsCommand = new CommandHandler(ExecuteLoadPreviousSnapshotsCommand, null);
        }

        public ICommand LoadSnapshotsCommand { get; set; }

        public ICommand LoadPreviousSnapshotsCommand { get; set; }

        private IEnumerable<AppId> _Applications;
        public IEnumerable<AppId> Applications {
            get { return _Applications; }
            set {
                _Applications = value;
                OnPropertyChanged(nameof(Applications));
                Snapshots = null;
                PreviousSnapshots = null;
            }
        }

        private IEnumerable<Snapshot> _Snapshots;
        public IEnumerable<Snapshot> Snapshots {
            get { return _Snapshots; }
            set {
                _Snapshots = value;
                OnPropertyChanged(nameof(Snapshots));
            }
        }

        private IEnumerable<Snapshot> _PreviousSnapshosts { get; set; }
        public IEnumerable<Snapshot> PreviousSnapshots {
            get { return _PreviousSnapshosts; }
            set {
                _PreviousSnapshosts = value;
                OnPropertyChanged(nameof(PreviousSnapshots));
            }
        }

        private AppId _SelectedApplication;
        public AppId SelectedApplication {
            get { return _SelectedApplication; }
            set {
                if (Equals(value, _SelectedApplication)) return;
                _SelectedApplication = value;
                OnPropertyChanged(nameof(SelectedApplication));
            }
        }

        private Snapshot _SelectedSnapshot;
        public Snapshot SelectedSnapshot {
            get { return _SelectedSnapshot; }
            set {
                if (Equals(value, _SelectedSnapshot)) return;
                _SelectedSnapshot = value;
                OnPropertyChanged(nameof(SelectedSnapshot));
            }
        }

        private Snapshot _PreviousSnapshot;
        public Snapshot PreviousSnapshot {
            get { return _PreviousSnapshot; }
            set {
                if (Equals(value, _PreviousSnapshot)) return;
                _PreviousSnapshot = value;
                OnPropertyChanged(nameof(PreviousSnapshot));
            }
        }

        private HLWSConnection _ActiveConnection;
        public HLWSConnection ActiveConnection {
            get { return _ActiveConnection; }
            set {
                _ActiveConnection = value;
                OnPropertyChanged(nameof(ActiveConnection));
            }
        }

        /// <summary>
        /// Implement command that loads the list of snapshots for the selected application
        /// </summary>
        private void ExecuteLoadSnapshotsCommand(object parameter) {
            if (SelectedApplication != null) {
                var accountService = new AccountService(ActiveConnection);
                Snapshots = accountService.GetAvailableSnapshots(SelectedApplication)
                    .OrderByDescending(_ => _.SnapshotDate);
                SelectedSnapshot = Snapshots.FirstOrDefault();
            } else {
                Snapshots = null;
            }
        }

        /// <summary>
        /// Implement command that loads the list of snapshots older than the selected snapshot
        /// </summary>
        private void ExecuteLoadPreviousSnapshotsCommand(object parameter) {
            if (SelectedSnapshot != null) {
                PreviousSnapshots = Snapshots?.Where(_ => _.SnapshotDate.CompareTo(SelectedSnapshot.SnapshotDate) < 0)
                    .OrderByDescending(_ => _.SnapshotDate);
            } else {
                PreviousSnapshots = null;
            }
        }

        /// <summary>
        /// Load list of apps from Highlight
        /// </summary>
        public void InitializeFromWS() {
            // Load Highlight connection info from settings
            ActiveConnection = Setting?.GetActiveHLConnection();
            if (ActiveConnection == null) return;

            // Get list of applications
            if (ActiveConnection.Password != null && ActiveConnection.Login != null && ActiveConnection.CompanyId != null) {
                try {
                    var accountService = new AccountService(ActiveConnection);
                    Applications = accountService.GetAvailableApplications().OrderBy(_ => _.Name);
                } catch (Exception ex) {
                    MessageManager.OnErrorOccured(ex);
                }
            }
        }

        public void ActiveCurrentWebService(HLWSConnection connection) {
            Setting = SettingsBLL.AddConnection(connection, true, out StatesEnum state);
            MessageManager.OnServiceAdded(state);
        }

        public HighlightData LoadApplicationData() {
            if (SelectedApplication!= null) {
                var appService = new ApplicationService(ActiveConnection);
                var appInfo = appService.GetResults(SelectedApplication);
                if (appInfo == null) return null;
                if (SelectedSnapshot != null) {
                    // if a current snapshot + a (optional) previous snapshot are selected,
                    // remove all metrics other than those of selected snapshots
                    for (var i = appInfo.Metrics.Count - 1; i >= 0; i--) {
                        var snapshot = appInfo.Metrics[i];
                        if (snapshot == SelectedSnapshot || snapshot == PreviousSnapshot) continue;
                        appInfo.Metrics.RemoveAt(i);
                    }
                }
                return new HighlightData(appInfo);
            } else {
                return null;
            }
        }
    }
}
