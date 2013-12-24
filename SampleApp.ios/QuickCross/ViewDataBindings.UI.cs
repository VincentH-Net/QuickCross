using System;
using System.Collections;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace QuickCross
{
    public partial class ViewDataBindings
    {
        #region View types that support command binding

        private void AddCommandHandler(DataBinding binding)
        {
            var view = binding.View;
            if (view == null) return;
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
				/* TODO
				if (binding.CommandParameterListView != null)
				{
					var adapter = binding.CommandParameterListView.GetAdapter() as IDataBindableListAdapter;
					if (adapter != null)
					{
						var adapterView = binding.CommandParameterListView;
						if (adapterView is AbsListView)
						{
							var absListView = (AbsListView)adapterView;
							switch (absListView.ChoiceMode)
							{
								case ChoiceMode.Single:
									parameter = adapter.GetItemAsObject(absListView.CheckedItemPosition);
									break;
								case ChoiceMode.Multiple:
									{
										var checkedItems = new ArrayList();
										var positions = absListView.CheckedItemPositions;
										for (int i = 0; i < positions.Size(); i++)
										{
											if (positions.ValueAt(i))
											{
												int position = positions.KeyAt(i);
												checkedItems.Add(adapter.GetItemAsObject(position));
											}
										}
										if (checkedItems.Count > 0) parameter = checkedItems;
									}
									break;
							}
						}
						else
						{
							parameter = adapter.GetItemAsObject(adapterView.SelectedItemPosition);
						}
					}
				} */
				ExecuteCommand(binding, parameter);
			}
        }

        private void RemoveCommandHandler(DataBinding binding)
        {
            var view = binding.View;
            if (view == null) return;
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

		public static void UpdateView(UIView view, object value)
        {
            if (view != null)
            {
                string viewTypeName = view.GetType().FullName;
                switch (viewTypeName)
                {
                    // TODO: Add cases here for specialized view types, as needed
					case "MonoTouch.UIKit.UILabel":
						((UILabel)view).Text = value.ToString();
						break;
					case "MonoTouch.UIKit.UITextField":
						{
							var textField = (UITextField)view;
							string text = value.ToString();
							if (textField.Text != text) textField.Text = text;
						}
						break;
					case "MonoTouch.UIKit.UITextView":
						{
							var textView = (UITextView)view;
							string text = value.ToString();
							if (textView.Text != text) textView.Text = text;
						}
						break;
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
							throw new NotImplementedException("View type not implemented: " + viewTypeName);
						}
						break;
                }
            }
        }

        #endregion View types that support one-way data binding

        #region View types that support two-way data binding

		private NSObject textFieldTextDidChangeObserver;

		private void AddTwoWayHandler(DataBinding binding)
        {
            var view = binding.View;
            if (view == null) return;
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
            var view = binding.View;
            if (view == null) return;
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