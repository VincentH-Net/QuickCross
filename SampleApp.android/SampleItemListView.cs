using Android.App;
using Android.OS;
using Android.Content.PM;
using QuickCross;
using SampleApp.Shared;
using SampleApp.Shared.ViewModels;

namespace SampleApp
{
    [Activity(Label = "SampleApp", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, Icon = "@drawable/icon")]
    public class SampleItemListView : ActivityViewBase<SampleItemListViewModel>
    {
        private SampleAppApplication EnsureApplication()
        {
            return SampleAppApplication.Instance ?? new SampleAppApplication(SampleAppNavigator.Instance);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SampleItemListView);
            AndroidHelpers.Initialize(typeof(Resource));
            EnsureApplication();
            SampleAppNavigator.Instance.NavigationContext = this;
            SampleAppApplication.Instance.ContinueToSampleItemList();
            Initialize(FindViewById(Resource.Id.SampleItemListView), SampleAppApplication.Instance.SampleItemListViewModel);
        }
    }
}

