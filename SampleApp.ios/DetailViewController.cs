using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using QuickCross;
using SampleApp.Shared;
using SampleApp.Shared.ViewModels;

namespace SampleApp.ios
{
	public partial class DetailViewController : ViewControllerBase
    {
        UIPopoverController masterPopoverController;

		private SampleItemViewModel ViewModel { get { return SampleAppApplication.Instance.SampleItemViewModel; } }

        public DetailViewController(IntPtr handle) : base(handle)
        {
        }

		public void DismissMasterPopoverController()
        {
            if (masterPopoverController != null)
                masterPopoverController.Dismiss(true);
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			base.InitializeBindings(View, ViewModel);
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
