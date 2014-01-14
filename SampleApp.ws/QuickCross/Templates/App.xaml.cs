#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using QuickCrossLibrary.Templates;

namespace QuickCross.Templates
{
    sealed partial class App : Application
    {
        public static _APPNAME_Application Ensure_APPNAME_Application(Frame navigationContext)
        {
            _APPNAME_Navigator.Instance.NavigationContext = navigationContext;
            return _APPNAME_Application.Instance ?? new _APPNAME_Application(_APPNAME_Navigator.Instance);
        }

        // TODO: Replace the if (rootFrame.Content == null) { ... } code in OnLaunched() with this:
        //    Ensure_APPNAME_Application(rootFrame);
        //    if (rootFrame.Content == null) _APPNAME_Application.Instance.ContinueToMain();
    }
}
#endif // TEMPLATE