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
    }

    // TODO: In Application.Main(), add the following code before the call to UIApplication.Main():
    //    QuickCross.ViewDataBindings.RegisterBindKey();

    // TODO: Add the following code to FinishedLaunching before the call to window.MakeKeyAndVisible():
    //    if (window.RootViewController == null) window.RootViewController = new UINavigationController();
	// 	  var navigator = new _APPNAME_Navigator((UINavigationController)window.RootViewController);
	//    Ensure_APPNAME_Application(navigator).ContinueToMain();
}
