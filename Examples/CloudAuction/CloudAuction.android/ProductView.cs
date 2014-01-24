using Android.App;
using Android.OS;
using QuickCross;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
    [Activity(Label = "Product Details")]
    public class ProductView : ActivityViewBase<ProductViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ProductView);
            CloudAuctionNavigator.Instance.NavigationContext = this;
            Initialize(FindViewById(Resource.Id.ProductView), CloudAuctionApplication.Instance.ProductViewModel);
        }
    }
}