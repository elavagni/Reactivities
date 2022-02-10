using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
  public class IsHostRequirement : IAuthorizationRequirement
  {
  }

  public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
  {
    private readonly DataContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public IsHostRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
      _dbContext = dbContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        IsHostRequirement requirement)
    {

      //We need to get the attendee from our join table ActivityAttendee and check the value for the IsHost field. 
      //For this we need the activity id and the user id

      //Get userId from the Claims Principal (context.User)
      var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

      //If the user is null complete the task
      if (userId == null) return Task.CompletedTask;

      //Get the activity Id from the route (url)
      var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
            .SingleOrDefault(x => x.Key == "id").Value?.ToString());

      //Get the attendee using the userId and the activityId
      //We don't want EF to track this object in memory (EF core would load the memory into memory and won't reload it to 
      //refresh dependencies, this would generate issues when trying to save activities (it would clear the list of attendees)      
      var attendee = _dbContext.ActivityAttendees
          .AsNoTracking()
          .SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activityId)
          .Result;

      //If the attendee is null complete the task
      if (attendee == null) return Task.CompletedTask;

      //If the attendee is the host, satisfy the requirement
      if (attendee.IsHost) context.Succeed(requirement);

      //Complete the task
      return Task.CompletedTask;

    }
  }
}