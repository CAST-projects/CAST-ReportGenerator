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
using CastReporting.UI.WPF.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace CastReporting.UI.WPF.Core.View.Pages
{
    /// <summary>
    /// Logique d'interaction pour ReportingTabs.xaml
    /// </summary>
    public partial class ReportingTabs : Page
    {
        public ReportingTabs() {
            InitializeComponent();
            ReportingContext = new ReportingVM();
            Loaded += OnLoaded;
        }

        private ReportingVM ReportingContext {
            get => DataContext as ReportingVM;
            set {
                if (DataContext == value) return;
                DataContext = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e) {
            ReportingContext.ImagingContext.InitializeFromWS();
            ReportingContext.HighlightContext.InitializeFromWS();
            ReportingContext.Templates.RefreshTemplates(ReportingContext.ReportingMode);
            if (ExtendBLL.CheckExtendValid()) {
                ReportingContext.MessageManager.OnExtendSearchLatestVersion(!ExtendBLL.IsRGVersionLatest());
            } else { 
                ReportingContext.MessageManager.OnExtendCheck(false);
            }
        }

        /// <summary>
        /// Show Selection File Dialog 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButtonClicked(object sender, RoutedEventArgs e) {
            if (!ReportingContext.IsDataFilledIn) return;

            var templateFile = ReportingContext.Templates.SelectedTemplateFile;

            string name;
            switch (ReportingContext.ReportingMode) {
                case ReportingMode.Application:
                    name = ReportingContext.ImagingContext.SelectedApplication?.Application.Name
                        ?? ReportingContext.HighlightContext.SelectedApplication?.Name
                        ?? "Missing name";
                    break;
                case ReportingMode.Portfolio:
                    name = ReportingContext.ImagingContext.SelectedTag;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SaveFileDialog dialog = new SaveFileDialog {
                Filter = templateFile.Extension != ".xlsx"
                    ? string.Format("*{0}, *.pdf|*{0};*.pdf", templateFile.Extension)
                    : string.Format("*{0}|*{0}", templateFile.Extension),

                DefaultExt = templateFile.Extension,
                FileName = templateFile.Name.Replace('-', '_').Replace(templateFile.Extension, $"_{name}_{DateTime.Now:MM-dd_HH-mm-ss}{templateFile.Extension}")
            };

            var settings =  SettingsBLL.GetSetting();

            if (string.IsNullOrEmpty(settings.ReportingParameter.GeneratedFilePath) || !Directory.Exists(settings.ReportingParameter.GeneratedFilePath)) {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            } else {
                dialog.InitialDirectory = settings.ReportingParameter.GeneratedFilePath;
            }

            if (dialog.ShowDialog() == DialogResult.OK) {
                ReportingContext.ReportFileName = dialog.FileName;
                settings.ReportingParameter.GeneratedFilePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                SettingsBLL.SaveSetting(settings); // NOTE: because ViewModelBase.Setting is a Singleton (static), it is now out of sync!
            } else {
                ReportingContext.ReportFileName = string.Empty;
            }
        }

        private void ActivateWebService_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void ActivateWebService_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            var list = e.Parameter as List<object>;
            if (list != null) {
                var connection = new WSConnection {
                    Url = (string)list[0],
                    Login = (string)list[1],
                    Password = (string)list[2],
                    ApiKey = (bool)list[3],
                    ServerCertificateValidation = SettingsBLL.GetCertificateValidationStrategy()
                };
                ReportingContext.ImagingContext.ActiveCurrentWebService(connection);
            }
            ReportingContext.ImagingContext.InitializeFromWS();
            e.Handled = true;
        }

        private void ActivateHLWebService_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void ActivateHLWebService_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            var list = e.Parameter as List<object>;
            if (list != null) {
                var connection = new HLWSConnection {
                    Url = (string)list[0],
                    Login = (string)list[1],
                    Password = (string)list[2],
                    CompanyId = (string)list[3],
                    ApiKey = (bool)list[4],
                    ServerCertificateValidation = SettingsBLL.GetCertificateValidationStrategy()
                };
                ReportingContext.HighlightContext.ActiveCurrentWebService(connection);
            }
            ReportingContext.HighlightContext.InitializeFromWS();
            e.Handled = true;
        }
    }
}
