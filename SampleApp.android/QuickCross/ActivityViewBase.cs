using System.Collections.Specialized;

using Android.App;
using Android.Views;
using Android.OS;

namespace QuickCross
{
    public abstract class ActivityViewBase : Activity
    {
        private bool areHandlersAdded;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AndroidHelpers.SetCurrentActivity(this);
        }

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

        protected override void OnDestroy()
        {
            EnsureHandlersAreRemoved();
            AndroidHelpers.ClearActivityReference(this);
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            EnsureHandlersAreRemoved();
            AndroidHelpers.ClearActivityReference(this);
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            AndroidHelpers.SetCurrentActivity(this);
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
    
    public class ActivityViewBase<ViewModelType> : ActivityViewBase, ViewDataBindings.IViewExtensionPoints where ViewModelType : ViewModelBase
    {
        private bool isJustInitialized;
        protected ViewModelType ViewModel { get; private set; }
        protected ViewDataBindings Bindings { get; private set; }

        /// <summary>
        /// Call Initialize() in the OnCreate method of a derived view class to create the data bindings and update the view with the current view model values.
        /// </summary>
        /// <param name="rootView">The view that should display the viewModel</param>
        /// <param name="viewModel">The view model</param>
        /// <param name="bindingsParameters">Optional binding parameters; use to override default parameter values for specific bindings, or as an alternative for specifying binding parameters in the view tag attribute in AXML. Note that any binding parameters specified in the tag attribute wil override bindingsParameters.</param>
        /// <param name="idPrefix">The name prefix used to match view Id to property name. Default value is the root view class name + "_"</param>
        protected void Initialize(View rootView, ViewModelType viewModel, BindingParameters[] bindingsParameters = null, string idPrefix = null)
        {
            Bindings = new ViewDataBindings(rootView, viewModel, LayoutInflater, idPrefix ?? this.GetType().Name + "_", this);
            ViewModel = viewModel;
            base.Initialize();
            Bindings.AddBindings(bindingsParameters); // First add any bindings that were specified in code 
            Bindings.EnsureCommandBindings();  // Then add any command bindings that were not specified in code (based on the Id naming convention)
            ViewModel.RaisePropertiesChanged(); // Finally add any property bindings that were not specified in code (based on the Id naming convention), and update the root view with the current property values
            isJustInitialized = true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (isJustInitialized) isJustInitialized = false; else Bindings.UpdateView(findViews: false);
        }

        /// <summary>
        /// Override this method in a derived view class to register additional event handlers for your view. Always call base.AddHandlers() in your override.
        /// </summary>
        protected override void AddHandlers()
        {
            base.AddHandlers();
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
            base.RemoveHandlers();
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
        /// <param name="view"></param>
        /// <param name="viewProperty"></param>
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