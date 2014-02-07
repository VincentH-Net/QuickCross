using System;
using SampleApp.Shared;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SampleApp
{
	public class SampleAppNavigator : NSObject, ISampleAppNavigator
    {
        private static readonly Lazy<SampleAppNavigator> lazy = new Lazy<SampleAppNavigator>(() => new SampleAppNavigator());

        public static SampleAppNavigator Instance { get { return lazy.Value; } }

        private SampleAppNavigator() { }
            // If your app requires multiple navigation contexts, add additional constructor parameters or public properties
            // to pass them in, and then let the navigator manage when which context should be used.
            // E.g. you could use this in a universal app running in PAD mode when you have a master view and a detail view on the same screen.

        public UINavigationController NavigationContext { get; set; }
        public UINavigationController DetailNavigationController { get; set; }

        #region Generic navigation helpers

        private static bool IsPhone { get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; } }

        /// <summary>
        /// Navigate to a view controller instance.
        /// </summary>
        /// <param name="viewController"></param>
        /// <param name="animated"></param>
        private void Navigate(UIViewController viewController, bool animated = false)
        {
            if (Object.ReferenceEquals(NavigationContext.TopViewController, viewController)) return;
            if (NavigationContext.ViewControllers != null)
            {
                foreach (var stackViewController in NavigationContext.ViewControllers)
                {
                    if (Object.ReferenceEquals(stackViewController, viewController))
                    {
                        NavigationContext.PopToViewController(viewController, animated);
                        return;
                    }
                }
            }
            NavigationContext.PushViewController(viewController, animated);
        }

        /// <summary>
        /// Navigate to a view based on a storyboard identifier and/or a view controller type.
        /// Assumes that no more than one instance of the specified controller type should exist in the navigation stack.
        /// </summary>
        /// <param name="viewControllerIdentifier">The storyboard identifier for a storyboard view controller; otherwise null.</param>
        /// <param name="viewControllerType">The view controller type. Specify for automatically navigating back to an existing instance if that exists on the navigation stack. Also specify to create non-storyboard view controller if none exists in the navigation stack.</param>
        /// <param name="animated">A boolean indicating whether the navigation transition should be animated</param>
        private void Navigate(string viewControllerIdentifier, Type viewControllerType = null, bool animated = false, UIStoryboard storyBoard = null)
        {
            if (viewControllerType != null)
            {
                if (NavigationContext.TopViewController != null && viewControllerType == NavigationContext.TopViewController.GetType()) return;
                if (NavigationContext.ViewControllers != null)
                {
                    foreach (var stackViewController in NavigationContext.ViewControllers)
                    {
                        if (stackViewController.GetType() == viewControllerType)
                        {
                            NavigationContext.PopToViewController(stackViewController, animated);
                            return;
                        }
                    }
                }
            }

            if (storyBoard == null) storyBoard = NavigationContext.Storyboard;

            var viewController = (viewControllerIdentifier != null && storyBoard != null) ?
                                 (UIViewController)storyBoard.InstantiateViewController(viewControllerIdentifier) :
                                 (UIViewController)Activator.CreateInstance(viewControllerType);
            NavigationContext.PushViewController(viewController, animated);
        }

        /// <summary>
        /// Navigate to a view based on a view controller type.
        /// Assumes that no more than one instance of the specified controller type should exist in the navigation stack.
        /// </summary>
        /// <param name="viewControllerType">The view controller type</param>
        /// <param name="animated">A boolean indicating whether the navigation transition should be animated</param>
        private void Navigate(Type viewControllerType, bool animated = false)
        {
            Navigate(null, viewControllerType, animated);
        }

        private void NavigateSegue(string segueIdentifier, Type viewControllerType = null)
        {
            if (NavigationContext.TopViewController != null)
            {
                if (viewControllerType != null && viewControllerType == NavigationContext.TopViewController.GetType()) return;
                NavigationContext.TopViewController.PerformSegue(segueIdentifier, this);
            }
        }

		public void NavigateToPreviousView()
		{
			if (NavigationContext == null || NavigationContext.ViewControllers == null || NavigationContext.ViewControllers.Length < 2) return;
			NavigationContext.PopViewControllerAnimated(true);
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
				var detailViewController = (DetailView)DetailNavigationController.TopViewController;
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

