using API.Models;
using Azure;
using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthenticationController : ControllerBase
    {
        private IAuthService _authService { get; }
        public IConfiguration _configuration { get; }

        public AuthenticationController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
        {
            

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if (result.Message != "User Created Successfully,Confirmation Mail was send to his Email please confirm your email")
            {
                return BadRequest(JsonSerializer.Serialize(result.Message));
            }
           

            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] TokenRequestDto tokenRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var res = await _authService.GetTokenAsync(tokenRequestDto);
            if (!res.IsAuthenticated)
            {
                return BadRequest(res.Message);
            }
            return Ok(res);
        }
        /*[HttpPost("AddPatients/{username}/{Relationility}/{dateTime}")]
        public async Task<IActionResult> AddPatients([FromBody] RegisterDto model,string username,string Relationility,DateTime dateTime)
        {
           var result = await _authService.AddPatients(model, username, Relationility, dateTime);
            if (result.Message=="Done")
                return Ok(result);
            return BadRequest(result);
        }*/
        [HttpGet("ConfirmEmail")]
        public async Task <IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();
            
            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (result.IsConfirm)
            {
                return Redirect($"{_configuration["Mail:ServerLink"]}/confirmemail.html");
            }
            
            return BadRequest(result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var re = new ResetPasswordDto 
            {
                ConfirmPassword = model.ConfirmPassword,
                Email = model.Email,
                NewPassWord = model.NewPassWord,
                Token = model.Token
            };


            var result = await _authService.ResetPasswordAsync(re);
            if (result.IsPasswordReset)
            {
                return Redirect($"{_configuration["Mail:ServerLink"]}/ResetpasswordSucc.html");
            }

            return BadRequest(result);
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _authService.ForgetPasswordAsync(email);

            if (result.IsEmailSent)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }
        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword ([FromBody]ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var res = await _authService.ChangePasswordAsync(changePasswordDto);
            if (!res.PasswordIsChanged && !res.ErrorAppear)
            {
                return BadRequest(res.Message);
            }
            else if (!res.PasswordIsChanged && res.ErrorAppear)
            {
                return StatusCode(500, res.Message);
            }
            return Ok(res);
        }

    }
}
