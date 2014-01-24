#if TEMPLATE // To add a usercontrol view class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "New-View <name> -ViewType UserControl". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace CloudAuction and _VIEWNAME_ with the application and view names.
using Windows.UI.Xaml.Controls;
using CloudAuction.Shared;

namespace CloudAuction
{
    public sealed partial class _VIEWNAME_View : UserControl
    {
        public _VIEWNAME_View()
        {
            InitializeComponent();
            if (CloudAuctionApplication.Instance != null) DataContext = CloudAuctionApplication.Instance._VIEWNAME_ViewModel;
        }
    }
}
#endif // TEMPLATE