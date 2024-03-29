using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.UI.WPF.Core.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CastReporting.UI.WPF.Core.ViewModel
{
    public class TemplatesVM : ViewModelBase
    {
        public TemplatesVM() {
            RefreshTemplatesCommand = new CommandHandler(_ => RefreshTemplates((ReportingMode)(_ ?? _ReportingMode ?? ReportingMode.Application)), null);
        }

        public ICommand RefreshTemplatesCommand { get; init; }

        private IReadOnlyList<FileSystemNode> _TemplateDirAndFiles = [];
        public IReadOnlyList<FileSystemNode> TemplateDirAndFiles {
            get { return _TemplateDirAndFiles; }
            set {
                if (value != _TemplateDirAndFiles) {
                    _TemplateDirAndFiles = value;
                    SelectedTemplateFile = null;
                    OnPropertyChanged(nameof(TemplateDirAndFiles));
                }
            }
        }

        private FileInfo _SelectedTemplateFile;
        public FileInfo SelectedTemplateFile {
            get { return _SelectedTemplateFile; }
            set {
                if (value != _SelectedTemplateFile) {
                    _SelectedTemplateFile = value;
                    OnPropertyChanged(nameof(SelectedTemplateFile));
                }
            }
        }

        private ReportingMode? _ReportingMode = null;

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
            TemplateDirAndFiles = root?.Children.ToList() ?? [];
        }
    }
}
