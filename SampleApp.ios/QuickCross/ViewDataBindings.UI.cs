using System;
using System.Collections;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
#if __DIALOG__
using MonoTouch.Dialog;
#endif

namespace QuickCross
{
    public partial class ViewDataBindings
    {
        #region View types that support command binding

        private void AddCommandHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.ContainingObject == null) return;
            var view = binding.ViewProperty.ContainingObject;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
#if __DIALOG__
                case "MonoTouch.Dialog.StringElement":
                case "MonoTouch.Dialog.StyledStringElement":
                    ((StringElement)view).Tapped += () => ExecuteCommand(binding);
                    break;
#endif
                case "MonoTouch.UIKit.UIButton":
					{
						var button = (UIButton)view;
						button.TouchUpInside += HandleTouchUpInside;
						var command = (RelayCommand)binding.ViewModelPropertyInfo.GetValue(viewModel);
						if (command != null)
						{
							command.CanExecuteChanged += binding.Command_CanExecuteChanged;
							button.Enabled = command.IsEnabled;
						}
					}
					break;
                case "MonoTouch.UIKit.UIBarButtonItem":
                    {
                        var button = (UIBarButtonItem)view;
                        button.Clicked += (s, e) => ExecuteCommand(binding);
                        var command = (RelayCommand)binding.ViewModelPropertyInfo.GetValue(viewModel);
                        if (command != null)
                        {
                            command.CanExecuteChanged += binding.Command_CanExecuteChanged;
                            button.Enabled = command.IsEnabled;
                        }
                    }
                    break;
            }
        }

        void HandleTouchUpInside (object sender, EventArgs e)
        {
			var view = (UIView)sender;
			var binding = FindBindingForView(view);
			if (binding != null)
			{
				object parameter = null;
				ExecuteCommand(binding, parameter);
			}
        }

        private void RemoveCommandHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.ContainingObject == null) return;
            var view = binding.ViewProperty.ContainingObject;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
				case "MonoTouch.UIKit.UIButton":
					{
						var command = (RelayCommand)binding.ViewModelPropertyInfo.GetValue(viewModel);
						if (command != null) command.CanExecuteChanged -= binding.Command_CanExecuteChanged;
						((UIButton)view).TouchUpInside -= HandleTouchUpInside;
					}
					break;
            }
        }

        #endregion View types that support command binding

        #region View types that support one-way data binding

        public static Dictionary<string, string> ViewDefaultPropertyOrFieldName = new Dictionary<string, string>
        { // Key: full type name of view, Value: name of a property or field on the view
            { "MonoTouch.UIKit.UILabel"             , "Text" },
            { "MonoTouch.UIKit.UITextField"         , "Text" },
            { "MonoTouch.UIKit.UITextView"          , "Text" },
            { "MonoTouch.UIKit.UINavigationItem"    , "Title"},
 
            #if __DIALOG__
            { "MonoTouch.Dialog.EntryElement"       , "Value" },
            { "MonoTouch.Dialog.BooleanElement"     , "Value" },
            { "MonoTouch.Dialog.BooleanImageElement", "Value" },
            { "MonoTouch.Dialog.CheckboxElement"    , "Value" },
            { "MonoTouch.Dialog.DateElement"        , "DateValue" },
            { "MonoTouch.Dialog.DateTimeElement"    , "DateValue" },
            { "MonoTouch.Dialog.FloatElement"       , "Value" },
            { "MonoTouch.Dialog.ImageElement"       , "Value" },
            { "MonoTouch.Dialog.RadioElement"       , "Value" },
            { "MonoTouch.Dialog.RootElement"        , "RadioSelected" },
            { "MonoTouch.Dialog.StringElement"      , "Value" },
            { "MonoTouch.Dialog.StyledStringElement", "Value" },
            { "MonoTouch.Dialog.TimeElement"        , "DateValue" }
            #endif
        };

        public static void UpdateView(PropertyReference viewProperty, object value)
        {
            if (viewProperty == null || viewProperty.ContainingObject == null) return;

            if (!string.IsNullOrEmpty(viewProperty.PropertyOrFieldName))
            {
                viewProperty.Value = value;
                return;
            }

            var view = viewProperty.ContainingObject;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
				default:
					if (view is UITableView)
					{
						var tableView = (UITableView)view;
						var source = tableView.Source as DataBindableUITableViewSource;
						if (source != null)
						{
							var indexPath = source.GetIndexPath(value);
							tableView.SelectRow(indexPath, true, UITableViewScrollPosition.Middle);
						}
					}
                    else throw new NotImplementedException(string.Format("Unsupported view type {0} for UpdateView(). Add the type to either ViewDefaultPropertyOrFieldName or UpdateView()", viewTypeName));
					break;
            }
        }

        #endregion View types that support one-way data binding

        #region View types that support two-way data binding

		private NSObject textFieldTextDidChangeObserver;

		private void AddTwoWayHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.ContainingObject == null) return;
            var view = binding.ViewProperty.ContainingObject;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
				case "MonoTouch.UIKit.UITextField":
					if (textFieldTextDidChangeObserver == null) {
						textFieldTextDidChangeObserver = NSNotificationCenter.DefaultCenter.AddObserver(
							UITextField.TextFieldTextDidChangeNotification, HandleTextFieldTextDidChangeNotification
						);
					}
					break;
				case "MonoTouch.UIKit.UITextView" : ((UITextView)view).Changed += HandleTextViewChanged; 
                    break;

                #if __DIALOG__
                case "MonoTouch.Dialog.EntryElement": {
                        var element = (EntryElement)view;
                        element.Changed += (s, e) => binding.UpdateViewModel(viewModel, element.Value);
                    }
                    break;
                case "MonoTouch.Dialog.BooleanElement":
                case "MonoTouch.Dialog.BooleanImageElement":
                    {
                        var element = (BoolElement)view;
                        element.ValueChanged += (s, e) => binding.UpdateViewModel(viewModel, element.Value);
                    }
                    break;
                case "MonoTouch.Dialog.CheckboxElement":
                    {
                        var element = (CheckboxElement)view;
                        element.Tapped += () => binding.UpdateViewModel(viewModel, element.Value);
                    }
                    break;
                case "MonoTouch.Dialog.DateTimeElement":
                case "MonoTouch.Dialog.DateElement":
                case "MonoTouch.Dialog.TimeElement":
                    {
                        var element = (DateTimeElement)view;
                        element.DateSelected += (d) => binding.UpdateViewModel(viewModel, d.DateValue);
                    }
                    break;
                case "MonoTouch.Dialog.RootElement":
                    {
                        var rootElement = (RootElement)view;
                        foreach (var section in rootElement)
                        {
                            foreach (var element in section.Elements)
                            {
                                var radioElement = element as RadioElement;
                                if (radioElement != null)
                                {
                                    radioElement.Tapped += () =>
                                    {
                                        if (binding.ViewModelListPropertyInfo != null)
                                        {
                                            var list = binding.ViewModelListPropertyInfo.GetValue(viewModel) as IList;
                                            int i = rootElement.RadioSelected;
                                            if (list != null && i >= 0 && i < list.Count) binding.UpdateViewModel(viewModel, list[i]);
                                        }
                                    };
                                }
                            }
                        }
                    }
                    break;
                #endif

                default: 
					if (view is UITableView) break;
					throw new NotImplementedException("View type not implemented: " + viewTypeName);
            }
        }

		private void HandleTextFieldTextDidChangeNotification(NSNotification notification)
		{
			UITextField view = (UITextField)notification.Object;
			var binding = FindBindingForView(view);
			if (binding != null) binding.UpdateViewModel(viewModel, view.Text);
		}

        void HandleTextViewChanged (object sender, EventArgs e)
        {
			var view = (UITextView)sender;
			var binding = FindBindingForView(view);
            if (binding != null) binding.UpdateViewModel(viewModel, view.Text);
        }

        private void RemoveTwoWayHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.ContainingObject == null) return;
            var view = binding.ViewProperty.ContainingObject;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
				case "MonoTouch.UIKit.UITextField":
					 if (textFieldTextDidChangeObserver != null)	{
					     NSNotificationCenter.DefaultCenter.RemoveObserver(textFieldTextDidChangeObserver);
						 textFieldTextDidChangeObserver = null;
					 }
					 break;
				case "MonoTouch.UIKit.UITextView" : ((UITextView)view).Changed -= HandleTextViewChanged; break;
            }
        }

        #endregion View types that support two-way data binding
    }
}