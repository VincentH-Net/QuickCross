#if TEMPLATE // To add a new code view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command with option -ViewType Code (enter "Get-Help New-View" for details). Alternatively you can do all the actions that the New-View command does manually - either in Visual Studio or in Xamarin Studio: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWNAME_ and _APPNAME_ with the viewmodel resp application name. Then follow the instructions in the comments on the first line and in the comments that start with "TODO: For each view, " in these files: Shared project: QuickCross\Templates\_VIEWNAME_ViewModel.cs, I_APPNAME_Navigator.cs and _APPNAME_Application.cs; App project: _APPNAME_Navigator.cs.
using System;
using System.Drawing;
using MonoTouch.UIKit;
using QuickCross;
using QuickCross.Templates;
using QuickCrossLibrary.Templates;
using QuickCrossLibrary.Templates.ViewModels;

namespace QuickCross.Templates
{
	public partial class _VIEWNAME_View : ViewBase
    {
        private _VIEWNAME_ViewModel ViewModel { get { return _APPNAME_Application.Instance._VIEWNAME_ViewModel; } }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
            Title = "_VIEWNAME_";
            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;

            #region Bind to example property and command
            var button = UIButton.FromType(UIButtonType.System);
            float buttonWidth = 200, buttonHeight = 30;
            button.Frame = new RectangleF(View.Frame.Width / 2 - buttonWidth / 2, View.Frame.Height / 2 - buttonHeight / 2, buttonWidth, buttonHeight);
            button.SetTitle("Click me", UIControlState.Normal);
            View.AddSubview(button);

            var label = new UILabel(new RectangleF(button.Frame.Left, button.Frame.Bottom + 2, button.Frame.Width, button.Frame.Height));
            label.TextAlignment = UITextAlignment.Center;
            View.AddSubview(label);

            var bindingsParameters = new BindingParameters[] {
                new BindingParameters { View = button, PropertyName = _VIEWNAME_ViewModel.COMMANDNAME_IncreaseCountCommand, Mode = BindingMode.Command },
                new BindingParameters { View = label, PropertyName = _VIEWNAME_ViewModel.PROPERTYNAME_Count }
            };
            #endregion Bind to example property and command

            InitializeBindings(View, ViewModel, bindingsParameters);
		}
    }
}
#endif // TEMPLATE