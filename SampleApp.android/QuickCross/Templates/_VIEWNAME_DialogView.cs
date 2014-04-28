#if TEMPLATE // To add a new fragment view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command with option -ViewType Fragment (enter "Get-Help New-View" for details). Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and _APPNAME_ with the viewmodel resp application name.
using Android.OS;
using Android.Views;
using Android.Widget;
using QuickCross;
using QuickCrossLibrary.Templates;
using QuickCrossLibrary.Templates.ViewModels;

namespace QuickCross.Templates
{
    /// <summary>
    /// A Dialog that shows a custom view. See https://github.com/MacawNL/QuickCross#android-dialogs
    /// for guidance on how to implement the dialog logic in your application and viewmodels.
    /// </summary>
    public class _VIEWNAME_View : DialogFragmentViewBase<_VIEWNAME_ViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout._VIEWNAME_View, container, false);
            Initialize(view, _APPNAME_Application.Instance._VIEWNAME_ViewModel, inflater);
            Dialog.SetTitle("_VIEWNAME_");

            // TODO: add event handler that calls Dismiss() on each UI element that should dismiss the dialog, e.g. on a button:
            // view.FindViewById<Button>(Resource.Id._VIEWNAME__OkCommand).Click += (s, e) => Dismiss();
            // Note that the actual logic is coded in the bound command in the viewmodel; these event handlers are only needed to dismiss the dialog.

            return view;
        }
    }
}
#endif // TEMPLATE