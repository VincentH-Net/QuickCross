using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using CloudAuction.Shared;

namespace CloudAuction
{
    public static class OrderResultView
    {
        private static async Task MessageBox(string msg)
        {
            var msgDlg = new MessageDialog(msg);
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();
        }
        
        public static async void Show()
        {
            var vm = CloudAuctionApplication.Instance.OrderResultViewModel;
            await MessageBox(vm.Message);
            vm.DoneCommand.Execute(null);
        }
    }
}
