#if TEMPLATE // To add a navigator interface class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
namespace QuickCross.Templates
{
    public interface I_APPNAME_Navigator : QuickCross.INavigator
    {
        void NavigateToPreviousView();

        /* TODO: For each view, add a method to navigate to that view with a signature like this:
        void NavigateTo_VIEWNAME_View();
         * DO NOT REMOVE this comment; the New-View command uses this to add the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}

namespace QuickCross
{
    /// <summary>
    /// This interface has no members, but serves to detect whether an object is a navigator in QuickCross code,
    /// without needing a dependency on the application specific QuickCross.Templates namespace.
    /// </summary>
    public interface INavigator { }
}

#endif // TEMPLATE