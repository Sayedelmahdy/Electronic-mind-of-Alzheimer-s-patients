using BLL.DTOs.AuthenticationDto;
using BLL.Helper;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<IMailService> _mailServiceMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<IBaseRepository<Patient>> _patientRepositoryMock;
    private readonly IOptions<JWT> _jwtOptions;
    private readonly IOptions<Mail> _mailOptions;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null, null, null, null, null, null, null, null);

        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            new Mock<IRoleStore<IdentityRole>>().Object,
            null, null, null, null);

        _mailServiceMock = new Mock<IMailService>();
        _envMock = new Mock<IWebHostEnvironment>();
        _patientRepositoryMock = new Mock<IBaseRepository<Patient>>();

        _jwtOptions = Options.Create(new JWT { Key = "secretKey", Issuer = "issuer", Audience = "audience", DurationInDays = 1 });
        _mailOptions = Options.Create(new Mail { ServerLink = "https://electronicmindofalzheimerpatients.azurewebsites.net", FromMail = "electronic_mind_of_alzheimer_patients@hotmail.com", Password = "ASEHOM@#2392023" });

        _authService = new AuthService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _jwtOptions,
            _mailOptions,
            _mailServiceMock.Object,
            _envMock.Object,
            _patientRepositoryMock.Object,
            _envMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnMessage_WhenEmailAlreadyExists()
    {
        // Arrange
        var registerDto = new RegisterDto { Email = "sayed.work2223@gmail.com" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(registerDto.Email)).ReturnsAsync(new User());

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.Equal("Email is already registered!", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnMessage_WhenRoleIsEmpty()
    {
        // Arrange
        var registerDto = new RegisterDto { Email = "sayed.work2223@gmail.com", Role = "" };

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.Equal("Error,You need to add role", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterFamily_WhenRoleIsFamily()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "family@example.com",
            Role = "family",
            FullName = "Family Name",
            PhoneNumber = "1234567890",
            Age = 30,
            MainLatitude = 10.0,
            MainLongitude = 20.0,
            Password = "Test123@",
            Avatar = new FormFile(new MemoryStream(), 0, 0, "Avatar", "avatar.jpg")
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(registerDto.Email)).ReturnsAsync((User)null);
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), registerDto.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "family")).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.Equal("User Created Successfully,Confirmation Mail was send to his Email please confirm your email", result.Message);
        Assert.True(result.NeedToConfirm);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnMessage_WhenRoleIsInvalid()
    {
        // Arrange
        var registerDto = new RegisterDto { Email = "sayed.work2223@gmail.com", Role = "invalid" };

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.Equal("Invalid Role", result.Message);
    }

    [Fact]
    public async Task GetTokenAsync_ShouldReturnMessage_WhenCredentialsAreInvalid()
    {
        // Arrange
        var tokenRequestDto = new TokenRequestDto { Email = "sayed.work2223@gmail.com", Password = "Test123@" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(tokenRequestDto.Email)).ReturnsAsync((User)null);

        // Act
        var result = await _authService.GetTokenAsync(tokenRequestDto);

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.Equal("Email or Password is incorrect!", result.Message);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnEmailConfirmation_WhenUserNotFound()
    {
        // Arrange
        var userId = "21260245-eb95-46b3-93d4-594cc8902508";
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _authService.ConfirmEmailAsync(userId, token);

        // Assert
        Assert.False(result.IsConfirm);
        Assert.Equal("User not found", result.Message);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnEmailConfirmation_WhenTokenIsInvalid()
    {
        // Arrange
        var userId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ23129.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U"));
        var user = new User();
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

        // Act
        var result = await _authService.ConfirmEmailAsync(userId, token);

        // Assert
        Assert.False(result.IsConfirm);
        Assert.Equal("Email did not confirm", result.Message);
    }

    [Fact]
    public async Task ForgetPasswordAsync_ShouldReturnForgetPassword_WhenUserNotFound()
    {
        // Arrange
        var email = "sayed.work2223@gmail.com";
        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null);

        // Act
        var result = await _authService.ForgetPasswordAsync(email);

        // Assert
        Assert.False(result.IsEmailSent);
        Assert.Equal("No user associated with email", result.Message);
    }
    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnResetPassword_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDto { Email = "sayed.work2223@gmail.com", NewPassWord = "Test123@", ConfirmPassword = "differentPassword" };

        // Act
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);

        // Assert
        Assert.False(result.IsPasswordReset);
        Assert.Equal("Password doesn't match its confirmation", result.Message);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnResetPassword_WhenUserNotFound()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDto { Email = "sayed.work2223@gmail.com", NewPassWord = "Test123@", ConfirmPassword = "password", Token = "token" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(resetPasswordDto.Email)).ReturnsAsync((User)null);

        // Act
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);

        // Assert
        Assert.False(result.IsPasswordReset);
        Assert.Equal("No user associated with email", result.Message);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnResetPassword_WhenPasswordResetSucceeds()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDto { Email = "sayed.work2223@gmail.com", NewPassWord = "Test123@", ConfirmPassword = "password", Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("token")) };
        var user = new User();
        _userManagerMock.Setup(x => x.FindByEmailAsync(resetPasswordDto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), resetPasswordDto.NewPassWord)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);

        // Assert
        Assert.True(result.IsPasswordReset);
        Assert.Equal("Password has been reset successfully!", result.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnChangePassword_WhenEmailNotFound()
    {
        // Arrange
        var changePasswordDto = new ChangePasswordDto { Email = "sayed.work2223@gmail.com", OldPassword = "Test123@", NewPassword = "newPassword", ConfirmNewPassword = "newPassword" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(changePasswordDto.Email)).ReturnsAsync((User)null);

        // Act
        var result = await _authService.ChangePasswordAsync(changePasswordDto);

        // Assert
        Assert.False(result.PasswordIsChanged);
        Assert.Equal("Email Not Found", result.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnChangePassword_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var changePasswordDto = new ChangePasswordDto
        {
            Email = "ndrdclub@gmail.com",
            OldPassword = "Ess123@",
            NewPassword = "Ess123@",
            ConfirmNewPassword = "Ess123@2"
        };

        var user = new User { Email = changePasswordDto.Email };

        // Mock the UserManager behavior
        _userManagerMock.Setup(x => x.FindByEmailAsync(changePasswordDto.Email)).ReturnsAsync(user);

        // Act
        var result = await _authService.ChangePasswordAsync(changePasswordDto);

        // Assert
        Assert.False(result.PasswordIsChanged);
        Assert.Equal("Password doesn't match its confirmation", result.Message);
    }


    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnChangePassword_WhenPasswordChangeSucceeds()
    {
        // Arrange
        var changePasswordDto = new ChangePasswordDto { Email = "sayed.work2223@gmail.com", OldPassword = "oldPassword", NewPassword = "newPassword", ConfirmNewPassword = "newPassword" };
        var user = new User();
        _userManagerMock.Setup(x => x.FindByEmailAsync(changePasswordDto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.ChangePasswordAsync(changePasswordDto);

        // Assert
        Assert.True(result.PasswordIsChanged);
        Assert.Equal("Password changed successfully", result.Message);
    }



    private Task<string?> MockFaceIDAiAsync(LoginWithFaceIdDto model)
    {
        // Simulate the Face ID AI response
        return Task.FromResult<string?>(null);
    }
}
