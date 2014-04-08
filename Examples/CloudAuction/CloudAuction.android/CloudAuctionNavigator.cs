using System;

using Android.Content;
using QuickCross;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
    class CloudAuctionNavigator : ICloudAuctionNavigator
    {
        private static readonly Lazy<CloudAuctionNavigator> lazy = new Lazy<CloudAuctionNavigator>(() => new CloudAuctionNavigator());

        public static CloudAuctionNavigator Instance { get { return lazy.Value; } }

        private CloudAuctionNavigator() { }

        public Context NavigationContext { get; set; }

        private void Navigate(Type type)
        {
            if (NavigationContext == null) return;
            if (AndroidHelpers.CurrentActivity != null && AndroidHelpers.CurrentActivity.GetType() == type) return;
            NavigationContext.StartActivity(type);
        }

        public void NavigateToPreviousView()
        {
            if (AndroidHelpers.CurrentActivity != null) AndroidHelpers.CurrentActivity.Finish();
        }

        public void NavigateToMainView(MainViewModel.SubView? subView)
        {
            if (subView.HasValue) MainView.CurrentSubView = subView.Value;
            Navigate(typeof(MainView));
        }

        public void NavigateToProductView()
        {
            Navigate(typeof(ProductView));
        }

        public void NavigateToOrderView()
        {
            Navigate(typeof(OrderView));
        }

        public void NavigateToOrderResultView()
        {
            Navigate(typeof(OrderResultView));
        }

        /* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate(typeof(_VIEWNAME_View));
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
