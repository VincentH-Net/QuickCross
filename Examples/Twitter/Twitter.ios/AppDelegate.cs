using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Twitter
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
		public override UIWindow Window { get; set; }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			var navigator = new TwitterNavigator(InitializeNavigationContext());
            EnsureTwitterApplication(navigator).ContinueToMain();

			Window.AddGestureRecognizer(new KeyboardDismissGestureRecognizer());
			Window.MakeKeyAndVisible();

            return true;
        }

		class KeyboardDismissGestureRecognizer : UITapGestureRecognizer
		{
			public KeyboardDismissGestureRecognizer() : base(() => { }) { CancelsTouchesInView = false; }

			public override void TouchesBegan(NSSet touches, UIEvent evt)
			{
				base.TouchesBegan(touches, evt);
				var touch = evt.AllTouches.AnyObject as UITouch;
				if (touch != null && touch.View != null && !(touch.View.CanBecomeFirstResponder) && View != null) View.EndEditing(true);
			}
		}
    }
}