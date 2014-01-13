using Android.App;
using Android.OS;
using Android.Content.PM;
using QuickCross;
using Twitter.Shared;
using Twitter.Shared.ViewModels;

namespace Twitter
{
    [Activity(Label = "Twitter Main", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, Icon = "@drawable/icon")]
    public class MainView : ActivityViewBase<MainViewModel>
    {
        private TwitterApplication EnsureApplication()
        {
            return TwitterApplication.Instance ?? new TwitterApplication(TwitterNavigator.Instance);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainView);
            AndroidHelpers.Initialize(typeof(Resource));
            EnsureApplication();
            TwitterNavigator.Instance.NavigationContext = this;
            TwitterApplication.Instance.ContinueToMain();
            Initialize(FindViewById(Resource.Id.MainView), TwitterApplication.Instance.MainViewModel);
        }
    }
}
