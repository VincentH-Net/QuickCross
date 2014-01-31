using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using QuickCross;
using CloudAuction;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;

namespace CloudAuction
{
	public partial class OrderResultView : ViewBase
	{
		private OrderResultViewModel ViewModel { get { return CloudAuctionApplication.Instance.OrderResultViewModel; } }

		public OrderResultView(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			InitializeBindings(View, ViewModel);
			NavigationItem.RightBarButtonItem.Clicked += (s, e) => ViewModel.DoneCommand.Execute(null); // This event is internal to the view, no need to remove it later, so we can use a lambda expression.
			NavigationItem.RightBarButtonItem.Enabled = ViewModel.DoneCommand.IsEnabled;
		}

		protected override void AddHandlers()
		{
			base.AddHandlers();
			ViewModel.DoneCommand.CanExecuteChanged += HandleDoneCommandCanExecuteChanged; // This event is external to the view, so we need to use a named delegate so we can remove it later, to prevent memory leaks.
		}

		protected override void RemoveHandlers()
		{
			ViewModel.DoneCommand.CanExecuteChanged -= HandleDoneCommandCanExecuteChanged;
			base.RemoveHandlers();
		}

		void HandleDoneCommandCanExecuteChanged (object sender, EventArgs e)
		{
			NavigationItem.RightBarButtonItem.Enabled = ViewModel.DoneCommand.IsEnabled;
		}
	}
}