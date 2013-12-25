#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using System;
using MonoTouch.Foundation;
using QuickCross.Templates;
using MonoTouch.UIKit;

namespace MvvmQuickCross.Templates
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


		/* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate("_VIEWNAME_View", typeof(_VIEWNAME_View), true);
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/MvvmQuickCross#new-view). */
    }
}
#endif // TEMPLATE
