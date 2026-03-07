using System;

namespace ExtraAuthSettings.Configuration;

/// <summary>
/// Per-user restrictions managed by the plugin.
/// </summary>
public class ManagedUserAccessSettings
{
    /// <summary>
    /// Gets or sets the Jellyfin user id.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can change their own password.
    /// </summary>
    public bool AllowPasswordChange { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the user can change their own profile image.
    /// </summary>
    public bool AllowProfileImageChange { get; set; } = true;
}
