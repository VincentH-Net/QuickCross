using Windows.UI.Xaml.Controls;
using CloudAuction.Shared;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudAuction
{
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = CloudAuctionApplication.Instance.MainViewModel;
        }
    }
}
