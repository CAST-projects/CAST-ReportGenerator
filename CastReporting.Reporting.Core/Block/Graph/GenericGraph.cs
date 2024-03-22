using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;


namespace CastReporting.Reporting.Block.Graph
{
    [Block("GENERIC_GRAPH")]
    public class GenericGraph : ImagingGraphBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            // false for graph component
            return GenericContent.Content(reportData, options, false);
        }
    }
}
