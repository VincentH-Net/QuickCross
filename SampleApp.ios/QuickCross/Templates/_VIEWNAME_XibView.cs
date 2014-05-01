#if TEMPLATE // To add a new Xib view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command with option -ViewType Xib (enter "Get-Help New-View" for details) and then follow the instructions in the generated <name>View.cs file. Alternatively you can do all the actions that the New-View command does manually - either in Visual Studio or in Xamarin Studio: copy this file and the corresponding .designer.cs file, then in the copied files remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and _APPNAME_ with the viewmodel resp application name. Then follow the instructions in the comments on the first line and in the comments that start with "TODO: For each view, " in these files: Shared project: QuickCross\Templates\_VIEWNAME_ViewModel.cs, I_APPNAME_Navigator.cs and _APPNAME_Application.cs; App project: _APPNAME_Navigator.cs.

 /* TODO: To complete adding the _VIEWNAME_ Xib view, follow these steps:
 *
 * 1 In Xamarin Studio, add a new Xib file named _VIEWNAME_View (e.g. with the iPhone View project item template) and open it in XCode.
 * 2 In XCode, in the left pane select "File's Owner", then in the identity inspector
     (which is in the right pane), set the Class to _VIEWNAME_View.
 * 3 With "File's Owner" still selected, select the connections inspector (in the right pane), 
 *   and then drag the view outlet from the right pane to the View in the left pane.
 * 4 Save the Xib in XCode and switch back from XCode to Xamarin Studio.
 * 5 Check the TODO comments in the generated NavigateTo_VIEWNAME_View method in your
 *   _APPNAME_Navigator class to see if it uses the navigation method that you intend.
 * 6 Somewhere in your view models, navigate to this view by calling the NavigateTo_VIEWNAME_()
 *   method on the application.
 * 7 Delete this TODO comment.
 */
using System;
using System.Drawing;
using MonoTouch.UIKit;
using QuickCross;
using QuickCross.Templates;
using QuickCrossLibrary.Templates;
using QuickCrossLibrary.Templates.ViewModels;

namespace QuickCross.Templates
{
    public partial class _VIEWNAME_View : ViewBase<_VIEWNAME_ViewModel>
    {
        public _VIEWNAME_View() : base("_VIEWNAME_View", null) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "_VIEWNAME_View";
            InitializeBindings(View, _APPNAME_Application.Instance._VIEWNAME_ViewModel);
        }
    }
}
#endif // TEMPLATE