using System.Threading.Tasks;
using QuickCross;
using SampleApp.Shared.Models;
using SampleApp.Shared.Services;
using SampleApp.Shared.ViewModels;
using SampleApp.Shared.ViewModels.Design;
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
         * DO NOT REMOVE this comment; the New-View and New-ViewModel commands use this to add the above code automatically (see http://github.com/MacawNL/QuickCross#new-viewmodel). */

        public void ReturnToPreviousView()
		{
			RunOnUIThread(() => _navigator.NavigateToPreviousView());
		}

		public void ContinueToSampleItemList()
        {
            if (SampleItemListViewModel == null) SampleItemListViewModel = UseDesignViewModels ? new SampleItemListViewModelDesign() : new SampleItemListViewModel(_itemService);
            SampleItemListViewModel.Refresh();
            RunOnUIThread(() => _navigator.NavigateToSampleItemListView());
        }

		public void ContinueToSampleItem(SampleItem item = null)
        {
            if (SampleItemViewModel == null) SampleItemViewModel = UseDesignViewModels ? new SampleItemViewModelDesign() : new SampleItemViewModel(_itemService);
			SampleItemViewModel.Initialize(item);
			RunOnUIThread(() => _navigator.NavigateToSampleItemView());
        }

        /* TODO: For each view, add a method (with any parameters needed) to initialize its viewmodel
         * and then navigate to the view using the navigator, like this:

        public void ContinueTo_VIEWNAME_()
        {
            if (_VIEWNAME_ViewModel == null) _VIEWNAME_ViewModel = UseDesignViewModels ? new _VIEWNAME_ViewModelDesign() : new _VIEWNAME_ViewModel();
            // Any actions to update the viewmodel go here
            RunOnUIThread(() => _navigator.NavigateTo_VIEWNAME_View());
        }
         * DO NOT REMOVE this comment; the New-View command uses this to add the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
