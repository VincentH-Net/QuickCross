using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CloudAuction.Shared;

namespace CloudAuction
{
    sealed partial class App : Application
    {
        public static CloudAuctionApplication EnsureCloudAuctionApplication(Frame navigationContext)
        {
            CloudAuctionNavigator.Instance.NavigationContext = navigationContext;
            return CloudAuctionApplication.Instance ?? new CloudAuctionApplication(CloudAuctionNavigator.Instance);
        }

        // TODO: Replace the if (rootFrame.Content == null) { ... } code in OnLaunched() with this:
        //    EnsureCloudAuctionApplication(rootFrame);
        //    if (rootFrame.Content == null) CloudAuctionApplication.Instance.ContinueToMain();
    }
}
