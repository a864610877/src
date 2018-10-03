using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Moonlit.ServiceModel.Authentication;

namespace Ecard.Nurse
{
    public class WcfAuthenticateProvider : IAuthenticateSvc
    {
        private readonly IMembershipService _membershipService;
        private static ICacheManager _cacheManager = CacheFactory.GetCacheManager("auth");

        public WcfAuthenticateProvider(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public AuthenticateToken Authenticate(AuthenticateRequest request)
        {
            var user = _membershipService.GetUserByName(request.Name);
            if (user == null) return null;

            var result = user.State == UserStates.Normal && string.Equals(User.SaltAndHash(request.Password, user.PasswordSalt), user.Password);
            if (result)
            {
                user.LastSignInTime = DateTime.Now;
                _membershipService.UpdateUser(user);
                var userRoles = _membershipService.QueryRoles(new RoleRequest() { UserId = user.UserId }).ToList();
                var permissions = from x in userRoles
                                  from p in x.Permissions.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                  select p;
                var token = new AuthenticateToken { Name = request.Name, Roles = permissions.ToList(), Token = Guid.NewGuid().ToString() };
                _cacheManager.Add(request.Name, token);
                _cacheManager.Add(token.Name, request.Name);
                return token;
            }
            return null;
        }

        public AuthenticateToken GetToken(string token)
        {
            var userName = _cacheManager.GetData(token) as string;
            if (userName == null)
                return null;

            var tokenInstance = _cacheManager.GetData(userName) as AuthenticateToken;
            if (tokenInstance == null)
                return null;

            return tokenInstance;
        }

        public string Key
        {
            get { throw new NotImplementedException(); }
        }
    }
}
