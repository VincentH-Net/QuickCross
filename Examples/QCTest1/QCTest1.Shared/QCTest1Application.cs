using System;
using System.Threading.Tasks;
using QuickCross;
using QCTest1.Shared.ViewModels;
using QCTest1.Shared.ViewModels.Design;

namespace QCTest1.Shared
{
    public sealed class QCTest1Application : ApplicationBase
    {
        private IQCTest1Navigator _navigator;

        public QCTest1Application(IQCTest1Navigator navigator, TaskScheduler uiTaskScheduler = null)
            : base(uiTaskScheduler)
        {
            // Services that have a platform-specific implementation, such as the navigator,
            // are instantiated in a platform-specific project and passed to this application 
            // as a cross-platform interface.
            _navigator = navigator;

            // TODO: Create instances for all services that have a cross-platform implementation
        }

        new public static QCTest1Application Instance { get { return (QCTest1Application)ApplicationBase.Instance; } }

        public MainViewModel MainViewModel { get; private set; }
        public ProductListViewModel ProductListViewModel { get; private set; }
        public TestXib2ViewModel TestXib2ViewModel { get; private set; }

        /* TODO: For each viewmodel, add a public property with a private setter like this:
        public _VIEWNAME_ViewModel _VIEWNAME_ViewModel { get; private set; }
         * Note that the New-View and New-ViewModel commands add the above code automatically (see http://github.com/MacawNL/QuickCross#new-viewmodel). */

        public void ContinueToMain()
        {
            if (MainViewModel == null) MainViewModel = new MainViewModelDesign(); // TODO: Once MainViewModel has runtime data, instantiate that instead of MainViewModelDesign
            // Any actions to update the viewmodel go here
            RunOnUIThread(() => _navigator.NavigateToMainView());
        }

        public void ContinueToProductList()
        {
            if (ProductListViewModel == null) ProductListViewModel = new ProductListViewModelDesign(); // TODO: Once ProductListViewModel has runtime data, instantiate that instead of ProductListViewModelDesign
            // Any actions to update the viewmodel go here
            RunOnUIThread(() => _navigator.NavigateToProductListView());
        }

        public void ContinueToTestXib2()
        {
            if (TestXib2ViewModel == null) TestXib2ViewModel = new TestXib2ViewModelDesign(); // TODO: Once TestXib2ViewModel has runtime data, instantiate that instead of TestXib2ViewModelDesign
            // Any actions to update the viewmodel go here
            RunOnUIThread(() => _navigator.NavigateToTestXib2View());
        }

        /* TODO: For each view, add a method (with any parameters needed) to initialize its viewmodel
         * and then navigate to the view using the navigator, like this:

        public void ContinueTo_VIEWNAME_()
        {
            if (_VIEWNAME_ViewModel == null) _VIEWNAME_ViewModel = new _VIEWNAME_ViewModelDesign(); // TODO: Once _VIEWNAME_ViewModel has runtime data, instantiate that instead of _VIEWNAME_ViewModelDesign
            // Any actions to update the viewmodel go here
            RunOnUIThread(() => _navigator.NavigateTo_VIEWNAME_View());
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
