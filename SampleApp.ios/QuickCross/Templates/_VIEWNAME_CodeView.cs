#if TEMPLATE // To add a new storyboard table view class: in the Visual Studio Package Manager Console (menu View | Other Windows), use the New-View command (enter "Get-Help New-View" for details) and then follow the instructions in the generated <name>View.TODO.cs file. Alternatively you can do all the actions that the New-View command does manually - either in Visual Studio or in Xamarin Studio, by following the instructions in the comments on the first line and comments that start with "TODO: For each view, " in these files: Shared project: QuickCross\Templates\_VIEWNAME_ViewModel.cs, I_APPNAME_Navigator.cs and I_APPNAME_Application.cs; App project: _APPNAME_Navigator.cs.
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
        UIButton button;
        int numClicks = 0;
        float buttonWidth = 200;
        float buttonHeight = 50;

        private _VIEWNAME_ViewModel ViewModel { get { return _APPNAME_Application.Instance._VIEWNAME_ViewModel; } }

		public _VIEWNAME_View(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            button = UIButton.FromType(UIButtonType.RoundedRect);

            button.Frame = new RectangleF(
                View.Frame.Width / 2 - buttonWidth / 2,
                View.Frame.Height / 2 - buttonHeight / 2,
                buttonWidth,
                buttonHeight);

            button.SetTitle("Click me", UIControlState.Normal);

            button.TouchUpInside += (object sender, EventArgs e) =>
            {
                button.SetTitle(String.Format("clicked {0} times", numClicks++), UIControlState.Normal);
            };

            button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
                UIViewAutoresizing.FlexibleBottomMargin;

            View.AddSubview(button);

            var bindingParameters = new BindingParameters[] {
                new BindingParameters { View = button, PropertyName = ViewModel.COMMANDNAME_IncreaseCountCommand, Mode = BindingMode.Command }
                // TODO: ***HERE add label to set nr of clicked
            };
            InitializeBindings(View, ViewModel);
		}
    }
}
#endif // TEMPLATE