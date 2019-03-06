Topshelf.MicrosoftDependencyInjection
================

Topshelf.MicrosoftDependencyInjection provides extensions to construct your service class from your MicrosoftDependencyInjection service provider.



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
