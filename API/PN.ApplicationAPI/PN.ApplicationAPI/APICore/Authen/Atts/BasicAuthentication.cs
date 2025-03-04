using PN.SmartLib.Helper;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace APICore.Authen.Atts
{
    public class BasicAuthentication : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null)
            {
                var authToken = actionContext.Request.Headers.Authorization.Parameter;

                // Decoding authToken we get decode value in 'Username:Password' format
                var decodeauthToken = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(authToken));

                // Splitting decodeauthToken using ':'
                var arrUserNameandPassword = decodeauthToken.Split(':');

                // At 0th position of array we get username and at 1st we get password
                if (IsAuthorizedUser(arrUserNameandPassword[0], arrUserNameandPassword[1]))
                {
                    // Setting current principle
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(arrUserNameandPassword[0]), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
            }
        }
        public static bool IsAuthorizedUser(string Username, string Password)
        {
            // In this method we can handle our database logic here...
            return Username == ConfigurationManager.AppSettings["ApiUser"]
                && Password == StringUtils.DecryptString(ConfigurationManager.AppSettings["LogApi"]);
        }
    }
}
