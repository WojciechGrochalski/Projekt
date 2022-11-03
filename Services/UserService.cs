using angularapi.Controllers;
using angularapi.Models;
using angularapi.MyTools;
using AngularApi.DataBase;
using AngularApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace angularapi.Services
{
    public class UserService : IUserService
    {
        private readonly CashDBContext _context;
        private readonly IUserAuthenticate _userAuthenticate;
        private readonly ILogger<BackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IMailService _mailService;


        public UserService(CashDBContext context,
            ILogger<BackgroundService> logger,
            IServiceScopeFactory scopeFactory,
               IMailService mailService,
               IUserAuthenticate userAuthenticate)
        {
            _context = context;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _mailService = mailService;
            _userAuthenticate = userAuthenticate;
        }


        public (UserDBModel, string) AuthenticateLogin(string username, string password)
        {
            if (_userAuthenticate.Authenticate(username, password))
            {
                var User = _context.userDBModels.FirstOrDefault(q => q.Name == username);
                _userAuthenticate.RemoveRefreshTokens(User.ID);
                string jwt = _userAuthenticate.GenerateToken(username, User.ID);
                return (User, jwt);
            }
            return (null, null);
        }
        public UserDBModel CreateAsync(UserDBModel user)
        {
            // validation
            _userAuthenticate.ValidateUser(user);
            var newUser = _userAuthenticate.AddUserToDatabase(user);

            var newMessage = _mailService.GenerateMessageForUserVerification(UserController.ClientBaseUrl, TokenManager.GenerateRegisterToken(newUser.VeryficationToken));
            _mailService.SendMail(newUser.Email, newMessage.Item1, newMessage.Item2);
            BackgroundTask task = new BackgroundTask(_logger, _scopeFactory);
            Task.Factory.StartNew(() => task.RemoveUnverifiedUserAsync(user.VeryficationToken));

            return user;
        }
        public bool VerifyEmail([FromBody]string token)
        {
            try
            {
                string verifyToken = TokenManager.ValidateJwtToken(token);
                if (verifyToken == null)
                {
                    return false;
                }
                var account = _context.userDBModels.SingleOrDefault(x => x.VeryficationToken == verifyToken);

                if (account == null)
                {
                    return false;
                }
                account.IsVerify = true;
                account.VeryficationToken = null;

                _context.userDBModels.Update(account);
                _context.SaveChanges();
                return true;
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(e.Message);
            }

        }
        public bool VerifyPasswordToken(string token)
        {
            try
            {
                string verifyToken = TokenManager.ValidateJwtToken(token);
                if (verifyToken == null)
                {
                    return false;
                }
                var account = _context.userDBModels.SingleOrDefault(x => x.ResetPasswordToken == verifyToken);

                if (account == null)
                {
                    return false;
                }
                account.ResetPasswordToken = null;

                _context.userDBModels.Update(account);
                _context.SaveChanges();
                return true;
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(e.Message);
            }

        }


        public Tokens Refresh(Claim userClaim, Claim refreshClaim)
        {
            var user = _context.userDBModels.FirstOrDefault(s => s.Name == userClaim.Value);
            if (user == null)
            {
                throw new ApplicationException("User doesn't exist");
            }
            RefreshToken token = _context.refreshTokens.FirstOrDefault(s => s.UserID == user.ID && s.Token == refreshClaim.Value);

            if (token != null)
            {

                var newToken = TokenManager.GenerateRefreshToken(user.Name);
                RefreshToken refreshToken = new RefreshToken(newToken.refreshTokenKey, user.ID);

                _context.refreshTokens.Add(refreshToken);

                _context.refreshTokens.Remove(token);
                _context.SaveChanges();

                return new Tokens
                {
                    AccessToken = TokenManager.GenerateAccessToken(user.Name),
                    RefreshToken = newToken.jwt
                };
            }
            else
            {
                throw new ApplicationException("Refresh token incorrect");
            }
        }
    }
}
