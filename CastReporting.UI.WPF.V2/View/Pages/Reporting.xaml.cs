﻿/*
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
using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.UI.WPF.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TreeView = System.Windows.Controls.TreeView;

namespace CastReporting.UI.WPF.Core.View
{
    /// <summary>
    /// Interaction logic for Reporting1.xaml
    /// </summary>
    public partial class Reporting : Page
    {
        private static readonly List<string> ExtensionList = new List<string> { ".xlsx", ".docx", ".pptx" };

        public Reporting()
        {
            InitializeComponent();
            DataContext = new ReportingVM();
            Loaded += OnLoaded;
            LoadTemplates();
        }

        public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            if (item?.Items.Count != 1 || !(item.Items[0] is string)) return;
            item.Items.Clear();

            DirectoryInfo _dir = item.Tag as DirectoryInfo;
            if (_dir == null) return;
            try
            {
                ListDirectory((TreeView)sender, _dir.FullName);
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is SecurityException || ex is UnauthorizedAccessException || ex is InvalidOperationException)
            {
                LogHelper.LogError("Cannot expand folders : " + ex.Message);
            }
        }

        private static void ListDirectory(TreeView treeView, string path)
        {
            treeView.Items.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            if (!rootDirectoryInfo.Exists) return;
            foreach (var directory in rootDirectoryInfo.GetDirectories())
                treeView.Items.Add(CreateDirectoryNode(directory));
            foreach (var file in rootDirectoryInfo.GetFiles())
            {
                if (ExtensionList.Contains(file.Extension))
                {
                    treeView.Items.Add(new TreeViewItem { Header = file.Name, Tag = file.FullName });
                }
            }
        }

        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name, Tag = directoryInfo.FullName };
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Items.Add(CreateDirectoryNode(directory));

            foreach (var file in directoryInfo.GetFiles())
            {
                if (ExtensionList.Contains(file.Extension))
                {
                    directoryNode.Items.Add(new TreeViewItem { Header = file.Name, Tag = file.FullName });
                }
            }

            return directoryNode;
        }


        private void ReloadTemplatesClicked(object sender, RoutedEventArgs e)
        {
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            ReportingVM _reportingVm = (ReportingVM)DataContext;
            switch (_reportingVm.SelectedTab)
            {
                case 0:
                    ListDirectory(TrvStructure, SettingsBLL.GetApplicationTemplateRootPath());
                    break;
                case 1:
                    ListDirectory(TrvStructure, SettingsBLL.GetPortfolioTemplateRootPath());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public WSConnection ActiveConnection { get; set; }

        public new System.Windows.Input.CommandBindingCollection CommandBindings { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ReportingVM _reportingVm = DataContext as ReportingVM;
            _reportingVm?.InitializeFromWS();
            if (!ExtendBLL.CheckExtendValid())
            {
                _reportingVm.MessageManager.OnExtendCheck(false);
            }
            else
            {
                _reportingVm.MessageManager.OnExtendSearchLatestVersion(!ExtendBLL.IsRGVersionLatest());
            }
        }



        /// <summary>
        /// Show Selection File Dialog 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButtonClicked(object sender, RoutedEventArgs e)
        {
            ReportingVM _reportingVm = DataContext as ReportingVM;
            string name;
            if (_reportingVm == null) return;
            switch (_reportingVm.SelectedTab)
            {
                case 0:
                    name = _reportingVm.SelectedApplication.Application.Name;
                    break;
                case 1:
                    name = _reportingVm.SelectedTag;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = _reportingVm.SelectedTemplateFile.Extension != ".xlsx" ?
                    string.Format("*{0}, *.pdf|*{0};*.pdf", _reportingVm.SelectedTemplateFile.Extension)
                    : string.Format("*{0}|*{0}", _reportingVm.SelectedTemplateFile.Extension),
                DefaultExt = _reportingVm.SelectedTemplateFile.Extension,
                FileName = _reportingVm.SelectedTemplateFile.Name.Replace('-', '_').Replace(_reportingVm.SelectedTemplateFile.Extension, $"_{name}_{DateTime.Now:MM-dd_HH-mm-ss}{_reportingVm.SelectedTemplateFile.Extension}")
            };


            var settings = SettingsBLL.GetSetting();

            if (string.IsNullOrEmpty(settings.ReportingParameter.GeneratedFilePath) || !Directory.Exists(settings.ReportingParameter.GeneratedFilePath))
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                dialog.InitialDirectory = settings.ReportingParameter.GeneratedFilePath;
            }

            var result = dialog.ShowDialog();
            var _vm = (ReportingVM)DataContext;
            if (result != null && result.Value)
            {
                settings.ReportingParameter.GeneratedFilePath = Path.GetDirectoryName(dialog.FileName);

                SettingsBLL.SaveSetting(settings);

                if (_vm != null) _vm.ReportFileName = dialog.FileName;
            }
            else
            {

                if (_vm != null) _vm.ReportFileName = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileListDoubleClicked(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedTreeViewItem = (TreeViewItem)TrvStructure.SelectedItem;
            FileInfo selectedFileInfo = new FileInfo(selectedTreeViewItem.Tag.ToString());
            if (selectedFileInfo.Exists && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(selectedFileInfo.FullName) { UseShellExecute = true });
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedTreeViewItem = (TreeViewItem)TrvStructure.SelectedItem;
            if (selectedTreeViewItem == null) return;
            FileInfo selectedFileInfo = new FileInfo(selectedTreeViewItem.Tag.ToString());
            if (selectedFileInfo.Exists)
            {
                ((ReportingVM)DataContext).SelectedTemplateFile = selectedFileInfo;
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

                (DataContext as ReportingVM)?.ActiveCurrentWebService(connection);
            }
            (DataContext as ReportingVM)?.InitializeFromWS();
            e.Handled = true;
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTemplates();
        }
    }
}