#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using System;
using MonoTouch.Foundation;
using QuickCross.Templates;
using QuickCrossLibrary.Templates;
using MonoTouch.UIKit;

namespace QuickCross.Templates
{
	public class _APPNAME_Navigator : NSObject, I_APPNAME_Navigator
    {
		private UINavigationController navigationContext;

		public _APPNAME_Navigator(UINavigationController navigationContext)
		{
			this.navigationContext = navigationContext;
			// TODO: If your app requires multiple navigation contexts, add additional constructor parameters
			// to pass them in, and then let the navigator manage when which context should be used.
			// E.g. you could use this in a universal app running in PAD mode when you have a master view and a detail view on the same screen.
		}

		#region Generic navigation helpers

		private static bool IsPhone { get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; } }

		/// <summary>
		/// Navigate to a view controller instance.
		/// </summary>
		/// <param name="viewController"></param>
		/// <param name="animated"></param>
        private void Navigate(UIViewController viewController, bool animated = false)
		{
			if (Object.ReferenceEquals(navigationContext.TopViewController, viewController)) return;
            if (navigationContext.ViewControllers != null)
            {
                foreach (var stackViewController in navigationContext.ViewControllers)
                {
                    if (Object.ReferenceEquals(stackViewController, viewController))
                    {
                        navigationContext.PopToViewController(viewController, animated);
                        return;
                    }
                }
            }
			navigationContext.PushViewController(viewController, animated);
		}

		/// <summary>
		/// Navigate to a view based on a storyboard identifier and/or a view controller type.
        /// Assumes that no more than one instance of the specified controller type should exist in the navigation stack.
		/// </summary>
		/// <param name="viewControllerIdentifier">The storyboard identifier for a storyboard view controller; otherwise null.</param>
		/// <param name="viewControllerType">The view controller type. Specify for automatically navigating back to an existing instance if that exists on the navigation stack. Also specify to create non-storyboard view controller if none exists in the navigation stack.</param>
        /// <param name="animated">A boolean indicating whether the navigation transition should be animated</param>
        private void Navigate(string viewControllerIdentifier, Type viewControllerType = null, bool animated = false)
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

            var viewController = (viewControllerIdentifier != null && navigationContext.Storyboard != null) ?
                (UIViewController)navigationContext.Storyboard.InstantiateViewController(viewControllerIdentifier) :
                (UIViewController)Activator.CreateInstance(viewControllerType);
			navigationContext.PushViewController(viewController, animated);
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


		/* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate("_VIEWNAME_View", typeof(_VIEWNAME_View), true); // TODO: If this is not a storyboard view, remove the viewControllerIdentifier parameter
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
#endif // TEMPLATE
