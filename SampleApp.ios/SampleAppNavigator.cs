using System;
using SampleApp.Shared;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SampleApp.ios
{
	public class SampleAppNavigator : NSObject, ISampleAppNavigator
    {
		public static MasterViewController masterViewController;

		// TODO: figure universal app nav out better...
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
			if (masterNavigationController.TopViewController is MasterViewController) return;
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				if (masterNavigationController.TopViewController is DetailViewController)
				{
					masterNavigationController.PopViewControllerAnimated(true);
				}
			}
		}

		public void NavigateToSampleItemView(object navigationContext)
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				if (masterNavigationController.TopViewController is DetailViewController)
					return;
				// Segue navigation works:
				// masterNavigationController.TopViewController.PerformSegue("showDetail", this);

				// Non-segue nav works also; todo check if we need to cache the ctrl instance in the navigator or is sb does that for us 
				var ctrl = (UIViewController)masterNavigationController.Storyboard.InstantiateViewController("DetailViewController");
				masterNavigationController.PushViewController(ctrl, true);
			}
		}
    }
}

