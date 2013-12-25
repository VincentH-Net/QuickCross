#if TEMPLATE // To add a new view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command (enter "Get-Help New-View" for details). Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and _APPNAME_ with the viewmodel resp application name.
using System;
using QuickCross.Templates.ViewModels;

namespace QuickCross.Templates
{
	public partial class _VIEWNAME_View : ViewBase
    {
		private _VIEWNAME_ViewModel ViewModel { get { return _APPNAME_Application.Instance._VIEWNAME_ViewModel; } }

		public _VIEWNAME_View(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			InitializeBindings(View, ViewModel);
		}
    }
}
#endif // TEMPLATE