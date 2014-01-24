using System;
using Windows.UI.Xaml.Controls;
using CloudAuction.Shared;

namespace CloudAuction
{
    class CloudAuctionNavigator : ICloudAuctionNavigator
    {
        private static readonly Lazy<CloudAuctionNavigator> lazy = new Lazy<CloudAuctionNavigator>(() => new CloudAuctionNavigator());

        public static CloudAuctionNavigator Instance { get { return lazy.Value; } }

        private CloudAuctionNavigator() { }

        public Frame NavigationContext { get; set; }

        private void Navigate(Type sourcePageType)
        {
            if (NavigationContext == null || NavigationContext.CurrentSourcePageType == sourcePageType) return;
            NavigationContext.Navigate(sourcePageType);
        }

        public void NavigateToMainView(Shared.ViewModels.MainViewModel.SubView? subView)
        {
            if (!subView.HasValue || subView.Value == Shared.ViewModels.MainViewModel.SubView.Auction)
            {
                Navigate(typeof(AuctionView));
                return;
            }
            throw new NotImplementedException();
        }

        public void NavigateToProductView()
        {
            throw new NotImplementedException();
        }

        public void NavigateToOrderView()
        {
            Navigate(typeof(OrderView));
        }

        public void NavigateToOrderResultView()
        {
            OrderResultView.Show();
        }

        /* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate(typeof(_VIEWNAME_View));
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
