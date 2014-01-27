using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CloudAuction.Shared;

namespace CloudAuction
{
    public partial class AppDelegate : UIApplicationDelegate
    {
        public static CloudAuctionApplication EnsureCloudAuctionApplication(ICloudAuctionNavigator navigator)
		{
            return CloudAuctionApplication.Instance ?? new CloudAuctionApplication(navigator);
		}

		private UINavigationController InitializeNavigationContext()
		{
			if (Window == null ) Window = new UIWindow(UIScreen.MainScreen.Bounds);
			var navigationContext = Window.RootViewController as UINavigationController;
			if (navigationContext == null) {
				navigationContext = new UINavigationController();
				if (Window.RootViewController != null) navigationContext.ViewControllers = new UIViewController[] { Window.RootViewController };
				Window.RootViewController = navigationContext;
			}
			return navigationContext;
		}
    }
}
