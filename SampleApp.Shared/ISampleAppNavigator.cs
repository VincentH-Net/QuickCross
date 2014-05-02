namespace SampleApp.Shared
{
    public interface ISampleAppNavigator : QuickCross.INavigator
    {
		void NavigateToPreviousView();
		void NavigateToSampleItemListView();
        void NavigateToSampleItemView();

        /* TODO: For each view, add a method to navigate to that view with a signature like this:
        void NavigateTo_VIEWNAME_View();
         * DO NOT REMOVE this comment; the New-View command uses this to add the above code automatically (see http://github.com/MacawNL/QuickCross#new-view). */
    }

    namespace QuickCross
    {
        /// <summary>
        /// This interface has no members, but serves to detect whether an object is a navigator in QuickCross code,
        /// without needing a dependency on the application specific QuickCross.Templates namespace.
        /// </summary>
        public interface INavigator { }
    }
}
