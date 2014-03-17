using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.ComponentModel;

using Android.Views;
using Android.Widget;

namespace QuickCross
{
    public enum BindingMode { OneWay, TwoWay, Command };

    public class BindingParameters
    {
        public string ViewModelPropertyName;
        public Expression<Func<object>> Property { set { ViewModelPropertyName = PropertyReference.GetMemberName(value); } }

        public object View;
        public string ViewMemberName;

        /// <summary>An optional Linq expression that specifies the ViewMemberName in a typesafe manner, 
        /// e.g.: <example>() => view.AProperty</example> or <example>() => view.AField</example>
        /// </summary>
        public Expression<Func<object>> ViewMember { set { ViewMemberName = PropertyReference.GetMemberName(value); } } // We use a set-only property instead of a Set method because it allows to use array initializer syntax for BindingParameters; ease of use outweights the frowned upon - but intentional - side effect on the corresponding Name property.

        /// <summary>
        /// An optional action to update the view from the viewmodel, e.g. <example>() => myView.SetTextColor(ViewModel.MyBoolean ? Color.Green : Color.Red)</example>
        /// <remarks>You can use this action to convert values, invoke methods on the view, combine multiple viewmodel properties etc.</remarks>
        /// </summary>
        public Action UpdateView;

        /// <summary>
        /// An optional action to update the viewmodel from the view for two-way bindings, e.g. <example>() => ViewModel.MyBoolean = myView.CurrentTextColor == Color.Green.ToArgb</example>
        /// <remarks>You can use this action to convert values, invoke methods on the view, set multiple viewmodel properties etc.</remarks>
        /// </summary>
        public Action UpdateViewModel;

        public string ListPropertyName;

        /// <summary>An optional Linq expression that specifies the ListPropertyName in a typesafe manner, 
        /// e.g.: <example>() => ViewModel.AProperty</example>
        /// </summary>
        public Expression<Func<object>> ListProperty { set { ListPropertyName = PropertyReference.GetMemberName(value); } } // We use a set-only property instead of a Set method because it allows to use array initializer syntax for BindingParameters; ease of use outweights the frowned upon - but intentional - side effect on the corresponding Name property.

        public BindingMode Mode = BindingMode.OneWay;
        public AdapterView CommandParameterSelectedItemAdapterView;
    }

    public partial class ViewDataBindings
    {
        private class DataBinding
        {
            public BindingMode Mode;

            public PropertyReference ViewProperty;
            public Action UpdateViewAction;
            public Action UpdateViewModelAction; 

            public PropertyInfo ViewModelPropertyInfo;
            public int? ResourceId;

            public PropertyInfo ViewModelListPropertyInfo;
            public IDataBindableListAdapter ListAdapter;

            public int? CommandParameterListId;
            public AdapterView CommandParameterListView;

            public void UpdateViewModel(ViewModelBase viewModel, object value)
            {
                if (UpdateViewModelAction != null)
                {
                    UpdateViewModelAction();
                }
                else
                {
                    ViewModelPropertyInfo.SetValue(viewModel, value);
                }
            }

            public void Command_CanExecuteChanged(object sender, EventArgs e)
            {
                var view = ViewProperty.ContainingObject;
                if (view is View) ((View)view).Enabled = ((RelayCommand)sender).IsEnabled;
            }
        }

        private readonly View rootView;
        private readonly IViewExtensionPoints rootViewExtensionPoints;
        private ViewModelBase viewModel;
        private readonly LayoutInflater layoutInflater;
        private readonly string idPrefix;

        private Dictionary<string, DataBinding> dataBindings = new Dictionary<string, DataBinding>();

        public interface IViewExtensionPoints  // Implement these methods as virtual in a view base class
        {
            void UpdateView(PropertyReference viewProperty, object value);
            void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
        }

		public ViewDataBindings(View rootView, ViewModelBase viewModel, LayoutInflater layoutInflater, string idPrefix, IViewExtensionPoints viewExtensionPoints)
        {
            if (rootView == null) throw new ArgumentNullException("rootView");
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            if (layoutInflater == null) throw new ArgumentNullException("layoutInflater");
            this.rootView = rootView;
            this.rootViewExtensionPoints = viewExtensionPoints;
            this.viewModel = viewModel;
            this.layoutInflater = layoutInflater;
            this.idPrefix = idPrefix;
        }

        public void SetViewModel(ViewModelBase newViewModel)
        {
            if (Object.ReferenceEquals(viewModel, newViewModel)) return;
            RemoveHandlers();
            viewModel = newViewModel;
            AddHandlers();
            UpdateView(findViews: false);
        }

        public void EnsureCommandBindings()
        {
            foreach (string commandName in viewModel.CommandNames)
            {
                DataBinding binding;
                if (!dataBindings.TryGetValue(IdName(commandName), out binding))
                {
                    AddBinding(commandName, BindingMode.Command);
                }
            }
        }

        public void UpdateView(bool findViews = true)
        {
            foreach (var item in dataBindings)
            {
                var binding = item.Value;
                if (findViews)
                {
                    if (binding.ResourceId.HasValue)
                    {
                        var view = rootView.FindViewById(binding.ResourceId.Value);
                        binding.ViewProperty = view == null ? null : new PropertyReference(view, binding.ViewProperty == null ? null : binding.ViewProperty.PropertyOrFieldName);
                    }
                    if (binding.CommandParameterListId.HasValue) binding.CommandParameterListView = rootView.FindViewById<AdapterView>(binding.CommandParameterListId.Value);
                }
                UpdateList(binding);
                UpdateView(binding);
            }
        }

        public void UpdateView(string propertyName)
        {
            DataBinding binding;
            if (dataBindings.TryGetValue(IdName(propertyName), out binding))
            {
                UpdateList(binding);
                UpdateView(binding);
                return;
            }

            binding = FindBindingForListProperty(propertyName);
            if (binding != null)
            {
                UpdateList(binding);
                return;
            }

            binding = AddBinding(propertyName);
            if (binding != null)
            {
                UpdateList(binding);
                UpdateView(binding);
            }
        }

        public void RemoveHandlers()
        {
            foreach (var item in dataBindings)
            {
                var binding = item.Value;
                RemoveListHandlers(binding);
                switch (binding.Mode)
                {
                    case BindingMode.TwoWay: RemoveTwoWayHandler(binding); break;
                    case BindingMode.Command: RemoveCommandHandler(binding); break;
                }
            }
        }

        public void AddHandlers()
        {
            foreach (var item in dataBindings)
            {
                var binding = item.Value;
                AddListHandlers(binding);
                switch (binding.Mode)
                {
                    case BindingMode.TwoWay: AddTwoWayHandler(binding); break;
                    case BindingMode.Command: AddCommandHandler(binding); break;
                }
            }
        }

        private void RemoveListHandlers(DataBinding binding)
        {
            if (binding != null && binding.ListAdapter != null) binding.ListAdapter.RemoveHandlers();
        }

        private void AddListHandlers(DataBinding binding)
        {
            if (binding != null && binding.ListAdapter != null) binding.ListAdapter.AddHandlers();
        }

        public void AddBindings(BindingParameters[] bindingsParameters)
        {
            if (bindingsParameters != null)
            {
                foreach (var bp in bindingsParameters)
                {
                    if (bp.View != null && FindBindingForView(bp.View) != null) throw new ArgumentException("Cannot add binding because a binding already exists for the view with Id " + bp.View.Id.ToString());
                    if (dataBindings.ContainsKey(IdName(bp.ViewModelPropertyName))) throw new ArgumentException("Cannot add binding because a binding already exists for the view with Id " + IdName(bp.ViewModelPropertyName));
                    AddBinding(bp.ViewModelPropertyName, bp.Mode, bp.ListPropertyName, bp.View, bp.ViewMemberName, bp.UpdateView, bp.UpdateViewModel, bp.CommandParameterSelectedItemAdapterView);
                }
            }
        }

        private string IdName(string name) { return idPrefix + name; }

        private DataBinding AddBinding(string propertyName, BindingMode mode = BindingMode.OneWay, string listPropertyName = null, object view = null, string viewMemberName = null, Action updateViewAction = null, Action updateViewModelAction = null, AdapterView commandParameterSelectedItemAdapterView = null)
        {
            var androidView = view as View;
            string idName = (androidView != null) ? androidView.Id.ToString() : IdName(propertyName);
            int? resourceId = AndroidHelpers.FindResourceId(idName);
            if (view == null && resourceId.HasValue) view = rootView.FindViewById(resourceId.Value);
            if (view == null) return null;

            bool itemIsValue = false;
            string itemTemplateName = null, itemValueId = null;
            int? commandParameterListId = null;
            if (view is View && ((View)view).Tag != null)
            {
                // Get optional parameters from tag:
                // {Binding propertyName, Mode=OneWay|TwoWay|Command}
                // {List ItemsSource=listPropertyName, ItemIsValue=false|true, ItemTemplate=listItemTemplateName, ItemValueId=listItemValueId}
                // {CommandParameter ListId=<view Id>}
                // Defaults:
                //   propertyName is known by convention from view Id = <rootview prefix><propertyName>; the default for the rootview prefix is the rootview class name + "_".
                //   Mode = OneWay
                // Additional defaults for views derived from AdapterView (i.e. lists):
                //   ItemsSource = propertyName + "List"
                //   ItemIsValue = false
                //   ItemTemplate = ItemsSource + "Item"
                //   ItemValueId : if ItemIsValue = true then the default for ItemValueId = ItemTemplate
                string tag = ((View)view).Tag.ToString();
                if (tag != null && tag.Contains("{"))
                {
                    var match = Regex.Match(tag, @"({Binding\s+((?<assignment>[^,{}]+),?)+\s*})?(\s*{List\s+((?<assignment>[^,{}]+),?)+\s*})?(\s*{CommandParameter\s+((?<assignment>[^,{}]+),?)+\s*})?");
                    if (match.Success)
                    {
                        var gc = match.Groups["assignment"];
                        if (gc != null)
                        {
                            var cc = gc.Captures;
                            if (cc != null)
                            {
                                for (int i = 0; i < cc.Count; i++)
                                {
                                    string[] assignmentElements = cc[i].Value.Split('=');
                                    if (assignmentElements.Length == 1)
                                    {
                                        string value = assignmentElements[0].Trim();
                                        if (value != "") propertyName = value;
                                    }
                                    else if (assignmentElements.Length == 2)
                                    {
                                        string name = assignmentElements[0].Trim();
                                        string value = assignmentElements[1].Trim();
                                        if (name.StartsWith("."))
                                        {
                                            viewMemberName = name.Substring(1);
                                            if (value != "") propertyName = value;
                                        }
                                        else
                                        {
                                            switch (name)
                                            {
                                                case "Mode": Enum.TryParse<BindingMode>(value, true, out mode); break;
                                                case "ItemsSource": listPropertyName = value; break;
                                                case "ItemIsValue": Boolean.TryParse(value, out itemIsValue); break;
                                                case "ItemTemplate": itemTemplateName = value; break;
                                                case "ItemValueId": itemValueId = value; break;
                                                case "ListId":
                                                    commandParameterListId = AndroidHelpers.FindResourceId(value);
                                                    if (commandParameterSelectedItemAdapterView == null && commandParameterListId.HasValue) commandParameterSelectedItemAdapterView = rootView.FindViewById<AdapterView>(commandParameterListId.Value);
                                                    break;
                                                default: throw new ArgumentException("Unknown tag binding parameter: " + name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if ((mode == BindingMode.OneWay || mode == BindingMode.TwoWay) && updateViewAction == null && viewMemberName == null)
            {
                var typeName = view.GetType().FullName;
                if (!ViewDefaultPropertyOrFieldName.TryGetValue(typeName, out viewMemberName))
                    throw new ArgumentException(string.Format("No default property or field name exists for view type {0}. Please specify the name of a property or field in the ViewMemberName binding parameter", typeName), "ViewMemberName");
            }

            var viewModelPropertyInfo = (string.IsNullOrEmpty(propertyName) || propertyName == ".") ? null : viewModel.GetType().GetProperty(propertyName);

            var binding = new DataBinding
            {
                ViewProperty = new PropertyReference(view, viewMemberName, viewModelPropertyInfo != null ? viewModelPropertyInfo.PropertyType : null),
                ResourceId = resourceId,
                UpdateViewAction = updateViewAction,
                UpdateViewModelAction = updateViewModelAction,
                Mode = mode,
                ViewModelPropertyInfo = viewModelPropertyInfo,
                CommandParameterListId = commandParameterListId,
                CommandParameterListView = commandParameterSelectedItemAdapterView
            };

            if (listPropertyName == null) listPropertyName = propertyName + "List";
            var pi = viewModel.GetType().GetProperty(listPropertyName);
            if (pi == null && binding.ViewModelPropertyInfo.PropertyType.GetInterface("IList") != null)
            {
                listPropertyName = propertyName;
                pi = binding.ViewModelPropertyInfo;
                binding.ViewModelPropertyInfo = null;
            }
            binding.ViewModelListPropertyInfo = pi;

            if (view is AdapterView)
            {
                pi = view.GetType().GetProperty("Adapter", BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    var adapter = pi.GetValue(view);
                    if (adapter == null)
                    {
                        if (itemTemplateName == null) itemTemplateName = listPropertyName + "Item";
                        if (itemIsValue && itemValueId == null) itemValueId = itemTemplateName;
                        int? itemTemplateResourceId = AndroidHelpers.FindResourceId(itemTemplateName, AndroidHelpers.ResourceCategory.Layout);
                        int? itemValueResourceId = AndroidHelpers.FindResourceId(itemValueId);
                        if (itemTemplateResourceId.HasValue)
                        {
                            adapter = new DataBindableListAdapter<object>(layoutInflater, itemTemplateResourceId.Value, itemTemplateName + "_", itemValueResourceId, rootViewExtensionPoints);
                            pi.SetValue(view, adapter);
                        }
                    }
                    binding.ListAdapter = adapter as IDataBindableListAdapter;
                }
            }

            switch (binding.Mode)
            {
                case BindingMode.TwoWay: AddTwoWayHandler(binding); break;
                case BindingMode.Command: AddCommandHandler(binding); break;
            }

            dataBindings.Add(idName, binding);
            return binding;
        }

        private DataBinding FindBindingForView(object view)
        {
            return dataBindings.FirstOrDefault(i => object.ReferenceEquals(i.Value.ViewProperty.ContainingObject, view)).Value;
        }

        private DataBinding FindBindingForListProperty(string propertyName)
        {
            return dataBindings.FirstOrDefault(i => i.Value.ViewModelListPropertyInfo != null && i.Value.ViewModelListPropertyInfo.Name == propertyName).Value;
        }

        private void UpdateView(DataBinding binding)
        {
            if (((binding.Mode == BindingMode.OneWay) || (binding.Mode == BindingMode.TwoWay)) && binding.ViewProperty != null && binding.ViewProperty.ContainingObject != null)
            {
				var viewProperty = binding.ViewProperty;
                if (binding.UpdateViewAction != null)
                {
                    binding.UpdateViewAction();
                }
                else
                {
                    var value = (binding.ViewModelPropertyInfo == null) ? viewModel : binding.ViewModelPropertyInfo.GetValue(viewModel);
                    if (rootViewExtensionPoints != null) rootViewExtensionPoints.UpdateView(viewProperty, value); else UpdateView(viewProperty, value);
                }
            }
        }

        private void UpdateList(DataBinding binding)
        {
            if (binding.ViewModelListPropertyInfo != null && binding.ListAdapter != null)
            {
                var list = (IList)binding.ViewModelListPropertyInfo.GetValue(viewModel);
                if (binding.ListAdapter.SetList(list))
                {
                    var listView = binding.ViewProperty.ContainingObject;
                    if (listView is AbsListView) ((AbsListView)listView).ClearChoices(); // Apparently, calling BaseAdapter.NotifyDataSetChanged() does not clear the choices, so we do that here.
                }
            }
        }
    }
}