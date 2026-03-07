using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;

namespace ExtraAuthSettings;

/// <summary>
/// Helpers for Jellyfin user entities.
/// </summary>
public static class UserExtensions
{
    /// <summary>
    /// Gets whether the user has a specific permission.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="permissionKind">The permission kind.</param>
    /// <returns><c>true</c> if the permission is enabled.</returns>
    public static bool HasPermission(this User user, PermissionKind permissionKind)
    {
        return user.Permissions.Any(permission => permission.Kind == permissionKind && permission.Value);
    }
}
