using System;
using System.Security.Claims;
using ItHappened.App.Authentication;
using ItHappened.App.Model;
using ItHappened.Application;
using ItHappened.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ItHappened.App.Controllers
{
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtIssuer _jwtIssuer;
        public UserController(IUserService userService, IJwtIssuer jwtIssuer)
        {
            _userService = userService;
            _jwtIssuer = jwtIssuer;
        }

        [Authorize]
        [HttpGet]
        [Route("self")]
        public IActionResult GetUser()
        {
            var actorId = Guid.Parse(User.FindFirstValue(JwtClaimTypes.Id));
            var optionUser = _userService.GetUserById(actorId,actorId);
            return optionUser.Match<IActionResult>(
                Some: user =>
                {
                    var userGetResponse = new UserGetResponse(user);
                    return Ok(userGetResponse);
                }, 
                None: NotFound(new
                {
                    errors = new
                    {
                        commonError = "User not found."
                    }
                }));
        }
        
        [HttpPost]
        [Route("signin")]
        public IActionResult LoginUser([FromBody] LoginRequest loginRequest)
        {
            var optionUser = _userService.LogInByCredentials(loginRequest.Username, loginRequest.Password);
            return optionUser.Match<IActionResult>(
                Some: user =>
                {
                    var token = _jwtIssuer.GenerateToken(user);
                    var userCreateResponse = new UserCreateResponse(token);
                    return Ok(userCreateResponse);
                }, 
                None: Ok(new
                {
                    errors = new
                    {
                        passwordError = "Invalid password.",
                        commonError = "There are errors with some fields bellow."
                    }
                }));
        }
        
        [HttpPost]
        [Route("signup")]
        public IActionResult CreateUser([FromBody] UserCreateRequest userCreateRequest)
        {
            var userForm = new UserForm(userCreateRequest.Username, userCreateRequest.Password);
            _userService.CreateUser(userForm);
            
            var optionUser = _userService.LogInByCredentials(userForm.Username, userForm.Password);

            return optionUser.Match<IActionResult>(
                Some: user =>
                {
                    var token = _jwtIssuer.GenerateToken(user);
                    var userCreateResponse = new UserCreateResponse(token);
                    return Ok(userCreateResponse);
                }, 
                None: Ok(new
                {
                    errors = new
                    {
                        usernameError = "Username already taken.",
                        commonError = "There are errors with some fields bellow."
                    }
                }));
        }
        
        [Authorize]
        [HttpPut]
        [Route("self")]
        public IActionResult UpdateUser([FromBody] UserCreateRequest userUpdateRequest)
        {
            var actorId = Guid.Parse(User.FindFirstValue(JwtClaimTypes.Id));
            var optionUser = _userService.GetUserById(actorId,actorId);
            return optionUser.Match<IActionResult>(
                Some: user =>
                {
                    var userForm = new UserForm(userUpdateRequest.Username, userUpdateRequest.Password);
                    _userService.EditUser(actorId, actorId, userForm);
                    return NoContent();
                }, 
                None: Ok(new
                {
                    errors = new
                    {
                        usernameError = "Username already taken.",
                        commonError = "There are errors with some fields bellow."
                    }
                }));
        }
        
        [Authorize]
        [HttpDelete]
        [Route("self")]
        public IActionResult UserDelete()
        {
            var actorId = Guid.Parse(User.FindFirstValue(JwtClaimTypes.Id));
            var optionUser = _userService.GetUserById(actorId,actorId);
            return optionUser.Match<IActionResult>(
                Some: user =>
                {
                    _userService.DeleteUser(actorId, actorId);
                    return NoContent();
                }, 
                None: Ok(new
                {
                    errors = new
                    {
                        commonError = "Internal error."
                    }
                }));
        }
        
    }
}