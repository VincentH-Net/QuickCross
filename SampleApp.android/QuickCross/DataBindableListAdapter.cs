using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Android.Views;
using Android.Widget;
using System.Collections.Specialized;
using System;

namespace QuickCross
{
    public interface IDataBindableListAdapter
    {
        int GetItemPosition(object item);
        object GetItemAsObject(int position);
        bool SetList(IList list);
        void AddHandlers();
        void RemoveHandlers();
    }

    public class DataBindableListAdapter<T> : BaseAdapter, IDataBindableListAdapter
    {
        private class ItemDataBinding
        {
            public readonly PropertyInfo ObjectPropertyInfo;
            public readonly FieldInfo ObjectFieldInfo;
            public readonly int ResourceId;

            public string Name { get { return (ObjectPropertyInfo != null) ? ObjectPropertyInfo.Name : ObjectFieldInfo.Name; } }
            public object GetValue(object item) { return (ObjectPropertyInfo != null) ? ObjectPropertyInfo.GetValue(item) : ObjectFieldInfo.GetValue(item); }

            public ItemDataBinding(PropertyInfo objectPropertyInfo, int resourceId)
            {
                this.ObjectPropertyInfo = objectPropertyInfo;
                this.ResourceId = resourceId;
            }

            public ItemDataBinding(FieldInfo objectFieldInfo, int resourceId)
            {
                this.ObjectFieldInfo = objectFieldInfo;
                this.ResourceId = resourceId;
            }
        }

        private readonly LayoutInflater layoutInflater;
        private readonly int itemTemplateResourceId;
        private readonly string idPrefix;
        private readonly int? itemValueResourceId;
        private readonly ViewDataBindings.IViewExtensionPoints viewExtensionPoints;

        private IList list;
        private List<ItemDataBinding> itemDataBindings;
        private bool? itemIsViewModel;

        public DataBindableListAdapter(LayoutInflater layoutInflater, int itemTemplateResourceId, string idPrefix, int? itemValueResourceId = null, ViewDataBindings.IViewExtensionPoints viewExtensionPoints = null)
        {
            if (layoutInflater == null) throw new ArgumentNullException("layoutInflater");
            this.layoutInflater = layoutInflater;
            this.itemTemplateResourceId = itemTemplateResourceId;
            this.idPrefix = idPrefix;
            this.itemValueResourceId = itemValueResourceId;
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
        /// Override this method in a derived adapter class to register additional event handlers for your adapter. Always call base.AddHandlers() in your override.
        /// </summary>
        public virtual void AddHandlers() 
        { 
            AddListHandler();
            foreach (var viewDataBindingsHolder in viewDataBindingsHolders) { viewDataBindingsHolder.AddHandlers(); }
        }

        /// <summary>
        /// Override this method in a derived adapter class to unregister additional event handlers for your adapter. Always call base.AddHandlers() in your override.
        /// </summary>
        public virtual void RemoveHandlers() 
        { 
            RemoveListHandler();
            foreach (var viewDataBindingsHolder in viewDataBindingsHolders) { viewDataBindingsHolder.RemoveHandlers(); }
        }

        /// <summary>
        /// Override this method in a derived adapter class to react to changes in a list if it implements INotifyCollectionChanged (e.g. an ObservableCollection)
        /// </summary>
        /// <param name="sender">The ObservableCollection that was changed</param>
        /// <param name="e">See http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html for details</param>
        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyDataSetChanged(); // MQC TODO: Check if this should & can be optimized, see for details documentation at http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html
            if (viewExtensionPoints != null) viewExtensionPoints.OnCollectionChanged(sender, e);
        }

        private void DataBindableListAdapter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(sender, e);
        }

        public int GetItemPosition(object item)
        {
            return (list == null) ? -1 : list.IndexOf(item);
        }

        public object GetItemAsObject(int position)
        {
            return (list == null || position < 0 || position >= list.Count) ? null : list[position];
        }

        public bool SetList(IList list)
        {
            if (Object.ReferenceEquals(this.list, list)) return false;
            RemoveListHandler();
            this.list = list;
            AddListHandler();
            NotifyDataSetChanged();
            return true;
        }

        public override int Count
        {
            get { return (list == null) ? 0 : list.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return (Wrapper<Object>)GetItemAsObject(position);
        }

        public override long GetItemId(int position)
        {
            object item = GetItemAsObject(position);
            return item == null? 0 : item.GetHashCode();
                // The default GetHashCode implementation represents the instance. Note however that after the object is
                // reclaimed during garbage collection, another object may then return the same hash.
                // To prevent this (highly unlikely) event, override GetHashCode on your list item object to return a 
                // unique stable int value that is calculated from a unique stable Id field (or combination of fields).
        }

        public override bool HasStableIds { get { return true; } }

        private string IdName(string name) { return idPrefix + name; }

        /// <summary>
        /// Override this method in a derived adapter class to change how a data-bound value is set for specific views
        /// </summary>
        /// <param name="view"></param>
        /// <param name="value"></param>
        protected virtual void UpdateView(View view, object value)
        {
            if (viewExtensionPoints != null) viewExtensionPoints.UpdateView(view, value); else ViewDataBindings.UpdateView(view, value);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var rootView = convertView ?? layoutInflater.Inflate(itemTemplateResourceId, parent, false);

            if (list != null)
            {
                if (itemValueResourceId.HasValue)
                {
                    var valueView = EnsureSingleViewHolder(rootView);
                    UpdateView(valueView, list[position]);
                }
                else
                {
                    object itemObject = list[position];
                    if (itemObject != null)
                    {
                        EnsureBindings(itemObject);
                        if (itemIsViewModel.Value)
                        {
                            EnsureViewDataBindingsHolder(rootView, (ViewModelBase)itemObject);
                        }
                        else
                        {
                            ListDictionary viewHolder = EnsureMultipleViewHolder(rootView);
                            foreach (var idb in itemDataBindings) UpdateView((View)viewHolder[idb.ResourceId], idb.GetValue(itemObject));
                        }
                    }
                }
            }
            return rootView;
        }

        // Implement the ViewHolder pattern; e.g. see http://www.jmanzano.es/blog/?p=166
        private View EnsureSingleViewHolder(View rootView)
        {
            var valueView = (View)rootView.Tag;
            if (valueView == null)
            {
                valueView = rootView.FindViewById(itemValueResourceId.Value);
                rootView.Tag = valueView;
            }
            return valueView;
        }

        // If the list item is a viewmodel, we can bind it using a ViewBindings instance, which then becomes the viewholder
        private void EnsureViewDataBindingsHolder(View rootView, ViewModelBase viewModel)
        {
            ViewDataBindingsHolder holder = (Wrapper<ViewDataBindingsHolder>)rootView.Tag;
            if (holder == null)
            {
                holder = new ViewDataBindingsHolder(rootView, viewModel, layoutInflater, idPrefix, viewExtensionPoints);
                viewDataBindingsHolders.Add(holder);
                rootView.Tag = (Wrapper<ViewDataBindingsHolder>)holder;
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

			public ViewDataBindingsHolder(View rootView, ViewModelBase viewModel, LayoutInflater layoutInflater, string idPrefix, ViewDataBindings.IViewExtensionPoints viewExtensionPoints)
            {
                this.viewModel = viewModel;
                bindings = new ViewDataBindings(rootView, viewModel, layoutInflater, idPrefix, viewExtensionPoints);
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

        private HashSet<ViewDataBindingsHolder> viewDataBindingsHolders = new HashSet<ViewDataBindingsHolder>();
        
        
        // Implement the ViewHolder pattern; e.g. see http://www.jmanzano.es/blog/?p=166
        private ListDictionary EnsureMultipleViewHolder(View rootView)
        {
            ListDictionary viewHolder = (Wrapper<ListDictionary>)rootView.Tag;
            if (viewHolder == null)
            {
                viewHolder = new ListDictionary();
                foreach (var idb in itemDataBindings) viewHolder.Add(idb.ResourceId, rootView.FindViewById(idb.ResourceId));
                rootView.Tag = (Wrapper<ListDictionary>)viewHolder;
            }
            return viewHolder;
        }

        private void EnsureBindings(object itemObject)
        {
            if (itemDataBindings == null && !itemIsViewModel.HasValue)
            {
                itemIsViewModel = itemObject is ViewModelBase;
                if (!itemIsViewModel.Value)
                {
                    itemDataBindings = new List<ItemDataBinding>();

                    Type itemType = itemObject.GetType();

                    foreach (var pi in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var resourceId = AndroidHelpers.FindResourceId(IdName(pi.Name));
                        if (resourceId.HasValue) itemDataBindings.Add(new ItemDataBinding(pi, resourceId.Value));
                    }

                    foreach (var fi in itemType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var resourceId = AndroidHelpers.FindResourceId(IdName(fi.Name));
                        if (resourceId.HasValue) itemDataBindings.Add(new ItemDataBinding(fi, resourceId.Value));
                    }
                }
            }
        }
    }
}