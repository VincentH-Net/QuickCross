using System;
using System.Reflection;
using Android.Widget;
using Android.App;

namespace QuickCross
{
    public static class AndroidHelpers
    {
        public enum ResourceCategory { Drawable, Layout, Menu, Values, Id };

        private static Type resourceClassType;
        private static PropertyInfo adapterViewRawAdapterPropertyInfo;

        public static Activity CurrentActivity { get; private set; }

        public static void SetCurrentActivity(Activity activity)
        {
            if (activity == null) throw new ArgumentNullException("activity");
            CurrentActivity = activity;
        }

        public static void ClearActivityReference(Activity activity)
        {
            if (Object.ReferenceEquals(CurrentActivity, activity)) CurrentActivity = null;
        }

        public static void Initialize(Type resourceClassType) { AndroidHelpers.resourceClassType = resourceClassType; }

        public static int? FindResourceId(string name, ResourceCategory category = ResourceCategory.Id)
        {
            if (string.IsNullOrEmpty(name) || resourceClassType == null) return null;
            var categoryClassType = resourceClassType.GetNestedType(category.ToString()); // MQC TODO: Check if optimize perf by caching type for each category is needed?
            if (categoryClassType == null) return null;
            var fieldInfo = categoryClassType.GetField(name);
            if (fieldInfo == null) return null;
            return (int)fieldInfo.GetValue(null);
        }

        public static object GetAdapter(this AdapterView adapterView)
        {
            if (adapterView == null) return null;
            if (adapterViewRawAdapterPropertyInfo == null) adapterViewRawAdapterPropertyInfo = typeof(AdapterView).GetProperty("RawAdapter", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            return adapterViewRawAdapterPropertyInfo.GetValue(adapterView);
        }


    }

    public class Wrapper<T> : Java.Lang.Object
    {
        public T DotNetObject;

        public static implicit operator T(Wrapper<T> wrapper) { return wrapper == null ? default(T) : wrapper.DotNetObject; }
        public static implicit operator Wrapper<T>(T dotNetObject) { return new Wrapper<T> { DotNetObject = dotNetObject }; }
    }
}