#if TEMPLATE // To add a usercontrol view class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "New-View <name> -ViewType UserControl". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ and _VIEWNAME_ with the application and view names.
using Windows.UI.Xaml.Controls;
using QuickCrossLibrary.Templates;

namespace QuickCross.Templates
{
    public sealed partial class _VIEWNAME_View : UserControl
    {
        public _VIEWNAME_View()
        {
            InitializeComponent();
            if (_APPNAME_Application.Instance != null) DataContext = _APPNAME_Application.Instance._VIEWNAME_ViewModel;
        }

        // TODO: Find a way to call OnUserInteractionStopped() in a UserControl at the appropriate time - UserControl does not have a method equivalent to OnNavigatingFrom()
    }
}
#endif // TEMPLATE