using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;


namespace CastReporting.Reporting.Block.Graph
{
    [Block("PF_GENERIC_GRAPH")]
    public class PortfolioGenericGraph : GraphBlock<ImagingData>
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            // false for graph component
            return PortfolioGenericContent.Content(reportData, options, false);
        }
    }
}
