using System.Collections.Specialized;

using Android.App;
using Android.Views;

namespace QuickCross
{
    public class FragmentViewBase : Fragment
    {
        private bool areHandlersAdded;

        /// <summary>
        /// Call Initialize() in the OnCreateView method of a derived view class to ensure handlers are added.
        /// </summary>
        protected void Initialize() 
        {
            EnsureHandlersAreAdded();
        }

        /// <summary>
        /// Override this method in a derived view class to register additional event handlers for your view. Always call base.AddHandlers() in your override.
        /// </summary>
        protected virtual void AddHandlers() { }

        /// <summary>
        /// Override this method in a derived view class to unregister additional event handlers for your view. Always call base.AddHandlers() in your override.
        /// </summary>
        protected virtual void RemoveHandlers() { }

        public override void OnDestroyView()
        {
            EnsureHandlersAreRemoved();
            base.OnDestroyView();
        }

        public override void OnPause() 
        { 
            EnsureHandlersAreRemoved(); 
            base.OnPause();
        }

        public override void OnResume() 
        { 
            base.OnResume(); 
            EnsureHandlersAreAdded(); 
        }

        private void EnsureHandlersAreAdded()
        {
            if (areHandlersAdded) return;
            AddHandlers();
            areHandlersAdded = true;
        }

        private void EnsureHandlersAreRemoved()
        {
            if (!areHandlersAdded) return;
            RemoveHandlers();
            areHandlersAdded = false;
        }
    }


    public class FragmentViewBase<ViewModelType> : FragmentViewBase, ViewDataBindings.IViewExtensionPoints where ViewModelType : ViewModelBase
    {
        private bool isJustInitialized;
        protected ViewModelType ViewModel { get; private set; }
        protected ViewDataBindings Bindings { get; private set; }

        /// <summary>
        /// Call Initialize() in the OnCreateView method of a derived view class to create the data bindings and update the view with the current view model values.
        /// </summary>
        /// <param name="rootView">The view that should display the viewModel</param>
        /// <param name="viewModel">The view model</param>
        /// <param name="layoutInflater">The LayoutInflater that you got as parameter of the OnCreateView method</param>
        /// <param name="bindingsParameters">Optional binding parameters; use to override default parameter values for specific bindings, or as an alternative for specifying binding parameters in the view tag attribute in AXML. Note that any binding parameters specified in the tag attribute wil override bindingsParameters.</param>
        /// <param name="idPrefix">The name prefix used to match view Id to property name. Default value is the root view class name + "_"</param>
        protected void Initialize(View rootView, ViewModelType viewModel, LayoutInflater layoutInflater, BindingParameters[] bindingsParameters = null, string idPrefix = null)
        {
            Bindings = new ViewDataBindings(rootView, viewModel, layoutInflater, idPrefix ?? this.GetType().Name + "_", this);
            ViewModel = viewModel;
            base.Initialize();
            Bindings.AddBindings(bindingsParameters); // First add any bindings that were specified in code 
            Bindings.EnsureCommandBindings();  // Then add any command bindings that were not specified in code (based on the Id naming convention)
            ViewModel.RaisePropertiesChanged(); // Finally add any property bindings that were not specified in code (based on the Id naming convention), and update the root view with the current property values
            isJustInitialized = true;
        }

        public override void OnPause()
        {
            if (ViewModel != null) ViewModel.OnUserInteractionStopped();
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            if (isJustInitialized) isJustInitialized = false; else Bindings.UpdateView(findViews: false);
        }

        /// <summary>
        /// Override this method in a derived view class to register additional event handlers for your view. Always call base.AddHandlers() in your override.
        /// </summary>
        protected override void AddHandlers()
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Bindings.AddHandlers();
        }

        /// <summary>
        /// Override this method in a derived view class to unregister additional event handlers for your view. Always call base.AddHandlers() in your override.
        /// </summary>
        protected override void RemoveHandlers()
        {
            Bindings.RemoveHandlers();
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        /// <summary>
        /// Override this method in a derived view class to handle changes for specific properties in custom code instead of through data binding.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            Bindings.UpdateView(propertyName);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Override this method in a derived view class to change how a data-bound value is set for specific views
        /// </summary>
        /// <param name="viewProperty"></param>
        /// <param name="value"></param>
        public virtual void UpdateView(PropertyReference viewProperty, object value)
        {
            ViewDataBindings.UpdateView(viewProperty, value);
        }

        /// <summary>
        /// Override this method in a derived view class to react to changes in lists that implement INotifyCollectionChanged (e.g. ObservableCollection) that are data-bound in that view
        /// </summary>
        /// <param name="sender">The ObservableCollection that was changed</param>
        /// <param name="e">See http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html for details</param>
        public virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }
    }
}