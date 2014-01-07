**QuickCross** is a lightweight (no binaries) cross-platform MVVM pattern to quickly build native Xamarin.iOS, Xamarin.Android, Windows Phone and Windows Store Apps with shared C# code.QuickCross provides data binding for Android and for iOS. It accelerates development with scaffolders and code snippets, also for a single platform app. For cross-platform apps QuickCross increases code sharing with an Application-Navigator pattern.QuickCross aims to leave you in full control; it does not get in the way if you want to do some things differently, and you can simply extend or modify it.
> **NOTE:** This version (2.0) of QuickCross is in **beta** now; iOS data bindings are working and documented but the documentation on creating an app from scratch is under construction. **This release only supports the iOS platform**; the other platforms and more elaborate iOS example documentation will be re-added before the 2.0 final release.> The purpose of this beta release is to gain feedback on the iOS data bindings implementation; if you are building a production application or if you are targeting other platforms than iOS, it is recommended to keep using the previous version until the QuickCross release version is published.Versions before 2.0 were published under the name **MvvmQuickCross**. They will continue to be supported at the [existing MvvmQuickCross GitHub repository](https://github.com/MacawNL/MvvmQuickCross), and [the existing MvvmQuickCross NuGet packages](http://nuget.org/packages/mvvmquickcross) will remain available.Upgrading from MvvmQuickCross to QuickCross is quick (pun intended); instructions will be documented here (TODO URL).## News ##**Jan 3, 2014**: QuickCross release: 2.0 beta is out. Adds simple iOS data binding and an iOS example app.**Coming up**: Documentation. Next planned QuickCross release: 2.0 final, which will add more iOS data bindings, the other platforms (besides iOS), and full documentation and examples. ETA Jan 15.## NuGet package - PRERELEASE ##[QuickCross NuGet packages](http://nuget.org/packages/quickcross)## Preliminary Documentation ##

Most of [the existing documentation for MvvmQuickCross](https://github.com/MacawNL/MvvmQuickCross) applies for QuickCross 2.0 as well; the MvvmQuickCross documentation will be copied here and updated soon. Until then, these are the key changes in QuickCross 2.0:

- Name change from MVVMQuickCross to QuickCross

- Minor change in shared code: `navigationContext` parameter was moved from the `Application` to the `Navigator` for more flexibility when using multiple navigation contexts. For existing code this means you need to pass the navigationContext parameter to the Navigator constructor instead of the Application constructor. The Navigator now manages navigation context instances and decides when to use/create which context (if more than one is needed).

- [iOS data binding](#ios-data-binding), with support for **coded views, Xib views and StoryBoard views**:

	- See the [QCTest1 app source](https://github.com/MacawNL/QuickCross/tree/master/Examples/QCTest1) in this repository for an example of a coded view, a Xib view, and a StoryBoard view with a UITableView, including navigation.

	- The `New-View` command in the VS Package Manager Console now has support for these view types in iOS: `Code`, `Xib`, `StoryBoard` and `StoryBoardTable`. When you add these views, the generated view class contains inline comments on how to complete the view in xCode, if needed. More view types will be added in the final 2.0 release.

Please feel free to run and inspect the two iOS example apps and provide feedback on what you think of the approach for iOS data bindings. Thanks and NJoy!

## iOS ##
Below is an overview of using QuickCross with Xamarin.iOS.

### Create an iOS App ###
Here is how to create an iOS Twitter app that demonstrates simple data binding:
> Note: this documentation topic is under construction

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
