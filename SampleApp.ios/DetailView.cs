using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using QuickCross;
using SampleApp.Shared;
using SampleApp.Shared.ViewModels;

namespace SampleApp.ios
{
	public partial class DetailView : ViewBase
    {
        UIPopoverController masterPopoverController;

		private SampleItemViewModel ViewModel { get { return SampleAppApplication.Instance.SampleItemViewModel; } }

		public DetailView(IntPtr handle) : base(handle) { }

		public void DismissMasterPopoverController()
        {
            if (masterPopoverController != null)
                masterPopoverController.Dismiss(true);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			InitializeBindings(View, ViewModel);
        }

        [Export("splitViewController:willHideViewController:withBarButtonItem:forPopoverController:")]
        public void WillHideViewController(UISplitViewController splitController, UIViewController viewController, UIBarButtonItem barButtonItem, UIPopoverController popoverController)
        {
            barButtonItem.Title = NSBundle.MainBundle.LocalizedString("Master", "Master");
            NavigationItem.SetLeftBarButtonItem(barButtonItem, true);
            masterPopoverController = popoverController;
        }

        [Export("splitViewController:willShowViewController:invalidatingBarButtonItem:")]
        public void WillShowViewController(UISplitViewController svc, UIViewController vc, UIBarButtonItem button)
        {
            // Called when the view is shown again in the split view, invalidating the button and popover controller.
            NavigationItem.SetLeftBarButtonItem(null, true);
            masterPopoverController = null;
        }
    }
}