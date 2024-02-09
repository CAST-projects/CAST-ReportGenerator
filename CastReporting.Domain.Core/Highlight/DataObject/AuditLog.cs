
using System.Collections.Generic;

namespace CastReporting.Domain.Highlight
{
    public class AuditLog
    {
        public string CompanyId { get; set; }
        public IList<AuditLine> Result { get; set; }
    }

}
