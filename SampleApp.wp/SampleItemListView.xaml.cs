using Microsoft.Phone.Controls;
using SampleApp.Shared;

namespace SampleApp
{
    public partial class SampleItemListView : PhoneApplicationPage
    {
        public SampleItemListView()
        {
            InitializeComponent();
            DataContext = SampleAppApplication.Instance.SampleItemListViewModel;
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var vm = DataContext as QuickCross.ViewModelBase;
            if (vm != null) vm.OnUserInteractionStopped();
            base.OnNavigatingFrom(e);
        }
    }
}