using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;
using System.Linq;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<ActivityDto>>>
        {
            public ActivityParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _maper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper maper, IUserAccessor userAccessor)
            {
                this._userAccessor = userAccessor;
                _maper = maper;
                _context = context;
            }

            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //This is just an expression tree, we are not querying the database here as we are returning a IQueryable
                var query = _context.Activities
                    .Where(d => d.Date >= request.Params.StartDate)
                    .OrderBy(d => d.Date)
                    .ProjectTo<ActivityDto>(_maper.ConfigurationProvider, new { currentUserName = _userAccessor.GetUserName() })
                    .AsQueryable();

                //Filter the events to the ones the current logged-in user is attending
                if (request.Params.IsGoing && !request.Params.IsHost)
                {

                    query = query.Where(x => x.Attendees.Any(a => a.UserName == _userAccessor.GetUserName()));
                }

                //Filter the events to the ones the current logged-in user is hosting. The IsGoing below corresponds to what the user selected in the filters
                //not the actual DB value.
                if (request.Params.IsHost && !request.Params.IsGoing)
                {
                    query = query.Where(x => x.HostUsename == _userAccessor.GetUserName());
                }

                //Since we are using project, we don't have to map our activities 
                //var activitiesToReturn = _maper.Map<List<ActivityDto>>(activities);

                return Result<PagedList<ActivityDto>>.Success(
                    await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber,
                        request.Params.PageSize)
                );
            }
        }
    }
}