#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using MonoTouch.UIKit;
using SampleApp.Shared;

namespace SampleApp.ios
{
    public partial class AppDelegate : UIApplicationDelegate
    {
		public static SampleAppApplication EnsureSampleAppApplication(ISampleAppNavigator navigator)
		{
			return SampleAppApplication.Instance ?? new SampleAppApplication(navigator);
		}
    }

	// TODO: Add the following code to FinishedLaunching:
	// 	  var navigator = new _APPNAME_Navigator((UINavigationController)Window.RootViewController);
	//    Ensure_APPNAME_Application(navigator).ContinueToMain();

	// TODO: Add the following code to Application_Activated:
	//    Ensure_APPNAME_Application(RootFrame);
}
#endif // TEMPLATE
