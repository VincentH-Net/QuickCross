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
    }
}
