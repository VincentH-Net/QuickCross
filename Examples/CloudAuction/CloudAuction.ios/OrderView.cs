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
    public partial class OrderView : DialogViewBase
	{
		private OrderViewModel ViewModel { get { return CloudAuctionApplication.Instance.OrderViewModel; } }

        public OrderView(IntPtr handle) : base(handle)
        {
            Pushing = true;
            Root = new RootElement ("Order") { 
                new Section () {
                    new EntryElement ("Email", "Enter Email", ViewModel.Email)
                } 
            };
            // TODO: figure out how to do data binding with monotouch.dialog
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
