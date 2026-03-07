using System;
using ExtraAuthSettings.Configuration;

namespace ExtraAuthSettings.Services;

/// <summary>
/// Resolves plugin-owned user access settings.
/// </summary>
public class UserAccessSettingsService
{
    /// <summary>
    /// Gets a value indicating whether the user can change their own password.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns><c>true</c> when allowed.</returns>
    public bool CanChangePassword(Guid userId)
    {
        return GetSettings(userId).AllowPasswordChange;
    }

    /// <summary>
    /// Gets a value indicating whether the user can change their own profile image.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns><c>true</c> when allowed.</returns>
    public bool CanChangeProfileImage(Guid userId)
    {
        return GetSettings(userId).AllowProfileImageChange;
    }

    private static ManagedUserAccessSettings GetSettings(Guid userId)
    {
        var configuration = Plugin.Instance?.Configuration ?? new PluginConfiguration();
        return configuration.GetSettingsForUser(userId);
    }
}
