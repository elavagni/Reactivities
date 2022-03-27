using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ListActivities
    {
        public class Query : IRequest<Result<List<UserActivityDto>>>
        {
            public string UserName { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {

            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {

                var activitiesQuery = _context.ActivityAttendees
                                        .Where(x => x.AppUser.UserName == request.UserName)
                                        .OrderBy(a => a.Activity.Date)
                                        .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                                        .AsQueryable();

                activitiesQuery = request.Predicate switch
                {
                    "past" => activitiesQuery.Where(x => x.Date <= DateTime.Now),
                    "hosting" => activitiesQuery.Where(x => x.HostUserName == request.UserName),
                    _ => activitiesQuery.Where(x => x.Date >= DateTime.Now)
                };
                var activities = await activitiesQuery.ToListAsync();

                return Result<List<UserActivityDto>>.Success(activities);
            }
        }
    }
}