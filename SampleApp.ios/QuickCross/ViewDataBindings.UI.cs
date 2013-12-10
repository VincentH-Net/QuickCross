using System;
using System.Collections;
using MonoTouch.UIKit;

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
					case "MonoTouch.UIKit.UITextView":
						{
							var v = (UITextView)view;
							string text = value.ToString();
							if (v.Text != text) v.Text = text;
						}
						break;
                    default:
						throw new NotImplementedException("View type not implemented: " + viewTypeName);
                }
            }
        }

        #endregion View types that support one-way data binding

        #region View types that support two-way data binding

        private void AddTwoWayHandler(DataBinding binding)
        {
            var view = binding.View;
            if (view == null) return;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
                default:
                    throw new NotImplementedException("View type not implemented: " + viewTypeName);
            }
        }

        private void RemoveTwoWayHandler(DataBinding binding)
        {
            var view = binding.View;
            if (view == null) return;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
                default:
                    throw new NotImplementedException("View type not implemented: " + viewTypeName);
            }
        }

        #endregion View types that support two-way data binding
    }
}