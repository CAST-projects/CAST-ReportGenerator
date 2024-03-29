namespace CastReporting.HL.Reporting.Core;

public class HLReporting
{
    public static void ForceAssemblyToLoad() {
        // This assembly contains only dynamically loaded artefacts.
        // By calling this method from the main program, it makes sure that the assembly is loaded
        // and that blocks will be resolved when generating the report.
    }
}
