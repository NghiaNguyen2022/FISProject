using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PN.ApplicationAPI.APICore.Models
{
    public enum PNResponseCode
    {
        Failed = -1,
        Succeed = 0,
        Pending = 1
    }
    

    public class BaseResponse
    {
        public int returncode { get; set; }
        public string description { get; set; }
        public BaseResponse(int code, string message)
        {
            returncode = code;
            description = message;
        }
    }
    public class PNResponse : BaseResponse
    {
        public PNResponse(int code, string message) : base(code, message)
        {
        } 
    }
}