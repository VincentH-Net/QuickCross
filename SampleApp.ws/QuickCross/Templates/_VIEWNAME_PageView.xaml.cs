#if TEMPLATE // To add a page view class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "New-View". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _APPNAME_ and _VIEWNAME_ with the application and view names.
using Windows.UI.Xaml.Controls;
using QuickCrossLibrary.Templates;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuickCross.Templates
{
    public sealed partial class _VIEWNAME_View : Page
    {
        public _VIEWNAME_View()
        {
            InitializeComponent();
            DataContext = _APPNAME_Application.Instance._VIEWNAME_ViewModel;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var vm = DataContext as QuickCross.ViewModelBase;
            if (vm != null) vm.OnUserInteractionStopped();
            base.OnNavigatingFrom(e);
        }
    }
}
#endif // TEMPLATE