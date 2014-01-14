using SampleApp.Shared;
using System;
using Windows.UI.Xaml.Controls;

namespace SampleApp
{
    class SampleAppNavigator : ISampleAppNavigator
    {
        private static readonly Lazy<SampleAppNavigator> lazy = new Lazy<SampleAppNavigator>(() => new SampleAppNavigator());

        public static SampleAppNavigator Instance { get { return lazy.Value; } }

        private SampleAppNavigator() { }

        public Frame NavigationContext { get; set; }

        private void Navigate(Type sourcePageType)
        {
            if (NavigationContext == null || NavigationContext.CurrentSourcePageType == sourcePageType) return;
            NavigationContext.Navigate(sourcePageType);
        }

        public void NavigateToSampleItemListView()
        {
            Navigate(typeof(SampleItemListView));
        }

        public void NavigateToSampleItemView()
        {
            Navigate(typeof(SampleItemView));
        }

        /* TODO: For each view, add a method to navigate to that view like this:

        public void NavigateTo_VIEWNAME_View()
        {
            Navigate(typeof(_VIEWNAME_View));
        }
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */

        
    }
}
