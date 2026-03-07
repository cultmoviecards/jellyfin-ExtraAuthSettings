using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MediaBrowser.Model.Plugins;

namespace ExtraAuthSettings.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        UserSettings = [];
    }

    /// <summary>
    /// Gets or sets the per-user settings.
    /// </summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Jellyfin plugin configuration serialization expects a simple mutable collection shape.")]
    public ManagedUserAccessSettings[] UserSettings { get; set; }

    /// <summary>
    /// Gets the settings for a user, defaulting to unrestricted if not configured.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>The resolved settings.</returns>
    public ManagedUserAccessSettings GetSettingsForUser(Guid userId)
    {
        return UserSettings.FirstOrDefault(settings => settings.UserId == userId)
            ?? new ManagedUserAccessSettings
            {
                UserId = userId,
                AllowPasswordChange = true,
                AllowProfileImageChange = true
            };
    }
}
