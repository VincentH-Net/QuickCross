using Microsoft.Phone.Controls;
using SampleApp.Shared;

namespace SampleApp
{
    public partial class SampleItemView : PhoneApplicationPage
    {
        public SampleItemView()
        {
            InitializeComponent();
            DataContext = SampleAppApplication.Instance.SampleItemViewModel;
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var vm = DataContext as QuickCross.ViewModelBase;
            if (vm != null) vm.OnUserInteractionStopped();
            base.OnNavigatingFrom(e);
        }
    }
}