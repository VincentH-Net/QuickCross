**QuickCross** is a lightweight (no binaries) cross-platform MVVM pattern to quickly build native Xamarin.iOS, Xamarin.Android, Windows Phone and Windows Store Apps with shared C# code.QuickCross provides data binding for Android and for iOS. It accelerates development with scaffolders and code snippets, also for a single platform app. For cross-platform apps QuickCross increases code sharing with an Application-Navigator pattern.QuickCross aims to leave you in full control; it does not get in the way if you want to do some things differently, and you can simply extend or modify it.
> **NOTE:** This version (2.0) of QuickCross is in **beta** now; iOS data bindings are working and documented, including how to creating an app from scratch. However **this release only supports the iOS platform**; the other platforms and more elaborate iOS example documentation will be re-added before the 2.0 final release.> The purpose of this beta release is to gain feedback on the iOS data bindings implementation; if you are building a production application or if you are targeting other platforms than iOS, it is recommended to keep using the previous version until the QuickCross release version is published.Versions before 2.0 were published under the name **MvvmQuickCross**. They will continue to be supported at the [existing MvvmQuickCross GitHub repository](https://github.com/MacawNL/MvvmQuickCross), and [the existing MvvmQuickCross NuGet packages](http://nuget.org/packages/mvvmquickcross) will remain available.Upgrading from MvvmQuickCross to QuickCross is quick (pun intended); instructions will be documented here (TODO URL).## News ##**Jan 3, 2014**: QuickCross release: 2.0 beta is out. Adds simple iOS data binding and an iOS example app.**Coming up**: Documentation. Next planned QuickCross release: 2.0 final, which will add more iOS data bindings, the other platforms (besides iOS), and full documentation and examples. ETA Jan 15.## NuGet package - PRERELEASE ##[QuickCross NuGet packages](http://nuget.org/packages/quickcross)## Preliminary Documentation ##

Most of [the existing documentation for MvvmQuickCross](https://github.com/MacawNL/MvvmQuickCross) applies for QuickCross 2.0 as well; the MvvmQuickCross documentation will be copied here and updated soon. Until then, these are the key changes in QuickCross 2.0:

- Name change from MVVMQuickCross to QuickCross

- Minor change in shared code: `navigationContext` parameter was moved from the `Application` to the `Navigator` for more flexibility when using multiple navigation contexts. For existing code this means you need to pass the navigationContext parameter to the Navigator constructor instead of the Application constructor. The Navigator now manages navigation context instances and decides when to use/create which context (if more than one is needed).

- [iOS data binding](#ios-data-binding), with support for **coded views, Xib views and StoryBoard views**:

	- See the [QCTest1 app source](https://github.com/MacawNL/QuickCross/tree/master/Examples/QCTest1) in this repository for an example of a coded view, a Xib view, and a StoryBoard view with a UITableView, including navigation.

	- The `New-View` command in the VS Package Manager Console now has support for these view types in iOS: `Code`, `Xib`, `StoryBoard` and `StoryBoardTable`. When you add these views, the generated view class contains inline comments on how to complete the view in xCode, if needed. More view types will be added in the final 2.0 release.

Please feel free to run and inspect the two iOS example apps and provide feedback on what you think of the approach for iOS data bindings. Thanks and NJoy!

## Getting Started ##
To create an app with QuickCross, follow these steps:

1. In Visual Studio, create a new solution with an application project for the **platform** (Windows Store, Windows Phone, Android, iOS) that you are most productive with. Add a class library project **for the same platform** to the solution. Reference the class library from the application project.

	**Note for Android:** Set the **API level** to 12 (Android 3.1) or higher in the Project properties for both projects. This is needed to support the Fragment view type. You can target lower API versions by either using the [Android Support Library](http://developer.android.com/tools/support-library/index.html) (which is supported in Xamarin) or by removing the Fragment view base class and template from the QuickCross folder in your application project.

	**Notes for iOS:**
	1) Select iOS 5.0 or higher (set the **Deployment Target** to 5.0 or higher in the **iOS Application** tab of the Project Properties window for the application project). This is needed to specify data binding parameters in XCode, which relies on a User-Defined Runtime Attribute named Bind.
	
	2) Check the **Allow unsafe code** option (in the **Build** tab of the Project Properties window) for the application project. This is needed for the QuickCross data binding code, which uses the Objective C runtime library to read the Bind User-Defined Runtime Attribute.

	3) Verify that in the Configuration Manager (menu Build) the Active solution platform is set to the emulator or device that you want to target (e.g. iPhoneSimulator).

	**Note for Windows Phone:** Select Windows Phone OS 8.0 or higher.

2. Install the [QuickCross NuGet package](http://nuget.org/packages/quickcross)

	The available QuickCross commands are now displayed in the package manager console.
	Type "**Get-Help *command* -Online**" for details.

3. In the Visual Studio package manager console (*menu View | Other Windows*) enter:

	**[Install-Mvvm](#install-mvvm)**

	An QuickCross folder is now added to your library project and your application project, and a few application-specific projects items are generated and opened in Visual Studio.

	**Note** that the package installation uses the first part of the solution filename (before the first dot) as the **application name** for naming new project items and classes.

4. Import the C# code snippets from the QuickCross\Templates\QuickCross.snippet file in your class library project into Visual Studio with the Code Snippets Manager (see [how](http://msdn.microsoft.com/en-us/library/ms165394\(v=vs.110\).aspx)). If you get a "Snippet With Same Name Exists" dialog, select Overwrite. 
	
	**Note** do not select the QuickCross\Templates folder itself as the location to import snippets **to**; that may prevent the snippets to be imported correctly, as this would mean copying the snippets file over itself.

5. Add new views and viewmodels with the [`New-View`](#new-view) and [`New-ViewModel`](#new-viewmodel) commands.

6. Add data-bindable properties and commands to your viewmodels with the [code snippets](#code-snippets).

7. Check the TODO comments in the Visual Studio Task List *(menu View | Tast List)* to find guidance on how to complete the viewmodel, application and navigator classes. You can also check out the CloudAuction example app in this GitHub repository (TODO: create iOS implementation).

## Adding platforms ##
To code your app for more platforms:


1. Create a new solution for each platform, with a class library project for that platform and an application project for that platform, just like you did for the first platform.

2. Add all code files from the existing class library project to the class library project for the new platform.

3. Install the QuickCross NuGet package and execute the Install-Mvvm command again (it won't overwrite existing files).

4. Code the views, navigator and any platform specific service implementations in the application project.

## Commands ##
After installing the QuickCross NuGet package, the below commands are available in the Visual Studio **Package Manager Console**.

**Note** that except for Install-Mvvm, anything that these commands do can also be done by hand; the manual steps are documented inline in the files that you add to your projects with Install-Mvvm. This makes it possible to create your initial solutions in Visual Studio, and then continue working in [Xamarin Studio](http://xamarin.com/studio) for Android or iOS, if you prefer that.

### Install-Mvvm ###

```posh
Install-Mvvm 
```
Installs the QuickCross support files in both your library project and your application project, in a subfolder QuickCross. The files in the QuickCross folders are not application specific; unless you want to modify the standard QuickCross templates, code snippets and/or functionality you don't need to edit these.

Install-Mvvm also generates a few application-specific project items for you. The generated project items are opened in the Visual Studio editor for your inspection.

**Note** that Install-Mvvm uses the first part of the solution filename (before the first dot) as the **application name** for naming generated project items, classes, properties and methods.

Check the **TODO comments** in the Visual Studio **Task List** to find guidance on how to complete the generated project items.

Install-Mvvm will not overwrite existing files or code. If you want to recreate the default files, remove the files that you want to recreate before running Install-Mvvm.

### New-View ###

```posh
New-View [-ViewName] <string> [[-ViewType] <string>] [[-ViewModelName] <string>] [-WithoutNavigation]
```
Generates a new view.

The specified `ViewName` will be suffixed with "View", and the specified `ViewModelName` will be suffixed with "ViewModel". If no ViewModelName is specified, it will be the same as the ViewName. If the viewmodel does not exist, it will be generated with the `New-ViewModel` command.

- On Windows Phone or Windows Store, the `ViewType` can be `Page` (default) or `UserControl`. 
- On Android, it can be `MainLauncher`, `Activity` (default) or `Fragment`.
- On iOS, it can be `Code` (default), `Xib`, `StoryBoard` or `StoryBoardTable`.

The specified view type determines which view templates are used. You can find these templates in the QuickCross\Templates folder of your application project. You can simply modify these templates or add your own (which is better) by adding similar named files there.

Unless the `-WithoutNavigation` switch is specified, New-View will also add basic navigation code to the navigator and application classes. The -WithoutNavigation switch is useful when creating views such as list item views, that do not need to navigated to directly from the application class.

E.g. this command:

```posh
New-View Person
```
will generate:

- A `PersonView` view markup file + class
- A `PersonViewModel` viewmodel class
- A `PersonViewModelDesign` viewmodel class
- A `PersonViewModel` property in the application class
- A `NavigateToPersonView()` method signature in the navigator interface
- A `NavigateToPersonView()` method implementation in the navigator class
- A `ContinueToPerson()` method in the application class

Now the only thing needed to display the view, bound to the viewmodel, is to call the `ContinueToPerson()` method on the application.

Check the **TODO comments** in the Visual Studio **Task List** to find guidance on how to complete the generated project items.

New-View will not overwrite existing files or code. If you want to recreate files or code fragments, remove the existing one(s) first.
  
### New-ViewModel ###

```posh
New-ViewModel [-ViewModelName] <string> [-NotInApplication]
```
Generates a new viewmodel. You can use this command to create viewmodels without creating any corresponding views (yet).

The specified `ViewModelName` will be suffixed with "ViewModel".

Unless the `-NotInApplication` switch is specified, New-ViewModel will also add a property to contain the instance of the viewmodel to the application class. The application class will then be responsible for providing an initialized viewmodel instance before navigating to the corresponding view. The -NotInApplication switch is useful when creating viewmodels such as list item viewmodels, that do not need to be instantiated and initialized directly by the application class.

E.g. this command:

```posh
New-ViewModel Person
```
will generate:

- A PersonViewModel viewmodel class
- A PersonViewModelDesign viewmodel class
- A PersonViewModel property in the application class

Check the **TODO comments** in the Visual Studio **Task List** to find guidance on how to complete the generated project items.

New-ViewModel will not overwrite existing files or code. If you want to recreate files or code fragments, remove the existing one(s) first.

### Customizing and Extending Command Templates ###
As Described above in the usage of the ViewType parameter of the [New-View command](#new-view), you can add your own code and markup templates for custom view types by adding properly named files in the `QuickCross\Templates` folder of your application project.

The code and markup template files are named:   `_VIEWNAME_<view type name>View.*`
If no markup file for the specified view type is found, the default markup file `_VIEWNAME_View.*` is used, so you do not need to have multiple copies of the same markup template.

Of course you can also modify existing templates in the `QuickCross\Templates` folder.

The library project also contains a viewmodel template that can be customized, at `QuickCross\Templates\_VIEWNAME_ViewModel.cs`.

In addition to the template files, you can also modify the inline code templates in the Application, INavigator and Navigator. Inline templates are used to add properties and methods in these files when a new viewmodel or view is added. An example of an inline template in the Application class for a `view` is:

```csharp
public sealed class _APPNAME_Application : ApplicationBase
{
	...

    /* TODO: For each view, add a method (with any parameters needed) to initialize its viewmodel
     * and then navigate to the view using the navigator, like this:

    public void ContinueTo_VIEWNAME_()
    {
        if (_VIEWNAME_ViewModel == null) _VIEWNAME_ViewModel = new _VIEWNAME_ViewModelDesign();
        RunOnUIThread(() => _navigator.NavigateTo_VIEWNAME_View());
    }
     * Note that the New-View command adds the above code automatically 
     * (see http://github.com/MacawNL/QuickCross#new-view). */
}
```
The format of an inline template is:

1. An empty line. The instantiated code will be placed directly before this line
2. A line starting with `/* TODO: For each <template name>,`
3. Optionally some lines starting with ` * `
4. Lines of actual template code
5. Optionally some lines starting with ` * `
6. A line ending with  ` */`

Currently the template name can be `view` or `viewmodel`.

## Code Snippets ##
When you run the Install-Mvvm command, the C# code snippets file `QuickCross\Templates\QuickCross.snippet` is added to your class library project. When you import this snippets file into Visual Studio with the Code Snippets Manager (see [how](http://msdn.microsoft.com/en-us/library/ms165394\(v=vs.110\).aspx)), the code snippets described below become available for coding viewmodels.

Note that the code snippets and their parameters have intellisense when you invoke them in the Visual Studio C# editor.

To instantiate a code snippet, place your cursor on an empty line in the `#region Data-bindable properties and commands` of a viewmodel class .cs file, type the code snippet shortcut (e.g. `propdb1`), and press Tab twice. Now you can enter the parameters of the code snippet (press Tab to cycle through all parameters). Press Enter to complete the snippet instance.

### propdb1 ###
Adds a one-way data-bindable property to a Viewmodel. You can specify the property type and name.
### propdbcol ###
Adds a one-way data-bindable collection property, a corresponding `(name)HasItems` property and an `Update(name)HasItems()` method to a Viewmodel. You can specify the generic collection type (e.g. `ObservableCollection` or `List`), the collection element type and the property name.

If you want specific UI elements in your view to only be visible when the collection has elements (e.g. a list of error messages), you can use the HasItems property to bind to the visibility of those UI elements. In that case, you should also call the UpdateHasItems() method after you have added or removed items (this is necessary even if this is an ObservableCollection).

**Note** that if you suffix a collection property name with **"List"**, you can benefit from the [list data binding naming convention in Android](#list-itemssource).

### propdb2 ###
Adds a two-way data-bindable property to a Viewmodel. You can specify the property type and name.
### propdb2c ###
Adds a two-way data-bindable property and a `On(name)Changed()` method for custom setter code to a Viewmodel. You can specify the property type and name.
### cmd ###
Adds a data-bindable command to a Viewmodel. You can specify the command name, which will be suffixed with **"Command"**.
### cmdp ###
Adds a data-bindable command with a parameter to a Viewmodel. You can specify the command name, which will be suffixed with **"Command"**, the parameter type and the parameter name.

## iOS ##
Below is an overview of using QuickCross with Xamarin.iOS.

### Create an iOS App ###
Here is how to create an iOS Twitter app that demonstrates simple data binding:
> Note that the complete source for this Twitter app example is available in this repository, [here](http://github.com/MacawNL/QuickCross/tree/master/Examples/Twitter).

This is what the Twitter app will look like in iOS:

![Twitter example on iPhone](https://raw.github.com/MacawNL/QuickCross/master/assets/twitter_ios.png)

So let's get started.

1.  Create a working iOS app by following steps 1 though 4 of [Getting Started](#getting-started) above; make sure you follow the **iOS notes** there!

	These are the project templates and project names you should use while following these steps: an `Empty Project` project from the **iOS\iPhone** project templates named `Twitter`, and a `Xamarin.iOS Library Project` project from the **iOS** project templates named `Twitter.Shared`.

	In the **iOS Application** tab of the application project properties window, set the **Application name**, **Identifier** and **Version** to some meaningful value. In the **Application** tab of the library project properties window, add the suffix `.ios` to the **Assembly name** - this is useful if you want to build for multiple platforms in the same project folder.

2.  Remove the `Class1.cs` file that was included by the Xamarin.iOS Library Project template

3.  Do what the **TODO comments** in the Visual Studio **Task List** say for the file **AppDelegate.cs**. This will ensure that on application startup QuickCross is initialized and the initial view is displayed.

4.  Now you can run the app on your device and test the example MainView that was generated by the Install-Mvvm command.

5.  Create a `Models` folder in your Twitter.Shared project and create this `Tweet.cs` class in it:

	```csharp
	using System;
	namespace Twitter.Shared.Models
	{
	    public class Tweet
	    {
	        public string Text { get; set; }
	        public string UserName { get; set; }
	        public DateTime CreatedAt { get; set; }
	    }
	}
	```

6.  In `ViewModels\MainViewModel.cs` in your library project, in the `#region Data-bindable properties and commands`, remove the example property and command add these properties and commands with the indicated [code snippets](#code-snippets):
	
    <table>
        <tr>
            <td><b>Snippet</b></td><td><b>Parameters</b></td><td><b>Generated code</b></td>
        </tr>
        <tr>
            <td><a href="#propdb2c">propdb2c</a></td><td>Tweet Tweet</td><td>
				public Tweet Tweet { ... }<br />
				private void OnTweetChanged() { ... }
            </td>
        </tr>
        <tr>
            <td><a href="#propdbcol">propdbcol</a></td><td>ObservableCollection Tweet TweetList</td><td>
				public ObservableCollection<Tweet> TweetList  { ... }<br />
				public bool TweetListHasItems { ... }<br />
				protected void UpdateTweetListHasItems() { ... }
            </td>
        </tr>
        <tr>
            <td><a href="#cmd">cmd</a></td><td>Delete</td><td>
				public RelayCommand DeleteCommand { ... }<br />
				private void Delete() { ... }
            </td>
        </tr>
        <tr>
            <td><a href="#propdb2c">propdb2c</a></td><td>string Text</td><td>
				public string Text { ... }<br />
				private void OnTextChanged() { ... }
            </td>
        </tr>
        <tr>
            <td><a href="#propdb1">propdb1</a></td><td>int CharactersLeft</td><td>
				public int CharactersLeft { ... }
            </td>
        </tr>
        <tr>
            <td><a href="#cmd">cmd</a></td><td>Send</td><td>
				public RelayCommand SendCommand { ... }<br />
				private void Send() { ... }
           </td>
        </tr>
    </table>
    	
7.  Add this code in the generated `MainModel` methods:
		
	```csharp
    public MainViewModel()
    {
        TweetList = new ObservableCollection<Tweet>();
        OnTweetChanged();
        Text = "";
    }
	
    private void OnTweetChanged()
    {
        DeleteCommand.IsEnabled = (Tweet != null);
    }

    private void Delete()
    {
        TweetList.Remove(Tweet);
        Tweet = null;
    }

    private void OnTextChanged()
    {
        CharactersLeft = 140 - Text.Length;
        SendCommand.IsEnabled = (Text.Length > 0 && CharactersLeft >= 0);
    }

    private void Send()
    {
        var newTweet = new Tweet { Text = this.Text, CreatedAt = DateTime.Now, UserName="Me" };
        TweetList.Insert(0, newTweet);
        Tweet = newTweet;
        Text = "";
    }
	```

8.  The `MainViewModel.cs` file also contains a `MainViewModelDesign` class where you can put some hardcoded viewmodel data. Put this code in the MainViewModelDesign constructor:
		
	```csharp
    public MainViewModelDesign()
    {
        Text = "Text for a new tweet";
        var now = DateTime.Now;
        TweetList.Insert(0, new Tweet { 
			Text = "Creating a simple Twitter app with QuickCross", 
			UserName = "Me", CreatedAt = now.AddSeconds(-115) });
        TweetList.Insert(0, new Tweet { 
			Text = "Created a solution with an application and a library project", 
			UserName = "Me", CreatedAt = now.AddSeconds(-63) });
        TweetList.Insert(0, new Tweet { 
			Text = "Added Tweet model class", 
			UserName = "Me", CreatedAt = now.AddSeconds(-45) });
        TweetList.Insert(0, new Tweet { 
			Text = "Added viewmodel properties and commands with code snippets", 
			UserName = "Me", CreatedAt = now.AddSeconds(-25) });
        TweetList.Insert(0, new Tweet { 
			Text = "Added some hardcoded design data for the viewmodel", 
			UserName = "Me", CreatedAt = now.AddSeconds(-1) });
    }
	```

9.  We are going to replace the **MainView**, which was generated as a **Code** view, with a **Storyboard** view. First delete the file `MainView.cs` in the application project, and then enter this command in the Package Manager Console:

	```posh
	New-View Main -ViewType StoryBoard
	```
	
	This will generate a MainView.TODO.cs file, with TODO comments on how to complete the view with Xamarin Studio and XCode. 

10. Make your source accessible from your build Mac (e.g. on a network share, via source control or just copy it with a USB stick), and open your solution on the Mac in Xamarin Studio.

11. In Xamarin Studio, set the build configuration for the application project to **Debug | iPhoneSimulator** or **Debug | iPhone**, and set the application project as the startup project.

12. In Xamarin Studio, add an **Empty iPhone Storyboard** and name it `Main` (could be any name). Open the storyboard with the **Source Code Editor** and then replace its contents by copy+pasting the contents from this [Main.storyboard file](https://github.com/MacawNL/QuickCross/blob/master/Examples/Twitter/assets/Main.storyboard). Save the storyboard and close its editor window. The storyboard now contains only the Twitter UI, without any custom classes or bindings.

	You could create this file yourself by adding an **Empty iPhone Storyboard** project item named `Main` (this could be any name) in Xamarin Studio, double-clicking to open it in XCode and then adding a view controller, with a view at the bottom containing a text field, label and two buttons. Then add a table view with a table cell and add three labels in the cell content.

13. Now follow the guidance in the **MainView.TODO.cs** comments to create the **MainView.cs** and **MainView.designer.cs** (you can skip adding your view in XCode since it is already in the downloaded storyboard).

14. Open the **Info.plist** file in Xamarin Studio, and set the **Main Interface** to the **Main** storyboard.

15. To dismiss the onscreen keyboard when you tap outside the edit view, add a [KeyboardDismissGestureRecognizer](https://gist.github.com/VincentH-Net/8352290) to your main window in FinishedLaunching. Now you can run the app - the Twitter UI should display (without data, because we still need to add bindings).

16. Now let's add the data binding parameters in XCode to complete the app. In XCode, add the **Bind** custom runtime attribute to these controls (see [how](#ios-binding-parameters-in-xcode)) and set it's value as indicated:
	
	<table>
		<tr><td><b>View</b></td><td><b>Bind value</b></td></tr>
		<tr><td>Round Style Text Field - What's up?</td><td>Text, Mode=TwoWay</td></tr>
		<tr><td>Label - 140</td><td>CharactersLeft</td></tr>
		<tr><td>Button - Send</td><td>SendCommand, Mode=Command</td></tr>
		<tr><td>Button - Delete</td><td>DeleteCommand, Mode=Command</td></tr>
		<tr><td>Table View</td><td>Tweet, Mode=TwoWay {List ItemsSource=TweetList}</td></tr>
		<tr><td>Label - Me</td><td>UserName</td></tr>
		<tr><td>Label - 10/25/2013 1:28:25 PM</td><td>CreatedAt</td></tr>
		<tr><td>Label - My tweet text</td><td>Text</td></tr>
	</table>

	As described [here](#list-itemtemplate), the default value for the List ItemTemplate binding parameter for our table view is TweetListItem (because we specified TweetList for the ItemsSource). Now set the (reuse) **Identifier** of the **Table View Cell** to to match the List ItemTemplate value: `TweetListItem`. Now the QuickCross `DataBindableUITableViewSource` can load the prototype cell that should be used for the tweet list items.

17. **Save** the storyboard, switch back to Xamarin Studio and run the app. Test the MainView. Notice how the Send and Delete buttons are enabled and disabled based on the text length and selected item state, and how the characters remaining count is updated as you type. Also note how the selected list item is highlighted both from the UI when tapped, and from code when adding a new tweet.

You have created a working app with QuickCross. Note that the only code that you needed to write is in the viewmodel; no view controller or table view controller code is needed. To get data binding working, the markup specifies some binding parameters in the Bind.

### iOS Data Binding ###
An iOS data binding is a one-on-one binding between an iOS `UIView`, such as a `UITextField` or `UIButton`, and a viewmodel property or command. You can specify bindings with (a combination of):

1. Code in the controller that creates the containing view
2. Segue identifiers
3. Binding parameters in the `Bind` user-defined runtime attribute (of type `String`) in XCode (this is supported in iOS 5.0 or later)

Note that for performance reasons, iOS data binding allows **no more than one view** (within the same rootview) to be bound to a property. If you need to update multiple views with the same property value, you can do that by overriding the `UpdateView()` method in your view controller class, and adding [a few lines of code](#binding-multiple-ios-views-to-a-viewmodel-property).

Finally, note that **you do not need to use the QuickCross implementation of iOS data binding** if you prefer not to. You can subscribe to the standard .NET data binding events (`PropertyChanged` on viewmodels, `CanExecuteChanged` on `RelayCommand` viewmodel commands, and `CollectionChanged` on `ObservableCollection` viewmodel properties) from your code in any view type, and handle data binding any way you like. In this case you can keep using the [New-View command](#new-view), if you [add your own view template files](#customizing-and-extending-command-templates).

#### iOS Binding Parameters in XCode ####
iOS `UIViews` do not have a native general-purpose property that can be used to specify binding parameters (such as the `Tag` property on Android `Views`). QuickCross solves this by adding support for a user-defined runtime attribute named `Bind`, of type `String`. In addition to using the Bind attribute, you can **bind a Storyboard Segue** to a command in XCode by setting the Segue identifier to the name of a viewmodel command.

To specify binding parameters in XCode through the Bind attribute:

1. Select the object for which you want to specify the binding parameters (e.g. a `UILabel`, a `UIButton` or a `UITableView`).
2. In the Identity Inspector (in the [utility area](https://developer.apple.com/library/mac/recipes/xcode_help-general/AbouttheUtilityArea/AbouttheUtilityArea.html)), under the **User Defined Runtime Attributes** header, click on the **+** to add an attribute.
3. Change the **Key Path** of the new attribute to `Bind`, set the **Type** to `String`, and specify the binding parameters (see below for details) in **Value**.

These are the binding parameters that you can specify in the `Bind` attribute (linebreaks added for readability):

    .|propertyName, Mode=OneWay|TwoWay|Command
    {List ItemsSource=listPropertyName, ItemTemplate=listItemTemplateName, 
          AddCommand=addCommandName, RemoveCommand=removeCommandName, 
          CanEdit=false|true|propertyName|fieldName, CanMove=false|true|propertyName|fieldName}
    
All of these parameters except the `.` or `propertyName` are optional. The iOS syntax is similar to the Android binding parameters syntax, however the {Binding ... } delimiters around .|propertyName and Mode are optional in iOS, since the Bind attribute will never contain anything else than binding parameters.

Note that you can specify binding parameters through code in addition to / instead of in the Bind attribute.

##### Binding .|propertyName #####
You can specify a property name to bind to the property of a viewmodel. For bindings in list items that are not viewmodels, you can also specify the name of a field. Specify `.` to bind to the `ToString()` value of the viewmodel or list item object. Note that viewmodel commands are just a special type of viewmodel property, so you can use the propertyName to specify a command name as well.

##### Binding Mode #####
Is `OneWay` by default. The mode specifies:

- `OneWay` data binding where the viewmodel property updates the view - e.g. a display-only UILabel. The bound property can be generated with the propdb1 or propdbcol code snippet.
- `TwoWay` data binding where the viewmodel property updates the view and vice versa, e.g. an editable UITextField. The bound property can be generated with the propdb2, propdb2c or propdbcol code snippet.
- `Command` binding (e.g. a UIButton). The bound command can be generated with the cmd or cmdp code snippet.

Note that you can also bind to the selected item in a `UITableView` with binding modes `TwoWay` or `Command`. When an item in the list is selected, the bound two-way property is set to the selected list item, or the bound command is invoked with the selected item as the command parameter. E.g. you can bind to a command to navigate to a detail view for the selected list item.

The `List` binding parameters are for use with views derived from `UITableView`. Support for collection views will be added soon.

##### List ItemsSource #####
Specifies the name of the viewmodel collection property that contains the list items. The property must implement the standard .NET `IList` interface. If the property also implements the standard .NET `INotifyCollectionChanged` interface (e.g an `ObservableCollection`), the view will automatically reflect added, replaced or removed items. The default value of ItemsSource is `propertyName` + "**List**".

The items in an ItemsSource viewmodel collection property can be:

- An object with fields or properties (e.g. a POCO model object)
- An value object, meaning an object that implements `ToString()` to present the value of the entire object as a human-readable text
- A viewmodel object that has data-bindable properties and/or commands. This is also called **composite viewmodels**, which makes it possible to e.g. automatically display changes of individual fields within existing list item objects.

##### List ItemTemplate #####
Specifies the reuse identifier of the `UITableViewCell` that represents a list item. E.g. the value "TweetListItem" corresponds to the UITableViewCell with reuse identifier TweetListItem. You can specify the reuse identifier in XCode by selecting the UITableViewCell and then specifying the **indentifier** field in the **Attributes Inspector**. The default value of the ItemTemplate binding parameter is the value of `ItemsSource` + "**Item**".

##### List AddCommand #####
Specifies the name of the viewmodel command (e.g. AddProductCommand) that must be executed when the user selects the [Insertion control in table editing mode](https://developer.apple.com/library/ios/documentation/userexperience/conceptual/tableview_iphone/ManageInsertDeleteRow/ManageInsertDeleteRow.html). The command parameter will be the list item object at the insert position, if any. If this binding parameter is not specified, no command will be executed.

##### List RemoveCommand #####
Specifies the name of the viewmodel command (e.g. RemoveProductCommand) that must be executed when the user selects the [Deletion control in table editing mode](https://developer.apple.com/library/ios/documentation/userexperience/conceptual/tableview_iphone/ManageInsertDeleteRow/ManageInsertDeleteRow.html). The command parameter will be the list item object to be deleted. If this binding parameter is not specified, no command will be executed.

##### List CanEdit #####
Specifies whether list items can be edited in table view editing mode. The value `true` or `false` enables resp. disables editing for all items in the list. You can also enable or disable editing per item by adding a boolean property or field to each list item and then specifying the name of that property or field, e.g. `CanEdit=ProductIsEditable`. The default value of the CanEdit binding parameter is `false`.

##### List CanMove #####
Specifies whether list items can be [moved in table view editing mode](https://developer.apple.com/library/ios/documentation/userexperience/conceptual/tableview_iphone/ManageReorderRow/ManageReorderRow.html). The value `true` or `false` enables resp. disables moving for all items in the list. You can also enable or disable moving per item by adding a boolean property or field to each list item and then specifying the name of that property or field, e.g. `CanMove=ProductCanBeReordered`. The default value of the CanMove binding parameter is `false`.

##### Binding a Storyboard Segue to a Command #####
You can bind a Storyboard Segue to a Command in XCode by selecting the Segue and then setting the **Storyboard Segue identifier** field to the name of the command, e.g. ShowDetail. When preparing for the segue, the controller base class will call it's `GetCommandParameter()` method, which you can override in your controller class to provide a parameter for the command, and then the base class will execute the command (if a command with the same name as the Segue identifier exists in the viewmodel).

#### iOS Binding Parameters in Code ####
As an alternative to using the Bind attribute in XCode, you can also specify bindings in code in an optional parameter of the `InitializeBindings()` method. The InitializeBindings method is implemented in the view controller base classes (e.g. `ViewBase` and `TableViewBase`), and it is called in your controller code from the `ViewDidLoad()` method. Here is an example of specifying binding parameters in code:

```csharp
public override void ViewDidLoad()
{
	base.ViewDidLoad();

    var button = ...
    var label = ...

    var bindingsParameters = new BindingParameters[] {
        new BindingParameters { View = button, PropertyName = MainViewModel.COMMANDNAME_IncreaseCountCommand, Mode = BindingMode.Command },
        new BindingParameters { View = label,  PropertyName = MainViewModel.PROPERTYNAME_Count }
    };

    InitializeBindings(View, ViewModel, bindingsParameters);
}
```
You can specify bindings in code for views created in code as well as views loaded from a xib or storyboard file (with outlets). You can specify some or all of the bindings, and in each binding some or all of the binding parameters. If you also specify parameters for a binding in Bind attribute in XCode, those will override or supplement any parameters that you specify in code.

When you specify a binding for a `UITableView`, and it does not have it's `Source` property set, a new `DataBindableUITableViewSource` is created and assigned to the `UITableView` `Source` property automatically when you call `InitializeBindings()`. Alternatively, you can create an instance of a `DataBindableUITableViewSource` (derived) class in code and set that as the `Source` before you call `InitializeBindings()`.

#### Customizing or Extending iOS Data Binding ####
QuickCross allows you to simply modify or extend it with overrides in the view controller base classes `ViewBase` and `TableViewBase`, and in the `ViewDataBindings` class. You can also add more view controller base classes if you need to derive your controllers from other classes besides `UIViewController` and `UITableViewController`.

##### Customizing Data Binding in iOS Views #####
By overriding the `OnPropertyChanged()` method in your view controller class, you can handle changes for specific properties yourself instead of with the standard data binding. E.g:

```csharp
// Example of how to handle specific viewmodel property changes in code instead of with (or in addition to) data binding:
protected override void OnPropertyChanged(string propertyName)
{
    switch (propertyName)
    {
        case OrderViewModel.PROPERTYNAME_DeliveryLocationListHasItems:
            var hasItems = ViewModel.GetPropertyValue<bool>(OrderViewModel.PROPERTYNAME_DeliveryLocationListHasItems);
            DeliveryLocationPicker.Hidden = !hasItems;
            break;
        default:
            base.OnPropertyChanged(propertyName); break;
    }
}
```

You can also override the `UpdateView()` method to change how the data binding mechanism sets a property value to a specific view (or modify multiple views based on the value etc.). E.g. to display a count value as a color, you could bind a `UILabel` named `CountColorLabel` to the `Count` property of a viewmodel, and then override `UpdateView()` like this:

```csharp
public override void UpdateView(UIView view, object value)
{
    if (Object.ReferenceEquals(view, CountColorLabel))
    {
        int count = (int)value;
		view.BackgroundColor = count > 0 ? UIColor.Green : UIColor.Red;
        return;
    }
    base.UpdateView(view, value);
}
```

> Note that `UpdateView()` is also called for **data bindings in all list items** for all data-bound lists in your view. This makes it possible to customize data binding within list items with code in a normal view, instead of writing a custom data bindable table view source to put that customization code in.

###### Binding multiple iOS views to a viewmodel property ######
Another scenario for overriding `UpdateView()` is when you want to update more than one view (within the same root view) from the same viewmodel property:

```csharp
public override void UpdateView(UIView view, object value)
{
    if (Object.ReferenceEquals(view, ProductNameLabel))
    {
        base.UpdateView(view, value); // Set the single data-bound view
        string text = value == null ? "" : value.ToString();
        if (ProductName2Label.Text != text) ProductName2Label.Text = text;
        return;
    }
    base.UpdateView(view, value);
}
```

You can also react to changes in lists that implement INotifyCollectionChanged (e.g. ObservableCollections) by overriding `OnCollectionChanged()` in your view:

```csharp
public override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
{
    ...
}
```
The sender parameter is the collection. You could use this e.g. if you want to do some animation when items are added or removed in a list.

Finally, you can modify or provide a parameter for a command when it is invoked, by overriding `GetCommandParameter()` in your view. E.g. you can pass the currently selected item in a table view to a command:

```csharp
public override object GetCommandParameter(string commandName, object parameter = null)
{
    if (commandName == MainViewModel.COMMANDNAME_ViewItemCommand)
    {
        var indexPath = TableView.IndexPathForSelectedRow;
        if (indexPath != null) return ViewModel.Items[indexPath.Row];
    }
    return base.GetCommandParameter(commandName);
}
```

> Note that `GetCommandParameter()` is also called for **commands in all list items that are viewmodels** for all data-bound lists in your view. This makes it possible to customize command parameter handling within list items with code in a normal view controller, instead of writing a custom data bindable table view source to put that customization code in.

##### Adding New Data-Bindable iOS Table View Sources #####
The built-in QuickCross `DataBindableUITableViewSource` class supports a simple list with one section, that can be data-bound to collections that implement the standard .NET interfaces `IList` and optionally `INotifyCollectionChanged`, such as `ObservableCollection<T>`. The collection items can be viewmodels or they can be simple .NET objects with properties and/or fields. If you implement `ToString()` on the list item object, you can also bind to `.` to display the value of an entire list item as text.

The `DataBindableUITableViewSource` class also supports using the native iOS table view editing mode to add, remove, edit and move items within a table view.

You can create specialized table view sources (e.g. if you want to display a table with multiple sections), by implementing a class derived from `DataBindableUITableViewSource` and then overriding methods from `DataBindableUITableViewSource` and/or the underlying `UITableViewSource` class. The inline documentation on the virtual methods (e.g. `AddHandlers`, `RemoveHandlers`, `OnCollectionChanged` and `UpdateView`) in the `DataBindableUITableViewSource` [source](https://github.com/MacawNL/QuickCross/blob/master/SampleApp.ios/QuickCross/DataBindableUITableViewSource.cs) provides guidance on this.

As [described](#ios-binding-parameters-in-code) above, you can create an instance of your custom table view source and assign it to the `Source` property on a table view in the `ViewDidLoad()` method of your view controller, before you call `InitializeBindings()`.

##### Adding New Data-Bindable iOS View Types #####
Out of the box QuickCross has default data binding support for these view types:
> Note: in 2.0 beta these are very minimal, more will be added in 2.0 final release. Don't let this discourage you to try out the beta; it is really simple to add view types on the fly when you need them.

One-Way binding:

- `UILabel`

Two-way binding:

- `UITextField`
- `UITextView`
- `UITableView` and derived types

Command binding:

- `UIButton`
- `UITableView` and derived types

To make more control types data bindable, you can simply add a case to the appropriate switch statements in the `QuickCross\ViewDataBindings.UI.cs` file in your application project, as indicated by the `// TODO: ` comments. E.g.:

```csharp
public static void UpdateView(View view, object value)
{
    if (view != null)
    {
        string viewTypeName = view.GetType().FullName;
        switch (viewTypeName)
        {
            // TODO: Add cases here for specialized view types, as needed
            case "Macaw.UIComponents.MultiImageView":
                {
                    if (value is Uri) value = ((Uri)value).AbsoluteUri;
                    var multiImageView = (Macaw.UIComponents.MultiImageView)view;
                    multiImageView.LoadImageList(value == null ? null : new[] { (string)value });
                }
                break;
			...
		}
	}
}
```
To prevent memory leaks, be sure that if you register an event handler on a view in `AddCommandHandler()` or `AddTwoWayHandler()`, you also unregister that handler in `RemoveCommandHandler()` resp. `RemoveTwoWayHandler()`.

##### Adding New iOS View Base Classes #####
If you need to derive your view controllers from other classes besides `UIViewController` and `UITableViewController`, you can simply copy the existing `QuickCross\ViewBase.cs` or `QuickCross\TableViewBase.cs` class and change the class that it derives from to the one that you need.

E.g. to support data bindable classes derived from `UIPageViewController`, you would copy ViewBase.cs to PageViewBase.cs and just change "UIViewController" to "UIPageViewController" in these lines:

```csharp
public class PageViewBase : UIPageViewController, ViewDataBindings.ViewExtensionPoints
{
    public PageViewBase() { }
    public PageViewBase(string nibName, NSBundle bundle) : base(nibName, bundle) { }
    public PageViewBase(IntPtr handle) : base(handle) { }

    ...
}
```
And then you can add some new constructor overloads to pass parameters to the base class constructor. Now you can use the new base class in your view:

```csharp
public partial class ProductView : PageViewBase
{
	...
}
```
It should take no more than a minute. To complete this, you could also [add a custom view template](#customizing-and-extending-command-templates) for this new view type.

### iOS View Lifecycle Support ###
To prevent memory leaks, you should remove and re-add any handlers in your view controller code that you register for external events. External means: events that are not defined on the view controller itself (or on contained objects that are only referenced in the view controller such as a `UIButton`). E.g. an event handler for a viewmodel event or a service event needs to be removed and re-added like this. The QuickCross view controller base classes do this removing and re-adding automatically at the `ViewWillAppear()` and `ViewDidDisappear()` view lifecycle events. To have your own external event handlers added, removed and re-added at the appropriate times, you only need to override the `AddHandlers()` and `RemoveHandlers()` methods and add/remove your handlers there. E.g.:

```csharp
protected override void AddHandlers()
{
    base.AddHandlers();
    SomeExternalService.SomeEvent += SomeExternalService_SomeEvent;
}

protected override void RemoveHandlers()
{
    SomeExternalService.SomeEvent -= SomeExternalService_SomeEvent;
    base.RemoveHandlers();
}
```
Note: Be sure to always call the base class method as well!

You should not call `AddHandlers()` yourself - that would mess up the base class tracking of added handlers. `AddHandlers()` is initially called by the view controller base class in the `InitializeBindings()` method, which you call in your view controller code in the `ViewDidLoad()` method.
