using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using QuickCross;
using CloudAuction;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
    public class Expense
    {
        [Section("Expense Entry")]

        [Entry("Enter expense name")]
        public string Name { 
            get { 
                return CloudAuctionApplication.Instance.OrderViewModel.City; } 
            set { 
                CloudAuctionApplication.Instance.OrderViewModel.City = value; }
        }

        [Section("Expense Details")]

        [Caption("Description")]
        [Entry]
        public string Details;

        [Checkbox]
        public bool IsApproved = true;
    }

    public partial class OrderView : DialogViewBase
	{
		private OrderViewModel ViewModel { get { return CloudAuctionApplication.Instance.OrderViewModel; } }

        public OrderView(IntPtr handle) : base(handle)
        {
            Pushing = true;
            var expense = new Expense ();
            var bctx = new BindingContext (null, expense, "Create a task");
            Root = bctx.Root;

//            Root = new RootElement("Order") { 
//                new Section() {
//                    new EntryElement("Email", "Enter Email", ViewModel.Email)
//                } 
//            };

            // TODO: figure out how to do data binding with monotouch.dialog -> choose the elements api, not the reflection api
            // 0) How to find the element given a propertyname?
            // 1) How to specify binding parameters for an element?
            // 2) How to capture changes in element value? 

            // Observations:
            // - Elements have different value property names and types, different event names and delegate signatures
            // - Not safe to bind to UIView that corresponds to an element, because that will conflict with the Mt.D built in binding mechanism
            //   -> we need to bind to elements or to the annotated models.
            // - bind to elements: 
            //   - automatically: base type change from UIView to NSObject, support Element as well as UIView, lots of change
            //     and also hard dependency on dialog (reference)
            //   - manually: is effectively not having binding support at all.
            // - bind to annotated model:
            //   - 

//            Section s;
//            Element e;
//            EntryElement ee;
//            DialogViewController dvc;
//            BooleanElement be;
//            BooleanImageElement bie;
//            CheckboxElement ce;
//            DateElement de;
//            DateTimeElement dte;
//            FloatElement fe;
//            ImageElement ie;
//            RadioElement re;
//            RootElement rte;
//            StringElement se;
//            StyledStringElement sse;
//            TimeElement te;
//            UIViewElement ue;

            //ee.Changed
            //be.ValueChanged
            //bie.Tapped
            //bie.ValueChanged
            //ce.Tapped
            //de.DateSelected

        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			InitializeBindings(View, ViewModel);

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
