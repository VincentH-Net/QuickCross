using SampleApp.Shared;
using System;
using Windows.UI.Xaml.Controls;

namespace SampleApp
{
    class SampleAppNavigator : ISampleAppNavigator
    {
        private void Navigate(object navigationContext, Type sourcePageType)
        {
            ((Frame)navigationContext).Navigate(sourcePageType);
        }

        public void NavigateToSampleItemListView(object navigationContext)
        {
            Navigate(navigationContext, typeof(SampleItemListView));
        }

        public void NavigateToSampleItemView(object navigationContext)
        {
            Navigate(navigationContext, typeof(SampleItemView));
        }
    }
}
