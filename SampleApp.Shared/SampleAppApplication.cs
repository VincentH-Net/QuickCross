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

        public SampleAppApplication(ISampleAppNavigator navigator, TaskScheduler uiTaskScheduler = null)
            : base(uiTaskScheduler)
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

		/* TODO: For each viewmodel, add a public property with a private setter like this:
        public _VIEWNAME_ViewModel _VIEWNAME_ViewModel { get; private set; }
         * Note that the New-View and New-ViewModel commands add the above code automatically (see http://github.com/MacawNL/MvvmQuickCross#new-viewmodel). */

        public void ContinueToSampleItemList()
        {
            if (SampleItemListViewModel == null) SampleItemListViewModel = new SampleItemListViewModel(_itemService);
            SampleItemListViewModel.Refresh();
            RunOnUIThread(() => _navigator.NavigateToSampleItemListView());
        }

		public void ContinueToSampleItem(SampleItem item = null)
        {
            if (SampleItemViewModel == null) SampleItemViewModel = new SampleItemViewModel(_itemService);
			SampleItemViewModel.Initialize(item);
			RunOnUIThread(() => _navigator.NavigateToSampleItemView());
        }

		/* TODO: For each view, add a method (with any parameters needed) to initialize its viewmodel
         * and then navigate to the view using the navigator, like this:

        public void ContinueTo_VIEWNAME_()
        {
            if (_VIEWNAME_ViewModel == null) _VIEWNAME_ViewModel = new _VIEWNAME_ViewModelDesign(); // TODO: Once _VIEWNAME_ViewModel has runtime data, instantiate that instead of _VIEWNAME_ViewModelDesign
            // Any actions to update the viewmodel go here
            RunOnUIThread(() => _navigator.NavigateTo_VIEWNAME_View());
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/MvvmQuickCross#new-view). */
	}
}
