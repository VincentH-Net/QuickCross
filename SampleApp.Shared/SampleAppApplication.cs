using System.Threading.Tasks;
using QuickCross;
using SampleApp.Shared.Models;
using SampleApp.Shared.Services;
using SampleApp.Shared.ViewModels;
using System;

namespace SampleApp.Shared
{
    public sealed class SampleAppApplication : ApplicationBase
    {
        private ISampleAppNavigator _navigator;
        private SampleItemService _itemService;

        public SampleAppApplication(ISampleAppNavigator navigator, object currentNavigationContext = null, TaskScheduler uiTaskScheduler = null)
            : base(currentNavigationContext, uiTaskScheduler)
        {
            // TODO: Create instances for all services that have a cross-platform implementation
            // (services that have a platform-specific implementation, such as the navigator,
            // are instantiated in a platform-specific project and passed to this application 
            // as a cross-platform interface).
            
            _navigator = navigator;
            _itemService = new SampleItemService();
        }

        new public static SampleAppApplication Instance { get { return (SampleAppApplication)ApplicationBase.Instance; } }

        public SampleItemListViewModel SampleItemListViewModel { get; private set; }
        public SampleItemViewModel SampleItemViewModel { get; private set; }

        public void ContinueToSampleItemList(bool skipNavigation = false)
        {
            if (SampleItemListViewModel == null) SampleItemListViewModel = new SampleItemListViewModel(_itemService);
            SampleItemListViewModel.Refresh();
            if (!skipNavigation) RunOnUIThread(() => _navigator.NavigateToSampleItemListView(CurrentNavigationContext));
        }

		public void ContinueToSampleItem(SampleItem item = null, bool skipNavigation = false)
        {
            if (SampleItemViewModel == null) SampleItemViewModel = new SampleItemViewModel(_itemService);
			SampleItemViewModel.Initialize(item);
			if (!skipNavigation) RunOnUIThread(() => _navigator.NavigateToSampleItemView(CurrentNavigationContext));
        }
    }
}
