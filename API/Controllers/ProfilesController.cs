using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseApiController
    {
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetProfile(string userName)
        {
            return HandleResult(await Mediator.Send(new Details.Query { UserName = userName }));
        }

        [HttpGet("{userName}/activities")]
        public async Task<IActionResult> GetActivitiesByUser(string userName, string predicate)
        {
            return HandleResult(await Mediator.Send(new ListActivities.Query
            { UserName = userName, Predicate = predicate }));
        }
    }
}