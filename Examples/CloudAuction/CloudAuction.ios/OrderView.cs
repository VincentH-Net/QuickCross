using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using QuickCross;
using CloudAuction;
using CloudAuction.Shared;
using CloudAuction.Shared.ViewModels;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace CloudAuction
{
	/*
    public class Expense
    {
        [Section("Expense Entry")]

        [Entry("Enter expense name")]
        public string Name { 
            get { 
                return CloudAuctionApplication.Instance.OrderViewModel.City; } 
            set { 
                CloudAuctionApplication.Instance.OrderViewModel.City = value; }
        }

        [Section("Expense Details")]

        [Caption("Description")]
        [Entry]
        public string Details;

        [Checkbox]
        public bool IsApproved = true;
    }

	public static class DialogBindings
	{
		public static Dictionary<UIViewController, Dictionary<string, Element>> Bindings = new Dictionary<UIViewController, Dictionary<string, Element>>();

		public static Element Bind(this Element element, string propertyName, BindingMode mode = BindingMode.OneWay)
		{
			if (element != null)
			{
				switch (element.GetType().Name)
				{
					case "EntryElement": ((EntryElement)element).Changed += ElementExtensions_Changed; break;
					case "BooleanElement": ((BooleanElement)element).ValueChanged += ElementExtensions_ValueChanged; break;
				}
			}
			return element;
		}

		static void ElementExtensions_ValueChanged(object sender, EventArgs e)
		{
		}

		static void ElementExtensions_Changed(object sender, EventArgs e)
		{
		}
	}

	public class Member<T>
	{
		private object obj;
		private PropertyInfo propertyInfo;
		private FieldInfo fieldInfo;

		private void Initialize(string memberName)
		{
			var type = obj.GetType();
			propertyInfo = type.GetProperty(memberName);
			if (propertyInfo != null) return;
			fieldInfo = type.GetField(memberName);
			if (fieldInfo == null) throw new ArgumentException(string.Format("Property or field '{0}' not found on type {1}", memberName, type.FullName));
		}

		public T Value {
			get { return (T)(propertyInfo != null ? propertyInfo.GetValue(obj) : fieldInfo.GetValue(obj)); }
			set	{ if (propertyInfo != null) propertyInfo.SetValue(obj, value); else fieldInfo.SetValue(obj, value); }
		}

		public Member(object obj, string memberName)
		{
			if (obj == null) throw new ArgumentNullException("obj");
			this.obj = obj;
			Initialize(memberName);
		}

		public Member(Expression<Func<T>> instanceMemberExpression)
		{
			var body = instanceMemberExpression.Body as MemberExpression;
			if (body != null)
			{
				var t1 = body.Expression.GetType();
			}

		}
	}

	public class ElementBinding  
		// *** HERE: looks like this will get significant part of regular binding code. 
		// So better make regular binding code accept any type of object instead of just uiview
		// Also: see if moving 2-way event handlers into binding itself eliminates findbindingforview and maybe also makes multiple bindings per vm prop easier
		// Also: if we also create a notifychanged event handler inside each data binding, we do not need to do lookup of binding.
		// but: lots of handlers... how will that scale e.g. in a list of viewmodel items? But: will make lookup unneeded.
	{
		private BindingMode mode;
		private Element element;
		private PropertyInfo elementPropertyInfo;
		private ViewModelBase viewModel;
		private PropertyInfo viewModelPropertyInfo;

		private FieldInfo fi;

		public void Bind(Element element, ViewModelBase viewModel, string viewModelPropertyName, BindingMode mode = BindingMode.OneWay)
		{
			this.element = element;
			this.viewModel = viewModel;
			this.mode = mode;

			viewModelPropertyInfo = viewModel.GetType().GetProperty(viewModelPropertyName);

			if (element == null || mode != BindingMode.TwoWay) return;
			switch (element.GetType().Name)
			{
				case "EntryElement":

					((EntryElement)element).Changed += ElementBinding_Changed; break;
				case "BooleanElement":
					((BooleanElement)element).ValueChanged += ElementBinding_Changed; break;
			}
		}

		public void Unbind()
		{
			if (element == null || mode != BindingMode.TwoWay) return;
			switch (element.GetType().Name)
			{
				case "EntryElement": ((EntryElement)element).Changed -= ElementBinding_Changed; break;
				case "BooleanElement": ((BooleanElement)element).ValueChanged -= ElementBinding_Changed; break;
			}
		}

		void ElementBinding_Changed(object sender, EventArgs e)
		{
			if (element == null || viewModel == null || viewModelPropertyInfo == null  || mode != BindingMode.TwoWay) return;
			switch (element.GetType().Name)
			{
				case "EntryElement": ((EntryElement)element).Changed += ElementBinding_Changed; break;
				case "BooleanElement": ((BooleanElement)element).ValueChanged += ElementBinding_Changed; break;
			}
		}
	} */
	
	public partial class OrderView : DialogViewBase
	{
		private OrderViewModel ViewModel { get { return CloudAuctionApplication.Instance.OrderViewModel; } }

        public OrderView(IntPtr handle) : base(handle)
        {
            Pushing = true;
			//var expense = new Expense ();
			//var bctx = new BindingContext (null, expense, "Create a task");
			//Root = bctx.Root;

			Root = new RootElement("Order") { 
                new Section() {
                    new EntryElement("Email", "Enter Email", ViewModel.Email), //.Bind(OrderViewModel.PROPERTYNAME_Email),
					new BooleanElement("Check", false) //.Bind(OrderViewModel.PROPERTYNAME_DeliveryLocationListHasItems)
                }
            };

			var p = new BindingParameters[] {
				new BindingParameters { Property = () => ViewModel.Email, Mode = BindingMode.TwoWay },
				new BindingParameters { ViewModelPropertyName = OrderViewModel.PROPERTYNAME_Email, Mode = BindingMode.TwoWay }
			};

            // TODO: figure out how to do data binding with monotouch.dialog -> choose the elements api, not the reflection api
            // 0) How to find the element given a propertyname?
            // 1) How to specify binding parameters for an element?
            // 2) How to capture changes in element value? 

            // Observations:
            // - Elements have different value property names and types, different event names and delegate signatures
            // - Not safe to bind to UIView that corresponds to an element, because that will conflict with the Mt.D built in binding mechanism
            //   -> we need to bind to elements or to the annotated models.
            // - bind to elements: 
            //   - automatically: base type change from UIView to NSObject, support Element as well as UIView, lots of change
            //     and also hard dependency on dialog (reference)
            //   - manually: is effectively not having binding support at all.
            // - bind to annotated model:
            //   - 

//            Section s;
//            Element e;
//            EntryElement ee;
//            DialogViewController dvc;
//            BooleanElement be;
//            BooleanImageElement bie;
//            CheckboxElement ce;
//            DateElement de;
//            DateTimeElement dte;
//            FloatElement fe;
//            ImageElement ie;
//            RadioElement re;
//            RootElement rte;
//            StringElement se;
//            StyledStringElement sse;
//            TimeElement te;
//            UIViewElement ue;

            //ee.Changed
            //be.ValueChanged
            //bie.Tapped
            //bie.ValueChanged
            //ce.Tapped
            //de.DateSelected

        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			InitializeBindings(View, ViewModel);

            var cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Done, (s, e) => ViewModel.CancelCommand.Execute(null));
			cancelButton.Enabled = ViewModel.CancelCommand.IsEnabled;
			NavigationItem.SetLeftBarButtonItem(cancelButton, true);

            var confirmButton = new UIBarButtonItem("Confirm", UIBarButtonItemStyle.Done, (s, e) => ViewModel.ConfirmCommand.Execute(null));
            confirmButton.Enabled = ViewModel.ConfirmCommand.IsEnabled;
            NavigationItem.SetRightBarButtonItem(confirmButton, true);
		}

		protected override void AddHandlers()
		{
			base.AddHandlers();
			ViewModel.CancelCommand.CanExecuteChanged += HandleCancelCommandCanExecuteChanged; // This event is external to the view, so we need to use a named delegate so we can remove it later, to prevent memory leaks.
			ViewModel.ConfirmCommand.CanExecuteChanged += HandleConfirmCommandCanExecuteChanged; // This event is external to the view, so we need to use a named delegate so we can remove it later, to prevent memory leaks.
		}

		protected override void RemoveHandlers()
		{
			ViewModel.ConfirmCommand.CanExecuteChanged -= HandleConfirmCommandCanExecuteChanged;
			ViewModel.CancelCommand.CanExecuteChanged -= HandleCancelCommandCanExecuteChanged;
			base.RemoveHandlers();
		}

		void HandleCancelCommandCanExecuteChanged (object sender, EventArgs e)
		{
			NavigationItem.LeftBarButtonItem.Enabled = ViewModel.CancelCommand.IsEnabled;
		}

		void HandleConfirmCommandCanExecuteChanged (object sender, EventArgs e)
		{
			NavigationItem.RightBarButtonItem.Enabled = ViewModel.ConfirmCommand.IsEnabled;
		}
	}
}
