#if TEMPLATE // To add a new fragment view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command with option -ViewType Fragment (enter "Get-Help New-View" for details). Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and _APPNAME_ with the viewmodel resp application name.
using Android.OS;
using Android.Views;
using QuickCross;
using QuickCrossLibrary.Templates;
using QuickCrossLibrary.Templates.ViewModels;

namespace QuickCross.Templates
{
    public class _VIEWNAME_View : FragmentViewBase<_VIEWNAME_ViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _APPNAME_Application.Instance.ContinueTo_VIEWNAME_(skipNavigation: true); // This line is only needed if this view can be navigated to by some other means than through ContinueTo_VIEWNAME_() 
            var view = inflater.Inflate(Resource.Layout._VIEWNAME_View, container, false);
            Initialize(view, _APPNAME_Application.Instance._VIEWNAME_ViewModel, inflater);
            return view;
        }
    }
}
#endif // TEMPLATE