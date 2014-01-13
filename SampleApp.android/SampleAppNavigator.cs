using System;

using Android.Content;
using QuickCross;
using SampleApp.Shared;


namespace SampleApp
{
    sealed class SampleAppNavigator : ISampleAppNavigator
    {
        private static readonly Lazy<SampleAppNavigator> lazy = new Lazy<SampleAppNavigator>(() => new SampleAppNavigator());

        public static SampleAppNavigator Instance { get { return lazy.Value; } }

        private SampleAppNavigator() { }

        public Context NavigationContext { get; set; }

        private void Navigate(Type type)
        {
            if (NavigationContext == null) return;
            if (AndroidHelpers.CurrentActivity != null && AndroidHelpers.CurrentActivity.GetType() == type) return;
            NavigationContext.StartActivity(type);
        }

        public void NavigateToSampleItemListView()
        {
            Navigate(typeof(SampleItemListView));
        }

        public void NavigateToSampleItemView()
        {
            Navigate(typeof(SampleItemView));
        }
    }
}
