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
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
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
    }
}