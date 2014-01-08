using System;
using System.Threading.Tasks;
using QuickCross;
using Twitter.Shared.ViewModels;
using Twitter.Shared.ViewModels.Design;

namespace Twitter.Shared
{
    public sealed class TwitterApplication : ApplicationBase
    {
        private ITwitterNavigator _navigator;

        public TwitterApplication(ITwitterNavigator navigator, TaskScheduler uiTaskScheduler = null)
            : base(uiTaskScheduler)
        {
            // Services that have a platform-specific implementation, such as the navigator,
            // are instantiated in a platform-specific project and passed to this application 
            // as a cross-platform interface.
            _navigator = navigator;

            // TODO: Create instances for all services that have a cross-platform implementation
        }

        new public static TwitterApplication Instance { get { return (TwitterApplication)ApplicationBase.Instance; } }

        public MainViewModel MainViewModel { get; private set; }

        /* TODO: For each viewmodel, add a public property with a private setter like this:
        public _VIEWNAME_ViewModel _VIEWNAME_ViewModel { get; private set; }
         * Note that the New-View and New-ViewModel commands add the above code automatically (see http://github.com/MacawNL/QuickCross#new-viewmodel). */

        public void ContinueToMain()
        {
            if (MainViewModel == null) MainViewModel = new MainViewModelDesign(); // TODO: Once MainViewModel has runtime data, instantiate that instead of MainViewModelDesign
            // Any actions to update the viewmodel go here
            RunOnUIThread(() => _navigator.NavigateToMainView());
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
