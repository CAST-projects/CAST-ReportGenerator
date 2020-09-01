using CastReporting.BLL;
using CastReporting.UI.WPF.Core.Resources.Languages;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Input;

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

        public static string LblAboutVersion => string.Format(Messages.lblAboutVersion, Assembly.GetExecutingAssembly().GetName().Version);

        public static string AboutVersionInformation
        {
            get
            {
                if (!ExtendBLL.CheckExtendValid())
                {
                    return Messages.extendUnavailable;
                }
                else
                {
                    if (ExtendBLL.IsRGVersionLatest())
                    {
                        return Messages.rgVersionUpTodate;
                    }
                    return Messages.rgVersionOutdated + " https://extend.castsoftware.com";
                }
            }
        }

        public void TextBoxRN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string rnUrl = "https://doc.castsoftware.com/display/DOCCOM/CAST+Report+Generator+-+Release+Notes";
            openLink(rnUrl);
        }

        public void TextBoxExtend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openLink(SettingsBLL.GetExtendUrl() + "/timeline#/extension?id=com.castsoftware.aip.reportgenerator&version=latest");
        }

        private void openLink(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
        }

    }
}
