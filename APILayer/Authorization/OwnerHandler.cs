using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// This authorization handler enforces the ownership rule for student resources.
// It checks whether the current user is either:
// - An Admin (full access), OR
// - The owner of the student record being requested
public enum OwnershipType
{
    User,
    Account
}
public class OwnershipResource
{
    
    public int Id { get; set; }
    public bool AdminCheack {  get; set; }
    public OwnershipType Type { get; set; }
}
public class OwnerHandler
    : AuthorizationHandler<OwnerRequirement, OwnershipResource>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnerRequirement requirement,
        OwnershipResource resource)
    {

        if (resource.AdminCheack && context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var authenticatedId = resource.Type == OwnershipType.Account ? context.User.FindFirstValue("AccountID") : context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(authenticatedId, out int id) &&
            id == resource.Id)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
