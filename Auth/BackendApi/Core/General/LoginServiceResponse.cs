using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendApi.Core.General
{
    public class LoginServiceResponse
    {
        public string? NewToken { get; set; }
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public string? Role { get; set; }

        //public string? Role { get; set; }
    }
}
