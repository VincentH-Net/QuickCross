using System;
using QuickCross;
using SampleApp.Shared.Models;
using SampleApp.Shared.Services;

namespace SampleApp.Shared.ViewModels
{
    public class SampleItemViewModel : ViewModelBase
    {
        protected SampleItemViewModel () { } // VS Design time support

        private int _itemId;
        private SampleItemService _itemService;

        public SampleItemViewModel(SampleItemService itemService, SampleItem item = null)
        {
            _itemService = itemService;
            Initialize(item);
        }

        public void Initialize(SampleItem item = null)
        {
            if (item == null)
            {
                _itemId = -1;
                Title = "";
                Description = "";
            }
            else
            {
                _itemId = item.Id;
                Title = item.Title;
                Description = item.Description;
            }
        }

        public string Title /* Two-way data-bindable property generated with propdb2 snippet */ { get { return _Title; } set { if (_Title != value) { _Title = value; RaisePropertyChanged(PROPERTYNAME_Title); } } } private string _Title; public const string PROPERTYNAME_Title = "Title";
        public string Description /* Two-way data-bindable property generated with propdb2 snippet */ { get { return _Description; } set { if (_Description != value) { _Description = value; RaisePropertyChanged(PROPERTYNAME_Description); } } } private string _Description; public const string PROPERTYNAME_Description = "Description";

        public RelayCommand SaveCommand /* Data-bindable command that calls Save(), generated with cmd snippet */ { get { if (_SaveCommand == null) _SaveCommand = new RelayCommand(Save); return _SaveCommand; } } private RelayCommand _SaveCommand;

        private void Save()
        {
            if (_itemId < 0)
            {
                _itemService.AddItem(Title, Description);
            }
            else
            {
                _itemService.UpdateItem(new SampleItem() { Id = _itemId, Title = this.Title, Description = this.Description });
            }
            SampleAppApplication.Instance.ContinueToSampleItemList();
        }
    }
}

namespace SampleApp.Shared.ViewModels.Design
{
    public class SampleItemViewModelDesign : SampleItemViewModel
    {
        public SampleItemViewModelDesign() 
        {
            Initialize(new SampleItem { Id = 0, Title = "Item Design", Description = "A design-time item" });
        }
    }
}