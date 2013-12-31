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

    // TODO: Add the following code to FinishedLaunching after Window.RootViewController has been set:
	// 	  var navigator = new _APPNAME_Navigator((UINavigationController)Window.RootViewController);
	//    Ensure_APPNAME_Application(navigator).ContinueToMain();
}
