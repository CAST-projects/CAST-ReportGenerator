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
using CastReporting.Reporting.Core.ReportingModel;
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

        private ReportingMode _ReportingMode = ReportingMode.Application;
        public ReportingMode ReportingMode {
            get => _ReportingMode;
            set {
                if (value == _ReportingMode) return;
                _ReportingMode = value;
                OnPropertyChanged(nameof(ReportingMode));
            }
        }

        public bool IsDataFilledIn {
            get {
                if (Templates.SelectedTemplateFile == null) return false;
                bool result = false;
                switch(ReportingMode) {
                    case ReportingMode.Application:
                        result = (ImagingContext.SelectedApplication != null && ImagingContext.SelectedSnapshot != null)
                            || (HighlightContext.SelectedApplication != null && HighlightContext.SelectedSnapshot != null);
                        break;
                    case ReportingMode.Portfolio:
                        result = (ImagingContext.SelectedTag != null);
                        break;
                }
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
            Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;

            // Set culture for the new thread
            if (!string.IsNullOrEmpty(Setting?.ReportingParameter.CultureName)) {
                var culture = new CultureInfo(Setting.ReportingParameter.CultureName);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            const double progressStep = 25; // 100 / 4

            switch (ReportingMode) {
                case ReportingMode.Application: {
                        Stopwatch stopWatchStep = new Stopwatch();
                        Stopwatch stopWatchGlobal = new Stopwatch();

                        try {
                            stopWatchGlobal.Start();
                            dispatcher?.Invoke(DispatcherPriority.Normal, MessageManager.SetBusyMode, true);

                            // Populate app data from Imaging
                            ImagingContext.LoadApplicationData(progressStep);

                            // Launch generation               
                            stopWatchStep.Restart();
                            GenerateApplicationReport();
                            stopWatchStep.Stop();
                            dispatcher?.Invoke(DispatcherPriority.Normal, MessageManager.OnStepDone, progressStep, Messages.msgReportGenerated, stopWatchStep.Elapsed);

                            // Show final message
                            dispatcher?.Invoke(DispatcherPriority.Normal, MessageManager.OnReportGenerated, ReportFileName, stopWatchGlobal.Elapsed);

                        } catch (System.Net.WebException webEx) {
                            LogHelper.LogErrorFormat($"Execution error: {webEx.Message}");
                            dispatcher?.Invoke(DispatcherPriority.Normal, MessageManager.OnStepDone, progressStep,
                                $"{Messages.msgErrorGeneratingReport} - {webEx.Message} - {Messages.msgReportErrorNoRestAPI}", stopWatchStep.Elapsed);
                            dispatcher?.Invoke(DispatcherPriority.Normal, WorkerThreadException, new CastReportingException(webEx.Message, webEx.InnerException));
                        } catch (Exception ex) {
                            dispatcher?.Invoke(DispatcherPriority.Normal, MessageManager.OnStepDone, progressStep,
                                $"{Messages.msgErrorGeneratingReport} - {ex.Message}", stopWatchStep.Elapsed);
                        } finally {
                            stopWatchGlobal.Stop();
                            // Unlock the screen   
                            dispatcher?.Invoke(DispatcherPriority.Normal, MessageManager.SetBusyMode, false);
                        }
                    }
                    break;

                case ReportingMode.Portfolio: {
                        List<Domain.Application> Apps = new List<Domain.Application>();
                        List<Snapshot> Snapshots = new List<Snapshot>();

                        Stopwatch stopWatchStep = new Stopwatch();
                        Stopwatch stopWatchGlobal = new Stopwatch();

                        try {
                            stopWatchGlobal.Start();
                            dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), true);

                            // GetActive Connection           
                            ImagingContext.ActiveConnection = Setting?.GetActiveConnection(); // DMA: why is this not using the ActiveConnection from this ViewModel?

                            // Get list of domains
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

                                string[] SnapsToIgnore = null;
                                //Get result for the Portfolio               
                                stopWatchStep.Restart();
                                string[] AppsToIgnorePortfolioResult = PortfolioBLL.BuildPortfolioResult(ImagingContext.ActiveConnection, SelectedApps);
                                stopWatchStep.Stop();
                                dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgBuildPortfolioResults, stopWatchStep.Elapsed);

                                Domain.Application[] N_SelectedApps = SelectedApps
                                    .Where(app => !AppsToIgnorePortfolioResult.Contains(app.Name))
                                    .ToArray();

                                // Get result for each app's latest snapshot
                                if (Snapshots != null) {
                                    Snapshot[] SelectedApps_Snapshots = Snapshots.ToArray<Snapshot>();

                                    // Get result for all snapshots in Portfolio               
                                    stopWatchStep.Restart();
                                    SnapsToIgnore = PortfolioSnapshotsBLL.BuildSnapshotResult(ImagingContext.ActiveConnection, SelectedApps_Snapshots, true);
                                    stopWatchStep.Stop();
                                    dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgBuildPortfSnapshotsResults,
                                        stopWatchStep.Elapsed);

                                    Snapshot[] N_SelectedApps_Snapshots = SelectedApps_Snapshots
                                        .Where(snap => !SnapsToIgnore.Contains(snap.Href))
                                        .ToArray();

                                    // Launch generaion               
                                    stopWatchStep.Restart();
                                    GeneratePortfolioReport(N_SelectedApps, N_SelectedApps_Snapshots, AppsToIgnorePortfolioResult, SnapsToIgnore);
                                    stopWatchStep.Stop();
                                }

                                StringBuilder sb = new StringBuilder();

                                if (AppsToIgnorePortfolioResult.Length > 0 || SnapsToIgnore?.Length > 0) {
                                    sb.Append(Messages.msgIgnoredAppSnaps);

                                    if (AppsToIgnorePortfolioResult.Length > 0) {
                                        AppsToIgnorePortfolioResult = AppsToIgnorePortfolioResult.Distinct().ToArray();
                                        sb.Append(' ');
                                        sb.Append(Messages.msgIgnoredApplications);
                                        sb.Append(' ');
                                        sb.Append(string.Join(", ", AppsToIgnorePortfolioResult));
                                        sb.Append('.');
                                    }

                                    if (SnapsToIgnore?.Length > 0) {
                                        SnapsToIgnore = SnapsToIgnore.Distinct().ToArray();
                                        sb.Append(' ');
                                        sb.Append(Messages.msgIgnoredSnapshots);
                                        sb.Append(' ');
                                        sb.Append(string.Join(", ", SnapsToIgnore.Select(s => $"{ImagingContext.ActiveConnection?.Url}/{s}")));
                                        sb.Append('.');
                                    }
                                    dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, sb + "", null);
                                }

                                dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgReportGenerated, stopWatchStep.Elapsed);


                                //Show final message
                                dispatcher?.Invoke(DispatcherPriority.Normal, new Action<string, TimeSpan>(MessageManager.OnReportGenerated), ReportFileName, stopWatchGlobal.Elapsed);
                            } else {
                                //Show final message and unlock the screen   
                                dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep, Messages.msgErrorGeneratingReport + " - " + Messages.msgReportErrorNoAAD, stopWatchGlobal.Elapsed);
                            }
                        } catch (System.Net.WebException webEx) {
                            LogHelper.LogErrorFormat($"Request URL '' - Error execution : {webEx.Message}");
                            dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep,
                                Messages.msgErrorGeneratingReport + " - " + webEx.Message + " - " + Messages.msgReportErrorNoRestAPI, stopWatchStep.Elapsed);
                            dispatcher?.Invoke(DispatcherPriority.Normal, new Action<CastReportingException>(WorkerThreadException), new CastReportingException(webEx.Message, webEx.InnerException));
                        } catch (Exception ex) {
                            dispatcher?.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(MessageManager.OnStepDone), progressStep,
                                Messages.msgErrorGeneratingReport + " - " + ex.Message, stopWatchStep.Elapsed);
                        } finally {
                            stopWatchGlobal.Stop();
                            // Unlock the screen   
                            dispatcher?.Invoke(DispatcherPriority.Normal, new Action<bool>(MessageManager.SetBusyMode), false);
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
                    hlData = HighlightContext.LoadApplicationData();
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
