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
        object detailItem;

		private SampleItemViewModel ViewModel { get { return SampleAppApplication.Instance.SampleItemViewModel; } }

        public DetailViewController(IntPtr handle) : base(handle)
        {
        }

        public void SetDetailItem(object newDetailItem)
        {
            if (detailItem != newDetailItem)
            {
                detailItem = newDetailItem;
				
                // Update the view
                ConfigureView();
            }
			
            if (masterPopoverController != null)
                masterPopoverController.Dismiss(true);
        }

        void ConfigureView()
        {
            // Update the user interface for the detail item
			// TODO: if (IsViewLoaded && detailItem != null)
			//    detailDescriptionLabel.Text = detailItem.ToString();
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
			
            // Perform any additional setup after loading the view, typically from a nib.
			//ConfigureView();
			SampleAppApplication.Instance.ContinueToSampleItem(); // Ensure that the viewmodel is initialized if not the application but the OS navigates to here
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

