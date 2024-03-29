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
    public partial class UcImaging : UserControl, INotifyPropertyChanged {

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
    }
}