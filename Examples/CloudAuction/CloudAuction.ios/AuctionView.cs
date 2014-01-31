using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using QuickCross;
using CloudAuction;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
	public partial class AuctionView : ViewBase
	{
		private AuctionViewModel ViewModel { get { return CloudAuctionApplication.Instance.AuctionViewModel; } }

		public AuctionView(IntPtr handle) : base(handle) { }

		private UIBarButtonItem logoutBarButtonItem;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			InitializeBindings(View, ViewModel);
			logoutBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ViewModel.LogoutCommand.Execute(null)); // This event is internal to the view, no need to remove it later, so we can use a lambda expression.
			logoutBarButtonItem.Title = "Logout";
			logoutBarButtonItem.Enabled = ViewModel.LogoutCommand.IsEnabled;
			//NavigationController.NavigationItem.SetRightBarButtonItem(logoutBarButtonItem, true);
			TabBarController.NavigationItem.SetRightBarButtonItem(logoutBarButtonItem, true);
		}

		protected override void AddHandlers()
		{
			base.AddHandlers();
			ViewModel.LogoutCommand.CanExecuteChanged += HandleLogoutCommandCanExecuteChanged; // This event is external to the view, so we need to use a named delegate so we can remove it later, to prevent memory leaks.
		}

		protected override void RemoveHandlers()
		{
			ViewModel.LogoutCommand.CanExecuteChanged -= HandleLogoutCommandCanExecuteChanged;
			base.RemoveHandlers();
		}

		void HandleLogoutCommandCanExecuteChanged (object sender, EventArgs e)
		{
			logoutBarButtonItem.Enabled = ViewModel.LogoutCommand.IsEnabled;
		}
	}
}
