using CastReporting.UI.WPF.Core.Resources.Languages;
using System.Reflection;
using System.Windows.Controls;

namespace CastReporting.UI.WPF.Core.View.Pages
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();
        }

        // ReSharper disable once InconsistentNaming
        public static string lblAboutVersion => string.Format(Messages.lblAboutVersion, Assembly.GetExecutingAssembly().GetName().Version);
    }
}
