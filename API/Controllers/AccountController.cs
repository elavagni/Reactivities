using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  //For simplicity user based operationS will not implement CQRS

  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    public AccountController(UserManager<AppUser> userManager, 
      SignInManager<AppUser> signInManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
    }


    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
    {
      var user = await _userManager.FindByEmailAsync(loginDto.Email);

      if (user == null) return Unauthorized();

      var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

      if (result.Succeeded) 
      {
        return new UserDto 
        {
          DisplayName  =user.DisplayName, 
          Image = null, 
          Token= "This will be a token",
          UserName = user.UserName
        };
      }
      return Unauthorized();      
    }


  }
}