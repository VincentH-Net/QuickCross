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
    }
}