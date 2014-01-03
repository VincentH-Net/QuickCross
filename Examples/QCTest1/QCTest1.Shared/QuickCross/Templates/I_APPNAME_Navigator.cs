#if TEMPLATE // To add a navigator interface class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace QCTest1 with the application name.
namespace QCTest1.Shared
{
    public interface IQCTest1Navigator
    {

        /* TODO: For each view, add a method to navigate to that view with a signature like this:
        void NavigateTo_VIEWNAME_View();
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
#endif // TEMPLATE