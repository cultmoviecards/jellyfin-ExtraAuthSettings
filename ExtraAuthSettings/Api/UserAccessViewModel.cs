using System;

namespace ExtraAuthSettings.Api;

/// <summary>
/// A user row displayed on the plugin configuration page.
/// </summary>
public class UserAccessViewModel
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user is an administrator.
    /// </summary>
    public bool IsAdministrator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can change their own password.
    /// </summary>
    public bool AllowPasswordChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can change their own profile image.
    /// </summary>
    public bool AllowProfileImageChange { get; set; }
}
