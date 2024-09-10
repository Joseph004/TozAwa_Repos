using TozawaMaui.Views.Mobile;
using TozawaMaui.Views.Desktop;

namespace TozawaMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
/* #if ANDROID || IOS
            MainPage = new NavigationPage(new MainPage());
#else
            MainPage = new NavigationPage(new DesktopStartPage());
#endif */
        }
    }
}
