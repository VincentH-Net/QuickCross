using Android.OS;
using Android.Views;
using QuickCross;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
    public class AuctionView : FragmentViewBase<AuctionViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            CloudAuctionApplication.Instance.ContinueToMain(subView: MainViewModel.SubView.Auction);
            var view = inflater.Inflate(Resource.Layout.AuctionView, container, false);
            Initialize(view, CloudAuctionApplication.Instance.AuctionViewModel, inflater);
            return view;
        }
    }
}