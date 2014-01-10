using MonoTouch.UIKit;
using SampleApp.Shared;

namespace SampleApp
{
    public partial class AppDelegate : UIApplicationDelegate
    {
        public static SampleAppApplication EnsureSampleAppApplication(ISampleAppNavigator navigator)
		{
            return SampleAppApplication.Instance ?? new SampleAppApplication(navigator);
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
