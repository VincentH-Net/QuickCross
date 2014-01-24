#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace CloudAuction with the application name.
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
#endif // TEMPLATE