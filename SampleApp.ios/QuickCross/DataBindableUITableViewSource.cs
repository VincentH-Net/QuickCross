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

			public string Name { get { return (ObjectPropertyInfo != null) ? ObjectPropertyInfo.Name : ObjectFieldInfo.Name; } }
			public object GetValue(object item) { return (ObjectPropertyInfo != null) ? ObjectPropertyInfo.GetValue(item) : ObjectFieldInfo.GetValue(item); }

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
		}

		private readonly ViewDataBindings.ViewExtensionPoints viewExtensionPoints;

		private IList list;

		private readonly UITableView tableView;
		private readonly NSString cellIdentifier;

		public DataBindableUITableViewSource(UITableView tableView, string cellIdentifier, ViewDataBindings.ViewExtensionPoints viewExtensionPoints = null)
        {
			this.tableView = tableView;
			this.cellIdentifier = new NSString(cellIdentifier);
			this.viewExtensionPoints = viewExtensionPoints;
        }

		private void AddListHandler()
		{
			if (list is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged)list).CollectionChanged += DataBindableListAdapter_CollectionChanged;
			}
		}

		private void RemoveListHandler()
		{
			if (list is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged)list).CollectionChanged -= DataBindableListAdapter_CollectionChanged;
			}
		}

		/// <summary>
		/// Override this method in a derived adapter class to react to changes in a list if it implements INotifyCollectionChanged (e.g. an ObservableCollection)
		/// </summary>
		/// <param name="sender">The ObservableCollection that was changed</param>
		/// <param name="e">See http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html for details</param>
		protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			tableView.ReloadData(); // MQC TODO: Check if this should & can be optimized, see for details documentation at http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html
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
			AddListHandler();
			tableView.ReloadData();
			return true;
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

		// Customize the appearance of table view cells.
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (UITableViewCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
			// At this point, the Bind property has been loaded and binding parameters have been created, grouped under for the root view
			if (list != null)
			{
				object itemObject = list[indexPath.Row];
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
				bindings.EnsureCommandBindings();  // Then add any command bindings that were not specified in code (based on the Id naming convention)
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

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			// Return false if you do not want the specified item to be editable.
			return true;
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete)
			{
				// Delete the row from the data source.
				// TODO: should this be a command instead? or is this the action as long as there is no command bound? how do we bind delete command?
				list.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
			}
			else if (editingStyle == UITableViewCellEditingStyle.Insert)
			{
				// Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
			}
		}
		/*		
			// Override to support rearranging the table view.
		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
		}
		*/
		/*
		// Override to support conditional rearranging of the table view.
		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			// Return false if you do not want the item to be re-orderable.
			return true;
		}
		*/

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			// TODO: this should be a command. if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)	controller.DetailViewController.SetDetailItem(objects[indexPath.Row]);
		}
    }
}

