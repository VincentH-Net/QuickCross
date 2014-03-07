using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using QuickCross;
using CloudAuction;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CloudAuction
{
	public partial class OrderView : DialogViewBase
	{
		private OrderViewModel ViewModel { get { return CloudAuctionApplication.Instance.OrderViewModel; } }

        private List<BindingParameters> bp = new List<BindingParameters>();

        public OrderView(IntPtr handle) : base(handle)
        {
            Pushing = true;

            var m = ViewModel;
            Root = new RootElement("Order") {
                new Section() {
                    new RootElement("Deliver", new RadioGroup(0)) {
                        new Section() { from i in ViewModel.DeliveryLocationList select new RadioElement(i) }
                    }                                          .Bind(bp, () => m.DeliveryLocation, listProperty: () => m.DeliveryLocationList)
                },
                new Section() {
                    new RootElement("Title", new RadioGroup(0)) {
                        new Section() { from i in ViewModel.TitleList select new RadioElement(i) }
                    }                                          .Bind(bp, () => m.Title, listProperty: () => m.TitleList),
                    new EntryElement("Name", "First name", "") .Bind(bp, () => m.FirstName),
                    new EntryElement(null, "Middle name", "")  .Bind(bp, () => m.MiddleName),
                    new EntryElement(null, "Last name", "")    .Bind(bp, () => m.LastName),
                    new EntryElement("Address", "Street", "")  .Bind(bp, () => m.Street),
                    new EntryElement(null, "Zip", "")          .Bind(bp, () => m.Zip),
                    new EntryElement(null, "City", "")         .Bind(bp, () => m.City),
                    new EntryElement(null, "Country", "")      .Bind(bp, () => m.Country),
                    new EntryElement("Email", "", "")          .Bind(bp, () => m.Email),
                    new EntryElement("Mobile", "", "")         .Bind(bp, () => m.Mobile),
                    new EntryElement("Phone", "", "")          .Bind(bp, "Phone")
                }
            };

            var _ = ChangeData(); // Just a gimmick to show that background updates to the viewmodel are displayed in Monotouch.Dialog 
        }

        private async Task ChangeData()
        {
            for (bool first = true; ; first = !first)
            {
                await Task.Delay(1000);
                if (first)
                {
                    ViewModel.FirstName = "!" + ViewModel.FirstName;
                    ViewModel.Phone = "!" + ViewModel.Phone;
                }
                else
                {
                    ViewModel.FirstName = ViewModel.FirstName.Substring(1);
                    ViewModel.Phone = ViewModel.Phone.Substring(1);
                }
            }
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            InitializeBindings(View, ViewModel, bp.ToArray());

            var cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Done, (s, e) => ViewModel.CancelCommand.Execute(null));
			cancelButton.Enabled = ViewModel.CancelCommand.IsEnabled;
			NavigationItem.SetLeftBarButtonItem(cancelButton, true);

            var confirmButton = new UIBarButtonItem("Confirm", UIBarButtonItemStyle.Done, (s, e) => ViewModel.ConfirmCommand.Execute(null));
            confirmButton.Enabled = ViewModel.ConfirmCommand.IsEnabled;
            NavigationItem.SetRightBarButtonItem(confirmButton, true);
		}

		protected override void AddHandlers()
		{
			base.AddHandlers();
			ViewModel.CancelCommand.CanExecuteChanged += HandleCancelCommandCanExecuteChanged; // This event is external to the view, so we need to use a named delegate so we can remove it later, to prevent memory leaks.
			ViewModel.ConfirmCommand.CanExecuteChanged += HandleConfirmCommandCanExecuteChanged; // This event is external to the view, so we need to use a named delegate so we can remove it later, to prevent memory leaks.
		}

		protected override void RemoveHandlers()
		{
			ViewModel.ConfirmCommand.CanExecuteChanged -= HandleConfirmCommandCanExecuteChanged;
			ViewModel.CancelCommand.CanExecuteChanged -= HandleCancelCommandCanExecuteChanged;
			base.RemoveHandlers();
		}

		void HandleCancelCommandCanExecuteChanged (object sender, EventArgs e)
		{
			NavigationItem.LeftBarButtonItem.Enabled = ViewModel.CancelCommand.IsEnabled;
		}

		void HandleConfirmCommandCanExecuteChanged (object sender, EventArgs e)
		{
			NavigationItem.RightBarButtonItem.Enabled = ViewModel.ConfirmCommand.IsEnabled;
		}
	}
}
