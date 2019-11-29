using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PandaDoc.Api
{
    public class AppUser
    {
        public AppUser()
        {
            Id = "12erf0";
            Username = "test";
            Password = "1234";
            ClientId = "ipd";
        }
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
    }
}