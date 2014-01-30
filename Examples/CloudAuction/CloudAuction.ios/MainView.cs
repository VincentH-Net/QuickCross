using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using QuickCross;
using CloudAuction;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;
using System.Collections.Generic;

namespace CloudAuction
{
	public partial class MainView : TabBarViewBase
	{
		private MainViewModel ViewModel { get { return CloudAuctionApplication.Instance.MainViewModel; } }

		public MainView(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			InitializeBindings(View, ViewModel);
		}
	}
}
