using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class List
    {
        public class Query : IRequest<Result<List<Profiles.Profile>>>
        {
            public string Predicate { get; set; }
            public string UserName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                this._userAccessor = userAccessor;
                this._mapper = mapper;
                this._context = context;

            }

            public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var profiles = new List<Profiles.Profile>();

                switch (request.Predicate)
                {
                    case "followers":
                        profiles = await _context.UserFollowings.Where(x => x.Target.UserName == request.UserName)
                            .Select(u => u.Observer)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider,
                                new { currentUserName = _userAccessor.GetUserName() })
                            .ToListAsync();
                        break;
                    case "following":
                        profiles = await _context.UserFollowings.Where(x => x.Observer.UserName == request.UserName)
                            .Select(u => u.Target)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider,
                               new { currentUserName = _userAccessor.GetUserName() })
                            .ToListAsync();
                        break;
                }

                return Result<List<Profiles.Profile>>.Success(profiles);


            }
        }
    }
}