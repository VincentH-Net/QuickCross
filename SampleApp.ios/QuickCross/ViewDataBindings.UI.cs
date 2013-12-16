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
                default:
                    break;
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
						throw new NotImplementedException("View type not implemented: " + viewTypeName);
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
				default: throw new NotImplementedException("View type not implemented: " + viewTypeName);
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
				default: throw new NotImplementedException("View type not implemented: " + viewTypeName);
            }
        }

        #endregion View types that support two-way data binding
    }
}