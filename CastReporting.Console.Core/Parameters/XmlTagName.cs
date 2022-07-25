using System;
using System.Xml.Serialization;

namespace CastReporting.Console.Argument
{
    /// <summary>
    /// XmlTagName Class
    /// </summary>
    [Serializable]
    public class XmlTagName
    {
        #region Properties

        /// <summary>
        /// Name
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return obj is XmlTagName name &&
                   Name == name.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        #endregion

        #region Method

        /// <summary>
        /// Debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }


        #endregion
    }
}
