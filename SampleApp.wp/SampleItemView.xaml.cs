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
    }
}