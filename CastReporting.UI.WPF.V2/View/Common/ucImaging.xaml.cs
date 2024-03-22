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
using CastReporting.UI.WPF.Core.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace CastReporting.UI.WPF.Core.Common
{
    /// <summary>
    /// Interaction logic for Reporting1.xaml
    /// </summary>
    public partial class UcImaging : UserControl,INotifyPropertyChanged {
        private static readonly List<string> ExtensionList = new List<string> { ".xlsx", ".docx", ".pptx" };

        public UcImaging() {
            InitializeComponent();
        }

        public new System.Windows.Input.CommandBindingCollection CommandBindings { get; set; }

        private ReportingVM ReportingContext => DataContext as ReportingVM;

        public event PropertyChangedEventHandler PropertyChanged {
            add {
                ((INotifyPropertyChanged)ReportingContext).PropertyChanged += value;
            }

            remove {
                ((INotifyPropertyChanged)ReportingContext).PropertyChanged -= value;
            }
        }

        private void ActivateWebService_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivateWebService_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var list = e.Parameter as List<object>;
            if (list != null)
            {
                var connection = new WSConnection
                {
                    Url = (string)list[0],
                    Login = (string)list[1],
                    Password = (string)list[2],
                    ApiKey = (bool)list[3],
                    ServerCertificateValidation = SettingsBLL.GetCertificateValidationStrategy()
                };

                ReportingContext?.ImagingContext.ActiveCurrentWebService(connection);
            }
            ReportingContext?.ImagingContext.InitializeFromWS();
            e.Handled = true;
        }
    }
}