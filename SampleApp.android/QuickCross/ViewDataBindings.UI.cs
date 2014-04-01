using System;

using Android.Views;
using Android.Widget;
using System.Collections;
using System.Collections.Generic;

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
                default:
                    if (view is AbsSpinner) ((AdapterView)view).ItemSelected += AdapterView_ItemSelected;
                    else if (view is AdapterView) ((AdapterView)view).ItemClick += AdapterView_ItemClick;
                    else if (view is View)
                    {
                        var androidView = (View)view;
                        androidView.Click += View_Click;
                        var command = (RelayCommand)binding.ViewModelPropertyInfo.GetValue(viewModel);
                        if (command != null)
                        {
                            command.CanExecuteChanged += binding.Command_CanExecuteChanged;
                            androidView.Enabled = command.IsEnabled;
                        }
                    }
                    break;
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
                default:
                    if (view is AbsSpinner) ((AdapterView)view).ItemSelected -= AdapterView_ItemSelected;
                    else if (view is AdapterView) ((AdapterView)view).ItemClick -= AdapterView_ItemClick;
                    else if (view is View)
                    {
                        var androidView = (View)view;
                        var command = (RelayCommand)binding.ViewModelPropertyInfo.GetValue(viewModel);
                        if (command != null) command.CanExecuteChanged -= binding.Command_CanExecuteChanged;
                        androidView.Click -= View_Click;
                    }
                    break;
            }
        }

        private void View_Click(object sender, EventArgs e)
        {
            var view = (View)sender;
            var binding = FindBindingForView(view);
            if (binding != null)
            {
                var command = (RelayCommand)binding.ViewModelPropertyInfo.GetValue(viewModel);
                object parameter = null;
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
                }
                command.Execute(parameter);
            }
        }

        #endregion View types that support command binding

        #region View types that support one-way data binding

        public static Dictionary<string, string> ViewDefaultPropertyOrFieldName = new Dictionary<string, string>
        { // Key: full type name of view, Value: name of a property or field on the view
            { "Android.Widget.ProgressBar", "Progress" }
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
                case "Android.Webkit.WebView":
                    {
                        var webView = (Android.Webkit.WebView)view;
                        if (value is Uri)
                        {
                            string newUrl = value.ToString();
                            if (webView.Url != newUrl) webView.LoadUrl(newUrl);
                        }
                        else
                        {
                            webView.LoadData(value == null ? "" : value.ToString(), "text/html", null);
                        }
                    }
                    break;

                default:
                    if (view is TextView)
                    {
                        var textView = (TextView)view;
                        string text = value == null ? "" : value.ToString();
                        if (textView.Text != text) textView.Text = text;
                    }
                    else if (view is AdapterView)
                    {
                        var adapterView = (AdapterView)view;
                        var adapter = adapterView.GetAdapter() as IDataBindableListAdapter;
                        if (adapter != null)
                        {
                            int position = adapter.GetItemPosition(value);
                            if (adapterView is AbsListView)
                            {
                                var absListView = (AbsListView)adapterView;
                                if (!absListView.IsItemChecked(position))
                                {
                                    absListView.SetItemChecked(position, true);
                                }
                            }
                            else
                            {
                                if (adapterView.SelectedItemPosition != position) adapterView.SetSelection(position);
                            }
                        }
                    }
                    else throw new NotImplementedException(string.Format("Unsupported view type {0} for UpdateView(). Add the type to either ViewDefaultPropertyOrFieldName or UpdateView()", viewTypeName));
                    break;
            }
        }

        #endregion View types that support one-way data binding

        #region View types that support two-way data binding

        private void AddTwoWayHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.ContainingObject == null) return;
            var view = binding.ViewProperty.ContainingObject;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
                default:
                    if (view is AbsSpinner) ((AdapterView)view).ItemSelected += AdapterView_ItemSelected;
                    else if (view is AbsListView) ((AdapterView)view).ItemClick += AdapterView_ItemClick;
                    else if (view is EditText) ((TextView)view).AfterTextChanged += TextView_AfterTextChanged;
                    else throw new NotImplementedException("View type not implemented: " + viewTypeName);
                    break;
            }
        }

        private void RemoveTwoWayHandler(DataBinding binding)
        {
            if (binding.ViewProperty == null || binding.ViewProperty.ContainingObject == null) return;
            var view = binding.ViewProperty.ContainingObject;
            string viewTypeName = view.GetType().FullName;
            switch (viewTypeName)
            {
                // TODO: Add cases here for specialized view types, as needed
                default:
                    if (view is AbsSpinner) ((AdapterView)view).ItemSelected -= AdapterView_ItemSelected;
                    else if (view is AbsListView) ((AdapterView)view).ItemClick -= AdapterView_ItemClick;
                    else if (view is EditText) ((TextView)view).AfterTextChanged -= TextView_AfterTextChanged;
                    else throw new NotImplementedException("View type not implemented: " + viewTypeName);
                    break;
            }
        }

        private void TextView_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            var view = (TextView)sender;
            var binding = FindBindingForView(view);
            if (binding != null) binding.UpdateViewModel(viewModel, view.Text);
        }

        private void HandleAdapterViewItemChosen(AdapterView adapterView, int itemPosition)
        {
            if (itemPosition >= 0)
            {
                var adapter = adapterView.GetAdapter() as IDataBindableListAdapter;
                var binding = FindBindingForView(adapterView);
                if (adapter != null && binding != null)
                {
                    var value = adapter.GetItemAsObject(itemPosition);
                    switch (binding.Mode)
                    {
                        case BindingMode.Command:
                            var command = (RelayCommand)binding.ViewModelPropertyInfo.GetValue(viewModel);
                            command.Execute(value);
                            break;
                        case BindingMode.TwoWay:
                            binding.UpdateViewModel(viewModel, value);
                            break;
                    }
                }
            }
        }

        private void AdapterView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            HandleAdapterViewItemChosen((AdapterView)sender, e.Position);
        }

        private void AdapterView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            HandleAdapterViewItemChosen((AdapterView)sender, e.Position);
        }

        #endregion View types that support two-way data binding
    }
}