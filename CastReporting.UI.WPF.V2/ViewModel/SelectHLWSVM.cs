/*
 *   Copyright (c) 2024 CAST
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
using CastReporting.BLL;
using CastReporting.HL.Domain;
using CastReporting.UI.WPF.Core.Common;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CastReporting.UI.WPF.Core.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectHLWSVM : ViewModelBase
    {

        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveCommand { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ICommand ActiveCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string _newConnectionUrl;
        public string NewConnectionUrl
        {
            get
            {
                return _newConnectionUrl;
            }
            set
            {
                _newConnectionUrl = value;

                OnPropertyChanged("NewConnectionUrl");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private string _newConnectionPassword;
        public string NewConnectionPassword
        {
            get
            {
                return _newConnectionPassword;
            }
            set
            {
                _newConnectionPassword = value;

                OnPropertyChanged("NewConnectionPassword");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private string _newConnectionLogin;
        public string NewConnectionLogin
        {
            get
            {
                return _newConnectionLogin;
            }
            set
            {
                _newConnectionLogin = value;

                OnPropertyChanged("NewConnectionLogin");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private bool _newConnectionApiKey;
        public bool NewConnectionApiKey
        {
            get
            {
                return _newConnectionApiKey;
            }
            set
            {
                _newConnectionApiKey = value;

                OnPropertyChanged("NewConnectionApiKey");
            }

        }

        /// <summary>
        /// 
        /// </summary>       
        private ObservableCollection<HLWSConnection> _wsHLConnections;
        public ObservableCollection<HLWSConnection> HLWSConnections
        {
            get
            {
                return _wsHLConnections;
            }
            set
            {
                _wsHLConnections = value;

                OnPropertyChanged("WSConnections");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private HLWSConnection _selectedHLWSConnection;
        public HLWSConnection SelectedHLWSConnection
        {
            get
            {
                return _selectedHLWSConnection;
            }
            set
            {
                _selectedHLWSConnection = value;

                OnPropertyChanged("SelectedWSConnection");
            }

        }

        /// <summary>
        ///
        /// </summary>
        public SelectHLWSVM()
        {
            RemoveCommand = new CommandHandler(ExecuteRemoveCommand, null);
            ActiveCommand = new CommandHandler(ExecuteActiveCommand, null);
            HLWSConnections = new ObservableCollection<HLWSConnection>(Setting.HLWSConnections);
        }

        /// <summary>
        /// Implement Add service Command
        /// </summary>
        public void ExecuteAddCommand(HLWSConnection conn)
        {
            try
            {
                StatesEnum state;
                Setting = SettingsBLL.AddConnection(conn, false, out state);

                if (state == StatesEnum.ConnectionAddedAndActivated || state == StatesEnum.ConnectionAddedSuccessfully)
                {
                    HLWSConnections = new ObservableCollection<HLWSConnection>(Setting.HLWSConnections);
                    NewConnectionUrl = NewConnectionLogin = NewConnectionPassword = string.Empty;
                }

                MessageManager.OnServiceAdded(state);
            }
            catch (UriFormatException ex)
            {
                LogHelper.LogInfo(ex.Message);
                MessageManager.OnServiceAdded(StatesEnum.ServiceInvalid);
            }
        }

        /// <summary>
        /// Implement remove service Command
        /// </summary>
        private void ExecuteRemoveCommand(object prameter)
        {
            if (SelectedHLWSConnection == null) return;

            Setting = SettingsBLL.RemoveConnection(SelectedHLWSConnection);
            HLWSConnections = new ObservableCollection<HLWSConnection>(Setting.HLWSConnections);

            MessageManager.OnServiceRemoved();
        }


        /// <summary>
        /// Implement active service Command
        /// </summary>
        private void ExecuteActiveCommand(object prameter)
        {
            if (SelectedHLWSConnection == null) return;
            Setting.ChangeActiveConnection(SelectedHLWSConnection);

            SettingsBLL.SaveSetting(Setting);

            MessageManager.OnServiceActivated();

            HLWSConnections = new ObservableCollection<HLWSConnection>(Setting.HLWSConnections);
        }

    }
}
