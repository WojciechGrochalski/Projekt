using angularapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularApi.Repository
{
    public interface IUserAuthenticate
    {
        bool Authenticate(string username, string password);
        void RemoveRefreshTokens(int userID);
        string GenerateToken(string username, int userID);
        void  ValidateUser(UserDBModel user);
        UserDBModel AddUserToDatabase(UserDBModel user);

    }
}
