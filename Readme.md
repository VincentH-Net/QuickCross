**QuickCross** is a lightweight (no binaries) cross-platform MVVM pattern to quickly build native Xamarin.iOS, Xamarin.Android, Windows Phone and Windows Store Apps with shared C# code.QuickCross provides data binding for Android and for iOS. It accelerates development with scaffolders and code snippets, also for a single platform app. For cross-platform apps QuickCross increases code sharing with an Application-Navigator pattern.QuickCross aims to leave you in full control; it does not get in the way if you want to do some things differently, and you can simply extend or modify it.
> **NOTE:** This version (2.0) of QuickCross is in **beta** now; the iOS data bindings are working but they are currently documented at a minimum level; more elaborate iOS example documentation will be added before the 2.0 final release.> The purpose of this beta release is to gain feedback on the iOS data bindings implementation; if you are building a production application or if you are targeting other platforms than iOS, it is recommended to keep using the previous version until the QuickCross release version is published.Versions before 2.0 were published under the name **MvvmQuickCross**. They will continue to be supported at the [existing MvvmQuickCross GitHub repository](https://github.com/MacawNL/MvvmQuickCross), and [the existing MvvmQuickCross NuGet packages](http://nuget.org/packages/mvvmquickcross) will remain available.Upgrading from MvvmQuickCross to QuickCross is quick (pun intended); instructions will be documented here (TODO URL).## News ##**Jan 3, 2014**: QuickCross release: 2.0 beta is out. Adds simple iOS data binding and an iOS example app.**Coming up**: Documentation. Next planned QuickCross release: 2.0 final, which will add more iOS data bindings, the other platforms (besides iOS), and full documentation and examples. ETA Jan 15.## NuGet package - PRERELEASE ##[QuickCross NuGet packages](http://nuget.org/packages/quickcross)## Preliminary Documentation ##

The changes in version 2.0 are:

- Name change from MVVMQuickCross to QuickCross

- Minor change in shared code: navigationContext parameter was moved from the Application to the Navigator for more flexibility when using multiple navigation contexts. For existing code this means you need to pass the navigationContext parameter to the Navigator constructor instead of the Application constructor. The Navigator now manages navigation context instances and decides when to use/create which context (if more than one is needed).

- iOS data bindings, with support for coded views, Xib views and StoryBoard views. When providing data binding parameters in markup, the parameter syntax is similar to the android data binding syntax, with these notable differences:

	- You add the parameters by adding a custom runtime attribute to views in XCode. The attribute is named **Bind**, of type **string**. These are example values:
		- `Count` : one-way binding to the viewmodel property Count
		- `Name, Mode=TwoWay` : two-way binding to the viewmodel property Name
		- `IncreaseCountCommand, Mode=Command` : binding to a viewmodel command
		- `{List ItemsSource=ProductList}` : Usually set on a UITableView; specifies the list data source property
		- There are specific command parameters in addition to ItemsSource (AddCommand, RemoveCommand, CanEdit, CanMove) in a list for adding, editing and deleting items in a list; also reordering is supported. For an example of this, see the **SampleApp.ios app** source in this repository.

	- To specify the `ItemTemplate` parameter for a list in xCode you need to set the reuse identifier in the UITableViewCell, e.g. to ProductListItem.

	- See the **QCTest1 app source** in the Examples folder in this repository for an example of a coded view, a Xib view, and a StoryBoard view with a UITableView, including navigation.

	- **New-View** has support for these view types: Code, Xib, StoryBoard and StoryBoardTable. When you add these views, the generated view class contains inline comments on how to complete the view in xCode, if needed. More view types will be added in the final 2.0 release.

Please feel free to run and inspect the two iOS example apps and provide feedback on what you think of the approach for iOS data bindings. Thanks and NJoy!
