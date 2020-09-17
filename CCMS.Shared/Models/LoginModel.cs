using System;
using System.Collections.Generic;
using System.Text;

namespace CCMS.Shared.Models
{
    public class LoginModel
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public int PlatformId { get; set; }
    }
}
