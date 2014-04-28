#if TEMPLATE // To add a navigator class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "Install-Mvvm". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ with the application name.
using System;

using Android.Content;
using Android.App;
using QuickCross;
using QuickCrossLibrary.Templates;
using System.Collections.Generic;

namespace QuickCross.Templates
{
    class _APPNAME_Navigator : I_APPNAME_Navigator
    {
        private static readonly Lazy<_APPNAME_Navigator> lazy = new Lazy<_APPNAME_Navigator>(() => new _APPNAME_Navigator());
		
        public static _APPNAME_Navigator Instance { get { return lazy.Value; } }

        private _APPNAME_Navigator() { }

        public Context NavigationContext { get; set; }

        #region Generic navigation helpers

        private void Navigate(Type type)
        {
            if (type.IsSubclassOf(typeof(DialogFragmentViewBase)))
            {
                if (AndroidHelpers.CurrentActivity == null) return;
                var dialogFragment = (DialogFragment)Activator.CreateInstance(type);
                dialogFragment.Show(AndroidHelpers.CurrentActivity.FragmentManager, type.Name);
                return;
            }

            if (NavigationContext == null) return;
            if (AndroidHelpers.CurrentActivity != null && AndroidHelpers.CurrentActivity.GetType() == type) return;
			NavigationContext.StartActivity(type);
        }

        #endregion Generic navigation helpers

        public void NavigateToPreviousView()
        {
            if (AndroidHelpers.CurrentActivity != null) AndroidHelpers.CurrentActivity.Finish();
        }

        /* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate(typeof(_VIEWNAME_View));
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
#endif // TEMPLATE