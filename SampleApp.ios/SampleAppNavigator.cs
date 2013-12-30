using System;
using SampleApp.Shared;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SampleApp
{
	public class SampleAppNavigator : NSObject, ISampleAppNavigator
    {
		private UINavigationController navigationContext, detailNavigationController;

		public SampleAppNavigator(UINavigationController navigationContext, UINavigationController detailNavigationController = null)
		{
			this.navigationContext = navigationContext;
			this.detailNavigationController = detailNavigationController;
		}

		#region Generic navigation helpers

		private static bool IsPhone { get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; } }

		private void Navigate(UIViewController viewController, bool animated = false)
		{
			if (Object.ReferenceEquals(navigationContext.TopViewController, viewController)) return;
			foreach (var stackViewController in navigationContext.ViewControllers)
			{
				if (Object.ReferenceEquals(stackViewController, viewController))
				{
					navigationContext.PopToViewController(viewController, animated);
					return;
				}
			}
			navigationContext.PushViewController(viewController, animated);
		}

		private void Navigate(string viewControllerIdentifier = null, Type viewControllerType = null, bool animated = false)
		{
			if (viewControllerType != null)
			{
				if (navigationContext.TopViewController != null && viewControllerType == navigationContext.TopViewController.GetType()) return;
				if (navigationContext.ViewControllers != null)
				{
					foreach (var stackViewController in navigationContext.ViewControllers)
					{
						if (stackViewController.GetType() == viewControllerType)
						{
							navigationContext.PopToViewController(stackViewController, animated);
							return;
						}
					}
				}
			}

			if (viewControllerIdentifier != null)
			{
				var viewController = (UIViewController)navigationContext.Storyboard.InstantiateViewController(viewControllerIdentifier);
				navigationContext.PushViewController(viewController, animated);
			}
		}

		private void NavigateBack(bool animated = false)
		{
			navigationContext.PopViewControllerAnimated(animated);
		}

		private void NavigateSegue(string segueIdentifier, Type viewControllerType = null)
		{
			if (navigationContext.TopViewController != null)
			{
				if (viewControllerType != null && viewControllerType == navigationContext.TopViewController.GetType()) return;
				navigationContext.TopViewController.PerformSegue(segueIdentifier, this);
			}
		}

		#endregion Generic navigation helpers

		public void NavigateToSampleItemListView()
		{
			if (IsPhone) Navigate(null, typeof(MasterView), true);
		}

		public void NavigateToSampleItemView()
		{
			if (IsPhone)
			{
				NavigateSegue("showDetail", typeof(DetailView)); // Navigate with a segue
				// Navigate("DetailView", typeof(DetailView), true); // Navigate without a segue
			} else {
				var detailViewController = (DetailView)detailNavigationController.TopViewController;
				detailViewController.DismissMasterPopoverController();
			}
		}

		/* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate("_VIEWNAME_View", typeof(_VIEWNAME_View), true);
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
		}
}

