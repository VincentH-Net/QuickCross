**QuickCross** is a lightweight (no binaries) cross-platform MVVM pattern to quickly build native Xamarin.iOS, Xamarin.Android, Windows Phone and Windows Store Apps with shared C# code.QuickCross provides data binding for Android and for iOS. It accelerates development with scaffolders and code snippets, also for a single platform app. For cross-platform apps QuickCross increases code sharing with an Application-Navigator pattern.QuickCross aims to leave you in full control; it does not get in the way if you want to do some things differently, and you can simply extend or modify it.
> **NOTE:** This version (2.0) of QuickCross is in **beta** now; the iOS data bindings are working but they are currently documented at a minimum level. **This release only supports the iOS platform**; the other platforms and more elaborate iOS example documentation will be re-added before the 2.0 final release.> The purpose of this beta release is to gain feedback on the iOS data bindings implementation; if you are building a production application or if you are targeting other platforms than iOS, it is recommended to keep using the previous version until the QuickCross release version is published.Versions before 2.0 were published under the name **MvvmQuickCross**. They will continue to be supported at the [existing MvvmQuickCross GitHub repository](https://github.com/MacawNL/MvvmQuickCross), and [the existing MvvmQuickCross NuGet packages](http://nuget.org/packages/mvvmquickcross) will remain available.Upgrading from MvvmQuickCross to QuickCross is quick (pun intended); instructions will be documented here (TODO URL).## News ##**Jan 3, 2014**: QuickCross release: 2.0 beta is out. Adds simple iOS data binding and an iOS example app.**Coming up**: Documentation. Next planned QuickCross release: 2.0 final, which will add more iOS data bindings, the other platforms (besides iOS), and full documentation and examples. ETA Jan 15.## NuGet package - PRERELEASE ##[QuickCross NuGet packages](http://nuget.org/packages/quickcross)## Preliminary Documentation ##

The changes in version 2.0 are:

- Name change from MVVMQuickCross to QuickCross

- Minor change in shared code: **navigationContext** parameter was moved from the Application to the Navigator for more flexibility when using multiple navigation contexts. For existing code this means you need to pass the navigationContext parameter to the Navigator constructor instead of the Application constructor. The Navigator now manages navigation context instances and decides when to use/create which context (if more than one is needed).

- [iOS data binding](#ios-data-binding), with support for **coded views, Xib views and StoryBoard views**. When providing data binding parameters in markup, the parameter syntax is similar to the android data binding syntax, with these notable differences:

	- See the **QCTest1 app source** in the Examples folder in this repository for an example of a coded view, a Xib view, and a StoryBoard view with a UITableView, including navigation.

	- **New-View** has support for these view types: Code, Xib, StoryBoard and StoryBoardTable. When you add these views, the generated view class contains inline comments on how to complete the view in xCode, if needed. More view types will be added in the final 2.0 release.

Please feel free to run and inspect the two iOS example apps and provide feedback on what you think of the approach for iOS data bindings. Thanks and NJoy!

### iOS Data Binding ###
> This documentation topic is under construction

An iOS data binding is a one-on-one binding between an iOS UIView, such as a UITextField or UIButton, and a viewmodel property or command. You can specify bindings with (a combination of):

1. Code in the controller that creates the containing view
2. Binding parameters in the `Bind` user-defined runtime attribute (of type `String`) in XCode

Note that for performance reasons, iOS data binding allows **no more than one view** (within the same rootview) to be bound to a property. If you need to update multiple views with the same property value, you can do that by overriding the `UpdateView()` method in your controller class, and adding [a few lines of code](#binding-multiple-ios-views-to-a-viewmodel-property).

Finally, note that **you do not need to use the QuickCross implementation of iOS data binding** if you prefer not to. You can subscribe to the standard .NET data binding events (`PropertyChanged` on viewmodels, `CanExecuteChanged` on `RelayCommand` viewmodel commands, and `CollectionChanged` on `ObservableCollection` viewmodel properties) from your code in any view type, and handle data binding any way you like. In this case you can keep using the [New-View command](#new-view), if you [add your own view template files](#customizing-and-extending-command-templates).

#### iOS Binding Parameters in XCode ####
iOS UIViews do not have a native general-purpose property that can be used to specify binding parameters (such as the Tag property on Android views). QuickCross solves this by adding support for a user-defined runtime attribute named `Bind`, of type `String`.

To specify binding parameters in XCode:

1. Select the object for which you want to specify the binding parameters (e.g. a UILabel, a UIButton or a UITableView).
2. In the Identity Inspector (in the [utility area](https://developer.apple.com/library/mac/recipes/xcode_help-general/AbouttheUtilityArea/AbouttheUtilityArea.html)), under the **User Defined Runtime Attributes** header, click on the + to add an attribute.
3. Change the **Key Path** of the new attribute to `Bind`, set the **Type** to `String`, and specify the binding parameters (see below for details) in **Value**.

These are the binding parameters that you can specify in the Bind attribute (linebreaks added for readability):

    .|propertyName, Mode=OneWay|TwoWay|Command
    {List ItemsSource=listPropertyName, ItemTemplate=listItemTemplateName, 
          AddCommand=addCommandName, RemoveCommand=removeCommandName, 
          CanEdit=false|true|propertyName|fieldName, CanMove=false|true|propertyName|fieldName}
    
All of these parameters except the `.` or `propertyName` are optional. The iOS syntax is similar to the Android binding parameters syntax, however the {Binding ... } delimiters are optional in iOS, since the Bind attribute will never contain something else than binding parameters.

Note that you can specify binding parameters through code in addition to / instead of in the Bind attribute.

##### Binding .|propertyName #####
You can specify a property name to bind to the property of a viewmodel. For bindings in list items that are not viewmodels, you can also specify the name of a field. Specify `.` to bind to the `ToString()` value of the viewmodel or list item object. Note that viewmodel commands are just a special type of viewmodel property, so you can use the propertyName to specify a command name as well.

##### Binding Mode #####
Is `OneWay` by default. The mode specifies:

- `OneWay` data binding where the viewmodel property updates the view - e.g. a display-only UILabel. The bound property can be generated with the propdb1 or propdbcol code snippet.
- `TwoWay` data binding where the viewmodel property updates the view and vice versa, e.g. an editable UITextField. The bound property can be generated with the propdb2, propdb2c or propdbcol code snippet.
- `Command` binding (e.g. a UIButton). The bound command can be generated with the cmd or cmdp code snippet.

Note that you can also bind to the selected item in a UITableView with binding modes TwoWay or Command. When an item in the list is selected, the bound two-way property is set to the selected list item, or the bound command is invoked with the selected item as the command parameter. E.g. you can bind to a command to navigate to a detail view for the selected list item.

The `List` binding parameters are for use with views derived from `UITableView`. Support for collection views will be added soon.

##### List ItemsSource #####
Specifies the name of the viewmodel collection property that contains the list items. The property must implement the standard .NET `IList` interface. If the property also implements the standard .NET `INotifyCollectionChanged` interface (e.g an `ObservableCollection`), the view will automatically reflect added, replaced or removed items. The default value of ItemsSource is `propertyName` + "**List**".

The items in an ItemsSource viewmodel collection property can be:

- An object with fields or properties (e.g. a POCO model object)
- An 'ValueItem' object, meaning an object that implements `ToString()` to present the value of the entire object as a human-readable text
- A viewmodel object that has data-bindable properties and/or commands. This is also called **composite viewmodels**, which makes it possible to e.g. automatically display changes of individual fields within existing list item objects.

##### List ItemTemplate #####
Specifies the reuse identifier of the UITableViewCell that represents a list item. E.g. the value "TweetListItem" corresponds to the UITableViewCell with reuse identifier TweetListItem. You can specify the reuse identifier in XCode by selecting the UITableViewCell and then specifying the **indentifier** field in the **Attributes Inspector**. The default value of the ItemTemplate binding parameter is the value of `ItemsSource` + "**Item**".
