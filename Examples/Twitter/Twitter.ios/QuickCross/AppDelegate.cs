using MonoTouch.UIKit;
using Twitter.Shared;

namespace Twitter
{
    public partial class AppDelegate : UIApplicationDelegate
    {
        public static TwitterApplication EnsureTwitterApplication(ITwitterNavigator navigator)
		{
            return TwitterApplication.Instance ?? new TwitterApplication(navigator);
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
