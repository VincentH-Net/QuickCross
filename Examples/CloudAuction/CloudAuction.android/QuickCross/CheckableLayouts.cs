using System;

using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.Util;
using Android;

namespace QuickCross
{
    // Based on https://github.com/xamarin/monodroid-samples/blob/master/CustomChoiceList/CheckableLinearLayout.cs
    // For usage tip on propogating child state changes, see: http://chris.banes.me/2013/03/22/checkable-views/

    public class CheckableLinearLayout : LinearLayout, ICheckable
    {
        static readonly int[] CHECKED_STATE_SET = { Resource.Attribute.StateChecked };
        private bool _checked = false;

        public CheckableLinearLayout(Context context) : base(context) { }
        public CheckableLinearLayout(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public CheckableLinearLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public CheckableLinearLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        protected override int[] OnCreateDrawableState(int extraSpace)
        {
            int[] drawableState = base.OnCreateDrawableState(extraSpace + 1);
            if (Checked) MergeDrawableStates(drawableState, CHECKED_STATE_SET);
            return drawableState;
        }

        public bool Checked
        {
            get { return _checked; }
            set { if (_checked != value) { _checked = value; RefreshDrawableState(); } }
        }

        public void Toggle() { Checked = !Checked; }
    }

    public class CheckableRelativeLayout : RelativeLayout, ICheckable
    {
        static readonly int[] CHECKED_STATE_SET = { Resource.Attribute.StateChecked };
        private bool _checked = false;

        public CheckableRelativeLayout(Context context) : base(context) { }
        public CheckableRelativeLayout(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public CheckableRelativeLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public CheckableRelativeLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        protected override int[] OnCreateDrawableState(int extraSpace)
        {
            int[] drawableState = base.OnCreateDrawableState(extraSpace + 1);
            if (Checked) MergeDrawableStates(drawableState, CHECKED_STATE_SET);
            return drawableState;
        }

        public bool Checked
        {
            get { return _checked; }
            set { if (_checked != value) { _checked = value; RefreshDrawableState(); } }
        }

        public void Toggle() { Checked = !Checked; }
    }
}