using Android.App;
using Android.Views;
using Android.OS;
using Android.Content.PM;
using QuickCross;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
    [Activity(Label = "Cloud Auction", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, Icon = "@drawable/icon")]
    public class MainView : ActivityViewBase<MainViewModel>
    {
        public static MainViewModel.SubView CurrentSubView;

        private CloudAuctionApplication EnsureApplication()
        {
            return CloudAuctionApplication.Instance ?? new CloudAuctionApplication(CloudAuctionNavigator.Instance);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainView);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            // One-time initialization of application and helpers.
            // This can be done in the startup view, or in the android application object, or in the application loading screen.
            EnsureApplication();
            AndroidHelpers.Initialize(typeof(Resource));

            CloudAuctionNavigator.Instance.NavigationContext = this;
            CloudAuctionApplication.Instance.ContinueToMain();
                // Ensure that our viewmodel is initialized, in case we navigated to this view by some other means 
                // than through a call to CloudAuctionApplication.Instance.ContinueToMain().
                // E.g. when the OS creates this view at application startup, because it is marked as MainLauncher.

            ActionBar.Tab[] tabs = new ActionBar.Tab[] { 
                ActionBar.NewTab().SetText("Auction").SetTag(new AuctionView()), 
                ActionBar.NewTab().SetText("Products").SetTag(new ProductsView()), 
                ActionBar.NewTab().SetText("Help").SetTag(new Fragment()) };
            foreach (var tab in tabs) tab.TabSelected += Tab_TabSelected;

            Initialize(FindViewById(Resource.Id.MainView), CloudAuctionApplication.Instance.MainViewModel);
                // This call initializes data binding and updates the view with the current viewmodel values.

            for (int position = 0; position < tabs.Length; position++)
            {
                ActionBar.AddTab(tabs[position], setSelected: (MainViewModel.SubView)position == CurrentSubView);
            }
        }

        private void EnsureCurrentTabIsSelected()
        {
            int index = (int)CurrentSubView;
            if (ActionBar.SelectedNavigationIndex != index)
            {
                ActionBar.SetSelectedNavigationItem(index);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            EnsureCurrentTabIsSelected();
        }

        private void Tab_TabSelected(object sender, ActionBar.TabEventArgs e)
        {
            var tab = (ActionBar.Tab)sender;
            e.FragmentTransaction.Replace(Resource.Id.MainView_TabFragmentContainer, (Fragment)tab.Tag);
            // TODO: Check if we should also use .Remove in TabUnselected event? E.g. see http://arvid-g.de/12/android-4-actionbar-with-tabs-example
            CurrentSubView = (MainViewModel.SubView)tab.Position;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.LogoutMenuItem: ViewModel.LogoutCommand.Execute(null); return true;
                default: return base.OnOptionsItemSelected(item);
            }
        }
    }
}

