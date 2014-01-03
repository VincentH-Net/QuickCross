using MonoTouch.UIKit;
using QCTest1.Shared;

namespace QCTest1
{
    public partial class AppDelegate : UIApplicationDelegate
    {
        public static QCTest1Application EnsureQCTest1Application(IQCTest1Navigator navigator)
		{
            return QCTest1Application.Instance ?? new QCTest1Application(navigator);
		}
    }

    // TODO: In Application.Main(), add the following code before the call to UIApplication.Main():
    //    QuickCross.ViewDataBindings.RegisterBindKey();

    // TODO: Add the following code to FinishedLaunching after Window.RootViewController has been set:
	// 	  var navigator = new QCTest1Navigator((UINavigationController)Window.RootViewController);
	//    EnsureQCTest1Application(navigator).ContinueToMain();
}
