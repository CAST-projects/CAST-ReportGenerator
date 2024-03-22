using System.Windows.Input;

namespace CastReporting.UI.WPF.Core.Commands
{
    public static class WebServiceCommand
    {
        public static RoutedCommand ActivateWebService { get; set; }
        public static RoutedCommand ActivateHLWebService { get; set; }

        static WebServiceCommand()
        {
            ActivateWebService = new RoutedCommand();
            ActivateHLWebService = new RoutedCommand();
        }
    }
}
