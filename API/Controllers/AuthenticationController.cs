﻿using BLL.DTOs;
using BLL.Interfaces;
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
            if (result.Message != "User Created Successfully")
            {
                return BadRequest(JsonSerializer.Serialize(result.Message));
            }
           

            return Ok(result);
        }
        [HttpPost("AddPatients/{username}/{Relationility}/{dateTime}")]
        public async Task<IActionResult> AddPatients([FromBody] RegisterDto model,string username,string Relationility,DateTime dateTime)
        {
           var result = await _authService.AddPatients(model, username, Relationility, dateTime);
            if (result.Message=="Done")
                return Ok(result);
            return BadRequest(result);
        }
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
    }
}
