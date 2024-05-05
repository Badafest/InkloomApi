using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inkloom.Api.EmailTemplates;

public static class HtmlRendererFactory
{
    public static HtmlRenderer GetRenderer()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        return new HtmlRenderer(serviceProvider, loggerFactory);
    }
}