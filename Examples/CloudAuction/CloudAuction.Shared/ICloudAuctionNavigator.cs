using CloudAuction.Shared.ViewModels;
namespace CloudAuction.Shared
{
    public interface ICloudAuctionNavigator
    {
        void NavigateToMainView(MainViewModel.SubView? subView);
        void NavigateToProductView();
        void NavigateToOrderView();
        void NavigateToOrderResultView();

        /* TODO: For each view, add a method to navigate to that view with a signature like this:
        void NavigateTo_VIEWNAME_View();
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
