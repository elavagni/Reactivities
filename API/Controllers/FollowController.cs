using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Followers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FollowController : BaseApiController
    {
        [HttpPost("{username}")]
        public async Task<IActionResult> Follow(string userName)
        {
            return HandleResult(await Mediator.Send(new FollowToggle.Command { TargetUserName = userName }));
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetFollowings(string userName, string predicate)
        {
            return HandleResult(await Mediator.Send(new List.Query { UserName = userName, Predicate = predicate }));
        }
    }
}