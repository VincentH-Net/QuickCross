using System;
using SampleApp.Shared;
using MonoTouch.UIKit;

namespace SampleApp.ios
{
	public class SampleAppNavigator : ISampleAppNavigator
    {

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
			// TODO: throw new NotImplementedException();
		}

		public void NavigateToSampleItemView(object navigationContext)
		{
			// TODO: throw new NotImplementedException();
		}
    }
}

