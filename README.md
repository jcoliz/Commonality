# Commonality

Commonality is a .NET Standard (1.3) library of platform-independent, domain-independent 
classes which are useful to accomplish fundamental tasks in .NET applications.

## Service Locator

* [ServiceLocator](/Commonality/ServiceLocator.cs). Implements the service locator pattern. Some classes in this library use this service locator to find dependant services. By convention, all classes in this library tolerate missing services without any error or exception.

## Clock Handlers

Provides an abstract way of getting/setting the time, and an implementation for using the system clock.

* [IClock](/Commonality/IClock.cs). Defines a platform-dependent service to access the time
* [DotNetClock](/Commonality/DotNetClock.cs). Provides a standard dotnet 'DateTime.Now' in the IClock interface, with the ability to set the time for internal use only. (Doesn't affect the system clock)

## Logging

Provides an abstract way of logging instrumentation events, and a filesystem-based implementation for writing them to disk.

* [ILogger](/Commonality/ILogger.cs). Defines a platform-dependent service to log events and errors. Typically this is backed by a server-side instrumentation service, but it can also be used to capture all events locally.
* [FileSystemLogger](/Commonality/FileSystemLogger.cs). This is a simple logger which just logs to the local file system. No cloud analytics here.

## Common Patterns for XAML ViewModel Binding

When writing viewmodels which bind to XAML, a few common patterns recur, which these classes can help with.

* [ViewModelBase](/Commonality/ViewModelBase.cs). Provides some basic common functionality that all ViewModels want, including INotifyPropertyChanged handling, and message/error propagation to the UI.
* [DelegateCommand](/Commonality/DelegateCommand.cs). Quick and reusable ICommand as described in http://www.wpftutorial.net/delegatecommand.html
* [RangeObservableCollection](/Commonality/RangeObservableCollection.cs). Extends ObservableCollection to add an AddRange.

## Value Converters

Classes to help with common situations in XAML where values need to be converted, but does so in a platform-independent way.

* [IBaseValueConverter](/Commonality/IBaseValueConverter.cs). Defines a platform-independent way to implement view converters. In order to actually use a platform-independent view converter, you'll usually still need to convert to a platform-specific valueconverter. Still, you can convert any valueconveter based on this interface to something platform-specific with a single generic.
* [TimeSpanFormatConverter](/Commonality/TimeSpanFormatConverter.cs). Value converter to apply a custom timespan format to convert a timespan to a string
* [DateTimeFormatConverter](/Commonality/DateTimeFormatConverter.cs). Value converter to apply a custom datetime format to convert a datetime to a string
* [DefaultConverter](/Commonality/DefaultConverter.cs). General purpose "bool to {something}" converter. Instead of just converting from bool, it converts from ANYTHING, by comparing it with the default value for that type.

## Standard Interfaces

Interfaces which can abstract away commonly-needed services.

* [ISettings](/Commonality/ISettings.cs). This interface can abstract away the platform-specific way to get at key/value pair settings.
* [IApplicationInfo](/Commonality/IApplicationInfo.cs). This interface can be used by shared/portable code logic modules to get at information about the app context in which it's running.
