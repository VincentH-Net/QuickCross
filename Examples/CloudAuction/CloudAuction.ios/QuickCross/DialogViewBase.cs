using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CloudAuction;
using CloudAuction.Shared;

namespace QuickCross
{
    public class DialogViewBase : DialogViewController, ViewDataBindings.IViewExtensionPoints
    {
        public DialogViewBase(IntPtr handle)    : base(handle) { }
        public DialogViewBase(RootElement root, bool pushing = false) : base(root, pushing) { }

		private bool areHandlersAdded;
		private ViewModelBase viewModel;

		protected ViewDataBindings Bindings { get; private set; }

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (!(sender is CloudAuctionNavigator) && viewModel != null)
			{
				string commandName = segue.Identifier;
				if (viewModel.ExecuteCommand(commandName, GetCommandParameter(commandName))) return;
			} 
			base.PrepareForSegue(segue, sender);
		}

		/// <summary>
		/// Override this method in a derived view class to register additional event handlers for your view. Always call base.AddHandlers() in your override.
		/// </summary>
		protected virtual void AddHandlers() 
		{
			viewModel.PropertyChanged += ViewModel_PropertyChanged;
			Bindings.AddHandlers();
		}

		/// <summary>
		/// Override this method in a derived view class to unregister additional event handlers for your view. Always call base.AddHandlers() in your override.
		/// </summary>
		protected virtual void RemoveHandlers() 
		{ 
			Bindings.RemoveHandlers();
			viewModel.PropertyChanged -= ViewModel_PropertyChanged;
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

		protected virtual object GetCommandParameter(string commandName) { return null; }

		/// <summary>
		/// Call InitializeBindings() in the ViewDidLoad method of a derived view class to create the data bindings and update the view with the current view model values.
		/// </summary>
		/// <param name="rootView">The view that should display the viewModel</param>
		/// <param name="viewModel">The view model</param>
		/// <param name="bindingsParameters">Optional binding parameters; use to override default parameter values for specific bindings, or as an alternative for specifying binding parameters in the view tag attribute in AXML. Note that any binding parameters specified in the tag attribute wil override bindingsParameters.</param>
		/// <param name="idPrefix">The name prefix used to match view Id to property name. Default value is the root view class name + "_"</param>
		protected void InitializeBindings(UIView rootView, ViewModelBase viewModel, BindingParameters[] bindingsParameters = null, string idPrefix = null)
		{
			Bindings = new ViewDataBindings(viewModel, idPrefix ?? this.GetType().Name + "_", this);
			this.viewModel = viewModel;

			EnsureHandlersAreAdded();

			Bindings.AddBindings(bindingsParameters); // First add any bindings that were specified in code
			// TODO: Bindings.EnsureCommandBindings();  // Then add any command bindings that were not specified in code (based on the Id naming convention)

			List<BindingParameters> bindingParametersList;
			if (ViewDataBindings.RootViewBindingParameters.TryGetValue(rootView, out bindingParametersList))
			{
				Console.WriteLine("Adding bindings from markup ...");
				ViewDataBindings.RootViewBindingParameters.Remove(rootView); // Remove the static reference to the views to prevent memory leaks. Note that if we would want to recreate the bindings later, we could also store the parameters list in the bindings.
				Bindings.AddBindings(bindingParametersList.ToArray());
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			EnsureHandlersAreAdded();
			viewModel.RaisePropertiesChanged(); // Update the root view with the current property values
		}

		public override void ViewDidDisappear(bool animated)
		{
			EnsureHandlersAreRemoved();
			base.ViewDidDisappear(animated);
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
        public virtual void UpdateView(InstanceProperty viewProperty, object value)
		{
            ViewDataBindings.UpdateView(viewProperty, value);
		}

		/// <summary>
		/// Override this method in a derived view class to react to changes in lists that implement INotifyCollectionChanged (e.g. ObservableCollection) that are data-bound in that view
		/// </summary>
		/// <param name="sender">The ObservableCollection that was changed</param>
		/// <param name="e">See http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html for details</param>
		public virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }


		/// <summary>
		/// Override this method in a derived view class to supply of modify the parameter for a command in code, when the command is executed.
		/// </summary>
		/// <returns>The command parameter. Can be null.</returns>
		/// <param name="commandName">The command name</param>
		/// <param name="parameter">The command parameter as specified in the binding, or null if none was specified</param>
		public virtual object GetCommandParameter(string commandName, object parameter = null) { return parameter; }
    }
}

