#if TEMPLATE // To add an application class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using System;
using System.Threading.Tasks;
using QuickCross;
using QuickCross.Templates.ViewModels;
using QuickCross.Templates.ViewModels.Design;

namespace QuickCross.Templates
{
    public sealed class _APPNAME_Application : ApplicationBase
    {
        private I_APPNAME_Navigator _navigator;

        public _APPNAME_Application(I_APPNAME_Navigator navigator, object currentNavigationContext = null, TaskScheduler uiTaskScheduler = null)
            : base(currentNavigationContext, uiTaskScheduler)
        {
            // Services that have a platform-specific implementation, such as the navigator,
            // are instantiated in a platform-specific project and passed to this application 
            // as a cross-platform interface.
            _navigator = navigator;

            // TODO: Create instances for all services that have a cross-platform implementation
        }

        new public static _APPNAME_Application Instance { get { return (_APPNAME_Application)ApplicationBase.Instance; } }


        /* TODO: For each viewmodel, add a public property with a private setter like this:
        public _VIEWNAME_ViewModel _VIEWNAME_ViewModel { get; private set; }
         * Note that the New-View and New-ViewModel commands add the above code automatically (see http://github.com/MacawNL/QuickCross#new-viewmodel). */

        /* TODO: For each view, add a method (with any parameters needed) to initialize its viewmodel
         * and then navigate to the view using the navigator, like this:

        public void ContinueTo_VIEWNAME_(bool skipNavigation = false)
        {
            if (_VIEWNAME_ViewModel == null) _VIEWNAME_ViewModel = new _VIEWNAME_ViewModelDesign(); // TODO: Once _VIEWNAME_ViewModel has runtime data, instantiate that instead of _VIEWNAME_ViewModelDesign
            // Any actions to update the viewmodel go here
            if (!skipNavigation) RunOnUIThread(() => _navigator.NavigateTo_VIEWNAME_View(CurrentNavigationContext));
        }
         * The skipNavigation parameter is needed in cases where the OS has already navigated to the view for you;
         * in that case you only need to initialize the viewmodel.
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}

#endif // TEMPLATE