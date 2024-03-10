using BLL.DTOs;
using BLL.Helper;
using BLL.Interfaces;
using DAL.Dto;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
        private readonly IMailService _mailService;
        private readonly IBaseRepository<FamilyPatient> _familyPatient;
       
        private readonly JWT _jwt;
        private readonly Mail _mail;
        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt,
            IOptions<Mail> Mail
            ,IMailService mailService
            ,IBaseRepository<FamilyPatient> familyPatient
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
           _mailService = mailService;
            _mail = Mail.Value;
            _familyPatient = familyPatient;
            _jwt = jwt.Value;

        }
        public async Task<RegisterAuthDto> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new RegisterAuthDto { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new RegisterAuthDto { Message = "Username is already registered!" };

            IdentityResult? result = null;
       
            if (model.Role.Count() > 0)
            {

                        string htmlContent = @"<!DOCTYPE html>
                <html lang=""en"">

                <head>
                    <meta charset=""utf-8"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Email Confirmation</title>
                        <style>
                            body {
                         font-family: 'Arial', sans-serif;
                         background-color: #f8f8f8;
                    margin: 0;
                    padding: 0;
                }

                .container {
                    position: relative;
                    width: 80%;
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #fff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    border: 1px solid #ddd; /* Add border for an elegant frame */
                    overflow: hidden; /* Clear the float */
                }

                h1 {
                    color: #3498db;
                    margin-bottom: 20px;
                }

                p {
                    font-size: 16px;
                    color: #333;
                    margin-bottom: 10px;
                }

                a {
                    text-decoration: none;
                    color: #3498db;
                    font-weight: bold;
                }

                a:hover {
                    text-decoration: underline;
                }

                .button {
                    display: inline-block;
                    padding: 10px 20px;
                    font-size: 14px;
                    font-weight: bold;
                    text-align: center;
                    text-decoration: none;
                    background-color: #3498db;
                    color: #fff;
                    border-radius: 5px;
                }

                .footer {
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                }

                /* Emotes on the side */
                .emotes {
                    position: absolute;
                    top: 0;
                    right: 0;
                    padding: 10px;
                }

                .emotes img {
                    width: 30px;
                    height: 30px;
                    margin-left: 10px;
                }
            </style>
        </head>

        <body>
            <div class=""container"">
                <div class=""emotes"">
                    <img src=""https://drive.google.com/uc?export=view&id=1j1q86kEhug5VC18WGIrL1U_m9vlPAjxU"" alt=""Congratulations Emote 1"">
                    <img src=""https://drive.google.com/uc?export=view&id=1dn-moFQyJ_hlehjv4-9Mc5I6L6VhAe7Q"" alt=""Congratulations Emote 2"">
                </div>
                <h1>Welcome to the Electronic Mind of Alzheimer Patient</h1>
                <p>Dear {FullName},</p>
                <p>We are thrilled to have you on board! To ensure the security of your account, please confirm your email address by clicking the link below:</p>
                <p><a class=""button"" href='{url}'>Confirm Your Email</a></p>
                <p>If you did not create an account or need further assistance, please disregard this email.</p>
                <div class=""footer"">
                    <p>Best regards,</p>
                    <p>The Electronic Mind Team</p>
                </div>
            </div>
        </body>

        </html>


        ";
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
                                return new RegisterAuthDto { Message = errors };
                            }
                            await _userManager.AddToRoleAsync(family, "Family");
                            await _userManager.UpdateAsync(family);
                            
                            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(family);
                            
                            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                    
                            string url = $"{_mail.ServerLink}/api/Authentication/confirmemail?userid={family.Id}&token={validEmailToken}";
                           htmlContent = htmlContent.Replace("{FullName}", family.FullName).Replace("{url}", url);
                            await _mailService.SendEmailAsync(family.Email, _mail.FromMail, _mail.Password,"Confirm your email", htmlContent);

                   
                }
                else if (model.Role.FirstOrDefault().ToLower() == "patient")
                {
                        
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

                            return new RegisterAuthDto { Message = errors };
                        }
                        await _userManager.AddToRoleAsync(caregiver, "caregiver");
                        var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(caregiver);
                    
                        var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                        var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                        string url = $"{_mail.ServerLink}/api/Authentication/confirmemail?userid={caregiver.Id}&token={validEmailToken}";
                    htmlContent = htmlContent.Replace("{FullName}", caregiver.FullName).Replace("{url}", url);
                    await _mailService.SendEmailAsync(caregiver.Email, _mail.FromMail, _mail.Password, "Confirm your email", htmlContent);
            
                }
                    else
                    return new RegisterAuthDto { Message = "Invalid Role" };



            }
            else return new RegisterAuthDto { Message = "Error,You need to add role" };

            

            

            return new RegisterAuthDto
            {
                Message = $"User Created Successfully",               
                NeedToConfirm = true ,
                
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
            if (user.EmailConfirmed == false)
            {
                AuthDto.Message = "This User Need To Confirm Before Login ";
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
        public async Task<EmailConfirmation> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new EmailConfirmation
                {
                    IsConfirm = false,
                    Message = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new EmailConfirmation
                {
                    Message = "Email confirmed successfully!",
                    IsConfirm = true,
                };

            return new EmailConfirmation
            {
                IsConfirm = false,
                Message = "Email did not confirm",
            };
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

        //forget password
        public async Task<ForgetPassword> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ForgetPassword
                {
                    IsEmailSent = false,
                    Message = "No user associated with email",
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);
            string htmlContent = @"<!DOCTYPE html>
                    <html lang=""en"">

                    <head>
                        <meta charset=""utf-8"" />
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Password Reset</title>
                        <style>
                            body {
                                font-family: 'Arial', sans-serif;
                                background-color: #f8f8f8;
                                margin: 0;
                                padding: 0;
                            }

                            .container {
                                position: relative;
                                width: 80%;
                                max-width: 600px;
                                margin: 20px auto;
                                background-color: #fff;
                                padding: 20px;
                                border-radius: 8px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                border: 1px solid #ddd;
                                /* Add border for an elegant frame */
                                overflow: hidden;
                                /* Clear the float */
                            }

                            h1 {
                                color: #3498db;
                                margin-bottom: 20px;
                            }

                            p {
                                font-size: 16px;
                                color: #333;
                                margin-bottom: 10px;
                            }

                            a {
                                text-decoration: none;
                                color: #3498db;
                                font-weight: bold;
                            }

                            a:hover {
                                text-decoration: underline;
                            }

                            .button {
                                display: inline-block;
                                padding: 10px 20px;
                                font-size: 14px;
                                font-weight: bold;
                                text-align: center;
                                text-decoration: none;
                                background-color: #3498db;
                                color: #fff;
                                border-radius: 5px;
                            }

                            .footer {
                                margin-top: 20px;
                                font-size: 12px;
                                color: #777;
                            }

                            /* Emotes on the side */
                            .emotes {
                                position: absolute;
                                top: 0;
                                right: 0;
                                padding: 10px;
                            }

                            .emotes img {
                                width: 30px;
                                height: 30px;
                                margin-left: 10px;
                            }
                        </style>
                    </head>

                    <body>
                        <div class=""container"">

                            <h1>Password Reset</h1>
                            <p>Dear {FullName},</p>
                            <p>We've received a request to reset your password. If you didn't make this request, you can safely ignore this email.</p>
                            <p>To reset your password, please click the link below:</p>
                            <p><a class=""button"" href='{url}'>Reset Password</a></p>
                            <p>If you need further assistance, please don't hesitate to contact us.</p>
                            <div class=""footer"">
                                <p>Best regards,</p>
                                <p>The Electronic Mind Team</p>
                            </div>
                        </div>
                    </body>

                    </html>


            ";


            string url = $"{_mail.ServerLink}/ResetPassword?email={email}&token={validToken}";
            
            htmlContent = htmlContent.Replace("{FullName}", email).Replace("{url}", url);
            await _mailService.SendEmailAsync(email, _mail.FromMail, _mail.Password, "forget password email", htmlContent);

            return new ForgetPassword
            {
                IsEmailSent = true,
                Message = "Reset password URL has been sent to the email successfully!"
            };

        }
        //reset password 
        public async Task<ResetPassword> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ResetPassword
                {
                    IsPasswordReset = false,
                    Message = "No user associated with email",
                };
            }
            if (model.NewPassWord != model.ConfirmPassword)
            {
                return new ResetPassword
                {
                    IsPasswordReset = false,
                    Message = "Password doesn't match its confirmation",
                };
            }
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var resetResult = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassWord);

            if (resetResult.Succeeded)
            {
                return new ResetPassword
                {
                    Message = "Password has been reset successfully!",
                    IsPasswordReset = true,
                };
            }
            return new ResetPassword
            {
                Message = "Something went wrong",
                IsPasswordReset = false,

            };
        }
    }
}
