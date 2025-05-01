using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Entities.Dto
{
    public class LoginServiceResponse
    {
        public string? NewToken { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
    }
}
