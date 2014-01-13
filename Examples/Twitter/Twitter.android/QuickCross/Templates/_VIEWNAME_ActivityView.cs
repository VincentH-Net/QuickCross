#if TEMPLATE // To add a new activity view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command (enter "Get-Help New-View" for details). Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and Twitter with the viewmodel resp application name.
using Android.App;
using Android.OS;
using Android.Content.PM;
using QuickCross;
using Twitter.Shared;
using Twitter.Shared.ViewModels;

namespace Twitter
{
    [Activity(Label = "Twitter _VIEWNAME_", LaunchMode = LaunchMode.SingleTask, Icon = "@drawable/icon")]
    public class _VIEWNAME_View : ActivityViewBase<_VIEWNAME_ViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout._VIEWNAME_View);
            TwitterNavigator.Instance.NavigationContext = this;
            Initialize(FindViewById(Resource.Id._VIEWNAME_View), TwitterApplication.Instance._VIEWNAME_ViewModel);
        }
    }
}
#endif // TEMPLATE