using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using QuickCross;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace CloudAuction
{
	public partial class OrderView : DialogViewBase
	{
		private OrderViewModel ViewModel { get { return CloudAuctionApplication.Instance.OrderViewModel; } }

        private List<BindingParameters> bp = new List<BindingParameters>();

        public OrderView(IntPtr handle) : base(handle)
        {
            Pushing = true;

            var deliveryLocationList = new Section();
            deliveryLocationList.AddAll(from i in ViewModel.DeliveryLocationList select new RadioElement(i));

            var titleList = new Section();
            titleList.AddAll(from i in ViewModel.TitleList select new RadioElement(i));

            var m = ViewModel;
            Root = new RootElement("Order") {
                new Section() {
                    new RootElement("Deliver", new RadioGroup(0)) { 
                        deliveryLocationList }                    .Bind(bp, () => m.DeliveryLocation, listProperty: () => m.DeliveryLocationList)
                },
                new Section() {
                    new RootElement("Title", new RadioGroup(0)) {
                        titleList }                               .Bind(bp, () => m.Title, listProperty: () => m.TitleList),
                    new EntryElement("Name"   , "First name", "") .Bind(bp, () => m.FirstName), // Note that you MUST specify "" for the initial element value; any other value will update the viewmodel property in a 2-way binding.
                    new EntryElement(null     , "Middle name", "").Bind(bp, () => m.MiddleName),
                    new EntryElement(null     , "Last name", "")  .Bind(bp, () => m.LastName),
                    new EntryElement("Address", "Street", "")     .Bind(bp, () => m.Street),
                    new EntryElement(null     , "Zip", "")        .Bind(bp, () => m.Zip),
                    new EntryElement(null     , "City", "")       .Bind(bp, () => m.City),
                    new EntryElement(null     , "Country", "")    .Bind(bp, () => m.Country),
                    new EntryElement("Email"  , "", "")           .Bind(bp, () => m.Email),
                    new EntryElement("Mobile" , "", "")           .Bind(bp, () => m.Mobile),
                    new EntryElement("Phone"  , "", "")           .Bind(bp, () => m.Phone)
                }
            };
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
            InitializeBindings(View, ViewModel, bp.ToArray());
		}
	}
}
