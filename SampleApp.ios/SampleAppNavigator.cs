using System;
using SampleApp.Shared;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SampleApp.ios
{
	public class SampleAppNavigator : NSObject, ISampleAppNavigator
    {
		private UINavigationController masterNavigationController, detailNavigationController;

		public SampleAppNavigator(UINavigationController masterNavigationController, UINavigationController detailNavigationController = null)
		{
			this.masterNavigationController = masterNavigationController;
			this.detailNavigationController = detailNavigationController;
		}

		private void Navigate(UINavigationController navigationController, UIViewController viewController, bool animated = false)
		{
			navigationController.PushViewController(viewController, animated);
		}

		public void NavigateToSampleItemListView(object navigationContext)
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				if (masterNavigationController.TopViewController is MasterViewController) return;
				masterNavigationController.PopToRootViewController(true);
			}
		}

		public void NavigateToSampleItemView(object navigationContext)
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				if (masterNavigationController.TopViewController is DetailViewController) return;

				const bool useSegue = true; // Just a hardcoded switch so we can test navigation with and without a segue
				if (useSegue)
				{
					masterNavigationController.TopViewController.PerformSegue("showDetail", this);
				} else {
					var detailViewController = (DetailViewController)masterNavigationController.Storyboard.InstantiateViewController("DetailViewController");
					masterNavigationController.PushViewController(detailViewController, true);
				}
			} else {
				var detailViewController = (DetailViewController)detailNavigationController.TopViewController;
				detailViewController.DismissMasterPopoverController();
			}
		}
    }
}

