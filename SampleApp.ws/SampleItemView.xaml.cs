using SampleApp.Shared;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SampleItemView : Page
    {
        public SampleItemView()
        {
            this.InitializeComponent();
            DataContext = SampleAppApplication.Instance.SampleItemViewModel;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var vm = DataContext as QuickCross.ViewModelBase;
            if (vm != null) vm.OnUserInteractionStopped();
            base.OnNavigatingFrom(e);
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SampleAppNavigator.Instance.NavigateToPreviousView();
        }
    }
}
