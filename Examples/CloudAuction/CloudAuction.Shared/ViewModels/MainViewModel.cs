using System;
using QuickCross;

namespace CloudAuction.Shared.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public enum SubView { Auction, Products, Help }

        #region Data-bindable properties and commands
        public RelayCommand LogoutCommand /* Data-bindable command that calls Logout(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_LogoutCommand == null) _LogoutCommand = new RelayCommand(Logout); return _LogoutCommand; } } private RelayCommand _LogoutCommand;
        #endregion

        private void Logout()
        {
            // TODO: Implement Logout()
        }
    }
}
