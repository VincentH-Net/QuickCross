using System;
using MonoTouch.UIKit;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Collections.Specialized;

namespace QuickCross
{
	public class DataBindableUITableViewSource : UITableViewSource
    {
		private class ItemDataBinding
		{
			public readonly PropertyInfo ObjectPropertyInfo;
			public readonly FieldInfo ObjectFieldInfo;
			public readonly UIView View;

			public string Name 
			{ 
				get {
					if (ObjectPropertyInfo != null) return ObjectPropertyInfo.Name;
					if (ObjectFieldInfo != null) return ObjectFieldInfo.Name;
					return ".";
				} 
			}

			public object GetValue(object item) 
			{ 
				if (ObjectPropertyInfo != null) return ObjectPropertyInfo.GetValue(item);
				if (ObjectFieldInfo != null) return ObjectFieldInfo.GetValue(item);
				return item;
			}

			public ItemDataBinding(PropertyInfo objectPropertyInfo, UIView view)
			{
				this.ObjectPropertyInfo = objectPropertyInfo;
				this.View = view;
			}

			public ItemDataBinding(FieldInfo objectFieldInfo, UIView view)
			{
				this.ObjectFieldInfo = objectFieldInfo;
				this.View = view;
			}

			public ItemDataBinding(UIView view)
			{
				this.View = view;
			}
		}

		private readonly ViewDataBindings.ViewExtensionPoints viewExtensionPoints;

		private IList list;
		private bool listIsObservable, ignoreCollectionChanged;

		private readonly UITableView tableView;
		private readonly NSString cellIdentifier;

		private readonly string rowSelectedPropertyName, deleteRowCommandName, insertRowCommandName, canEdit, canMove;
		private readonly bool rowSelectedPropertyIsCommand;
		private readonly ViewModelBase viewModel;

		public DataBindableUITableViewSource(UITableView tableView, string cellIdentifier, ViewModelBase viewModel = null, string canEdit = null, string canMove = null, string rowSelectedPropertyName = null, string deleteRowCommandName= null, string insertRowCommandName = null, ViewDataBindings.ViewExtensionPoints viewExtensionPoints = null)
        {
			this.tableView = tableView;
			this.cellIdentifier = new NSString(cellIdentifier);
			this.viewModel = viewModel;
			this.canEdit = canEdit;
			this.canMove = canMove;
			this.viewExtensionPoints = viewExtensionPoints;
			this.rowSelectedPropertyName = rowSelectedPropertyName;
			this.deleteRowCommandName = deleteRowCommandName;
			this.insertRowCommandName = insertRowCommandName;

			if (this.rowSelectedPropertyName != null) this.rowSelectedPropertyIsCommand = this.viewModel.CommandNames.Contains(this.rowSelectedPropertyName);
        }

		private void AddListHandler()
		{
			if (listIsObservable)
			{
				((INotifyCollectionChanged)list).CollectionChanged += DataBindableListAdapter_CollectionChanged;
			}
		}

		private void RemoveListHandler()
		{
			if (listIsObservable)
			{
				((INotifyCollectionChanged)list).CollectionChanged -= DataBindableListAdapter_CollectionChanged;
			}
		}

		/// <summary>
		/// Override this method in a derived adapter class to register additional event handlers for your adapter. Always call base.AddHandlers() in your override.
		/// </summary>
		public virtual void AddHandlers() 
		{ 
			AddListHandler();
			foreach (var viewDataBindingsHolder in viewDataBindingsHolders) { viewDataBindingsHolder.Value.AddHandlers(); }
		}

		/// <summary>
		/// Override this method in a derived adapter class to unregister additional event handlers for your adapter. Always call base.AddHandlers() in your override.
		/// </summary>
		public virtual void RemoveHandlers() 
		{ 
			RemoveListHandler();
			foreach (var viewDataBindingsHolder in viewDataBindingsHolders) { viewDataBindingsHolder.Value.RemoveHandlers(); }
		}

		/// <summary>
		/// Override this method in a derived adapter class to react to changes in a list if it implements INotifyCollectionChanged (e.g. an ObservableCollection)
		/// </summary>
		/// <param name="sender">The ObservableCollection that was changed</param>
		/// <param name="e">See http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html for details</param>
		protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!ignoreCollectionChanged)
			{
				tableView.ReloadData(); // MQC TODO: Check if this should & can be optimized, see for details documentation at http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html
			}
			if (viewExtensionPoints != null) viewExtensionPoints.OnCollectionChanged(sender, e);
		}

		private void DataBindableListAdapter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChanged(sender, e);
		}

		public bool SetList(IList list)
		{
			if (Object.ReferenceEquals(this.list, list)) return false;
			RemoveListHandler();
			this.list = list;
			listIsObservable = list is INotifyCollectionChanged;
			AddListHandler();
			tableView.ReloadData();
			return true;
		}

		public object GetItem(NSIndexPath path)
		{
			if (path == null) return null;
			int i = path.Row;
			return (list != null && i >= 0 && i < list.Count) ? list[i] : null;
		}

		public NSIndexPath GetIndexPath(object itemObject)
		{
			if (list == null) return null;
			var row = list.IndexOf(itemObject);
			if (row < 0) return null;
			return NSIndexPath.FromRowSection(row, 0);
		}

		// Customize the number of sections in the table view.
		public override int NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection(UITableView tableview, int section)
		{
			return list == null ? 0 : list.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (UITableViewCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
			// At this point, the Bind property has been loaded and binding parameters have been created, grouped under the cell root view
			if (list != null)
			{
				object itemObject = GetItem(indexPath);
				if (itemObject != null)
				{
					if (itemObject is ViewModelBase)
					{
						EnsureViewDataBindingsHolder(cell, (ViewModelBase)itemObject); // TODO: rename to Update... put ensure in submethod?
					}
					else
					{
						EnsureItemDataBindingsHolder(cell, itemObject);
					}
				}
			}
			return cell;
		}

		private ItemDataBindingsHolder CreateItemDataBindings(UITableViewCell rootView, object itemObject)
		{
			ItemDataBindingsHolder itemDataBindings = null;
			Console.WriteLine("Creating bindings for rootView: " + rootView.ToString());
			List<BindingParameters> bindingParametersList;
			if (ViewDataBindings.RootViewBindingParameters.TryGetValue(rootView, out bindingParametersList))
			{
				Console.WriteLine("Adding list bindings from markup ...");
				Type itemType = itemObject.GetType();
				itemDataBindings = new ItemDataBindingsHolder();
				foreach (var bindingParameter in bindingParametersList)
				{
					if (bindingParameter.PropertyName == ".")
					{
						itemDataBindings.Add(new ItemDataBinding(bindingParameter.View));
					} else {
						var pi = itemType.GetProperty(bindingParameter.PropertyName);
						if (pi != null)
						{
							itemDataBindings.Add(new ItemDataBinding(pi, bindingParameter.View));
						} else {
							var fi = itemType.GetField(bindingParameter.PropertyName);
							if (fi != null) itemDataBindings.Add(new ItemDataBinding(fi, bindingParameter.View));
						}
					}
				}
			}
			return itemDataBindings;
		}

		// If the list item is a viewmodel, we can bind it using a ViewBindings instance, which then becomes the viewholder
		private void EnsureViewDataBindingsHolder(UITableViewCell rootView, ViewModelBase viewModel)
		{
			ViewDataBindingsHolder holder;
			if (!viewDataBindingsHolders.TryGetValue(rootView.Handle, out holder))
			{
				holder = new ViewDataBindingsHolder(rootView, viewModel, "TODO:"); // TODO: do we need a prefix?
				viewDataBindingsHolders.Add(rootView.Handle, holder);
			}
			else
			{
				holder.SetViewModel(viewModel);
			}
		}

		private class ViewDataBindingsHolder
		{
			private ViewModelBase viewModel;
			private readonly ViewDataBindings bindings;

			public ViewDataBindingsHolder(UIView rootView, ViewModelBase viewModel, string idPrefix)
			{
				this.viewModel = viewModel;
				bindings = new ViewDataBindings(rootView, viewModel, idPrefix);
				AddHandlers();
				viewModel.RaisePropertiesChanged();
			}

			public void AddHandlers() { viewModel.PropertyChanged += viewModel_PropertyChanged; }
			public void RemoveHandlers() { viewModel.PropertyChanged -= viewModel_PropertyChanged; }

			public void SetViewModel(ViewModelBase newViewModel)
			{
				RemoveHandlers();
				viewModel = newViewModel;
				AddHandlers();
				bindings.SetViewModel(newViewModel);
			}

			private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
				bindings.UpdateView(e.PropertyName);
			}
		}

		private Dictionary<IntPtr, ViewDataBindingsHolder> viewDataBindingsHolders = new Dictionary<IntPtr, ViewDataBindingsHolder>();

		private class ItemDataBindingsHolder : List<ItemDataBinding> { }

		private Dictionary<IntPtr, ItemDataBindingsHolder> itemDataBindingsHolders = new Dictionary<IntPtr, ItemDataBindingsHolder>();

		// If the list item is not a viewmodel, we bind it using a list of ItemDataBindings, which then becomes the viewholder
		private void EnsureItemDataBindingsHolder(UITableViewCell rootView, object itemObject)
		{
			ItemDataBindingsHolder holder;
			if (!itemDataBindingsHolders.TryGetValue(rootView.Handle, out holder))
			{
				holder = CreateItemDataBindings(rootView, itemObject);
				if (holder == null)	return;
				itemDataBindingsHolders.Add(rootView.Handle, holder);
			}
			foreach (var idb in holder) UpdateView(idb.View, idb.GetValue(itemObject));
		}

		/// <summary>
		/// Override this method in a derived tableviewsource class to change how a data-bound value is set for specific views
		/// </summary>
		/// <param name="view"></param>
		/// <param name="value"></param>
		protected virtual void UpdateView(UIView view, object value)
		{
			if (viewExtensionPoints != null) viewExtensionPoints.UpdateView(view, value); else ViewDataBindings.UpdateView(view, value);
		}

		protected bool GetItemFlag(NSIndexPath indexPath, string flag)
		{
			// Check for constant values true or false
			if (string.IsNullOrEmpty(flag) || flag.ToLower() == "false") return false;
			if (flag.ToLower() == "true") return true;

			// The flag is the name of a boolean property or field of the item object
			var itemObject = GetItem(indexPath);
			if (itemObject != null)
			{
				Type itemType = itemObject.GetType();
				var pi = itemType.GetProperty(flag);
				if (pi != null)	return (bool)pi.GetValue(itemObject);
				var fi = itemType.GetField(flag);
				if (fi != null)	return (bool)fi.GetValue(itemObject);
			}
			return false;
		}

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return GetItemFlag(indexPath, canEdit);
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (viewModel != null)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete)
				{
					if (viewModel.ExecuteCommand(deleteRowCommandName, GetItem(indexPath)))
					{
						if (!listIsObservable) tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
					}
				}
				else if (editingStyle == UITableViewCellEditingStyle.Insert)
				{
					// TODO: check if the indexpath is relevant here? E.g. is it the insert position or the previous/next item?
					if (viewModel.ExecuteCommand(insertRowCommandName, GetItem(indexPath)))
					{
						if (!listIsObservable) tableView.InsertRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
					}
				}
			}
		}

		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			var itemObject = GetItem(sourceIndexPath);
			int deleteAt = sourceIndexPath.Row;
			int insertAt = destinationIndexPath.Row;
			if (itemObject == null || deleteAt == insertAt) return;

			ignoreCollectionChanged = true;
			list.RemoveAt(deleteAt);
			list.Insert(insertAt, itemObject);
			ignoreCollectionChanged = false;
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return GetItemFlag(indexPath, canMove);
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if (viewModel != null) 
			{
				var itemObject = GetItem(indexPath);
				if (rowSelectedPropertyIsCommand) {
					viewModel.ExecuteCommand(rowSelectedPropertyName, itemObject);
				} else {
					viewModel.SetPropertyValue(rowSelectedPropertyName, itemObject);
				}
			}
		}
    }
}

