using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCASGradePortal.Core.Interfaces
{
    public interface IAuthService
    {
        Task<GeneralServiceResponse> RegisterAsync(string username, string password);
        Task<LoginServiceResponse> LoginAsync(string username, string password);

    }
}
