using System;
using System.Drawing;
using MonoTouch.UIKit;
using QuickCross;
using QCTest1;
using QCTest1.Shared;
using QCTest1.Shared.ViewModels;

namespace QCTest1
{
	public partial class MainView : ViewBase
    {
        private MainViewModel ViewModel { get { return QCTest1Application.Instance.MainViewModel; } }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
            Title = "Main 1";
            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;

            var button = UIButton.FromType(UIButtonType.System);
            float buttonWidth = 200, buttonHeight = 30;
            button.Frame = new RectangleF(View.Frame.Width / 2 - buttonWidth / 2, View.Frame.Height / 2 - buttonHeight / 2, buttonWidth, buttonHeight);
            button.SetTitle("Click me", UIControlState.Normal);
            View.AddSubview(button);

            var label = new UILabel(new RectangleF(button.Frame.Left, button.Frame.Bottom + 2, button.Frame.Width, button.Frame.Height));
            label.TextAlignment = UITextAlignment.Center;
            View.AddSubview(label);

            var bindingsParameters = new BindingParameters[] {
                new BindingParameters { View = button, PropertyName = MainViewModel.COMMANDNAME_IncreaseCountCommand, Mode = BindingMode.Command },
                new BindingParameters { View = label, PropertyName = MainViewModel.PROPERTYNAME_Count }
            };

            InitializeBindings(View, ViewModel, bindingsParameters);
		}
    }
}
