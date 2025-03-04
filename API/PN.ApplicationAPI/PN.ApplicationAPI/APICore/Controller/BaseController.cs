using PN.ApplicationAPI.APICore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PN.ApplicationAPI.APICore.Controller
{
    public class BaseController : ApiController
    {
        internal PNResponse ResponseSuccessed(string message)
        {

            return new PNResponse((int)PNResponseCode.Succeed, message);
        }
        internal PNResponse ResponseFaild(string message)
        {
            return new PNResponse((int)PNResponseCode.Failed, message);
        }
    }
}