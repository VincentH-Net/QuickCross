using Windows.UI.Xaml.Controls;
using CloudAuction.Shared;

namespace CloudAuction
{
    public sealed partial class OrderView : Page
    {
        public OrderView()
        {
            this.InitializeComponent();
            DataContext = CloudAuctionApplication.Instance.OrderViewModel;
        }
    }
}
