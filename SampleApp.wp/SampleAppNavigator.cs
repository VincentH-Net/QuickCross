using System;
using System.Windows.Controls;
using SampleApp.Shared;

namespace SampleApp
{
    class SampleAppNavigator : ISampleAppNavigator
    {
        private static readonly Lazy<SampleAppNavigator> lazy = new Lazy<SampleAppNavigator>(() => new SampleAppNavigator());

        public static SampleAppNavigator Instance { get { return lazy.Value; } }

        private SampleAppNavigator() { }

        public Frame NavigationContext { get; set; }

        private void Navigate(string uri)
        {
            var source = new Uri(uri, UriKind.Relative);
            if (NavigationContext == null || NavigationContext.CurrentSource == source) return;
            NavigationContext.Navigate(source);
        }

		public void NavigateToPreviousView()
		{
			if (NavigationContext == null || !NavigationContext.CanGoBack) return;
			NavigationContext.GoBack();
		}

		public void NavigateToSampleItemListView()
        {
			Navigate("/SampleItemListView.xaml");
        }

        public void NavigateToSampleItemView()
        {
            Navigate("/SampleItemView.xaml");
        }

        /* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate("/_VIEWNAME_View.xaml");
        }
         * DO NOT REMOVE this comment; the New-View command uses this to add the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }
}
