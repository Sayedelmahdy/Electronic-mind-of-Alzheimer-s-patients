using BLL.DTOs.AuthenticationDto;
using BLL.Helper;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _env;
        private readonly IBaseRepository<Patient> _patient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly JWT _jwt;
        private readonly Mail _mail;
        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt,
            IOptions<Mail> Mail
            , IMailService mailService
            , IWebHostEnvironment env
            , IBaseRepository<Patient> patient,
            IWebHostEnvironment webHostEnvironment

            )
        {
            _patient = patient;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
            _mailService = mailService;
            _mail = Mail.Value;
            _env = env;
            _jwt = jwt.Value;

        }
        public async Task<RegisterAuthDto> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new RegisterAuthDto { Message = "Email is already registered!" };


            IdentityResult? result = null;

            if (model.Role != string.Empty)
            {
                string htmlFilePath = $"{_webHostEnvironment.WebRootPath}/EmailTemplates/RegisterEmailTemplete.html";
                string htmlContent = System.IO.File.ReadAllText(htmlFilePath);
                if (model.Role.ToLower() == "family")
                {
                    Family family = new Family
                    {

                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        FullName = model.FullName,
                        Age = model.Age,
                        UserName = model.Email.Split('@')[0],
                        MainLatitude = model.MainLatitude,
                        MainLongitude = model.MainLongitude,
                    };

                    result = await _userManager.CreateAsync(family, model.Password);
                    if (result == null || !result.Succeeded)
                    {
                        var errors = string.Empty;
                        foreach (var error in result.Errors)
                            errors += $"{error.Description},";
                        return new RegisterAuthDto { Message = errors };
                    }
                    await _userManager.AddToRoleAsync(family, "family");
                    string MediaId = Guid.NewGuid().ToString();

                    string filePath = Path.Combine("User Avatar", $"{family.Id}_{MediaId}{Path.GetExtension(model.Avatar.FileName)}");
                    string directoryPath = Path.Combine(_env.WebRootPath, "User Avatar");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    using (FileStream filestream = File.Create(Path.Combine(_env.WebRootPath, filePath)))
                    {
                        model.Avatar.CopyTo(filestream);
                        filestream.Flush();
                    }
                    family.imageUrl = filePath;
                    await _userManager.UpdateAsync(family);

                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(family);

                    var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                    var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                    string url = $"{_mail.ServerLink}/api/Authentication/confirmemail?userid={family.Id}&token={validEmailToken}";
                    htmlContent = htmlContent.Replace("{FullName}", family.FullName).Replace("{url}", url);
                    //await _mailService.SendEmailAsync(family.Email, _mail.FromMail, _mail.Password,"Confirm your email", htmlContent);
                    //Todo: send email when project is ready

                }

                else if (model.Role.ToLower() == "caregiver")
                {
                    Caregiver caregiver = new Caregiver
                    {

                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        FullName = model.FullName,
                        Age = model.Age,
                        UserName = model.Email.Split('@')[0],
                        MainLatitude = model.MainLatitude,
                        MainLongitude = model.MainLongitude,

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
                    string MediaId = Guid.NewGuid().ToString();

                    string filePath = Path.Combine("User Avatar", $"{caregiver.Id}_{MediaId}{Path.GetExtension(model.Avatar.FileName)}");
                    string directoryPath = Path.Combine(_env.WebRootPath, "User Avatar");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    using (FileStream filestream = File.Create(Path.Combine(_env.WebRootPath, filePath)))
                    {
                        model.Avatar.CopyTo(filestream);
                        filestream.Flush();
                    }
                    caregiver.imageUrl = filePath;
                    await _userManager.UpdateAsync(caregiver);
                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(caregiver);

                    var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                    var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                    string url = $"{_mail.ServerLink}/api/Authentication/confirmemail?userid={caregiver.Id}&token={validEmailToken}";
                    htmlContent = htmlContent.Replace("{FullName}", caregiver.FullName).Replace("{url}", url);
                    //  await _mailService.SendEmailAsync(caregiver.Email, _mail.FromMail, _mail.Password, "Confirm your email", htmlContent);
                    //Todo: send email when project is ready
                }
                else
                    return new RegisterAuthDto { Message = "Invalid Role" };



            }
            else return new RegisterAuthDto { Message = "Error,You need to add role" };





            return new RegisterAuthDto
            {
                Message = $"User Created Successfully,Confirmation Mail was send to his Email please confirm your email",
                NeedToConfirm = true,

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

            //todo: check if email is confirmed when project is ready
            /*if (user.EmailConfirmed == false)
            {
                AuthDto.Message = "This User Need To Confirm Before Login ";
                return AuthDto;

            }*/

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            AuthDto.IsAuthenticated = true;
            AuthDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

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

            var patient = _patient.Find(i => i.Id == user.Id);
            var patientClaims = new List<Claim>();
            if (patient != null)
            {
                patientClaims = new List<Claim>
                {
                     new Claim("MaxDistance",patient.MaximumDistance.ToString())
                };

            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FullName",user.FullName),
                new Claim("PhoneNumber",user.PhoneNumber),
                new Claim("uid", user.Id),
                new Claim ("UserAvatar",GetMediaUrl(user.imageUrl)),
                new Claim("MainLatitude",user.MainLatitude.ToString()),
                new Claim("MainLongitude",user.MainLongitude.ToString()),


            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(patientClaims);


            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

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
            string htmlFilePath = $"{_webHostEnvironment.WebRootPath}/EmailTemplates/ForgetPasswordEmailTemplete.html";
            string htmlContent = System.IO.File.ReadAllText(htmlFilePath);


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
        public async Task<ChangePassword> ChangePasswordAsync(ChangePasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ChangePassword
                {
                    Message = "Email Not Found",
                    PasswordIsChanged = false,

                };

            }
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return new ChangePassword
                {
                    Message = "Password doesn't match its confirmation",
                    PasswordIsChanged = false,

                };
            }
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {

                return new ChangePassword
                {
                    Message = "Password changed successfully",
                    PasswordIsChanged = true,
                };


            }
            return new ChangePassword
            {
                Message = string.Join(" | ", result.Errors.Select(e => e.Description)),
                PasswordIsChanged = false,
                ErrorAppear = true,
            };
        }
        private string GetMediaUrl(string imagePath)
        {

            string baseUrl = _mail.ServerLink;
            string relativePath = imagePath.Replace(_env.WebRootPath, "").Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
        }

        public async Task<AuthDto> LoginWithFaceIdAsync(LoginWithFaceIdDto model)
        {
            var response = await FaceIDAi(model);
            if (response == null)
            {
                return new AuthDto
                {
                    IsAuthenticated = false,
                    Message = "Something went wrong while login with FaceId ,Ai Server is Down",

                };
            }
            dynamic result = JsonConvert.DeserializeObject(response);
            string status = result["status"];
            if (status == "Authentication Failed")
            {
                return new AuthDto
                {
                    IsAuthenticated = false,
                    Message = "Your FaceId is not registered with us,Please Try Again",
                };
            }
            else if (status == "Error")
            {
                return new AuthDto()
                {
                    IsAuthenticated = false,
                    Message = "Something went wrong while login with FaceId ,Ai Server has some problem",
                };
            }
            else if (status == "Authenticated")
            {
                string patientId = result["patient_id"];
                var user = await _userManager.FindByIdAsync(patientId);
                if (user == null)
                {
                    return new AuthDto()
                    {
                        IsAuthenticated = false,
                        Message = "Your FaceId is not registered with us,Please Try Again",
                    };
                }
                //todo: check if email is confirmed when project is ready
                /* if (user.EmailConfirmed == false)
                 {
                     return new AuthDto()
                     {
                         IsAuthenticated = false,
                         Message = "Please confirm your email first to login with FaceId",
                     };

                 }
 */

                var jwtSecurityToken = await CreateJwtToken(user);
                var rolesList = await _userManager.GetRolesAsync(user);
                return new AuthDto()
                {
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    Message = "Login Successful",
                };
            }
            return new AuthDto
            {
                IsAuthenticated = false,
                Message = "Something went wrong while login with FaceId ,Ai Server has some problem",
            };


        }
      public async Task<string?> FaceIDAi(LoginWithFaceIdDto model)
        {
            string endpoint = "https://excited-hound-vastly.ngrok-free.app/login_patient";

            using (HttpClient httpClient = new HttpClient())
            {

                var multipartContent = new MultipartFormDataContent();

                multipartContent.Add(new StreamContent(model.Image.OpenReadStream()), "image", "image.jpg");



                var response = await httpClient.PostAsync(endpoint, multipartContent);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {

                    return responseBody;
                }
                else
                {

                    return null;
                }
            }
        }
    }
}
