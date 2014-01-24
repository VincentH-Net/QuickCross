#if TEMPLATE // To add a page view class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "New-View". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace CloudAuction and _VIEWNAME_ with the application and view names.
using Windows.UI.Xaml.Controls;
using CloudAuction.Shared;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudAuction
{
    public sealed partial class _VIEWNAME_View : Page
    {
        public _VIEWNAME_View()
        {
            InitializeComponent();
            DataContext = CloudAuctionApplication.Instance._VIEWNAME_ViewModel;
        }
    }
}
#endif // TEMPLATE