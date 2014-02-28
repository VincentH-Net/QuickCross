using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;

using MonoMac;
using System.Collections;
using MonoTouch;

namespace QuickCross
{
	public enum BindingMode { OneWay, TwoWay, Command };

	public class BindingParameters
	{
		public string PropertyName;
		public BindingMode Mode = BindingMode.OneWay;
		public object View;
		public string ListPropertyName;
		public string ListItemTemplateName;
		public string ListAddItemCommandName;
		public string ListRemoveItemCommandName;
		public string ListCanEditItem;
		public string ListCanMoveItem;
	}
	
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
			UIView self = (UIView) Runtime.GetNSObject(selfPtr);
			var value = Runtime.GetNSObject(valuePtr);
			var key = (NSString) Runtime.GetNSObject(undefinedKeyPtr);
			if (key == BindKey) {
				AddBinding(self, value.ToString());
			} else {
				Console.WriteLine("Value for unknown key: {0} = {1}", key.ToString(), value.ToString() );
				// Call original implementation on super class of UIView:
				void_objc_msgSendSuper_intptr_intptr(UIViewSuperClass, SetValueForUndefinedKeySelector, valuePtr, undefinedKeyPtr);
			}
		}

		private static string BindKey;
		private static IntPtr UIViewSuperClass, SetValueForUndefinedKeySelector;

		public static void RegisterBindKey(string key = "Bind")
		{
			RootViewBindingParameters = new Dictionary<UIView, List<BindingParameters> >();
			BindKey = key;
			Console.WriteLine("Replacing implementation of SetValueForUndefinedKey on UIView...");
			var uiViewClass = Class.GetHandle("UIView");
			UIViewSuperClass = ObjcMagic.GetSuperClass(uiViewClass);
			SetValueForUndefinedKeySelector = Selector.GetHandle("setValue:forUndefinedKey:");
			ObjcMagic.AddMethod(uiViewClass, SetValueForUndefinedKeySelector, SetValueForUndefinedKeyDelegate, "v@:@@");
		}

		#endregion Add support for user defined runtime attribute named "Bind" (default, type string) on UIView

		public static Dictionary<UIView, List<BindingParameters> > RootViewBindingParameters { get; private set; }

		private static void AddBinding(UIView view, string bindingParameters)
		{
			Console.WriteLine("Binding parameters: {0}", bindingParameters);

			// Get the rootview so we can group binding parameters under it.
			var rootView = view;
			while (rootView.Superview != null && rootView.Superview != rootView) {
				rootView = rootView.Superview;
				Console.Write(".");
			}
			Console.WriteLine("rootView = {0}", rootView.ToString());

			var bp = ParseBindingParameters(bindingParameters);
			if (bp == null)
				throw new ArgumentException("Invalid data binding parameters: " + bindingParameters);
			if (string.IsNullOrEmpty(bp.PropertyName) && string.IsNullOrEmpty(bp.ListPropertyName))
				throw new ArgumentException("At least one of PropertyName and ListPropertyName must be specified in data binding parameters: " + bindingParameters);
			bp.View = view;

			List<BindingParameters> bindingParametersList;
			if (!RootViewBindingParameters.TryGetValue(rootView, out bindingParametersList))
			{
				bindingParametersList = new List<BindingParameters>();
				RootViewBindingParameters.Add(rootView, bindingParametersList);
			}

			bindingParametersList.Add(bp);
		}

		private class DataBinding
		{
			public BindingMode Mode;
			public object View;
			public PropertyInfo ViewModelPropertyInfo;

			public PropertyInfo ViewModelListPropertyInfo;
			public DataBindableUITableViewSource TableViewSource;

			public void Command_CanExecuteChanged(object sender, EventArgs e)
			{
				var control = View as UIControl;
				if (control != null) control.Enabled = ((RelayCommand)sender).IsEnabled;
			}
		}

		private readonly UIView rootView;
		private readonly IViewExtensionPoints rootViewExtensionPoints;
		private ViewModelBase viewModel;
		private readonly string idPrefix;

		private Dictionary<string, DataBinding> dataBindings = new Dictionary<string, DataBinding>();
		// the string key is the idname which is a prefix + the name of the vm property

		public interface IViewExtensionPoints  // Implement these methods as virtual in a view base class
		{
			void UpdateView(object view, object value);
			void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
			object GetCommandParameter(string commandName, object parameter = null);
		}

		public ViewDataBindings(UIView rootView, ViewModelBase viewModel, string idPrefix, IViewExtensionPoints rootViewExtensionPoints = null)
		{
			if (rootView == null)
				throw new ArgumentNullException("rootView");
			if (viewModel == null)
				throw new ArgumentNullException("viewModel");
			this.rootView = rootView;
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

		public void AddBindings(BindingParameters[] bindingsParameters)
		{
			if (bindingsParameters != null)
			{
				foreach (var bp in bindingsParameters)
				{
					if (bp.View != null && FindBindingForView(bp.View) != null) throw new ArgumentException("Cannot add binding because a binding already exists for the view " + bp.View.ToString());
					if (dataBindings.ContainsKey(IdName(bp.PropertyName))) throw new ArgumentException("Cannot add binding because a binding already exists for the view with Id " + IdName(bp.PropertyName));
					AddBinding(bp); 
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
									if (value != "") bp.PropertyName = value;
								}
								else if (assignmentElements.Length == 2)
								{
									string name = assignmentElements[0].Trim();
									string value = assignmentElements[1].Trim();
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
			return bp;
		}


		private DataBinding AddBinding(BindingParameters bp)
		{
			var view = bp.View;
			if (view == null) return null;
			var propertyName = bp.PropertyName;
			var mode = bp.Mode;
			var listPropertyName = bp.ListPropertyName;
			var itemTemplateName = bp.ListItemTemplateName;

			var idName = IdName(propertyName);

			var binding = new DataBinding
			{
				View = view,
				Mode = mode,
				ViewModelPropertyInfo = (string.IsNullOrEmpty(propertyName) || propertyName == ".") ? null : viewModel.GetType().GetProperty(propertyName)
			};

			if (binding.View is UITableView)
			{
				if (listPropertyName == null) listPropertyName = propertyName + "List";
				var pi = viewModel.GetType().GetProperty(listPropertyName);
				if (pi == null && binding.ViewModelPropertyInfo != null && binding.ViewModelPropertyInfo.PropertyType.GetInterface("IList") != null)
				{
					listPropertyName = propertyName;
					pi = binding.ViewModelPropertyInfo;
					binding.ViewModelPropertyInfo = null;
				}
				binding.ViewModelListPropertyInfo = pi;

				var tableView = (UITableView)binding.View;
				if (tableView.Source == null)
				{
					if (itemTemplateName == null) itemTemplateName = listPropertyName + "Item";
					string listItemSelectedPropertyName = (mode == BindingMode.Command || mode == BindingMode.TwoWay) ? bp.PropertyName : null;
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
			return dataBindings.FirstOrDefault(i => object.ReferenceEquals(i.Value.View, view)).Value;
		}

		private DataBinding FindBindingForListProperty(string propertyName)
		{
			return dataBindings.FirstOrDefault(i => i.Value.ViewModelListPropertyInfo != null && i.Value.ViewModelListPropertyInfo.Name == propertyName).Value;
		}

		private void UpdateView(DataBinding binding)
		{
			if ((binding.Mode == BindingMode.OneWay) || (binding.Mode == BindingMode.TwoWay) && binding.View != null)
			{
				var view = binding.View;
				var value = (binding.ViewModelPropertyInfo == null) ? viewModel : binding.ViewModelPropertyInfo.GetValue(viewModel);
				if (rootViewExtensionPoints != null) rootViewExtensionPoints.UpdateView(view, value); else UpdateView(view, value);
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
