using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "associatedValue")]
    public class AssociatedValueObject
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "values")]
        public ComponentWrapper[] Values { get; set; }
    }

    [DataContract(Name = "componentWrapper")]
    public class ComponentWrapper
    {
        [DataMember(Name = "component")]
        public Component Component { get; set; }
    }
}