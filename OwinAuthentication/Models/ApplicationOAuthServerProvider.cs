

using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;

namespace OwinAuthentication.Models
{
    public class ApplicationOAuthServerProvider
        : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.FromResult(context.Validated());
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
           // return base.GrantResourceOwnerCredentials(context);
           if(context.Password != "password")
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                context.Rejected();
                return;
            }

           // Create or retrive a ClaimsIdentity to represent the
           // Authenticated user:
            ClaimsIdentity identity =
                 new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("user_name", context.UserName));

            // Add a Role Claim:
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            context.Validated(identity);
        }
    }
}
