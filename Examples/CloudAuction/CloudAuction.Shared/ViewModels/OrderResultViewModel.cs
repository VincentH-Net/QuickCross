using System;
using QuickCross;

namespace CloudAuction.Shared.ViewModels
{
    public class OrderResultViewModel : ViewModelBase
    {
        public OrderResultViewModel()
        {
            // TODO: pass any services that this model needs as contructor parameters. 
        }

        public void Initialize(Bid bid)
        {
            Message = String.Format(
                "Congratulations with your purchase:\r\n\r\n{0}\r\n\r\nYou will receive an email with delivery or pickup details.\r\n\r\nEnyoy your Brand purchase!",
                bid.ProductName
            );
        }

        #region Data-bindable properties and commands
        public string Message /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Message; } protected set { if (_Message != value) { _Message = value; RaisePropertyChanged(PROPERTYNAME_Message); } } } private string _Message; public const string PROPERTYNAME_Message = "Message";

        public RelayCommand DoneCommand /* Data-bindable command that calls Done(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_DoneCommand == null) _DoneCommand = new RelayCommand(Done); return _DoneCommand; } } private RelayCommand _DoneCommand;
        #endregion

        private void Done()
        {
            CloudAuctionApplication.Instance.ContinueToMain();
        }
    }
}
