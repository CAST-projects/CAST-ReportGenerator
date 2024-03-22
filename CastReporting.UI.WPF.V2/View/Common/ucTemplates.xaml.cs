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
using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.UI.WPF.Core.View.Pages;
using CastReporting.UI.WPF.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TreeView = System.Windows.Controls.TreeView;

namespace CastReporting.UI.WPF.Core.Common
{
    /// <summary>
    /// Interaction logic for Reporting1.xaml
    /// </summary>
    public partial class UcTemplates: UserControl ,INotifyPropertyChanged{
        public UcTemplates() {
            InitializeComponent();
        }

        TemplatesVM TemplatesContext => DataContext as TemplatesVM;

        public event PropertyChangedEventHandler PropertyChanged {
            add {
                ((INotifyPropertyChanged)TemplatesContext).PropertyChanged += value;
            }

            remove {
                ((INotifyPropertyChanged)TemplatesContext).PropertyChanged -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileListDoubleClicked(object sender, RoutedEventArgs e)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                var selectedFile = TrvStructure.SelectedItem as FileLeaf;
                if (selectedFile != null && Path.Exists(selectedFile.FullName)) {
                    Process.Start(new ProcessStartInfo(selectedFile.FullName) { UseShellExecute = true });
                }
            }
        }

        private void TrvStructure_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            var selectedFileInfo = (e.NewValue as FileLeaf)?.FileInfo;
            if (selectedFileInfo!=TemplatesContext.SelectedTemplateFile) {
                TemplatesContext.SelectedTemplateFile = selectedFileInfo;
            }
            TemplatesContext.SelectedTemplateFile = (e.NewValue as FileLeaf)?.FileInfo;
        }
    }
}