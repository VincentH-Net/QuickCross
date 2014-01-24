The MultiImageView is an ImageView that has the ability to load multiple images 
(or URLs, which will then be downloaded and cached) and lets the user swipe to 
the left and right to switch images.

Optionally an image can be displayed in the top left, if this is pressed a
ZoomImageEvent will fire.

```csharp
using Macaw.UIComponents;
...

protected override void OnCreate(Bundle bundle)
{
	imageView = FindViewById<MultiImageView>(Resource.Id.multiImageView);
	
	// Sets images for the slider icons and their size
    imageView.SliderSelectedIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.slider_blt_grn);
    imageView.SliderUnselectedIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.slider_blt_trans);
    imageView.SetSliderIconDimensions(30, 30);

	// Sets an image for the Magnify button (and its size) in the top left and enables the ZoomImage event 
    imageView.MagnifyIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Magnify);
    imageView.SetMagnifyIconDimensions(70, 70);
    imageView.MagnifyEnabled = true;

    // I want to show pictures in the imageview that are online, giving the MultiImageView a list of URLs to download at SampleSize 4 (4x scaled down) 
    imageView.DownloadedImageSampleSize = 4;
    imageView.LoadImageList(new [] { 
		"http://blog.xamarin.com/wp-content/uploads/2013/01/evolve-badge.png",
        "http://oi50.tinypic.com/dfzo0k.jpg",
        "http://oi49.tinypic.com/kd6fcp.jpg" });

    // Adding eventhandlers for when an image is loaded so I can update the imageview to show its images, and an eventhandler for when the Magnify button is pressed
    imageView.ImagesLoaded += (sender, e) =>
    {   // Loads the first image in the list
        RunOnUiThread(imageView.LoadImage);
    };
    imageView.ZoomImageEvent += (sender, e) => {
        // Fire whatever code you want to happen on a Magnify click
    };
}
```

