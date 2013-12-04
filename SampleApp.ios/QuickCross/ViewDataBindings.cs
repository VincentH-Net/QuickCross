using System;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoMac;
using MonoTouch.Foundation;
using System.Runtime.InteropServices;

namespace QuickCross
{
    public class ViewDataBindings
    {
		#region Add support for user defined runtime attribute named "Bind" (default, type string) on UIView

		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		private static extern void void_objc_msgSendSuper_intptr_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		private static void SetValueForUndefinedKey(IntPtr selfPtr, IntPtr cmdPtr, IntPtr valuePtr, IntPtr undefinedKeyPtr)
		{
			UIView self = (UIView) Runtime.GetNSObject(selfPtr); // this is	basically your "this"
			var value = Runtime.GetNSObject(valuePtr);
			var key = (NSString) Runtime.GetNSObject(undefinedKeyPtr);
			if (key == BindKey) {
				AddBinding(self, value.ToString());
			} else {
				Console.WriteLine("Value for unknown key: {0} = {1}", key.ToString(), value.ToString() );
				// Call original implementation on super class of UIView:
				void_objc_msgSendSuper_intptr_intptr(UIViewSuperClass, SetValueForUndefinedKeySelector, valuePtr, undefinedKeyPtr);
			}
		}

		private static string BindKey;
		private static Action<IntPtr, IntPtr, IntPtr, IntPtr> SetValueForUndefinedKeyDelegate = SetValueForUndefinedKey;
		private static IntPtr UIViewSuperClass, SetValueForUndefinedKeySelector;

		public static void RegisterBindKey(string key = "Bind")
		{
			BindKey = key;
			Console.WriteLine("Replacing implementation of SetValueForUndefinedKey on UIView...");
			var uiViewClass = Class.GetHandle("UIView");
			UIViewSuperClass = ObjcMagic.GetSuperClass(uiViewClass);
			SetValueForUndefinedKeySelector = Selector.GetHandle("setValue:forUndefinedKey:");
			ObjcMagic.AddMethod(uiViewClass, SetValueForUndefinedKeySelector, SetValueForUndefinedKeyDelegate, "v@:@@");
		}
		#endregion Add support for user defined runtime attribute named "Bind" (default, type string) on UIView

		private static void AddBinding(UIView view, string bindingParameters)
		{
			Console.WriteLine("Binding parameters: {0}", bindingParameters);
			// First store all binding properties and UIView objects? or just the id? or the Ptr?
			// How do we group the UIViews? On controller? On root views?
			// How do we know the grouping object?
			// How do we cleanup? Associated objects?

			// Below is just an experiment to get to a root view - what we really want is 
			// to find the viewbindings object for the toplevel view
			// Look at options: find controller from vie
			var topView = view;
			while (topView.Superview != null && topView.Superview != topView) {
				topView = topView.Superview;
				Console.Write(".");
			}
			Console.WriteLine("Topview = {0}", topView.ToString());

			// Do we use base classes for controllers?
		}

		public ViewDataBindings()
		{
		}
    }
}
