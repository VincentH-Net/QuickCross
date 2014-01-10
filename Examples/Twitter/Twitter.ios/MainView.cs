using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using QuickCross;
using Twitter;
using Twitter.Shared;
using Twitter.Shared.ViewModels;

namespace Twitter
{
	public partial class MainView : ViewBase
	{
		private MainViewModel ViewModel { get { return TwitterApplication.Instance.MainViewModel; } }

		public MainView(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			InitializeBindings(View, ViewModel);
		}
	}
}