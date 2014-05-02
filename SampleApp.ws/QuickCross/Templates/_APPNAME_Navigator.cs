#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using System;
using Windows.UI.Xaml.Controls;
using QuickCrossLibrary.Templates;

namespace QuickCross.Templates
{
    class _APPNAME_Navigator : I_APPNAME_Navigator
    {
        private static readonly Lazy<_APPNAME_Navigator> lazy = new Lazy<_APPNAME_Navigator>(() => new _APPNAME_Navigator());

        public static _APPNAME_Navigator Instance { get { return lazy.Value; } }

        private _APPNAME_Navigator() { }

        public Frame NavigationContext { get; set; }

        #region Generic navigation helpers

        private void Navigate(Type sourcePageType)
        {
            if (NavigationContext == null || NavigationContext.CurrentSourcePageType == sourcePageType) return;
            NavigationContext.Navigate(sourcePageType);
        }

        #endregion Generic navigation helpers

        public void NavigateToPreviousView()
        {
			if (NavigationContext == null || !NavigationContext.CanGoBack) return;
			NavigationContext.GoBack();
        }

        /* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate(typeof(_VIEWNAME_View));
        }
         * DO NOT REMOVE this comment; the New-View command uses this to add the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
#endif // TEMPLATE