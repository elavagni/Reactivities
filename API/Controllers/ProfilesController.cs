using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
  }
}