#if TEMPLATE // To add a new Xib view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command with option -ViewType Xib (enter "Get-Help New-View" for details) and then follow the instructions in the generated <name>View.cs file. Alternatively you can do all the actions that the New-View command does manually - either in Visual Studio or in Xamarin Studio: copy this file and the corresponding .designer.cs file, then in the copied files remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and QCTest1 with the viewmodel resp application name. Then follow the instructions in the comments on the first line and in the comments that start with "TODO: For each view, " in these files: Shared project: QuickCross\Templates\_VIEWNAME_ViewModel.cs, IQCTest1Navigator.cs and QCTest1Application.cs; App project: QCTest1Navigator.cs.
using System;
using System.Drawing;
using MonoTouch.UIKit;
using QuickCross;
using QCTest1;
using QCTest1.Shared;
using QCTest1.Shared.ViewModels;

namespace QCTest1
{
	public partial class _VIEWNAME_View : ViewBase
    {
        private _VIEWNAME_ViewModel ViewModel { get { return QCTest1Application.Instance._VIEWNAME_ViewModel; } }

        public _VIEWNAME_View() : base("_VIEWNAME_View", null) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
            InitializeBindings(View, ViewModel);
		}
    }
}
#endif // TEMPLATE