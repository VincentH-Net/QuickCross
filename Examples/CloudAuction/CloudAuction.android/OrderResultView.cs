using Android.App;
using Android.OS;
using QuickCross;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
    [Activity(Label = "Order Confirmation")]
    public class OrderResultView : ActivityViewBase<OrderResultViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.OrderResultView);
            CloudAuctionNavigator.Instance.NavigationContext = this;
            Initialize(FindViewById(Resource.Id.OrderResultView), CloudAuctionApplication.Instance.OrderResultViewModel);
        }
    }
}