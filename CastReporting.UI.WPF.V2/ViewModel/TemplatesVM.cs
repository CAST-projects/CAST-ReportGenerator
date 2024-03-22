using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.UI.WPF.Core.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CastReporting.UI.WPF.Core.ViewModel
{
    public class TemplatesVM : ViewModelBase
    {
        public TemplatesVM() {
            _ReportingMode = ReportingMode.Application;
            RefreshTemplatesCommand = new CommandHandler(_ => RefreshTemplates(_ReportingMode), null);
        }

        public ICommand RefreshTemplatesCommand { get; init; }

        private IReadOnlyList<FileSystemNode> _TemplateDirAndFiles = [];
        public IReadOnlyList<FileSystemNode> TemplateDirAndFiles {
            get { return _TemplateDirAndFiles; }
            set {
                if (value != _TemplateDirAndFiles) {
                    _TemplateDirAndFiles = value;
                    _SelectedTemplateFile = null;
                    OnPropertyChanged("TemplateDirAndFiles");
                    OnPropertyChanged("SelectedTemplateFile");
                }
            }
        }

        private FileInfo _SelectedTemplateFile;
        public FileInfo SelectedTemplateFile {
            get { return _SelectedTemplateFile; }
            set {
                if (value != _SelectedTemplateFile) {
                    _SelectedTemplateFile = value;
                    OnPropertyChanged("SelectedTemplateFile");
                }
            }
        }

        private ReportingMode _ReportingMode;

        public void RefreshTemplates(ReportingMode reportingMode) {
            _ReportingMode = reportingMode;
            DirNode root;
            switch (_ReportingMode) {
                case ReportingMode.Application:
                    root = SettingsBLL.LoadTemplatesForApplication(Setting.ReportingParameter);
                    break;
                case ReportingMode.Portfolio:
                    root = SettingsBLL.LoadTemplatesForPortfolio(Setting.ReportingParameter);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(ReportingMode), (int)reportingMode, typeof(ReportingMode));
            }
            SelectedTemplateFile = null;
            TemplateDirAndFiles = root?.Children.ToList() ?? [];
            Debug.WriteLine($"Found {TemplateDirAndFiles.Count} templates");
        }
    }
}
