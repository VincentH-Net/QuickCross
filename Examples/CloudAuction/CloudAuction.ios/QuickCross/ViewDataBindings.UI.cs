using System;
using System.Collections;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace QuickCross
{
    public partial class ViewDataBindings
    {
        #region View types that support command binding

        private void AddCommandHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.Instance == null) return;
            var view = binding.ViewProperty.Instance;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
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
                default:
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
            if (binding.ViewProperty == null || binding.ViewProperty.Instance == null) return;
            var view = binding.ViewProperty.Instance;
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
                default:
                    break;
            }
        }

        #endregion View types that support command binding

        #region View types that support one-way data binding

        public static Dictionary<string, string> ViewDefaultPropertyOrFieldName = new Dictionary<string, string>
        { // Key: full type name of view. Value: name of a property or field on the view.
            { "MonoTouch.UIKit.UILabel", "Text" },
            { "MonoTouch.UIKit.UITextField", "Text" },
            { "MonoTouch.UIKit.UITextView", "Text" }
        };

        public static void UpdateView(InstanceProperty viewProperty, object value)
        {
            if (viewProperty == null || viewProperty.Instance == null) return;
            var view = viewProperty.Instance;
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
					else
					{
                        viewProperty.Value = value.ToString();
                    }
					break;
            }
        }

        #endregion View types that support one-way data binding

        #region View types that support two-way data binding

		private NSObject textFieldTextDidChangeObserver;

		private void AddTwoWayHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.Instance == null) return;
            var view = binding.ViewProperty.Instance;
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
				case "MonoTouch.UIKit.UITextView" : ((UITextView)view).Changed += HandleTextViewChanged; break;
				default: 
					if (view is UITableView) break;
					throw new NotImplementedException("View type not implemented: " + viewTypeName);
            }
        }

		private void HandleTextFieldTextDidChangeNotification(NSNotification notification)
		{
			UITextField view = (UITextField)notification.Object;
			var binding = FindBindingForView(view);
			if (binding != null) binding.ViewModelPropertyInfo.SetValue(viewModel, view.Text);
		}

        void HandleTextViewChanged (object sender, EventArgs e)
        {
			var view = (UITextView)sender;
			var binding = FindBindingForView(view);
			if (binding != null) binding.ViewModelPropertyInfo.SetValue(viewModel, view.Text);
        }

        private void RemoveTwoWayHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.Instance == null) return;
            var view = binding.ViewProperty.Instance;
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
				case "MonoTouch.UIKit.UITextView" : ((UITextView)view).Changed       -= HandleTextViewChanged; break;
				default:
					 if (view is UITableView) break;
					 throw new NotImplementedException("View type not implemented: " + viewTypeName);
            }
        }

        #endregion View types that support two-way data binding
    }
}