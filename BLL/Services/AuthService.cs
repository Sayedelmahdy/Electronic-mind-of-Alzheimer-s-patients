using BLL.DTOs;
using BLL.Helper;
using BLL.Interfaces;
using DAL.Dto;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBaseRepository<FamilyPatient> _familyPatient;
       
        private readonly JWT _jwt;
        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt,
            IBaseRepository<FamilyPatient> familyPatient
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _familyPatient = familyPatient;
            _jwt = jwt.Value;

        }
        public async Task<AuthDto> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthDto { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthDto { Message = "Username is already registered!" };

            IdentityResult? result = null;
            JwtSecurityToken? jwtSecurityToken=null;
            RefreshToken? refreshToken = null;
            if (model.Role.Count() > 0)
            {
               
                    if (model.Role.FirstOrDefault().ToLower() == "family")
                    {
                        Family family = new Family
                        {
                            UserName=model.Username,
                            Email=model.Email,
                            PhoneNumber=model.PhoneNumber,
                            FullName=model.FullName,
                            BirthDate=model.BirthDate,
                        };
                   
                             result = await _userManager.CreateAsync(family, model.Password);
                            if (result == null || !result.Succeeded)
                            {
                                var errors = string.Empty;
                                foreach (var error in result.Errors)
                                    errors += $"{error.Description},";
                                return new AuthDto { Message = errors };
                            }
                            await _userManager.AddToRoleAsync(family, "Family");
                             jwtSecurityToken = await CreateJwtToken(family);
                             refreshToken = GenerateRefreshToken();
                            family.RefreshTokens?.Add(refreshToken);
                            await _userManager.UpdateAsync(family);
                    }
                    else if (model.Role.FirstOrDefault().ToLower() == "patient")
                    {
                             /*
                        Patient patient = new Patient
                        {
                            UserName = model.Username,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            FullName = model.FullName,

                        };
                        result = await _userManager.CreateAsync(patient, model.Password);
                        if (result == null || !result.Succeeded)
                        {
                            var errors = string.Empty;

                            foreach (var error in result.Errors)
                                errors += $"{error.Description},";

                            return new AuthDto { Message = errors };
                        }
                        await _userManager.AddToRoleAsync(patient, "Family");

                         jwtSecurityToken = await CreateJwtToken(patient);

                         refreshToken = GenerateRefreshToken();
                        patient.RefreshTokens?.Add(refreshToken);
                        await _userManager.UpdateAsync(patient);*/

                   /* return new AuthDto { Message = "You need to add patient from family dashboard" };*/
                }
                    else if (model.Role.FirstOrDefault().ToLower() == "caregiver")
                    {
                        Caregiver caregiver = new Caregiver
                        {
                            UserName = model.Username,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            FullName = model.FullName,
                            BirthDate = model.BirthDate,

                        };
                        result = await _userManager.CreateAsync(caregiver, model.Password);
                        if (result == null || !result.Succeeded)
                        {
                            var errors = string.Empty;

                            foreach (var error in result.Errors)
                                errors += $"{error.Description},";

                            return new AuthDto { Message = errors };
                        }
                        await _userManager.AddToRoleAsync(caregiver, "Family");
                        jwtSecurityToken = await CreateJwtToken(caregiver);
                        refreshToken = GenerateRefreshToken();
                        caregiver.RefreshTokens?.Add(refreshToken);
                        await _userManager.UpdateAsync(caregiver);
                    }
                    else
                    return new AuthDto { Message = "Invalid Role" };



            }
            else return new AuthDto { Message = "Error,You need to add role" };

            

            

            return new AuthDto
            {
                Message = $"User Created Successfully",               
                IsAuthenticated = true,             
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),           
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn,
                
            };


            
        }


        // login
        public async Task<AuthDto> GetTokenAsync(TokenRequestDto model)
        {
            var AuthDto = new AuthDto();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                AuthDto.Message = "Email or Password is incorrect!";
                return AuthDto;
            }
            if (user.LockoutEnabled == true && user.LockoutEnd > DateTime.Now)
            {
                AuthDto.Message = "This User is Banned";
                return AuthDto;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);
            
            AuthDto.IsAuthenticated = true;
            AuthDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var NewRefreshToken = GenerateRefreshToken();
            AuthDto.RefreshToken = NewRefreshToken.Token;
            AuthDto.RefreshTokenExpiration = NewRefreshToken.ExpiresOn;
            user.RefreshTokens.Add(NewRefreshToken);
            await _userManager.UpdateAsync(user);

            return AuthDto;
        }


        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FullName",user.FullName),
                new Claim("PhoneNumber",user.PhoneNumber),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        private RefreshToken GenerateRefreshToken()
        {
            var Random = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(Random);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(Random),
                ExpiresOn = DateTime.Now.AddDays(7),//numofdays
                CreatedOn = DateTime.Now
            };
        }
        public async Task<AuthDto> RefreshTokenAsync(string Token)
        {

            var AuthDto = new AuthDto();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == Token));

            if (user == null)
            {
                AuthDto.Message = "Invalid token / Need To Login First";
                return AuthDto;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == Token);

            if (!refreshToken.IsActive)
            {
                AuthDto.Message = "Inactive token / Token Expaired You Need To login";
                return AuthDto;
            }

            refreshToken.RevokedOn = DateTime.Now;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateAsync(user);

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            AuthDto.IsAuthenticated = true;
            AuthDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var NewRefreshToken = GenerateRefreshToken();
            AuthDto.RefreshToken = NewRefreshToken.Token;
            AuthDto.RefreshTokenExpiration = NewRefreshToken.ExpiresOn;
            user.RefreshTokens.Add(NewRefreshToken);
            await _userManager.UpdateAsync(user);
            return AuthDto;
        }
       //test
        public async Task<AuthDto> AddPatients( RegisterDto model, string username, string Relationility,DateTime DiagnosisDate)
        {
            Patient patient = new Patient
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                DiagnosisDate = DiagnosisDate
                

            };
           var result = await _userManager.CreateAsync(patient, model.Password);
            if (result == null || !result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthDto { Message = errors };
            }
            await _userManager.AddToRoleAsync(patient, "Patient"); 
            var family = await _userManager.FindByNameAsync(username);
            FamilyPatient familyPatient = new FamilyPatient
            {
               
                Family = (Family)family,
                Patient = patient,
                Relationility = Relationility,  

            };
            _familyPatient.Add(familyPatient);
            return new AuthDto { IsAuthenticated = true ,Message="Done",Token=null,RefreshToken=null};


        }
        //Logout
        public async Task<bool> RevokeTokenAsync(string Token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == Token));
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == Token);

            if (!refreshToken.IsActive) return false;
            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
            return true;
        }
    }
}
