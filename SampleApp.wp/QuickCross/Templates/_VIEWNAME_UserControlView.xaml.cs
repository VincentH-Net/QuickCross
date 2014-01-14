#if TEMPLATE // To add a usercontrol view class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "New-View <name> -ViewType UserControl". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ and _VIEWNAME_ with the application and view names.
using System.Windows.Controls;
using QuickCrossLibrary.Templates;

namespace QuickCross.Templates
{
    public partial class _VIEWNAME_View : UserControl
    {
        public _VIEWNAME_View()
        {
            InitializeComponent();
            if (_APPNAME_Application.Instance != null) DataContext = _APPNAME_Application.Instance._VIEWNAME_ViewModel;
        }
    }
}
#endif // TEMPLATE