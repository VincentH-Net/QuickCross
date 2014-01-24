using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Macaw.UIComponents;
using Android.Graphics;

namespace TestingMultiView
{
    [Activity(Label = "TestingMultiView", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        MultiImageView imageView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            imageView = FindViewById<MultiImageView>(Resource.Id.imageView1);
            
            // Sets images for the slider icons and their size
            imageView.SliderSelectedIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.slider_blt_grn);
            imageView.SliderUnselectedIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.slider_blt_trans);
            imageView.SetSliderIconDimensions(30, 30);

            // Sets an image for the Magnify button (and its size) in the top left and enables the ZoomImage event 
            imageView.MagnifyIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Magnify);
            imageView.SetMagnifyIconDimensions(70, 70);
            imageView.MagnifyEnabled = true;

            // I want to show pictures in the imageview that are online, giving the MultiImageView a list of URLs to download at SampleSize 2 (2x scaled down) 
            imageView.DownloadedImageSampleSize = 2;
            imageView.LoadImageList(new [] { 
                "http://blog.xamarin.com/wp-content/uploads/2013/01/evolve-badge.png",
                "http://oi50.tinypic.com/dfzo0k.jpg",
                "http://oi49.tinypic.com/kd6fcp.jpg"
            });

            // Adding eventhandlers for when an image is loaded so I can update the imageview to show its images, and an eventhandler for when the Magnify button is pressed
            imageView.ImagesLoaded += (sender, e) =>
            {   // Loads the first image in the list
                RunOnUiThread(imageView.LoadImage);
            };
            imageView.ZoomImageEvent += (sender, e) => {
                // Fire whatever code you want to happen on a Magnify click
            };
        }
    }
}

