namespace SampleApp.Shared
{
    public interface ISampleAppNavigator
    {
        void NavigateToSampleItemListView();
        void NavigateToSampleItemView();

		/* TODO: For each view, add a method to navigate to that view with a signature like this:
        void NavigateTo_VIEWNAME_View();
         * Note that the New-View command adds the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
	}
}
