The MultiImageView is an ImageView that has the ability to load multiple images 
(or URLs, which will then be downloaded and cached) and lets the user swipe to 
the left and right to switch images

```csharp
using Macaw.UIComponents;
...

protected override void OnCreate(Bundle bundle)
{
	imageView = FindViewById<MultiImageView>(Resource.Id.multiImageView);
	
    imageView.LoadImageList(new [] { 
		"http://blog.xamarin.com/wp-content/uploads/2013/01/evolve-badge.png",
		"http://oi50.tinypic.com/dfzo0k.jpg",
		"http://oi49.tinypic.com/kd6fcp.jpg"});
	
    imageView.ImagesLoaded += (sender, e) =>
    {   // Loads the first image in the list
        RunOnUiThread(imageView.LoadImage);
    }; 
}
```
