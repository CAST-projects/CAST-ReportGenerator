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


using Cast.Util;
using Cast.Util.Log;
using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.Reporting.Builder;
using CastReporting.Reporting.Highlight.ReportingModel;
using CastReporting.Reporting.ReportingModel;
using CastReporting.UI.WPF.Core.Common;
using CastReporting.UI.WPF.Core.Resources.Languages;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
//using Microsoft.Office.Interop.Excel;

// ReSharper disable InconsistentNaming

namespace CastReporting.UI.WPF.Core.ViewModel
{
    /// <summary>
    ///  Implement the ViewModel for the reporting page
    /// </summary>
    public class ReportingVM : ViewModelBase {
        public ReportingVM() {
            GenerateCommand = new CommandHandler(_ => GenerateReport(), null);
            ImagingContext.PropertyChanged += UpdateIsDataFilledIn;
            HighlightContext.PropertyChanged += UpdateIsDataFilledIn;
            Templates.PropertyChanged += UpdateIsDataFilledIn;
        }

        public ICommand GenerateCommand { get; init; }

        public ImagingContextVM ImagingContext { get; } = new ImagingContextVM();
        public HighlightContextVM HighlightContext { get; } = new HighlightContextVM();
        public TemplatesVM Templates { get; } = new TemplatesVM();

        public override IMessageManager MessageManager {
            get => base.MessageManager;
            set {
                base.MessageManager = value;
                ImagingContext.MessageManager = value;
                HighlightContext.MessageManager = value;
                Templates.MessageManager = value;
            }
        }

        /// <summary>
        /// File name
        /// </summary>
        private string _ReportFileName;
        public string ReportFileName {
            get => _ReportFileName;
            set {
                if (value == _ReportFileName) return;
                _ReportFileName = value;
                OnPropertyChanged(nameof(ReportFileName));
            }
        }

        private ReportingMode _ReportingMode=ReportingMode.Application;
        public ReportingMode ReportingMode {
            get => _ReportingMode;
            set {
                if (value == _ReportingMode) return;
                _ReportingMode = value;
                OnPropertyChanged(nameof(ReportingMode));
                OnPropertyChanged(nameof(SelectedTab));
                Templates.RefreshTemplates(_ReportingMode);
                OnPropertyChanged(nameof(IsDataFilledIn));
            }
        }

        /// <summary>
        /// Selected tab
        /// </summary>
        public int SelectedTab {
            get => (int)ReportingMode;
            set {
                switch(value) {
                    case 0:
                        ReportingMode = ReportingMode.Application;
                        break;
                    case 1:
                        ReportingMode = ReportingMode.Portfolio;
                        break;
                    // other values should never happen
                }
            }
        }

        public bool IsDataFilledIn {
            get {
                if (Templates.SelectedTemplateFile == null) return false;
                bool result = false;
                switch(ReportingMode) {
                    case ReportingMode.Application:
                        result=((ImagingContext.SelectedApplication != null && ImagingContext.SelectedSnapshot != null)
                            || HighlightContext.SelectedApplication != null);
                        break;
                    case ReportingMode.Portfolio:
                        result=(ImagingContext.SelectedTag != null);
                        break;
                }
                    Debug.WriteLine($"{nameof(IsDataFilledIn)} = {result}");
                return result;
            }
            set { /* ignored */ }
        }

        private void UpdateIsDataFilledIn(object sender, PropertyChangedEventArgs e) {
            OnPropertyChanged(nameof(IsDataFilledIn));
        }

        /// <summary>
        /// Implement Command that Launch the report generation 
        /// </summary>
        private void GenerateReport() {
            if (string.IsNullOrEmpty(ReportFileName)) return;
            BackgroundWorker BackgroundWorker = new BackgroundWorker();
            BackgroundWorker.DoWork += BackgroundWorkerDoWork;
            BackgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e) {
            System.Windows.Application repGen = System.Windows.Application.Current;

            const double progressStep = 25; // 100 / 4
            Stopwatch stopWatchStep = new Stopwatch();
            Stopwatch stopWatchGlobal = new Stopwatch();

            switch (ReportingMode) {
                case ReportingMode.Application: {

                        try {
                            stopWatchGlobal.Start();
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), true);

                            //Set culture for the new thread
                            if (!string.IsNullOrEmpty(Setting.ReportingParameter.CultureName)) {
                                var culture = new CultureInfo(Setting.ReportingParameter.CultureName);
                                Thread.CurrentThread.CurrentCulture = culture;
                                Thread.CurrentThread.CurrentUICulture = culture;
                            }

                            if (ImagingContext.SelectedApplication?.Application != null) {
                                //Get result for the Application               
                                stopWatchStep.Restart();
                                ApplicationBLL.BuildApplicationResult(ImagingContext.ActiveConnection, ImagingContext.SelectedApplication.Application);
                                stopWatchStep.Stop();
                                repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgBuildApplicationResult, stopWatchStep.Elapsed);


                                //Get result for the selected snapshot                
                                stopWatchStep.Restart();
                                SnapshotBLL.BuildSnapshotResult(ImagingContext.ActiveConnection, ImagingContext.SelectedSnapshot, true);
                                stopWatchStep.Stop();
                                repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgBuildSnapshotResult, stopWatchStep.Elapsed);


                                //Get result for the previous snapshot                
                                if (ImagingContext.PreviousSnapshot != null) {
                                    stopWatchStep.Restart();
                                    SnapshotBLL.BuildSnapshotResult(ImagingContext.ActiveConnection, ImagingContext.PreviousSnapshot, false);
                                    stopWatchStep.Stop();

                                    repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgBuildPreviousSnapshotResult, stopWatchStep.Elapsed);
                                }
                            }

                            if (HighlightContext.SelectedApplication.Application != null) {
                                // get results from Highlight
                            }

                            //Launch generaion               
                            stopWatchStep.Restart();
                            GenerateApplicationReport();
                            stopWatchStep.Stop();

                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgReportGenerated, stopWatchStep.Elapsed);


                            //Show final message and unlock the screen   
                            stopWatchGlobal.Stop();
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<string, TimeSpan>(MessageManager.OnReportGenerated), ReportFileName, stopWatchGlobal.Elapsed);
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), false);
                        } catch (System.Net.WebException webEx) {
                            LogHelper.LogErrorFormat
                            ("Request URL '{0}' - Error execution :  {1}"
                                , ""
                                , webEx.Message
                            );

                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep,
                                Messages.msgErrorGeneratingReport + " - " + webEx.Message + " - " + Messages.msgReportErrorNoRestAPI, stopWatchStep.Elapsed);
                            stopWatchGlobal.Stop();
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), false);
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<CastReportingException>(WorkerThreadException), new CastReportingException(webEx.Message, webEx.InnerException));
                        } catch (Exception ex) {
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep,
                                Messages.msgErrorGeneratingReport + " - " + ex.Message, stopWatchStep.Elapsed);
                            stopWatchGlobal.Stop();
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), false);
                        }
                    }
                    break;

                case ReportingMode.Portfolio: {
                        List<Domain.Application> Apps = new List<Domain.Application>();
                        List<Snapshot> Snapshots = new List<Snapshot>();

                        try {
                            stopWatchGlobal.Start();
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), true);


                            //GetActive Connection           
                            ImagingContext.ActiveConnection = Setting?.GetActiveConnection();

                            //Get list of domains
                            if (ImagingContext.ActiveConnection != null) {
                                try {
                                    using (CastDomainBLL castDomainBLL = new CastDomainBLL(ImagingContext.ActiveConnection)) {
                                        Apps = castDomainBLL.GetCommonTaggedApplications(ImagingContext.SelectedTag, ImagingContext.SelectedCategory);
                                    }
                                } catch (Exception ex) {
                                    MessageManager.OnErrorOccured(ex);
                                }
                            }

                            if (Apps == null) return;


                            if (Apps.Count > 0) {
                                Domain.Application[] SelectedApps = Apps.ToArray<Domain.Application>();

                                //Set culture for the new thread
                                if (!string.IsNullOrEmpty(Setting?.ReportingParameter.CultureName)) {
                                    var culture = new CultureInfo(Setting.ReportingParameter.CultureName);
                                    Thread.CurrentThread.CurrentCulture = culture;
                                    Thread.CurrentThread.CurrentUICulture = culture;
                                }
                                string[] SnapsToIgnore = null;
                                //Get result for the Portfolio               
                                stopWatchStep.Restart();
                                string[] AppsToIgnorePortfolioResult = PortfolioBLL.BuildPortfolioResult(   ImagingContext. ActiveConnection, SelectedApps);
                                stopWatchStep.Stop();
                                repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgBuildPortfolioResults, stopWatchStep.Elapsed);

                                List<Domain.Application> N_Apps = new List<Domain.Application>();
                                //Remove from Array the Ignored Apps
                                foreach (Domain.Application app in SelectedApps) {
                                    int intAppYes = 0;
                                    foreach (string s in AppsToIgnorePortfolioResult) {
                                        if (s == app.Name) {
                                            intAppYes = 1;
                                            break;
                                        }
                                        intAppYes = 0;
                                    }

                                    if (intAppYes == 0) {
                                        N_Apps.Add(app);
                                    }
                                }

                                Domain.Application[] N_SelectedApps = N_Apps.ToArray();

                                List<Snapshot> N_Snaps = new List<Snapshot>();
                                //Get result for each app's latest snapshot
                                if (Snapshots != null) {
                                    Snapshot[] SelectedApps_Snapshots = Snapshots.ToArray<Snapshot>();

                                    //Get result for all snapshots in Portfolio               
                                    stopWatchStep.Restart();
                                    SnapsToIgnore = PortfolioSnapshotsBLL.BuildSnapshotResult(ImagingContext.ActiveConnection, SelectedApps_Snapshots, true);
                                    stopWatchStep.Stop();
                                    repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgBuildPortfSnapshotsResults,
                                        stopWatchStep.Elapsed);

                                    foreach (Snapshot snap in SelectedApps_Snapshots) {
                                        int intRemoveYes = 0;
                                        foreach (string s in SnapsToIgnore) {
                                            if (s == snap.Href) {
                                                intRemoveYes = 1;
                                                break;
                                            }
                                            intRemoveYes = 0;
                                        }
                                        if (intRemoveYes == 0) {
                                            N_Snaps.Add(snap);
                                        }
                                    }

                                    Snapshot[] N_SelectedApps_Snapshots = N_Snaps.ToArray();


                                    //Launch generaion               
                                    stopWatchStep.Restart();
                                    GeneratePortfolioReport(N_SelectedApps, N_SelectedApps_Snapshots, AppsToIgnorePortfolioResult, SnapsToIgnore);
                                    stopWatchStep.Stop();
                                }


                                StringBuilder sb = new StringBuilder();

                                if (AppsToIgnorePortfolioResult.Length > 0 || SnapsToIgnore?.Length > 0) {
                                    sb.Append(Messages.msgIgnoredAppSnaps);

                                    if (AppsToIgnorePortfolioResult.Length > 0) {
                                        AppsToIgnorePortfolioResult = AppsToIgnorePortfolioResult.Distinct().ToArray();
                                        sb.Append(Messages.msgIgnoredApplications);
                                        sb.Append(' ');
                                        for (int i = 0; i < AppsToIgnorePortfolioResult.Length; i++) {
                                            if (i > 0) {
                                                sb.Append(',');
                                            }
                                            sb.Append(AppsToIgnorePortfolioResult[i]);
                                        }
                                    }

                                    if (SnapsToIgnore?.Length > 0) {
                                        SnapsToIgnore = SnapsToIgnore.Distinct().ToArray();
                                        sb.Append(Messages.msgIgnoredSnapshots);
                                        sb.Append(' ');
                                        for (int i = 0; i < SnapsToIgnore.Length; i++) {
                                            if (i > 0) {
                                                sb.Append(',');
                                            }
                                            sb.Append(ImagingContext.ActiveConnection?.Url);
                                            sb.Append('/');
                                            sb.Append(SnapsToIgnore[i]);
                                        }
                                    }
                                    repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, sb + "", null);
                                }


                                repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgReportGenerated, stopWatchStep.Elapsed);


                                //Show final message and unlock the screen   
                                stopWatchGlobal.Stop();
                                repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<string, TimeSpan>(MessageManager.OnReportGenerated), ReportFileName, stopWatchGlobal.Elapsed);
                            } else {
                                //Show final message and unlock the screen   
                                stopWatchGlobal.Stop();
                                repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgErrorGeneratingReport + " - " + Messages.msgReportErrorNoAAD, stopWatchGlobal.Elapsed);
                            }

                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), false);
                        } catch (System.Net.WebException webEx) {
                            LogHelper.LogErrorFormat($"Request URL '' - Error execution : {webEx.Message}");

                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep,
                                Messages.msgErrorGeneratingReport + " - " + webEx.Message + " - " + Messages.msgReportErrorNoRestAPI, stopWatchStep.Elapsed);
                            stopWatchGlobal.Stop();
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), false);
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<CastReportingException>(WorkerThreadException), new CastReportingException(webEx.Message, webEx.InnerException));
                        } catch (Exception ex) {
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep,
                                Messages.msgErrorGeneratingReport + " - " + ex.Message, stopWatchStep.Elapsed);
                            stopWatchGlobal.Stop();
                            repGen.Dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), false);
                        }
                    }
                    break;
            }
        }

        private void GenerateApplicationReport() {
            string tmpReportFile = string.Empty;
            string tmpReportFileFlexi = string.Empty;

            try {

                //Create temporary report
                string workDirectory = SettingsBLL.GetApplicationPath();
                tmpReportFile = PathUtil.CreateTempCopy(workDirectory, Templates. SelectedTemplateFile.FullName);
                if (tmpReportFile.Contains(".xlsx")) {
                    tmpReportFileFlexi = PathUtil.CreateTempCopyFlexi(workDirectory, Templates.SelectedTemplateFile.FullName);
                }

                ImagingData imgData=null;
                if (ImagingContext.ActiveConnection != null && ImagingContext.SelectedApplication != null && ImagingContext.SelectedSnapshot != null) {
                    imgData = new ImagingData {
                        Application = ImagingContext.SelectedApplication.Application,
                        CurrentSnapshot = ImagingContext.SelectedSnapshot,
                        PreviousSnapshot = ImagingContext.PreviousSnapshot,
                        Parameter = Setting.ReportingParameter,
                        RuleExplorer = new RuleBLL(ImagingContext.ActiveConnection),
                        SnapshotExplorer = new SnapshotBLL(ImagingContext.ActiveConnection, ImagingContext.SelectedSnapshot),
                        CurrencySymbol = "$",
                        ServerVersion = CommonBLL.GetServiceVersion(ImagingContext.ActiveConnection)
                    };
                    }

                HighlightData hlData=null;
                if (HighlightContext.ActiveConnection!=null && HighlightContext.SelectedApplication!=null) {
                    hlData = new HighlightData { 
                        // TODO
                    };
                }

                //Build report
                ReportData reportData = new ReportData {
                    FileName = tmpReportFile,
                    ImagingData = imgData,
                    HighlightData = hlData,
                };

                using (IDocumentBuilder docBuilder = BuilderFactory.CreateBuilder(reportData, tmpReportFileFlexi)) {
                    docBuilder.BuildDocument();
                }

                if (tmpReportFile.Contains(".xlsx")) {
                    File.Delete(tmpReportFile);
                    tmpReportFile = tmpReportFileFlexi;
                }

                ConvertToPdfIfNeeded(tmpReportFile);
            } catch (Exception) {
                ReportFileName = string.Empty;
                throw;
            } finally {
                if (!string.IsNullOrEmpty(tmpReportFile)) File.Delete(tmpReportFile);
            }
        }

        private void GeneratePortfolioReport(Domain.Application[] ApplicationsArray, Snapshot[] ApplicationsSnapshots, string[] IgnoredApps, string[] IgnoredSnapshots) {
            string tmpReportFile = string.Empty;
            string tmpReportFileFlexi = string.Empty;

            try {

                //Create temporary report
                string workDirectory = SettingsBLL.GetApplicationPath();
                tmpReportFile = PathUtil.CreateTempCopy(workDirectory, Templates.SelectedTemplateFile.FullName);
                if (tmpReportFile.Contains(".xlsx")) {
                    tmpReportFileFlexi = PathUtil.CreateTempCopyFlexi(workDirectory, Templates.SelectedTemplateFile.FullName);
                }

                //Build report
                ReportData reportData = new ReportData {
                    FileName = tmpReportFile,
                    ImagingData = new ImagingData {
                        Application = null,
                        CurrentSnapshot = null,
                        PreviousSnapshot = null,
                        Parameter = Setting.ReportingParameter,
                        RuleExplorer = new RuleBLL(ImagingContext.ActiveConnection),
                        SnapshotExplorer = new SnapshotBLL(ImagingContext.ActiveConnection, ImagingContext.SelectedSnapshot),
                        CurrencySymbol = "$",
                        ServerVersion = CommonBLL.GetServiceVersion(ImagingContext.ActiveConnection),
                        Applications = ApplicationsArray,
                        Category = ImagingContext.SelectedCategory,
                        Tag = ImagingContext.SelectedTag,
                        Snapshots = ApplicationsSnapshots,
                        IgnoresApplications = IgnoredApps,
                        IgnoresSnapshots = IgnoredSnapshots
                    },
                    HighlightData = null,// not supported yet
                };

                using (IDocumentBuilder docBuilder = BuilderFactory.CreateBuilder(reportData, tmpReportFileFlexi)) {
                    docBuilder.BuildDocument();
                }

                if (tmpReportFile.Contains(".xlsx")) {
                    File.Delete(tmpReportFile);
                    tmpReportFile = tmpReportFileFlexi;
                }

                ConvertToPdfIfNeeded(tmpReportFile);
            } catch (Exception) {
                ReportFileName = string.Empty;
                throw;
            } finally {
                if (!string.IsNullOrEmpty(tmpReportFile)) File.Delete(tmpReportFile);
            }
        }

        private void ConvertToPdfIfNeeded(string tmpReportFile) {
            // convert docx or pptx to pdf
            if (ReportFileName.Contains(".pdf")) {
                if (tmpReportFile.Contains(".docx")) {
                    try {
                        Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                        Document wordDocument = appWord.Documents.Open(tmpReportFile);
                        wordDocument.ExportAsFixedFormat(ReportFileName, WdExportFormat.wdExportFormatPDF);
                        wordDocument.Close();
                        appWord.Quit();
                    } catch (Exception) {
                        // Error if office not installed, then do not save as pdf
                        ReportFileName = ReportFileName.Replace(".pdf",  Templates.SelectedTemplateFile.Extension);
                        File.Copy(tmpReportFile, ReportFileName, true);
                    }
                } else if (tmpReportFile.Contains(".pptx")) {
                    try {
                        Microsoft.Office.Interop.PowerPoint.Application appPowerpoint = new Microsoft.Office.Interop.PowerPoint.Application();
                        Presentation appPres = appPowerpoint.Presentations.Open(tmpReportFile);
                        appPres.ExportAsFixedFormat(ReportFileName, PpFixedFormatType.ppFixedFormatTypePDF);
                        appPres.Close();
                        appPowerpoint.Quit();
                    } catch (Exception) {
                        // Error if office not installed, then do not save as pdf
                        ReportFileName = ReportFileName.Replace(".pdf", Templates.SelectedTemplateFile.Extension);
                        File.Copy(tmpReportFile, ReportFileName, true);
                    }
                }
                  /* Reports too ugly and unusable when converted from excel to pdf
                       * else if (tmpReportFile.Contains(".xlsx"))
                      {
                          Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
                          Workbook excelDoc = appExcel.Workbooks.Open(tmpReportFile);
                          excelDoc.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, ReportFileName);
                          excelDoc.Close();
                          appExcel.Quit();
                      }
                      */
                  else {
                    string report = ReportFileName.Replace(".pdf", Templates. SelectedTemplateFile.Extension);
                    File.Copy(tmpReportFile, report, true);
                }
            } else {
                //Copy report file to the selected destination
                File.Copy(tmpReportFile, ReportFileName, true);
            }
        }
    }
}
