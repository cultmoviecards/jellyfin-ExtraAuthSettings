using System;
using System.Linq;
using System.Threading.Tasks;
using ExtraAuthSettings.Services;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExtraAuthSettings.Security;

/// <summary>
/// Blocks self-service account mutations for users configured by the plugin.
/// </summary>
public class SelfServiceRestrictionFilter : IAsyncActionFilter
{
    private readonly IAuthorizationContext _authorizationContext;
    private readonly IUserManager _userManager;
    private readonly UserAccessSettingsService _userAccessSettingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelfServiceRestrictionFilter"/> class.
    /// </summary>
    /// <param name="authorizationContext">The authorization context.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="userAccessSettingsService">The user access settings service.</param>
    public SelfServiceRestrictionFilter(
        IAuthorizationContext authorizationContext,
        IUserManager userManager,
        UserAccessSettingsService userAccessSettingsService)
    {
        _authorizationContext = authorizationContext;
        _userManager = userManager;
        _userAccessSettingsService = userAccessSettingsService;
    }

    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!IsMutatingRequest(context.HttpContext.Request.Method))
        {
            await next().ConfigureAwait(false);
            return;
        }

        var actor = await GetActorAsync(context.HttpContext).ConfigureAwait(false);
        if (actor is null || actor.HasPermission(PermissionKind.IsAdministrator))
        {
            await next().ConfigureAwait(false);
            return;
        }

        var pathSegments = GetPathSegments(context.HttpContext.Request.Path);
        if (IsPasswordChangeRequest(pathSegments, out var passwordTargetUserId)
            && actor.Id == passwordTargetUserId
            && !_userAccessSettingsService.CanChangePassword(passwordTargetUserId))
        {
            context.Result = CreateForbiddenResult("Password changes for this account must be performed by an administrator.");
            return;
        }

        if (IsProfileImageChangeRequest(pathSegments, out var imageTargetUserId)
            && actor.Id == imageTargetUserId
            && !_userAccessSettingsService.CanChangeProfileImage(imageTargetUserId))
        {
            context.Result = CreateForbiddenResult("Profile picture changes for this account must be performed by an administrator.");
            return;
        }

        await next().ConfigureAwait(false);
    }

    private async Task<User?> GetActorAsync(HttpContext httpContext)
    {
        var authorizationInfo = await _authorizationContext.GetAuthorizationInfo(httpContext).ConfigureAwait(false);
        if (authorizationInfo?.User is not null)
        {
            return authorizationInfo.User;
        }

        return authorizationInfo?.UserId is Guid userId && userId != Guid.Empty
            ? _userManager.GetUserById(userId)
            : null;
    }

    private static bool IsMutatingRequest(string method)
    {
        return HttpMethods.IsDelete(method) || HttpMethods.IsPatch(method) || HttpMethods.IsPost(method) || HttpMethods.IsPut(method);
    }

    private static string[] GetPathSegments(PathString path)
    {
        return path.Value?
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? [];
    }

    private static bool IsPasswordChangeRequest(string[] pathSegments, out Guid userId)
    {
        userId = Guid.Empty;
        var normalizedSegments = NormalizeSegments(pathSegments);
        return normalizedSegments.Length >= 3
            && normalizedSegments[0].Equals("Users", StringComparison.OrdinalIgnoreCase)
            && Guid.TryParse(normalizedSegments[1], out userId)
            && normalizedSegments[2].Equals("Password", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsProfileImageChangeRequest(string[] pathSegments, out Guid userId)
    {
        userId = Guid.Empty;
        var normalizedSegments = NormalizeSegments(pathSegments);

        if (normalizedSegments.Length >= 3
            && normalizedSegments[0].Equals("Users", StringComparison.OrdinalIgnoreCase)
            && Guid.TryParse(normalizedSegments[1], out userId)
            && normalizedSegments[2].Equals("Images", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (normalizedSegments.Length >= 3
            && normalizedSegments[0].Equals("Items", StringComparison.OrdinalIgnoreCase)
            && Guid.TryParse(normalizedSegments[1], out userId)
            && normalizedSegments[2].Equals("Images", StringComparison.OrdinalIgnoreCase))
        {
            return _userManager.GetUserById(userId) is not null;
        }

        userId = Guid.Empty;
        return false;
    }

    private static string[] NormalizeSegments(string[] pathSegments)
    {
        if (pathSegments.Length > 0 && pathSegments[0].Equals("emby", StringComparison.OrdinalIgnoreCase))
        {
            return pathSegments.Skip(1).ToArray();
        }

        return pathSegments;
    }

    private static ObjectResult CreateForbiddenResult(string message)
    {
        return new ObjectResult(new { message })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}
