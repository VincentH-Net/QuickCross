using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using QuickCross;
using SampleApp.Shared;
using SampleApp.Shared.ViewModels;

namespace SampleApp
{
	public partial class MasterView : TableViewBase
    {
		private SampleItemListViewModel ViewModel { get { return SampleAppApplication.Instance.SampleItemListViewModel; } }

        public MasterView(IntPtr handle) : base(handle)
        {
            Title = NSBundle.MainBundle.LocalizedString("Master", "Master");
			
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                ContentSizeForViewInPopover = new SizeF(320f, 600f);
                ClearsSelectionOnViewWillAppear = false;
            }
        }

		public DetailView DetailViewController { get; set; }

		void AddNewItem(object sender, EventArgs args)
        {
			ViewModel.AddItemCommand.Execute(null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.LeftBarButtonItem = EditButtonItem;
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddNewItem);
			InitializeBindings(View, ViewModel);
        }

		protected override object GetCommandParameter(string commandName)
		{
			if (commandName == "ViewItemCommand")
			{
				var indexPath = TableView.IndexPathForSelectedRow;
				if (indexPath != null) return ViewModel.Items[indexPath.Row];
			}
			return base.GetCommandParameter(commandName);
		}
    }
}
