using System;
using QuickCross;
using QCTest1;
using QCTest1.Shared;
using QCTest1.Shared.ViewModels;

namespace QCTest1
{
	public partial class ProductListView : TableViewBase
	{
		private ProductListViewModel ViewModel { get { return QCTest1Application.Instance.ProductListViewModel; } }

		public ProductListView(IntPtr handle) : base(handle) { }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			InitializeBindings(View, ViewModel);
		}
	}
}
