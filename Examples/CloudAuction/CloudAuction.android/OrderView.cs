using Android.App;
using Android.OS;
using QuickCross;
using CloudAuction.Shared.ViewModels;
using CloudAuction.Shared;
using Android.Views;
using System.Collections.Specialized;

namespace CloudAuction
{
    [Activity(Label = "Order")]
    public class OrderView : ActivityViewBase<OrderViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.OrderView);

            //var spinner = FindViewById<Android.Widget.Spinner>(Resource.Id.OrderView_DeliveryLocation);
            CloudAuctionNavigator.Instance.NavigationContext = this;
            Initialize(FindViewById(Resource.Id.OrderView), CloudAuctionApplication.Instance.OrderViewModel);
        }
    }
}