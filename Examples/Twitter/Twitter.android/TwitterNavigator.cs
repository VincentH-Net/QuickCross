using System;

using Android.Content;
using QuickCross;
using Twitter.Shared;

namespace Twitter
{
    class TwitterNavigator : ITwitterNavigator
    {
        private static readonly Lazy<TwitterNavigator> lazy = new Lazy<TwitterNavigator>(() => new TwitterNavigator());

        public static TwitterNavigator Instance { get { return lazy.Value; } }

        private TwitterNavigator() { }

        public Context NavigationContext { get; set; }

        private void Navigate(Type type)
        {
            if (NavigationContext == null) return;
            if (AndroidHelpers.CurrentActivity != null && AndroidHelpers.CurrentActivity.GetType() == type) return;
            NavigationContext.StartActivity(type);
        }

        public void NavigateToMainView()
        {
            Navigate(typeof(MainView));
        }

        /* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate(typeof(_VIEWNAME_View));
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
