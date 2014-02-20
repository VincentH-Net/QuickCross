using System;
using System.Collections.ObjectModel;
using QuickCross;
using System.Collections.Generic;

namespace CloudAuction.Shared.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        public OrderViewModel()
        {
            // TODO: pass any services that this model needs as contructor parameters. 
        }

        private Bid bid;

        public void Initialize(Bid bid) { this.bid = bid; }

        #region Data-bindable properties and commands
        public ObservableCollection<string> DeliveryLocationList /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _DeliveryLocationList; } protected set { if (_DeliveryLocationList != value) { _DeliveryLocationList = value; RaisePropertyChanged(PROPERTYNAME_DeliveryLocationList); UpdateDeliveryLocationListHasItems(); } } } private ObservableCollection<string> _DeliveryLocationList; public const string PROPERTYNAME_DeliveryLocationList = "DeliveryLocationList";
        public bool DeliveryLocationListHasItems /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _DeliveryLocationListHasItems; } protected set { if (_DeliveryLocationListHasItems != value) { _DeliveryLocationListHasItems = value; RaisePropertyChanged(PROPERTYNAME_DeliveryLocationListHasItems); } } } private bool _DeliveryLocationListHasItems; public const string PROPERTYNAME_DeliveryLocationListHasItems = "DeliveryLocationListHasItems";
        protected void UpdateDeliveryLocationListHasItems() /* Helper method generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { DeliveryLocationListHasItems = _DeliveryLocationList != null && _DeliveryLocationList.Count > 0; }
        public string DeliveryLocation /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _DeliveryLocation; } set { if (_DeliveryLocation != value) { _DeliveryLocation = value; RaisePropertyChanged(PROPERTYNAME_DeliveryLocation); } } } private string _DeliveryLocation; public const string PROPERTYNAME_DeliveryLocation = "DeliveryLocation";

        public ObservableCollection<string> TitleList /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _TitleList; } protected set { if (_TitleList != value) { _TitleList = value; RaisePropertyChanged(PROPERTYNAME_TitleList); UpdateTitleListHasItems(); } } } private ObservableCollection<string> _TitleList; public const string PROPERTYNAME_TitleList = "TitleList";
        public bool TitleListHasItems /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _TitleListHasItems; } protected set { if (_TitleListHasItems != value) { _TitleListHasItems = value; RaisePropertyChanged(PROPERTYNAME_TitleListHasItems); } } } private bool _TitleListHasItems; public const string PROPERTYNAME_TitleListHasItems = "TitleListHasItems";
        protected void UpdateTitleListHasItems() /* Helper method generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { TitleListHasItems = _TitleList != null && _TitleList.Count > 0; }
        public string Title /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Title; } set { if (_Title != value) { _Title = value; RaisePropertyChanged(PROPERTYNAME_Title); } } } private string _Title; public const string PROPERTYNAME_Title = "Title";

        public string FirstName /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _FirstName; } set { if (_FirstName != value) { _FirstName = value; RaisePropertyChanged(PROPERTYNAME_FirstName); } } } private string _FirstName; public const string PROPERTYNAME_FirstName = "FirstName";
        public string MiddleName /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _MiddleName; } set { if (_MiddleName != value) { _MiddleName = value; RaisePropertyChanged(PROPERTYNAME_MiddleName); } } } private string _MiddleName; public const string PROPERTYNAME_MiddleName = "MiddleName";
        public string LastName /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _LastName; } set { if (_LastName != value) { _LastName = value; RaisePropertyChanged(PROPERTYNAME_LastName); } } } private string _LastName; public const string PROPERTYNAME_LastName = "LastName";
        public string Street /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Street; } set { if (_Street != value) { _Street = value; RaisePropertyChanged(PROPERTYNAME_Street); } } } private string _Street; public const string PROPERTYNAME_Street = "Street";
        public string Zip /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Zip; } set { if (_Zip != value) { _Zip = value; RaisePropertyChanged(PROPERTYNAME_Zip); } } } private string _Zip; public const string PROPERTYNAME_Zip = "Zip";
        public string City /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _City; } set { if (_City != value) { _City = value; RaisePropertyChanged(PROPERTYNAME_City); } } } private string _City; public const string PROPERTYNAME_City = "City";
        public string Country /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Country; } set { if (_Country != value) { _Country = value; RaisePropertyChanged(PROPERTYNAME_Country); } } } private string _Country; public const string PROPERTYNAME_Country = "Country";
        public string Email /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Email; } set { if (_Email != value) { _Email = value; RaisePropertyChanged(PROPERTYNAME_Email); } } } private string _Email; public const string PROPERTYNAME_Email = "Email";
        public string Mobile /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Mobile; } set { if (_Mobile != value) { _Mobile = value; RaisePropertyChanged(PROPERTYNAME_Mobile); } } } private string _Mobile; public const string PROPERTYNAME_Mobile = "Mobile";
        public string Phone /* Two-way data-bindable property generated with propdb2 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Phone; } set { if (_Phone != value) { _Phone = value; RaisePropertyChanged(PROPERTYNAME_Phone); } } } private string _Phone; public const string PROPERTYNAME_Phone = "Phone";

        public RelayCommand CancelCommand /* Data-bindable command that calls Cancel(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_CancelCommand == null) _CancelCommand = new RelayCommand(Cancel); return _CancelCommand; } } private RelayCommand _CancelCommand;
        public RelayCommand ConfirmCommand /* Data-bindable command that calls Confirm(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_ConfirmCommand == null) _ConfirmCommand = new RelayCommand(Confirm); return _ConfirmCommand; } } private RelayCommand _ConfirmCommand;
        #endregion

        private void Cancel()
        {
            CloudAuctionApplication.Instance.ReturnToPreviousView();
        }

        private void Confirm()
        {
            CloudAuctionApplication.Instance.ContinueToOrderResult(bid);
        }
    }
}

// Design-time data support
#if DEBUG
namespace CloudAuction.Shared.ViewModels.Design
{
    public class OrderViewModelDesign : OrderViewModel
    {
        public OrderViewModelDesign()
        {
            DeliveryLocationList = new ObservableCollection<string>(new string[] { "At home", "Pickup" });
            DeliveryLocation = DeliveryLocationList[0];
            TitleList = new ObservableCollection<string>(new string[] { "Mr.", "Ms." });
            Title = TitleList[0];
            FirstName = "First name";
            MiddleName = "Mid";
            LastName = "Last name";
            Street = "Street";
            Zip = "Zip";
            City = "City";
            Country = "Country";
            Email = "email@example.com";
            Mobile = "01 234 567 89";
            Phone = "555 234 567";
        }
    }
}
#endif
