using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace QuickCross
{
	public class ViewControllerBase : UIViewController, ViewDataBindings.ViewExtensionPoints
    {
		public ViewControllerBase(IntPtr handle) : base(handle)
        {
        }

		private ViewModelBase viewModel;

		protected ViewDataBindings Bindings { get; private set; }

		/// <summary>
		/// Call Initialize() in the OnCreate method of a derived view class to create the data bindings and update the view with the current view model values.
		/// </summary>
		/// <param name="rootView">The view that should display the viewModel</param>
		/// <param name="viewModel">The view model</param>
		/// <param name="bindingsParameters">Optional binding parameters; use to override default parameter values for specific bindings, or as an alternative for specifying binding parameters in the view tag attribute in AXML. Note that any binding parameters specified in the tag attribute wil override bindingsParameters.</param>
		/// <param name="idPrefix">The name prefix used to match view Id to property name. Default value is the root view class name + "_"</param>
		protected void InitializeBindings(UIView rootView, ViewModelBase viewModel, BindingParameters[] bindingsParameters = null, string idPrefix = null)
		{
			Bindings = new ViewDataBindings(rootView, viewModel, idPrefix ?? this.GetType().Name + "_");
			this.viewModel = viewModel;

			// TODO: base.Initialize();
			viewModel.PropertyChanged += ViewModel_PropertyChanged;


			Bindings.AddBindings(bindingsParameters); // First add any bindings that were specified in code
			// TODO: Bindings.EnsureCommandBindings();  // Then add any command bindings that were not specified in code (based on the Id naming convention)

			List<BindingParameters> bindingParametersList;
			if (ViewDataBindings.RootViewBindingParameters.TryGetValue(rootView, out bindingParametersList))
			{
				Console.WriteLine("Adding bindings from markup ...");
				Bindings.AddBindings(bindingParametersList.ToArray());
			}

			this.viewModel.RaisePropertiesChanged(); // Finally add any property bindings that were not specified in code (based on the Id naming convention), and update the root view with the current property values
			//isJustInitialized = true;
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
		/// <param name="value"></param>
		public virtual void UpdateView(UIView view, object value)
		{
			ViewDataBindings.UpdateView(view, value);
		}

		/// <summary>
		/// Override this method in a derived view class to react to changes in lists that implement INotifyCollectionChanged (e.g. ObservableCollection) that are data-bound in that view
		/// </summary>
		/// <param name="sender">The ObservableCollection that was changed</param>
		/// <param name="e">See http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html for details</param>
		public virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

    }
}

