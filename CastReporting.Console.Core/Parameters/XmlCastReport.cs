using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CastReporting.Console.Argument
{
    /// <summary>
    /// XmlCastReport Class
    /// </summary>
    [Serializable]
    [XmlRoot("castReport", Namespace = "http://tempuri.org/CastReportSchema.xsd", IsNullable = false)]
    public sealed class XmlCastReport
    {
        #region Properties

        /// <summary>
        /// ExtendPackId
        /// </summary>
        [XmlElement("extendpackid")]
        public XmlTagName ExtendPackId { get; set; }

        /// <summary>
        /// ExtendVersionId
        /// </summary>
        [XmlElement("extendversionid")]
        public XmlTagName ExtendVersionId { get; set; }

        /// <summary>
        /// ExtendUrl
        /// </summary>
        [XmlElement("extendurl")]
        public XmlTagName ExtendUrl { get; set; }

        /// <summary>
        /// ExtendKey
        /// </summary>
        [XmlElement("extendkey")]
        public XmlTagName ExtendKey { get; set; }

        /// <summary>
        /// ReportType
        /// </summary>
        [XmlElement("reporttype")]
        public XmlTagName ReportType { get; set; }

        /// <summary>
        /// Webservice
        /// </summary>
        [XmlElement("webservice")]
        public XmlTagName Webservice { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [XmlElement("username")]
        public XmlTagName Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [XmlElement("password")]
        public XmlTagName Password { get; set; }

        /// <summary>
        /// API Key
        /// </summary>
        [XmlElement("apikey")]
        public XmlTagName ApiKey { get; set; }

        /// <summary>
        /// Application
        /// </summary>
        [XmlElement("application")]
        public XmlTagName Application { get; set; }

        /// <summary>
        /// Template"
        /// </summary>
        [XmlElement("template")]
        public XmlTagName Template { get; set; }

        /// <summary>
        /// Database
        /// </summary>
        [XmlElement("database")]
        public XmlTagName Database { get; set; }

        /// <summary>
        /// Database
        /// </summary>
        [XmlElement("domain")]
        public XmlTagName Domain { get; set; }

        /// <summary>
        /// Snapshots
        /// </summary>
        [XmlElement("snapshot")]
        public XmlSnapshot Snapshot { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        [XmlElement("file")]
        public XmlTagName File { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        [XmlElement("category")]
        public XmlTagName Category { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        [XmlElement("tag")]
        public XmlTagName Tag { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        [XmlElement("culture")]
        public XmlTagName Culture { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Save XML Cast Report
        /// </summary>
        /// <param name="pOutputPath"></param>
        public void SaveXML(string pOutputPath)
        {
            if (!isValidPath(pOutputPath, false))
                throw new ArgumentNullException("pOutputPath");
            using (TextWriter tr = new StreamWriter(pOutputPath, false, Encoding.UTF8))
            {
                XmlSerializer sr = new XmlSerializer(typeof(XmlCastReport));
                sr.Serialize(tr, this);
            }
        }

        /// <summary>
        /// Load XML Cast Report
        /// </summary>
        /// <param name="pInputPath"></param>
        public static XmlCastReport LoadXML(string pInputPath)
        {
            if (!isValidPath(pInputPath, true))
                throw new ArgumentNullException("pInputPath");

            XmlCastReport report;
            using (TextReader tr = new StreamReader(pInputPath, Encoding.UTF8))
            {
                XmlSerializer sr = new XmlSerializer(typeof(XmlCastReport));
                report = sr.Deserialize(tr) as XmlCastReport;
            }
            return report;
        }

        private static bool isValidPath(String fileName, bool checkExists)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            FileInfo fi = null;
            try
            {
                fi = new System.IO.FileInfo(fileName);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }
            if (ReferenceEquals(fi, null))
            {
                // file name is not valid
                return false;
            }
            else
            {
                // file name is valid... May check for existence by calling fi.Exists.
                if (!checkExists) return true;

                if (fi.Exists)
                {
                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// Check 
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public bool Check()
        {
            if (!string.IsNullOrEmpty(ExtendPackId?.Name))
            {
                if (string.IsNullOrEmpty(ExtendUrl?.Name)) return false;
                if (string.IsNullOrEmpty(ExtendKey?.Name)) return false;
                return true;
            }

            if (string.IsNullOrEmpty(Webservice?.Name))
                return false;
            if (string.IsNullOrEmpty(Template?.Name))
                return false;
            if (string.IsNullOrEmpty(Username?.Name))
                return false;
            if (string.IsNullOrEmpty(Password?.Name))
                return false;
            return true;
        }

        /// <summary>
        /// Debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                // ReSharper disable once UseStringInterpolation
                return string.Format
                    ("Cast Report '{0}' to DB '{1}' and template '{2}'"
                    , !string.IsNullOrEmpty(Application?.Name) ? Application.Name : "?"
                    , !string.IsNullOrEmpty(Database?.Name) ? Database.Name : "?"
                    , !string.IsNullOrEmpty(Template?.Name) ? Template.Name : "?"
                    );
            }
            catch
            {
                return base.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is XmlCastReport report &&
                   EqualityComparer<XmlTagName>.Default.Equals(ExtendPackId, report.ExtendPackId) &&
                   EqualityComparer<XmlTagName>.Default.Equals(ExtendVersionId, report.ExtendVersionId) &&
                   EqualityComparer<XmlTagName>.Default.Equals(ExtendUrl, report.ExtendUrl) &&
                   EqualityComparer<XmlTagName>.Default.Equals(ExtendKey, report.ExtendKey) &&
                   EqualityComparer<XmlTagName>.Default.Equals(ReportType, report.ReportType) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Webservice, report.Webservice) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Username, report.Username) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Password, report.Password) &&
                   EqualityComparer<XmlTagName>.Default.Equals(ApiKey, report.ApiKey) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Application, report.Application) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Template, report.Template) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Database, report.Database) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Domain, report.Domain) &&
                   EqualityComparer<XmlSnapshot>.Default.Equals(Snapshot, report.Snapshot) &&
                   EqualityComparer<XmlTagName>.Default.Equals(File, report.File) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Category, report.Category) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Tag, report.Tag) &&
                   EqualityComparer<XmlTagName>.Default.Equals(Culture, report.Culture);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(ExtendPackId);
            hash.Add(ExtendVersionId);
            hash.Add(ExtendUrl);
            hash.Add(ExtendKey);
            hash.Add(ReportType);
            hash.Add(Webservice);
            hash.Add(Username);
            hash.Add(Password);
            hash.Add(ApiKey);
            hash.Add(Application);
            hash.Add(Template);
            hash.Add(Database);
            hash.Add(Domain);
            hash.Add(Snapshot);
            hash.Add(File);
            hash.Add(Category);
            hash.Add(Tag);
            hash.Add(Culture);
            return hash.ToHashCode();
        }
        #endregion
    }
}
