using Android.OS;
using Android.Views;
using QuickCross;
using CloudAuction.Shared.ViewModels;
using CloudAuction.Shared;

namespace CloudAuction
{
    public class ProductsView : FragmentViewBase<ProductsViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            CloudAuctionApplication.Instance.ContinueToMain(subView: MainViewModel.SubView.Products);
            var view = inflater.Inflate(Resource.Layout.ProductsView, container, false);
            Initialize(view, CloudAuctionApplication.Instance.ProductsViewModel, inflater);
            return view;
        }
    }
}