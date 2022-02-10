using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
  public class UpdateAttendance
  {
    public class Command : IRequest<Result<Unit>>
    {
      public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;
        _context = context;
      }
      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities
         .Include(a => a.Attendees)
           .ThenInclude(u => u.AppUser)
             .SingleOrDefaultAsync(x => x.Id == request.Id);

        if (activity == null) return null;

        var user = await _context.Users.FirstOrDefaultAsync(x =>
              x.UserName == _userAccessor.GetUserName());

        var hostUserName = activity.Attendees.FirstOrDefault(x => x.IsHost)?.AppUser?.UserName;

        var attendance = activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

        //If the current user is the host, cancel or restore the activity
        if (attendance != null && hostUserName == user.UserName)
          activity.IsCancelled = !activity.IsCancelled;

        //If the user is not the host, and it is attending the event, remove it from the list
        if (attendance != null && hostUserName != user.UserName)
          activity.Attendees.Remove(attendance);

        //If the user is not attending the event, add it to the  list
        if (attendance == null)
        {
          attendance = new ActivityAttendee
          {
            AppUser = user,
            Activity = activity,
            IsHost = false
          };

          activity.Attendees.Add(attendance);
        }

        var result = await _context.SaveChangesAsync() > 0;
        return result ?
                Result<Unit>.Success(Unit.Value) :
                Result<Unit>.Failure("Problem updating attendance");
      }
    }

  }
}