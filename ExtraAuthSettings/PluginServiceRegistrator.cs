using ExtraAuthSettings.Security;
using ExtraAuthSettings.Services;
using MediaBrowser.Controller.Plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ExtraAuthSettings;

/// <summary>
/// Registers plugin services.
/// </summary>
public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, MediaBrowser.Controller.IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton<UserAccessSettingsService>();
        serviceCollection.AddScoped<SelfServiceRestrictionFilter>();

        serviceCollection.Configure<MvcOptions>(options =>
        {
            options.Filters.AddService<SelfServiceRestrictionFilter>();
        });
    }
}
