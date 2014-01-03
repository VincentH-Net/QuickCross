#if TEMPLATE // To add an application class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace QCTest1 with the application name.
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


        /* TODO: For each viewmodel, add a public property with a private setter like this:
        public _VIEWNAME_ViewModel _VIEWNAME_ViewModel { get; private set; }
         * Note that the New-View and New-ViewModel commands add the above code automatically (see http://github.com/MacawNL/QuickCross#new-viewmodel). */

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

#endif // TEMPLATE