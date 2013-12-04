// 
// ObjcMagic.cs
//  
// Author:
//       Michael Hutchinson <mhutch@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace MonoMac
{
	//See http://developer.apple.com/library/mac/#documentation/Cocoa/Reference/ObjCRuntimeRef/Reference/reference.html
	public static class ObjcMagic
	{
		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr method_getImplementation (IntPtr method);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool class_addMethod (IntPtr cls, IntPtr sel, Delegate imp, string argTypes);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool class_addMethod (IntPtr cls, IntPtr sel, IntPtr imp, string argTypes);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_getMetaClass (string name);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getSuperclass (IntPtr cls);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool class_isMetaClass (IntPtr cls);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_getClass (string name);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getInstanceMethod (IntPtr cls, IntPtr sel);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getClassMethod (IntPtr cls, IntPtr sel);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_copyMethodList (IntPtr cls, out int count);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool method_exchangeImplementations (IntPtr m1, IntPtr m2);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_replaceMethod (IntPtr cls, IntPtr sel, IntPtr imp, string types);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern string method_getTypeEncoding (IntPtr method);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr method_getName (IntPtr method);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool sel_isEqual (IntPtr selLhs, IntPtr selRhs);

		[DllImport ("libc")]
		static extern void free (IntPtr ptr);

		struct objc_class {
			public IntPtr Isa;
		}

		public static void AddMethod (IntPtr cls, IntPtr sel, Delegate imp, string argTypes)
		{
			if (!class_addMethod (cls, sel, imp, argTypes))
				throw new Exception ("Failed to add method");
		}

		public static IntPtr GetClass (string name)
		{
			return objc_getClass (name);
		}

		public static IntPtr GetMetaClass (string name)
		{
			return objc_getMetaClass (name);
		}

		public static IntPtr GetClass (IntPtr handle)
		{
			unsafe { return ((objc_class *)handle)->Isa; }
		}

		public static void MethodExchange (IntPtr meth1, IntPtr meth2)
		{
			if (!method_exchangeImplementations (meth1, meth2))
				throw new Exception ("Failed to exchange implementations");
		}

		public static IntPtr GetClassMethod (IntPtr cls, IntPtr sel)
		{
			IntPtr m = class_getClassMethod (cls, sel);
			if (m == IntPtr.Zero)
				throw new Exception ("Class did not have a method for the selector");
			return m;
		}

		public static IntPtr GetInstanceMethod (IntPtr cls, IntPtr sel)
		{
			IntPtr m = class_getInstanceMethod (cls, sel);
			if (m == IntPtr.Zero)
				throw new Exception ("Instance did not have a method for the selector");
			return m;
		}

		public static IntPtr GetSuperClass (IntPtr cls)
		{
			return class_getSuperclass (cls);
		}

		unsafe static IntPtr GetMethodNonInherited (IntPtr cls, IntPtr sel)
		{
			int len;
			IntPtr list = class_copyMethodList (cls, out len);
			IntPtr *ptr = (IntPtr *)list;
			IntPtr *end = ptr + len;
			IntPtr found = IntPtr.Zero;
			while (ptr < end) {
				IntPtr meth = *ptr;
				if (sel_isEqual (method_getName (meth), sel)) {
					found = meth;
					break;
				}
				ptr++;
			}
			free (list);
			return found;
		}

		// See http://www.cocoadev.com/index.pl?MethodSwizzling
		// This version explicitly doesn't allow swizzling methods defined on superclasses.
		// In such cases, the method should simply be added as selOrig, and to chain to the
		// original it should call [super selOrig] instead of [self selNew].
		// Pulling the superclass implementation down to self is broken since chaining
		// could cause it to be called multiple times.
		public static void Swizzle (IntPtr cls, IntPtr selOrig, IntPtr selNew)
		{
			IntPtr methOrig = GetMethodNonInherited (cls, selOrig);
			if (methOrig == IntPtr.Zero)
				throw new Exception ("Class does not have method for original selector. Add a method instead.");
			IntPtr methNew  = GetMethodNonInherited (cls, selNew);
			if (methNew == IntPtr.Zero)
				throw new Exception ("Class does not have method for new selector.");
			method_exchangeImplementations (methOrig, methNew);
		}
	}
}