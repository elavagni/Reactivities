using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<ActivityDto>>> { }

        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {
            private readonly DataContext _context;
    private readonly IMapper _maper;
            public Handler(DataContext context, IMapper maper)
            {
                _maper = maper;
                _context = context;
            }

            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = await _context.Activities
                    // .Include(a => a.Attendees)
                    // .ThenInclude(u => u.AppUser)
                    //Instead of using include, we can project the query to make the sql query cleaner
                    .ProjectTo<ActivityDto>(_maper.ConfigurationProvider)                  
                    .ToListAsync(cancellationToken);

                //Since we are using project, we don't have to map our activities 
                //var activitiesToReturn = _maper.Map<List<ActivityDto>>(activities);

                return  Result<List<ActivityDto>>.Success(activities);
            }
        }
    }
}