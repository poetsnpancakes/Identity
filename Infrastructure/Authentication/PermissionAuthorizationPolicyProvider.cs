using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Identity_Infrastructure.Authentication
{
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions>options)
            : base(options)
        {
        }
        /*   public override async Task <AuthorizationPolicy> GetPolicyAsync (string policyName)
           {
               AuthorizationPolicy policy= await base.GetPolicyAsync (policyName);
               if(policy is not null)
               {
                   return policy;
               }
               return new AuthorizationPolicyBuilder()
                   .AddRequirements(new PermissionRequirement(policyName))
                   .Build();
           }*/
        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy policy = await base.GetPolicyAsync(policyName);
            if (policy is not null)
            {
                return policy;
            }

            // Split the policyName into resource and action
            var parts = policyName.Split(':');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Policy name must be in the format 'resource:action'.");
            }

            var resource = parts[0];
            var action = parts[1];

            // Create and return the policy with the required permission
            return new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(resource, action))
                .Build();
        }
    }
}
