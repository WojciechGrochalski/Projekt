using angularapi.Models;
using angularapi.MyTools;
using AngularApi.DataBase;
using AngularApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace angularapi.Services
{
    public class UserAuthenticateService : IUserAuthenticate
    {

        private readonly CashDBContext _context;
        public UserAuthenticateService(CashDBContext context)
        {
            _context = context;
        }
        public bool Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var user = _context.userDBModels.SingleOrDefault(x => x.Name == username);

            // check if username exists
            if (user == null)
            {
                return false;
            }

            // check if password is correct
            if (!SecurePasswordHasher.Verify(password, user.Pass))
            {
                return false;
            }
            return true;
        }
        public void RemoveRefreshTokens(int userID)
        {
            var rows = _context.refreshTokens.Where(x => x.UserID == userID).ToList();
            foreach (RefreshToken item in rows)
            {
                _context.refreshTokens.Remove(item);
            }
            _context.SaveChanges();
        }
        public  string GenerateToken(string username, int userID)
        {
            var newToken = TokenManager.GenerateRefreshToken(username);
            RefreshToken refreshToken = new RefreshToken(newToken.refreshTokenKey, userID);
            _context.refreshTokens.Add(refreshToken);
            _context.SaveChanges();
            return newToken.jwt;

        }

        public void ValidateUser(UserDBModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Pass))
            {
                throw new ApplicationException("Password is required");
            }

            if (_context.userDBModels.Any(x => x.Name == user.Name))
            {
                throw new ApplicationException("Username " + user.Name + " is already taken");
            }

            if (_context.userDBModels.Any(x => x.Email == user.Email))
            {
                throw new ApplicationException("Email " + user.Email + " is already taken");
            }
        }

        public UserDBModel AddUserToDatabase(UserDBModel user)
        {
            string PasswordHash = SecurePasswordHasher.Hash(user.Pass);
            user.Pass = PasswordHash;
            user.Created = DateTime.Now;
            user.IsVerify = false;
            user.VeryficationToken = TokenManager.RandomTokenString();
            _context.userDBModels.Add(user);
            _context.SaveChanges();

            return user;
        }
    }
}
