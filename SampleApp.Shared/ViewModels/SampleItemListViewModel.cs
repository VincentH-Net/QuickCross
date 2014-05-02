using System;
using System.Collections.ObjectModel;
using QuickCross;
using SampleApp.Shared.Models;
using SampleApp.Shared.Services;

namespace SampleApp.Shared.ViewModels
{
    public class SampleItemListViewModel : ViewModelBase
    {
        private SampleItemService _itemService;

        protected SampleItemListViewModel () { } // VS Design time support

        public SampleItemListViewModel(SampleItemService itemService)
        {
            _itemService = itemService;
        }

        public void Refresh()
        {
            if (_itemService == null) return;
            Items = new ObservableCollection<SampleItem>(_itemService.GetItems());
        }

        public ObservableCollection<SampleItem> Items /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Items; } protected set { if (_Items != value) { _Items = value; RaisePropertyChanged(PROPERTYNAME_Items); UpdateItemsHasItems(); } } } private ObservableCollection<SampleItem> _Items; public const string PROPERTYNAME_Items = "Items";
        public bool ItemsHasItems /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ItemsHasItems; } protected set { if (_ItemsHasItems != value) { _ItemsHasItems = value; RaisePropertyChanged(PROPERTYNAME_ItemsHasItems); } } } private bool _ItemsHasItems; public const string PROPERTYNAME_ItemsHasItems = "ItemsHasItems";
        protected void UpdateItemsHasItems() /* Helper method generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { ItemsHasItems = _Items != null && _Items.Count > 0; }

        public RelayCommand ViewItemCommand /* Data-bindable command with parameter that calls ViewItem(), generated with cmdp snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_ViewItemCommand == null) _ViewItemCommand = new RelayCommand(ViewItem); return _ViewItemCommand; } } private RelayCommand _ViewItemCommand;
        public RelayCommand AddItemCommand /* Data-bindable command that calls AddItem(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_AddItemCommand == null) _AddItemCommand = new RelayCommand(AddItem); return _AddItemCommand; } } private RelayCommand _AddItemCommand;
        public RelayCommand RemoveItemCommand /* Data-bindable command with parameter that calls RemoveItem(), generated with cmdp snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_RemoveItemCommand == null) _RemoveItemCommand = new RelayCommand(RemoveItem); return _RemoveItemCommand; } } private RelayCommand _RemoveItemCommand;

        private void ViewItem(object parameter)
        {
            var item = (SampleItem)parameter;
            if (item != null) SampleAppApplication.Instance.ContinueToSampleItem(item);
        }

        private void AddItem()
        {
            SampleAppApplication.Instance.ContinueToSampleItem();
        }

        private void RemoveItem(object parameter)
        {
            if (_itemService == null) return;
            var item = (SampleItem)parameter;
            if (item != null)
            {
                _itemService.RemoveItem(item);
                Items.Remove(item);
            }
        }
    }
}

namespace SampleApp.Shared.ViewModels.Design
{
    public class SampleItemListViewModelDesign : SampleItemListViewModel
    {
        private static SampleItem[] _itemData = new SampleItem[] {
            new SampleItem { Id = 1, Title = "One Design", Description = "First design-time item" },
            new SampleItem { Id = 2, Title = "Two Design", Description = "Second design-time item" },
            new SampleItem { Id = 3, Title = "Three Design", Description = "Third design-time item" }
        };

        public SampleItemListViewModelDesign()
        {
            Items = new ObservableCollection<SampleItem>(_itemData);
        }
    }
}