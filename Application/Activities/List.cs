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
            public PagingParams Params { get; set; }
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
                var query = _context.Activities
                    .OrderBy(d => d.Date)
                    .ProjectTo<ActivityDto>(_maper.ConfigurationProvider, new { currentUserName = _userAccessor.GetUserName() })
                    .AsQueryable();

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