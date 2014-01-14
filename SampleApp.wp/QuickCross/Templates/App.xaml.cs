#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using System.Windows;
using System.Windows.Controls;
using QuickCrossLibrary.Templates;

namespace QuickCross.Templates
{
    public partial class App : Application
    {
        public static _APPNAME_Application Ensure_APPNAME_Application(Frame navigationContext)
        {
            _APPNAME_Navigator.Instance.NavigationContext = navigationContext;
            return _APPNAME_Application.Instance ?? new _APPNAME_Application(_APPNAME_Navigator.Instance);
        }

        // TODO: Add the following code to Application_Launching:
        //    Ensure_APPNAME_Application(RootFrame).ContinueToMain();

        // TODO: Add the following code to Application_Activated:
        //    Ensure_APPNAME_Application(RootFrame);
    }
}
#endif // TEMPLATE