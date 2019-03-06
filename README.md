# Topshelf.MicrosoftDependencyInjection
MicrosoftDependencyInjection integration for Topshelf

Topshelf.MicrosoftDependencyInjection
================

Topshelf.MicrosoftDependencyInjection provides extensions to construct your service class from your MicrosoftDependencyInjection service provider.

Install
-------
It's available via [nuget package](https://www.nuget.org/packages/topshelf.microsoftdependencyinjection)  
PM> `Install-Package Topshelf.MicrosoftDependencyInjection`

Example Usage
-------------
```csharp
static void Main(string[] args)
{
	// Create your provider
	var provider = new ServiceCollection()
    .AddSingleton<ISampleDependency, SampleDependency>()
    .BuildServiceProvider();

	HostFactory.Run(c =>
	{
		// Pass it to Topshelf
		c.UseServiceProvider(provider);
		c.Service<SampleService>(s =>
		{
			// Let Topshelf use it
			s.ConstructUsingServiceProvider();
			s.WhenStarted((service, control) => service.Start());
			s.WhenStopped((service, control) => service.Stop());
		});
	});
}
```
