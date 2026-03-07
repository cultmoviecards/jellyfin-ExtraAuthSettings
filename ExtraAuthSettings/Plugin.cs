using System;
using System.Collections.Generic;
using System.Globalization;
using ExtraAuthSettings.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace ExtraAuthSettings;

/// <summary>
/// The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <inheritdoc />
    public override string Name => "ExtraAuthSettings";

    /// <inheritdoc />
    public override string Description => "Adds per-user restrictions for self-service password and profile picture changes.";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("0e1e569f-1060-4115-a35d-a9886e393725");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        ];
    }
}
