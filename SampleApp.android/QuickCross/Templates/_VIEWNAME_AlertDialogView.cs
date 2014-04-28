#if TEMPLATE // To add a new fragment view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command with option -ViewType Fragment (enter "Get-Help New-View" for details). Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and _APPNAME_ with the viewmodel resp application name.
using Android.OS;
using Android.Views;
using Android.App;
using QuickCross;
using QuickCrossLibrary.Templates;
using QuickCrossLibrary.Templates.ViewModels;

namespace QuickCross.Templates
{
    /// <summary>
    /// An AlertDialog
    /// See http://docs.xamarin.com/guides/android/platform_features/fragments/part_3_-_specialized_fragment_classes/ 
    /// for guidance on how to create AlertDialogs or dialogs with a custom view.
    /// </summary>
    public class _VIEWNAME_View : DialogFragmentViewBase<_VIEWNAME_ViewModel>
    {
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Initialize(_APPNAME_Application.Instance._VIEWNAME_ViewModel);
            var builder = new AlertDialog.Builder(Activity)
                .SetTitle("_VIEWNAME_")
                .SetMessage("This is an AlertDialog")
                // TODO: Call the appropriate viewmodel command when a button is clicked, e.g.:
                // .SetPositiveButton("OK", (s, a) => ViewModel.OkCommand.Execute(null))
                ;
            return builder.Create();
        }
    }
}
#endif // TEMPLATE