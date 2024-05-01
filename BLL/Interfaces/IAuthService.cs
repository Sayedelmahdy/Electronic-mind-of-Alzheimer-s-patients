﻿using BLL.DTOs.AuthenticationDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterAuthDto> RegisterAsync(RegisterDto model);
        Task<AuthDto> GetTokenAsync(TokenRequestDto model);
        //Task<AuthDto> RefreshTokenAsync(string Token);
        /*Task<bool> RevokeTokenAsync(string Token);*/

        /*Task<AuthDto> AddPatients(RegisterDto model, string username, string Relationility, DateTime DiagnosisDate);*/
        Task<EmailConfirmation> ConfirmEmailAsync(string UserId, string Token);
        Task<ChangePassword> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<ResetPassword> ResetPasswordAsync(ResetPasswordDto model);
        Task<ForgetPassword> ForgetPasswordAsync(string email);
        Task<AuthDto> LoginWithFaceIdAsync(LoginWithFaceIdDto model);
    }


}
