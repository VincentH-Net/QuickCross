#if TEMPLATE // To add a new main launcher view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command with option -ViewType MainLauncher (enter "Get-Help New-View" for details). Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and CloudAuction with the viewmodel resp application name.
using Android.App;
using Android.OS;
using Android.Content.PM;
using QuickCross;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
    [Activity(Label = "CloudAuction _VIEWNAME_", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, Icon = "@drawable/icon")]
    public class _VIEWNAME_View : ActivityViewBase<_VIEWNAME_ViewModel>
    {
        private CloudAuctionApplication EnsureApplication()
        {
            return CloudAuctionApplication.Instance ?? new CloudAuctionApplication(CloudAuctionNavigator.Instance);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout._VIEWNAME_View);
            AndroidHelpers.Initialize(typeof(Resource));
            EnsureApplication();
            CloudAuctionNavigator.Instance.NavigationContext = this;
            CloudAuctionApplication.Instance.ContinueTo_VIEWNAME_();
            Initialize(FindViewById(Resource.Id._VIEWNAME_View), CloudAuctionApplication.Instance._VIEWNAME_ViewModel);
        }
    }
}
#endif // TEMPLATE