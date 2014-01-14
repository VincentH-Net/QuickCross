using MonoTouch.Foundation;
using MonoTouch.UIKit;
using QuickCrossLibrary.Templates;

namespace QuickCross.Templates
{
    public partial class AppDelegate : UIApplicationDelegate
    {
        public static _APPNAME_Application Ensure_APPNAME_Application(I_APPNAME_Navigator navigator)
		{
            return _APPNAME_Application.Instance ?? new _APPNAME_Application(navigator);
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

    // TODO: In Application.Main(), add the following code before the call to UIApplication.Main():
    //    QuickCross.ViewDataBindings.RegisterBindKey();

	// TODO: Ensure that you override the Window property in AppDelegate like this:
	//    public override UIWindow Window { get; set; }
	// and then add the following code to FinishedLaunching before the call to Window.MakeKeyAndVisible():
    //    _APPNAME_Navigator.Instance.NavigationContext = InitializeNavigationContext();
    //    Ensure_APPNAME_Application(_APPNAME_Navigator.Instance).ContinueToMain();
}
