using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch;
using MonoMac;
#if __DIALOG__
using MonoTouch.Dialog;
#endif

namespace QuickCross
{
	public enum BindingMode { OneWay, TwoWay, Command };

	public class BindingParameters
	{
		public string ViewModelPropertyName;
		public Expression<Func<object>> Property { set { ViewModelPropertyName = PropertyReference.GetMemberName(value); } }

		public BindingMode Mode = BindingMode.OneWay;

		public object View;
		public string ViewMemberName;

		/// <summary>An optional Linq expression that specifies the ViewMemberName in a typesafe manner, 
        /// e.g.: <example>() => view.AProperty</example> or <example>() => view.AField</example>
		/// </summary>
		public Expression<Func<object>> ViewMember { set { ViewMemberName = PropertyReference.GetMemberName(value); } } // We use a set-only property instead of a Set method because it allows to use array initializer syntax for BindingParameters; ease of use outweights the frowned upon - but intentional - side effect on the corresponding Name property.

        /// <summary>
        /// An optional action to update the view from the viewmodel, e.g. <example>() => myView.Color = ViewModel.MyBoolean ? UIColor.Green : UIColor.Red</example>
        /// <remarks>You can use this action to convert values, invoke methods on the view, combine multiple viewmodel properties etc.</remarks>
        /// </summary>
        public Action UpdateView;

        /// <summary>
        /// An optional action to update the viewmodel from the view for two-way bindings, e.g. <example>() => ViewModel.MyBoolean = myView.Color == UIColor.Green</example>
        /// <remarks>You can use this action to convert values, invoke methods on the view, set multiple viewmodel properties etc.</remarks>
        /// </summary>
        public Action UpdateViewModel;

        public string ListPropertyName;

        /// <summary>An optional Linq expression that specifies the ListPropertyName in a typesafe manner, 
        /// e.g.: <example>() => ViewModel.AProperty</example>
		/// </summary>
		public Expression<Func<object>> ListProperty { set { ListPropertyName = PropertyReference.GetMemberName(value); } } // We use a set-only property instead of a Set method because it allows to use array initializer syntax for BindingParameters; ease of use outweights the frowned upon - but intentional - side effect on the corresponding Name property.

		public string ListItemTemplateName;
		public string ListAddItemCommandName;
		public string ListRemoveItemCommandName;
		public string ListCanEditItem;
		public string ListCanMoveItem;
	}

    #if __DIALOG__
    public static class ElementExtensions
    {
        public static Element Bind(
            this Element element,
            List<BindingParameters> bindingsParameters,
            Expression<Func<object>> property,
            BindingMode mode = BindingMode.TwoWay,
            Expression<Func<object>> viewMember = null,
            Action updateView = null,
            Action updateViewModel = null,
            Expression<Func<object>> listProperty = null)
        {
            bindingsParameters.Add(new BindingParameters
            {
                Property = property,
                Mode = mode,
                View = element,
                ViewMember = viewMember,
                UpdateView = updateView,
                UpdateViewModel = updateViewModel,
                ListProperty = listProperty
            });
            return element;
        }

        public static Element Bind(
             this Element element,
             List<BindingParameters> bindingsParameters,
             string property,
             BindingMode mode = BindingMode.TwoWay,
             Expression<Func<object>> viewMember = null,
             Action updateView = null,
             Action updateViewModel = null,
             string listProperty = null)
        {
            bindingsParameters.Add(new BindingParameters
            {
                ViewModelPropertyName = property,
                Mode = mode,
                View = element,
                ViewMember = viewMember,
                UpdateView = updateView,
                UpdateViewModel = updateViewModel,
                ListPropertyName = listProperty
            });
            return element;
        }
    }
    #endif

	public partial class ViewDataBindings
    {
		#region Add support for user defined runtime attribute named "Bind" (default, type string) on UIView

		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		static extern void void_objc_msgSendSuper_intptr_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		delegate void SetValueForUndefinedKeyCallBack (IntPtr selfPtr, IntPtr cmdPtr, IntPtr valuePtr, IntPtr undefinedKeyPtr);
		static SetValueForUndefinedKeyCallBack SetValueForUndefinedKeyDelegate = SetValueForUndefinedKey;

		[MonoPInvokeCallback (typeof(SetValueForUndefinedKeyCallBack))]
		private static void SetValueForUndefinedKey(IntPtr selfPtr, IntPtr cmdPtr, IntPtr valuePtr, IntPtr undefinedKeyPtr)
		{
            var self = Runtime.GetNSObject(selfPtr);
			var value = Runtime.GetNSObject(valuePtr);
			var key = (NSString) Runtime.GetNSObject(undefinedKeyPtr);
			if (key == BindKey) {
				AddBinding(self, value.ToString());
			} else {
				Console.WriteLine("Value for unknown key: {0} = {1}", key.ToString(), value.ToString() );
				// Call original implementation on super class:
                if (self is UIView) void_objc_msgSendSuper_intptr_intptr(UIViewSuperClass, SetValueForUndefinedKeySelector, valuePtr, undefinedKeyPtr);
                else if (self is UIBarItem) void_objc_msgSendSuper_intptr_intptr(UIBarItemSuperClass, SetValueForUndefinedKeySelector, valuePtr, undefinedKeyPtr);
            }
		}

		private static string BindKey;
        private static IntPtr UIViewSuperClass, UIBarItemSuperClass, SetValueForUndefinedKeySelector;

		public static void RegisterBindKey(string key = "Bind")
		{
			RootViewBindingParameters = new Dictionary<UIView, List<BindingParameters> >();
            UIBarItemBindingParameters = new Dictionary<UIBarItem, BindingParameters>();
			BindKey = key;
			Console.WriteLine("Replacing implementation of SetValueForUndefinedKey on UIView...");

            var uiViewClass = Class.GetHandle("UIView");
            UIViewSuperClass = ObjcMagic.GetSuperClass(uiViewClass);
            
            var uiBarItemClass = Class.GetHandle("UIBarItem");
            UIBarItemSuperClass = ObjcMagic.GetSuperClass(uiBarItemClass);

            SetValueForUndefinedKeySelector = Selector.GetHandle("setValue:forUndefinedKey:");
			ObjcMagic.AddMethod(uiViewClass   , SetValueForUndefinedKeySelector, SetValueForUndefinedKeyDelegate, "v@:@@");
            ObjcMagic.AddMethod(uiBarItemClass, SetValueForUndefinedKeySelector, SetValueForUndefinedKeyDelegate, "v@:@@");
        }

		#endregion Add support for user defined runtime attribute named "Bind" (default, type string) on UIView

		private static Dictionary<UIView, List<BindingParameters> > RootViewBindingParameters { get; set; }
        private static Dictionary<UIBarItem, BindingParameters> UIBarItemBindingParameters { get; set; }

		private static void AddBinding(NSObject view, string bindingParameters)
		{
			Console.WriteLine("Binding parameters: {0}", bindingParameters);

            var bp = ParseBindingParameters(bindingParameters);
            if (bp == null)
                throw new ArgumentException("Invalid data binding parameters: " + bindingParameters);
            if (string.IsNullOrEmpty(bp.ViewModelPropertyName) && string.IsNullOrEmpty(bp.ListPropertyName))
                throw new ArgumentException("At least one of PropertyName and ListPropertyName must be specified in data binding parameters: " + bindingParameters);
            bp.View = view;

            // Get the rootview so we can group binding parameters under it.
            if (view is UIView) {
                var rootView = (UIView)view;
                while (rootView.Superview != null && rootView.Superview != rootView) {
                    rootView = rootView.Superview;
                    Console.Write(".");
                }

                Console.WriteLine("rootView = {0}", rootView.ToString());

                List<BindingParameters> bindingParametersList;
                if (!RootViewBindingParameters.TryGetValue(rootView, out bindingParametersList))
                {
                    bindingParametersList = new List<BindingParameters>();
                    RootViewBindingParameters.Add(rootView, bindingParametersList);
                }

                bindingParametersList.Add(bp);
            }
            else if (view is UIBarItem)
            {
                // TODO: implement UIBarItem add
            }
            else {
                throw new ArgumentException(string.Format("Unsupported view type for Bind custom runtime attribute: {0}. Supported base types: UIView, UIBarItem", view.GetType().FullName));
            }
		}

		private class DataBinding
		{
			public BindingMode Mode;
			public PropertyReference ViewProperty;
            public Action UpdateViewAction;
            public Action UpdateViewModelAction;
            public PropertyInfo ViewModelPropertyInfo;

			public PropertyInfo ViewModelListPropertyInfo;
			public DataBindableUITableViewSource TableViewSource;

            public void UpdateViewModel(ViewModelBase viewModel, object value)
            {
                if (UpdateViewModelAction != null) {
                    UpdateViewModelAction();
                } else {
                    ViewModelPropertyInfo.SetValue(viewModel, value);
                }
            }

			public void Command_CanExecuteChanged(object sender, EventArgs e)
			{
				var control = ViewProperty.ContainingObject as UIControl;
				if (control != null) control.Enabled = ((RelayCommand)sender).IsEnabled;
			}
		}

		private readonly IViewExtensionPoints rootViewExtensionPoints;
		private ViewModelBase viewModel;
		private readonly string idPrefix;

		private Dictionary<string, DataBinding> dataBindings = new Dictionary<string, DataBinding>();
		// the string key is the idname which is a prefix + the name of the vm property

		public interface IViewExtensionPoints  // Implement these methods as virtual in a view base class
		{
            void UpdateView(PropertyReference viewProperty, object value);
			void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
			object GetCommandParameter(string commandName, object parameter = null);
		}

		public ViewDataBindings(ViewModelBase viewModel, string idPrefix, IViewExtensionPoints rootViewExtensionPoints = null)
		{
			if (viewModel == null) throw new ArgumentNullException("viewModel");
			this.rootViewExtensionPoints = rootViewExtensionPoints;
			this.viewModel = viewModel;
			this.idPrefix = idPrefix; // Note that on iOS we may use idPrefix only for connecting outlet names to vm property names;
			// The uiviews that have no outlets do not need a name or id; all info is in the Bind property
		}

		public void SetViewModel(ViewModelBase newViewModel)
		{
			if (Object.ReferenceEquals(viewModel, newViewModel)) return;
			RemoveHandlers();
			viewModel = newViewModel;
			AddHandlers();
			UpdateView();
		}

		public void UpdateView()
		{
			foreach (var item in dataBindings)
			{
				var binding = item.Value;
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
			if (binding != null && binding.TableViewSource != null) binding.TableViewSource.RemoveHandlers();
		}

		private void AddListHandlers(DataBinding binding)
		{
			if (binding != null && binding.TableViewSource != null) binding.TableViewSource.AddHandlers();
		}

		public void AddBindings(BindingParameters[] bindingsParameters = null, UIView rootView = null, UINavigationItem navigationItem = null)
		{
			if (bindingsParameters != null)
			{
                Console.WriteLine("Adding bindings from code ...");
                foreach (var bp in bindingsParameters) AddBinding(bp);
			}
            if (rootView != null)
            {
                List<BindingParameters> bindingParametersList;
                if (ViewDataBindings.RootViewBindingParameters.TryGetValue(rootView, out bindingParametersList))
                {
                    Console.WriteLine("Adding bindings from markup ...");
                    ViewDataBindings.RootViewBindingParameters.Remove(rootView); // Remove the static reference to the views to prevent memory leaks. Note that if we would want to recreate the bindings later, we could also store the parameters list in the bindings.
                    foreach (var bp in bindingParametersList) AddBinding(bp);
                }
            }
            if (navigationItem != null)
            {
                var uiBarItems = new List<UIBarItem>();
                // TODO: check if we need this eg when leftItemsSupplementBackButton is true? uiBarItems.Add(navigationItem.BackBarButtonItem);
                uiBarItems.AddRange(navigationItem.LeftBarButtonItems);
                uiBarItems.AddRange(navigationItem.RightBarButtonItems);
                // TODO: check if navigationItem.TitleView also needs Bind custom runtime attribute support?
                foreach (var uiBarItem in uiBarItems)
                {
                    // TODO: uiBarItem add parameters
                }
            }
        }

		private string IdName(string name) { return idPrefix + name; }

		private static BindingParameters ParseBindingParameters(string parameters)
		{
			BindingParameters bp = null;
			if (!string.IsNullOrEmpty(parameters))
			{
				var match = Regex.Match(parameters, @"(({Binding\s)?\s*((?<assignment>[^,{}]+),?)+\s*}?)?(\s*{List\s+((?<assignment>[^,{}]+),?)+\s*})?(\s*{CommandParameter\s+((?<assignment>[^,{}]+),?)+\s*})?");
				if (match.Success)
				{
					var gc = match.Groups["assignment"];
					if (gc != null)
					{
						var cc = gc.Captures;
						if (cc != null)
						{
							bp = new BindingParameters();
							for (int i = 0; i < cc.Count; i++)
							{
								string[] assignmentElements = cc[i].Value.Split('=');
								if (assignmentElements.Length == 1)
								{
									string value = assignmentElements[0].Trim();
									if (value != "") bp.ViewModelPropertyName = value;
								}
								else if (assignmentElements.Length == 2)
								{
									string name = assignmentElements[0].Trim();
									string value = assignmentElements[1].Trim();
									if (name.StartsWith("."))
									{
										bp.ViewMemberName = name.Substring(1);
										if (value != "") bp.ViewModelPropertyName = value;
									}
									else
									{
										switch (name)
										{
											case "Mode": Enum.TryParse<BindingMode>(value, true, out bp.Mode); break;
											case "ItemsSource": bp.ListPropertyName = value; break;
											case "ItemTemplate": bp.ListItemTemplateName = value; break;
											case "AddCommand": bp.ListAddItemCommandName = value; break;
											case "RemoveCommand": bp.ListRemoveItemCommandName = value; break;
											case "CanEdit": bp.ListCanEditItem = value; break;
											case "CanMove": bp.ListCanMoveItem = value; break;
											default: throw new ArgumentException("Unknown tag binding parameter: " + name);
										}
									}
								}
							}
						}
					}
				}
			}
			return bp;
		}


		private DataBinding AddBinding(BindingParameters bp)
		{
            if (bp.View != null && FindBindingForView(bp.View) != null) throw new ArgumentException("Cannot add binding because a binding already exists for the view " + bp.View.ToString());
            if (dataBindings.ContainsKey(IdName(bp.ViewModelPropertyName))) throw new ArgumentException("Cannot add binding because a binding already exists for the view with Id " + IdName(bp.ViewModelPropertyName));

            var view = bp.View;
			if (view == null) return null;
            var viewMemberName = bp.ViewMemberName;
            if ((bp.Mode == BindingMode.OneWay || bp.Mode == BindingMode.TwoWay) && bp.UpdateView == null && viewMemberName == null)
            {
                var typeName = view.GetType().FullName;
                if (!ViewDefaultPropertyOrFieldName.TryGetValue(typeName, out viewMemberName))
                    throw new ArgumentException(string.Format("No default property or field name exists for view type {0}. Please specify the name of a property or field in the ViewMemberName binding parameter", typeName), "ViewMemberName");
            }

			var propertyName = bp.ViewModelPropertyName;
			var mode = bp.Mode;
			var listPropertyName = bp.ListPropertyName;
			var itemTemplateName = bp.ListItemTemplateName;

			var idName = IdName(propertyName);
			var viewModelPropertyInfo = (string.IsNullOrEmpty(propertyName) || propertyName == ".") ? null : viewModel.GetType().GetProperty(propertyName);

			var binding = new DataBinding
			{
                ViewProperty = new PropertyReference(view, viewMemberName, viewModelPropertyInfo != null ? viewModelPropertyInfo.PropertyType : null),
                UpdateViewAction = bp.UpdateView,
                UpdateViewModelAction = bp.UpdateViewModel,
				Mode = mode,
				ViewModelPropertyInfo = viewModelPropertyInfo
			};

			if (listPropertyName == null) listPropertyName = propertyName + "List";
			var pi = viewModel.GetType().GetProperty(listPropertyName);
			if (pi == null && binding.ViewModelPropertyInfo != null && binding.ViewModelPropertyInfo.PropertyType.GetInterface("IList") != null)
			{
				listPropertyName = propertyName;
				pi = binding.ViewModelPropertyInfo;
				binding.ViewModelPropertyInfo = null;
			}
			binding.ViewModelListPropertyInfo = pi;

            if (binding.ViewProperty.ContainingObject is UITableView)
            {
                var tableView = (UITableView)binding.ViewProperty.ContainingObject;
				if (tableView.Source == null)
				{
					if (itemTemplateName == null) itemTemplateName = listPropertyName + "Item";
					string listItemSelectedPropertyName = (mode == BindingMode.Command || mode == BindingMode.TwoWay) ? bp.ViewModelPropertyName : null;
					tableView.Source = binding.TableViewSource = new DataBindableUITableViewSource(
						tableView, 
						itemTemplateName,
						viewModel,
						bp.ListCanEditItem,
						bp.ListCanMoveItem,
						listItemSelectedPropertyName,
						bp.ListRemoveItemCommandName,
						bp.ListAddItemCommandName,
						rootViewExtensionPoints
					);
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
                if (binding.UpdateViewAction != null) {
                    binding.UpdateViewAction();
                } else {
                    var value = (binding.ViewModelPropertyInfo == null) ? viewModel : binding.ViewModelPropertyInfo.GetValue(viewModel);
                    if (rootViewExtensionPoints != null) rootViewExtensionPoints.UpdateView(viewProperty, value); else UpdateView(viewProperty, value);
                }
			}
		}

		private void UpdateList(DataBinding binding)
		{
			if (binding.ViewModelListPropertyInfo != null && binding.TableViewSource != null)
			{
				var list = (IList)binding.ViewModelListPropertyInfo.GetValue(viewModel);
                binding.TableViewSource.SetList(list);
			}
		}

		private void ExecuteCommand(DataBinding binding, object parameter = null)
		{
			if (rootViewExtensionPoints != null) parameter = rootViewExtensionPoints.GetCommandParameter(binding.ViewModelPropertyInfo.Name, parameter);
			var command = (RelayCommand)binding.ViewModelPropertyInfo.GetValue(viewModel);
			command.Execute(parameter);
		}
    }
}
