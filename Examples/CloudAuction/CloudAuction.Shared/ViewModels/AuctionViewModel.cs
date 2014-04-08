using System;
using System.Threading.Tasks;
using QuickCross;

namespace CloudAuction.Shared.ViewModels
{
    public class AuctionViewModel : ViewModelBase
    {
        public AuctionViewModel()
        {
            // TODO: pass any services that this model needs as contructor parameters. 
        }

        #region Data-bindable properties and commands
        public string Name /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Name; } protected set { if (_Name != value) { _Name = value; RaisePropertyChanged(PROPERTYNAME_Name); } } } private string _Name; public const string PROPERTYNAME_Name = "Name";
        public string Intro /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Intro; } protected set { if (_Intro != value) { _Intro = value; RaisePropertyChanged(PROPERTYNAME_Intro); } } } private string _Intro; public const string PROPERTYNAME_Intro = "Intro";
        public Uri Image /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Image; } protected set { if (_Image != value) { _Image = value; RaisePropertyChanged(PROPERTYNAME_Image); } } } private Uri _Image; public const string PROPERTYNAME_Image = "Image";
        public string Description /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Description; } protected set { if (_Description != value) { _Description = value; RaisePropertyChanged(PROPERTYNAME_Description); } } } private string _Description; public const string PROPERTYNAME_Description = "Description";
        public string CurrentPrice /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _CurrentPrice; } protected set { if (_CurrentPrice != value) { _CurrentPrice = value; RaisePropertyChanged(PROPERTYNAME_CurrentPrice); } } } private string _CurrentPrice; public const string PROPERTYNAME_CurrentPrice = "CurrentPrice";
        public int TimePercentageRemaining /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _TimePercentageRemaining; } protected set { if (_TimePercentageRemaining != value) { _TimePercentageRemaining = value; RaisePropertyChanged(PROPERTYNAME_TimePercentageRemaining); } } } private int _TimePercentageRemaining; public const string PROPERTYNAME_TimePercentageRemaining = "TimePercentageRemaining";
        public string ListPrice /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ListPrice; } protected set { if (_ListPrice != value) { _ListPrice = value; RaisePropertyChanged(PROPERTYNAME_ListPrice); } } } private string _ListPrice; public const string PROPERTYNAME_ListPrice = "ListPrice";
        public int AvailableCount /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _AvailableCount; } protected set { if (_AvailableCount != value) { _AvailableCount = value; RaisePropertyChanged(PROPERTYNAME_AvailableCount); } } } private int _AvailableCount; public const string PROPERTYNAME_AvailableCount = "AvailableCount";
        public string Condition /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Condition; } protected set { if (_Condition != value) { _Condition = value; RaisePropertyChanged(PROPERTYNAME_Condition); } } } private string _Condition; public const string PROPERTYNAME_Condition = "Condition";
        public string TimeRemaining /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _TimeRemaining; } protected set { if (_TimeRemaining != value) { _TimeRemaining = value; RaisePropertyChanged(PROPERTYNAME_TimeRemaining); } } } private string _TimeRemaining; public const string PROPERTYNAME_TimeRemaining = "TimeRemaining";

        public RelayCommand LogoutCommand /* Data-bindable command that calls Logout(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_LogoutCommand == null) _LogoutCommand = new RelayCommand(Logout); return _LogoutCommand; } } private RelayCommand _LogoutCommand;
        public RelayCommand PlaceBidCommand /* Data-bindable command that calls PlaceBid(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_PlaceBidCommand == null) _PlaceBidCommand = new RelayCommand(PlaceBid); return _PlaceBidCommand; } } private RelayCommand _PlaceBidCommand;
        #endregion

        private void Logout()
        {
            throw new NotImplementedException(); // TODO: Implement Logout()
        }

        private void PlaceBid()
        {
            CloudAuctionApplication.Instance.ContinueToOrder(new Bid { 
                LotId = 1, 
                Price = CurrentPrice, 
                ProductName = Name 
            });
        }
    }
}

// Design data support
namespace CloudAuction.Shared.ViewModels.Design
{
    public class AuctionViewModelDesign : AuctionViewModel
    {
        public AuctionViewModelDesign()
        {
            Name = "PANASONIC DMC-TZ25EG";
            Intro = "Receive $20 NOW when you buy Panasonic LUMIX Traveller camera";
            Image = new Uri("https://raw.github.com/MacawNL/QuickCross/master/Examples/CloudAuction/assets/lot_3_1.jpg");
            Description = "This compact and powerful camera has an amazing 32x intelligent zoom and 16x optical zoom. Combine this huge zoom reach with the 24mm ultra wide angle and you have a camera that you can use to capture anything you want to!";
            CurrentPrice = "$ 148,00";
            TimePercentageRemaining = 42;
            ListPrice = "$ 349,00";
            AvailableCount = 48;
            Condition = "Damaged";
            TimeRemaining = "0:01:17";

            var _ = RunAuction();
        }

        private async Task RunAuction()
        {
            int maxPrice = 299, minPrice = 149;
            do
            {
                TimePercentageRemaining = 100;
                for (int secondsRemaining = 90; secondsRemaining > 0; secondsRemaining--)
                {
                    await Task.Delay(1000);
                    int price = minPrice + ((maxPrice - minPrice) * secondsRemaining / 90);
                    CurrentPrice = string.Format("$ {0},00", price);
                    TimePercentageRemaining = (100 * secondsRemaining) / 90;
                    TimeRemaining = TimeSpan.FromSeconds(secondsRemaining).ToString();
                }
            } while (--AvailableCount > 0);
        }
    }
}
