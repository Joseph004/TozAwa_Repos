using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tozawa.Client.Portal.HttpClients;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Models.FormModels;

namespace Tozawa.Client.Portal.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    public UserController()
    {
    }

    
    [HttpGet]
    [Route("get")]
    public UserDto GetUser()
    {
        // Instantiate a UserDto
        var userDto = new UserDto
        {
            UserName = "[]",
            IsAuthenticated = false
        };
        // Detect if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            // Set the username of the authenticated user
            userDto.UserName =
                User.Identity.Name;
            userDto.IsAuthenticated =
                User.Identity.IsAuthenticated;
        };
        return userDto;
    }
}

}

