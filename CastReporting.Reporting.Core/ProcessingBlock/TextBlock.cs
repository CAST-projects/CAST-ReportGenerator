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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OXD = DocumentFormat.OpenXml.Drawing;
using OXP = DocumentFormat.OpenXml.Presentation;
using OXW = DocumentFormat.OpenXml.Wordprocessing;

namespace CastReporting.Reporting.Builder.BlockProcessing
{
    [BlockType("TEXT")]
    public abstract class TextBlock<D> where D : IReportData
    {
        #region ABSTRACT - To be implemented by Inherited children
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract string Content(D client, Dictionary<string, string> options);
        #endregion ABSTRACT - To be implemented by Inherited children

        #region PROPERTIES

        /// <summary>
        /// Block Type Name
        /// </summary>
        public static string BlockTypeName => "TEXT";

        #endregion PROPERTIES

        #region METHODS
        public abstract D GetActualData(ReportData reportData);

        public static bool IsMatching(string blockType)
        {
            return BlockTypeName.Equals(blockType);
        }

        private static bool TryBuildContent<X>(X data, FormatType reportType ,OpenXmlPartContainer container, BlockItem block, string blockName, Dictionary<string, string> options) where X : IReportData {
            if (data != null) {
                TextBlock<X> instance = BlockHelper.GetAssociatedBlockInstance<TextBlock<X>>(blockName);
                if (instance != null) {
                    LogHelper.LogDebugFormat("Start TextBlock<{0}> generation : Type {1}", typeof(X), blockName);
                    Stopwatch treatmentWatch = Stopwatch.StartNew();
                    string content = instance.Content(data, options);
                    try {
                        if (null != content) {
                            ApplyContent(reportType, container, block, content);
                        }
                    } finally {
                        treatmentWatch.Stop();
                        LogHelper.LogDebugFormat(
                            "End TextBlock<{0}> generation ({1}) in {2} ms",
                            typeof(X), blockName, treatmentWatch.ElapsedMilliseconds.ToString()
                        );
                    }
                    return true;
                }
            }
            return false;
        }

        public static void BuildContent(ReportData client, OpenXmlPartContainer container, BlockItem block, string blockName, Dictionary<string, string> options)
        {
            if (TryBuildContent(client, client.ReportType, container, block, blockName, options)) {
                // OK, it was a generic block
            } else if (TryBuildContent(client.ImagingData, client.ReportType, container, block, blockName, options)) {
                // OK, it was a Imaging block and client.ImagingData != null
            } else if (TryBuildContent(client.HighlightData, client.ReportType, container, block, blockName, options)) {
                // OK, it was a Highlight block and client.HighlightData != null
            }
        }

        public static void ApplyContent(FormatType reportType, OpenXmlPartContainer container, BlockItem block, string content)
        {
            var contentblock = GetTextContentBlock(reportType, block);
            if (null != contentblock)
            {
                UpdateBlock(reportType, container, contentblock, content);
            }
        }

        private static void UpdateBlock(FormatType reportType, OpenXmlPartContainer container, OpenXmlElement block, string content)
        {
            switch (reportType)
            {
                case FormatType.Word: { UpdateWordBlock(container, block, content); } break;
                case FormatType.PowerPoint: { UpdatePowerPointBlock(container, block, content); } break;
                case FormatType.Excel: { UpdateExcelBlock(container, block, content); } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        // ReSharper disable once UnusedParameter.Local
        private static void UpdatePowerPointBlock(OpenXmlPartContainer container, OpenXmlElement block, string content)
        {
            OXP.Shape shape = (OXP.Shape)block.CloneNode(true);
            OXD.Run run = (OXD.Run)shape.TextBody.Descendants<OXD.Run>().First().CloneNode(true);
            run.Text = new OXD.Text(content);
            OXD.Paragraph paragraph = shape.TextBody.GetFirstChild<OXD.Paragraph>();
            paragraph.RemoveAllChildren<OXD.Run>();
            OXD.EndParagraphRunProperties endP = paragraph.GetFirstChild<OXD.EndParagraphRunProperties>();
            paragraph.InsertBefore(run, endP);
            block.Parent.ReplaceChild(shape, block);
        }
        private static void UpdateWordBlock(OpenXmlPartContainer container, OpenXmlElement block, string content)
        {
            OXW.Text new_text = new OXW.Text(content);
            if (!string.IsNullOrEmpty(content) && (char.IsWhiteSpace(content[0]) || char.IsWhiteSpace(content[content.Length - 1])))
            {
                new_text.Space = SpaceProcessingModeValues.Preserve;
            }
            OXW.Run run = new OXW.Run(new_text);
            OXW.RunProperties originalRunProp = block.Descendants<OXW.RunProperties>().FirstOrDefault();
            if (originalRunProp != null)
            {
                run.RunProperties = (OXW.RunProperties)originalRunProp.CloneNode(true);
            }
            OpenXmlElement finalBlock = run;
            if ("SdtRun" == block.Parent.GetType().Name)
            {
                // case text block in a content control
                var cbcontainer = block.Parent;
                cbcontainer?.Parent.ReplaceChild(finalBlock, cbcontainer);
            }
            else
            {
                // case text block is in a text box
                var oldTxt = block.Descendants<OXW.Run>().FirstOrDefault()?.Parent;
                oldTxt?.RemoveAllChildren();
                oldTxt?.AppendChild(finalBlock);
            }
            var docPart = container.GetPartsOfType<MainDocumentPart>().FirstOrDefault();
            if (docPart == null)
            {
                var p = container as OpenXmlPart;
                if (p != null)
                    docPart = p.GetParentParts().FirstOrDefault(_ => _ is MainDocumentPart) as MainDocumentPart;
            }
            docPart?.Document.Save();
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void UpdateExcelBlock(OpenXmlPartContainer container, OpenXmlElement block, string content)
        {
            // TODO : Finalize Excel alimentation
            throw new NotImplementedException();
        }

        private static OpenXmlElement GetTextContentBlock(FormatType reportType, BlockItem block)
        {
            switch (reportType)
            {
                case FormatType.Word:
                    var txtContent = block.OxpBlock.Descendants<OXW.SdtContentRun>().FirstOrDefault();
                    return txtContent ?? block.OxpBlock;
                // case text is in a text box
                case FormatType.PowerPoint: return block.OxpBlock;
                case FormatType.Excel:
                    return null;
                default: return null;
            }
        }
        #endregion METHODS
    }

    public abstract class ReportTextBlock : TextBlock<ReportData> {
        public override ReportData GetActualData(ReportData reportData) => reportData;
    }

    public abstract class ImagingTextBlock : TextBlock<ImagingData> {
        public override ImagingData GetActualData(ReportData reportData) => reportData.ImagingData;
    }
}
