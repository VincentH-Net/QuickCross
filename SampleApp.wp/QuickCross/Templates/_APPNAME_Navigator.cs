#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using System;
using System.Windows.Controls;
using QuickCrossLibrary.Templates;

namespace QuickCross.Templates
{
    class _APPNAME_Navigator : I_APPNAME_Navigator
    {
        private static readonly Lazy<_APPNAME_Navigator> lazy = new Lazy<_APPNAME_Navigator>(() => new _APPNAME_Navigator());

        public static _APPNAME_Navigator Instance { get { return lazy.Value; } }

        private _APPNAME_Navigator() { }

        public Frame NavigationContext { get; set; }

        private void Navigate(string uri)
        {
            var source = new Uri(uri, UriKind.Relative);
            if (NavigationContext == null || NavigationContext.CurrentSource == source) return;
            NavigationContext.Navigate(source);
        }

        /* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate("/_VIEWNAME_View.xaml");
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
#endif // TEMPLATE