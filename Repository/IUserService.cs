using angularapi.Models;
using System.Security.Claims;

namespace AngularApi.Repository
{
    public interface IUserService
    {
        (UserDBModel, string) AuthenticateLogin(string username, string password);

        UserDBModel CreateAsync(UserDBModel user);

        bool VerifyEmail(string token);

        bool VerifyPasswordToken(string token);

        Tokens Refresh(Claim username, Claim refreshtoken);
    }
}
