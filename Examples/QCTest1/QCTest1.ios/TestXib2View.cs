using System;
using System.Drawing;
using MonoTouch.UIKit;
using QuickCross;
using QCTest1;
using QCTest1.Shared;
using QCTest1.Shared.ViewModels;

namespace QCTest1
{
	public partial class TestXib2View : ViewBase
    {
        private TestXib2ViewModel ViewModel { get { return QCTest1Application.Instance.TestXib2ViewModel; } }

        public TestXib2View() : base("TestXib2View", null) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Title = "TestXib2View";
            InitializeBindings(View, ViewModel);
		}
    }
}
