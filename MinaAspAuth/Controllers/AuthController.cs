﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MinaSignerNet;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;

namespace MinaAspAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthController> _logger;
        private const string message = "Welcome to the mina asp auth, sign this message to authenticate {0}";
        private const string cacheConnectionKey = "GetConnection_{0}";

        public AuthController(ILogger<AuthController> logger, IConfiguration config, IMemoryCache cache)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
        }

        [AllowAnonymous]
        [HttpGet("RequestConnection")]
        public MessageVM RequestConnection(string account)
        {
            var connectVm = RequestEntry(account);

            return new MessageVM { Account = connectVm.Account, Message = string.Format(message, connectVm.Nonce) };
        }

        [AllowAnonymous]
        [HttpPost("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginVM login)
        {
            var user = await Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user.Account.ToLower());
                return Ok(new TokenVM() { Token = tokenString });
            }

            return Unauthorized();
        }

        private ConnectionVM CheckEntry(string account)
        {
            return GetCacheConnection(account);
        }

        private ConnectionVM RequestEntry(string account)
        {
            return GetCacheConnection(account);
        }

        private ConnectionVM GetCacheConnection(string account)
        {
            var key = string.Format(cacheConnectionKey, account);
            return _cache.GetOrCreate(key, (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3);
                var test = new ConnectionVM();
                return new ConnectionVM { Account = account, DateTime = DateTime.Now, Nonce = Guid.NewGuid() };
            });
        }

        private void ResetCache(string account)
        {
            var key = string.Format(cacheConnectionKey, account);
            _cache.Remove(cacheConnectionKey);
        }

        private string BuildToken(string account)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Name, account),
            new Claim(JwtRegisteredClaimNames.Email, account),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(JwtRegisteredClaimNames.Iss, _config["Jwt:Issuer"]),
              new Claim(JwtRegisteredClaimNames.Aud, _config["Jwt:Audience"]),
              new Claim(ClaimTypes.Name,account),
                  new Claim(ClaimTypes.NameIdentifier,account),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
             claims: claims,
             issuer: _config["Jwt:Issuer"],
             audience: _config["Jwt:Audience"],
              expires: DateTime.Now.AddMinutes(90),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserVM> Authenticate(LoginVM login)
        {
            // TODO: This method will authenticate the user recovering his Ethereum address through underlaying offline ecrecover method.

            var connectVM = CheckEntry(login.Signer);
            var messageVM = string.Format(message, connectVM.Nonce);

            // delete from cache to revoke reuse
            ResetCache(login.Signer);

            if (messageVM != login.Message)
            {
                _cache.Remove(cacheConnectionKey);
                throw new Exception("Authentification expired retry");
            }


            var messageToVerify = string.Format(message, connectVM.Nonce);
            UserVM user = null;

            var signature = new Signature()
            {
                R = BigInteger.Parse(login.Field),
                S = BigInteger.Parse(login.Scalar)
            };
            var validSignature = Signature.Verify(signature, messageToVerify, login.Signer, Network.Testnet);

            if (validSignature)
            {
                // read user from DB or create a new one
                // for now we fake a new user
                user = new UserVM { Account = login.Signer, Name = string.Empty, Email = string.Empty };
            }
            else
            {
                throw new Exception("Impossible to valid your auth, signature incorrect");
            }

            return user;
        }
    }
}
