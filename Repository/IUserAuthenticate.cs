using angularapi.Models;

namespace AngularApi.Repository
{
    public interface IUserAuthenticate
    {
        bool Authenticate(string username, string password);
        void RemoveRefreshTokens(int userID);
        string GenerateToken(string username, int userID);
        void ValidateUser(UserDBModel user);
        UserDBModel AddUserToDatabase(UserDBModel user);

    }
}
