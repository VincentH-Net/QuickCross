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
    }
}
