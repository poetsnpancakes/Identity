using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Net.Http.Json;
using Identity_Infrastructure.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;


namespace Identity_Infrastructure.Authentication
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Extract the JWT token from the current HttpContext (the request to the main API)
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Fail();
                return;
            }

            // Log the token for debugging purposes
           // Console.WriteLine($"Extracted token: {token}");

            // Call me-API to fetch permissions for this user
            var client = _httpClientFactory.CreateClient("PermissionApi");

            // Create the request and add the Authorization header with the token
            var request = new HttpRequestMessage(HttpMethod.Get, $"me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // Add the token to the header

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Log the failure for debugging purposes
                Console.WriteLine($"Failed to authenticate with me-API. Status code: {response.StatusCode}");
                context.Fail();
                return;
            }

            var userPermissions = await response.Content.ReadFromJsonAsync<UserPermissionResponse>();

            /*   if (userPermissions?.Permissions.Contains(requirement.Permission) == true)
               {
                   context.Succeed(requirement);  // User has the required permission
               }
               else
               {
                   context.Fail();  // User doesn't have the required permission
               }*/
            bool hasPermission = userPermissions.Permissions
              .Any(permission =>
                  permission.Resource.Equals(requirement.Resource, StringComparison.OrdinalIgnoreCase) &&
                  permission.Actions.Any(action => action.Type.Equals(requirement.Action, StringComparison.OrdinalIgnoreCase)));

        }
    }
    

    /* public class PermissionAuthorizationHandler: AuthorizationHandler<PermissionRequirement>
     {
         protected override Task HandleRequirementAsync(
             AuthorizationHandlerContext context,
             PermissionRequirement requirement)
         {
             var permissions = context
                 .User
                 .Claims
                 .Where(x => x.Type == CustomClaims.Permissions)
                 .Select(x => x.Value)
                 .ToHashSet();

             if (permissions.Contains(requirement.Permission))
             {
                 context.Succeed(requirement);
             }

             return Task.CompletedTask;
         }
     }*/

}
