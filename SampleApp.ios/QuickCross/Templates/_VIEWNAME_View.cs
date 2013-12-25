#if TEMPLATE
/* To add a new view to a storyboard:
 * 
 * Note: before you add a storyboard view, make sure that you used the New-View command in Visual Studio to add
 * the viewmodel class, the application viewmodel property, the application navigation method, 
 * the navigator interface method signature for the new view to your shared code project. 
 * The New-View command also adds a default navigation method implementation to your navigator - review it to see
 * if it uses the navigation method that you intend (segue / push / ...).
 * 
 * Alternatively, you can do all the actions that the New-View command does manually - either in Visual Studio or in Xamarin Studio,
 * by following the instructions in the comments in these files:
 * - Shared project: QuickCross\Templates\_VIEWNAME_ViewModel.cs
 * - Shared project: I_APPNAME_Navigator.cs
 * - Shared project: I_APPNAME_Application.cs
 * - App project:    _APPNAME_Navigator.cs
 * 
 * 1 From Xamarin Studio, open your storyboard file (in XCode)
 * 2 In XCode, add your view to the storyboard
 * 3 Set the Class name and the Storyboard ID to _VIEWNAME_View (check 'Use Storyboard ID' to also use the same name for the Restoration ID)
 * 4 Switch back from XCode to Xamarin Studio to have the _VIEWNAME_View.cs and _VIEWNAME_View.designer.cs files generated
 * 5 In the generated _VIEWNAME_View.cs, change the base class to ViewBase (or TableViewBase), 
 *   and add the ViewModel property and ViewDidLoad method as shown below
 */
using System;
using QuickCross;
using QuickCross.Templates;
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