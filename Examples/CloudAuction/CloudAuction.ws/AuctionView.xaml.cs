using Windows.UI.Xaml.Controls;
using CloudAuction.Shared;

namespace CloudAuction
{
    public sealed partial class AuctionView : Page
    {
        public AuctionView()
        {
            this.InitializeComponent();
            DataContext = CloudAuctionApplication.Instance.AuctionViewModel;
        }
    }
}
