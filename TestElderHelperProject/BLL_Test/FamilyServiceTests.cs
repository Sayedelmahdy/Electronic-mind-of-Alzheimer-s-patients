using BLL.DTOs.FamilyDto;
using BLL.Helper;
using BLL.Hubs;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;

public class FamilyServiceTests
{
    private readonly Mock<IBaseRepository<Family>> _familyRepositoryMock;
    private readonly Mock<IBaseRepository<Patient>> _patientRepositoryMock;
    private readonly Mock<IBaseRepository<Caregiver>> _caregiverRepositoryMock;
    private readonly Mock<IBaseRepository<Appointment>> _appointmentsRepositoryMock;
    private readonly Mock<IBaseRepository<Location>> _locationRepositoryMock;
    private readonly Mock<IBaseRepository<PersonWithoutAccount>> _personWithoutAccountRepositoryMock;
    private readonly Mock<IDecodeJwt> _jwtDecodeMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<IMailService> _mailServiceMock;
    private readonly Mock<IHubContext<AppointmentHub>> _appointmentHubMock;
    private readonly Mock<IBaseRepository<Media>> _mediaRepositoryMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IBaseRepository<Report>> _reportRepositoryMock;
    private readonly IOptions<Mail> _mailOptions;
    private readonly IOptions<JWT> _jwtOptions;
    private readonly FamilyService _familyService;

    public FamilyServiceTests()
    {
        _familyRepositoryMock = new Mock<IBaseRepository<Family>>();
        _patientRepositoryMock = new Mock<IBaseRepository<Patient>>();
        _caregiverRepositoryMock = new Mock<IBaseRepository<Caregiver>>();
        _appointmentsRepositoryMock = new Mock<IBaseRepository<Appointment>>();
        _locationRepositoryMock = new Mock<IBaseRepository<Location>>();
        _personWithoutAccountRepositoryMock = new Mock<IBaseRepository<PersonWithoutAccount>>();
        _jwtDecodeMock = new Mock<IDecodeJwt>();
        _envMock = new Mock<IWebHostEnvironment>();
        _mailServiceMock = new Mock<IMailService>();
        _appointmentHubMock = new Mock<IHubContext<AppointmentHub>>();
        _mediaRepositoryMock = new Mock<IBaseRepository<Media>>();
        _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _reportRepositoryMock = new Mock<IBaseRepository<Report>>();

        _mailOptions = Options.Create(new Mail { ServerLink = "http://localhost", FromMail = "test@example.com", Password = "password" });
        _jwtOptions = Options.Create(new JWT { Key = "test_key", Issuer = "test_issuer", Audience = "test_audience", DurationInDays = 1 });

        _familyService = new FamilyService(
            _mediaRepositoryMock.Object,
            _familyRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _caregiverRepositoryMock.Object,
            _appointmentsRepositoryMock.Object,
            _locationRepositoryMock.Object,
            _jwtDecodeMock.Object,
            _envMock.Object,
            _mailOptions,
            _jwtOptions,
            _mailServiceMock.Object,
            _appointmentHubMock.Object,
            _userManagerMock.Object,
            _reportRepositoryMock.Object,
            _personWithoutAccountRepositoryMock.Object,
            _envMock.Object
        );
    }

    [Fact]
    public async Task GetPatientCode_ShouldReturnPatientCode_WhenFamilyIdIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var family = new Family { Id = familyId, PatientId = patientId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);

        // Act
        var result = await _familyService.GetPatientCode(token);

        // Assert
        Assert.Equal(patientId, result);
    }

    [Fact]
    public async Task GetPatientCode_ShouldReturnNull_WhenFamilyIdIsInvalid()
    {
        // Arrange
        var token = "ineyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = (string)null;

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);

        // Act
        var result = await _familyService.GetPatientCode(token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePatientProfileAsync_ShouldReturnUpdatedProfile_WhenFamilyIdIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var family = new Family { Id = familyId, PatientId = patientId };
        var patient = new Patient { Id = patientId, FamilyCreatedId = familyId };
        var updateDto = new UpdatePatientProfileDto
        {
            Age = 30,
            PhoneNumber = "1234567890",
            DiagnosisDate = new DateOnly(2022, 1, 1),
            MaximumDistance = 10
        };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.Include(It.IsAny<Expression<Func<Family, object>>>()))
                             .Returns(new List<Family> { family }.AsQueryable());
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);

        // Act
        var result = await _familyService.UpdatePatientProfileAsync(token, updateDto);

        // Assert
        Assert.False(result.ErrorAppear);
        Assert.Equal("Patient Updated Succesfully", result.Message);
    }

    [Fact]
    public async Task UpdatePatientProfileAsync_ShouldReturnError_WhenFamilyIdIsInvalid()
    {
        // Arrange
        var token = "ineyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = (string)null;
        var updateDto = new UpdatePatientProfileDto();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);

        // Act
        var result = await _familyService.UpdatePatientProfileAsync(token, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPatientProfile_ShouldReturnProfile_WhenFamilyIdIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var family = new Family { Id = familyId, PatientId = patientId };
        var patient = new Patient
        {
            Id = patientId,
            FullName = "Patient Name",
            Age = 60,
            Email = "patient@example.com",
            PhoneNumber = "1234567890",
            DiagnosisDate = DateTime.Now,
            MaximumDistance = 10
        };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);
        _familyRepositoryMock.Setup(x => x.Include(It.IsAny<Expression<Func<Family, object>>>()))
                             .Returns(new List<Family> { family }.AsQueryable());

        // Act
        var result = await _familyService.GetPatientProfile(token);

        // Assert
        Assert.False(result.ErrorAppear);
        Assert.Equal("Patient Profile returned Succesfully", result.Message);
    }

    [Fact]
    public async Task GetPatientProfile_ShouldReturnError_WhenFamilyIdIsInvalid()
    {
        // Arrange
        var token = "ineyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = (string)null;

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);

        // Act
        var result = await _familyService.GetPatientProfile(token);

        // Assert
        Assert.True(result.ErrorAppear);
        Assert.Equal("Token Doesn't have FamilyId", result.Message);
    }

    [Fact]
    public async Task AddPatientAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var addPatientDto = new AddPatientDto
        {
            FullName = "Patient Name",
            Email = "patient@example.com",
            Password = "password",
            PhoneNumber = "1234567890",
            Age = 60,
            MainLongitude = 30.0,
            MainLatitude = 50.0,
            Avatar = Mock.Of<IFormFile>(),
            relationality = "Relative",
            DescriptionForPatient = "Description",
            DiagnosisDate = new DateOnly(2022, 1, 1),
            MaximumDistance = 10
        };
        var family = new Family { Id = familyId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);
        _userManagerMock.Setup(x => x.FindByEmailAsync(addPatientDto.Email)).ReturnsAsync((User)null);
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Patient>(), addPatientDto.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<Patient>(), "patient")).Returns(Task.FromResult(IdentityResult.Success));
        _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<Patient>())).ReturnsAsync("confirm_token");

        // Act
        var result = await _familyService.AddPatientAsync(token, addPatientDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Patient Added Succsfully, and you need to send a training images", result.message);
    }

    [Fact]
    public async Task AddPatientAsync_ShouldReturnError_WhenEmailIsAlreadyRegistered()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var addPatientDto = new AddPatientDto { Email = "patient@example.com" };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _userManagerMock.Setup(x => x.FindByEmailAsync(addPatientDto.Email)).ReturnsAsync(new User());

        // Act
        var result = await _familyService.AddPatientAsync(token, addPatientDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("Email is already registered!", result.message);
    }

    [Fact]
    public async Task AssignPatientToFamily_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var patientId = "patientId";
        var assignPatientDto = new AssignPatientDto { PatientCode = patientId, relationility = "Relative" };
        var family = new Family { Id = familyId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(new Patient());

        // Act
        var result = await _familyService.AssignPatientToFamily(token, assignPatientDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Patient Assigned to this family succesfully, and you need to send a training images", result.message);
    }

    [Fact]
    public async Task AssignPatientToFamily_ShouldReturnError_WhenFamilyAlreadyHasPatient()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var patientId = "patientId";
        var assignPatientDto = new AssignPatientDto { PatientCode = patientId, relationility = "Relative" };
        var family = new Family { Id = familyId, PatientId = patientId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);

        // Act
        var result = await _familyService.AssignPatientToFamily(token, assignPatientDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("family alredy have assigned to patient", result.message);
    }

    [Fact]
    public async Task GetPatientAppointmentsAsync_ShouldReturnAppointments_WhenFamilyAndPatientIdAreValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var patientId = "patientId";
        var family = new Family { Id = familyId, PatientId = patientId };
        var appointments = new List<Appointment>
        {
            new Appointment { AppointmentId = "1", PatientId = patientId, FamilyId = familyId, Date = DateTime.Now, Location = "Location 1", Notes = "Notes 1" },
            new Appointment { AppointmentId = "2", PatientId = patientId, FamilyId = "anotherFamilyId", Date = DateTime.Now, Location = "Location 2", Notes = "Notes 2" }
        }.AsQueryable();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);
        _appointmentsRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Appointment, bool>>>())).Returns(appointments);

        // Act
        var result = await _familyService.GetPatientAppointmentsAsync(token);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPatientAppointmentsAsync_ShouldReturnEmpty_WhenFamilyIdIsInvalid()
    {
        // Arrange
        var token = "ineyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = (string)null;

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);

        // Act
        var result = await _familyService.GetPatientAppointmentsAsync(token);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAppointmentAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var patientId = "patientId";
        var addAppointmentDto = new AddAppointmentDto { Date = DateTime.Now, Location = "Location", Notes = "Notes" };
        var family = new Family { Id = familyId, PatientId = patientId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);

        // Act
        var result = await _familyService.AddAppointmentAsync(token, addAppointmentDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Appointment added successfully", result.message);
    }

    [Fact]
    public async Task AddAppointmentAsync_ShouldReturnError_WhenFamilyDoesNotHavePatient()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var addAppointmentDto = new AddAppointmentDto { Date = DateTime.Now, Location = "Location", Notes = "Notes" };
        var family = new Family { Id = familyId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);

        // Act
        var result = await _familyService.AddAppointmentAsync(token, addAppointmentDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("This person doesn't have Patient yet", result.message);
    }

    [Fact]
    public async Task DeleteAppointmentAsync_ShouldReturnSuccess_WhenFamilyCreatedTheAppointment()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var appointmentId = "appointmentId";
        var appointment = new Appointment { AppointmentId = appointmentId, FamilyId = familyId, PatientId = "patientId" };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _appointmentsRepositoryMock.Setup(x => x.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

        // Act
        var result = await _familyService.DeleteAppointmentAsync(token, appointmentId);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Appointment Deleted Successfully", result.message);
    }

    [Fact]
    public async Task DeleteAppointmentAsync_ShouldReturnError_WhenFamilyDidNotCreateTheAppointment()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var appointmentId = "appointmentId";
        var appointment = new Appointment { AppointmentId = appointmentId, FamilyId = "anotherFamilyId", PatientId = "patientId" };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _appointmentsRepositoryMock.Setup(x => x.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

        // Act
        var result = await _familyService.DeleteAppointmentAsync(token, appointmentId);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("This user didn't Create this appointment so he cann't delete it", result.message);
    }

    [Fact]
    public async Task GetMediaForFamilyAsync_ShouldReturnMedia_WhenFamilyIdIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var patientId = "patientId";
        var family = new Family { Id = familyId, PatientId = patientId };
        var media = new List<Media>
        {
            new Media { Media_Id = "1", FamilyId = familyId, PatientId = patientId, Caption = "Caption 1", Image_Path = "path1.jpg", Upload_Date = DateTime.Now, Extension = ".jpg" },
            new Media { Media_Id = "2", FamilyId = familyId, PatientId = patientId, Caption = "Caption 2", Image_Path = "path2.jpg", Upload_Date = DateTime.Now, Extension = ".jpg" }
        };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);
        _mediaRepositoryMock.Setup(x => x.Include(It.IsAny<Expression<Func<Media, object>>>())).Returns(media.AsQueryable());

        // Act
        var result = await _familyService.GetMediaForFamilyAsync(token);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetMediaForFamilyAsync_ShouldReturnEmpty_WhenFamilyIdIsInvalid()
    {
        // Arrange
        var token = "ineyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = (string)null;

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);

        // Act
        var result = await _familyService.GetMediaForFamilyAsync(token);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task UploadMediaAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var patientId = "patientId";
        var addMediaDto = new AddMediaDto { MediaFile = Mock.Of<IFormFile>(), Caption = "Caption" };
        var family = new Family { Id = familyId, PatientId = patientId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);

        // Act
        var result = await _familyService.UploadMediaAsync(token, addMediaDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Media Added Succesfully", result.message);
    }

    [Fact]
    public async Task UploadMediaAsync_ShouldReturnError_WhenFamilyDoesNotHavePatient()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var addMediaDto = new AddMediaDto { MediaFile = Mock.Of<IFormFile>(), Caption = "Caption" };
        var family = new Family { Id = familyId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);

        // Act
        var result = await _familyService.UploadMediaAsync(token, addMediaDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("This Family doesn't have patient yet", result.message);
    }

    [Fact]
    public async Task AssignPatientToCaregiver_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var patientId = "patientId";
        var caregiverCode = "caregiverCode";
        var family = new Family { Id = familyId, PatientId = patientId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);
        _caregiverRepositoryMock.Setup(x => x.GetById(caregiverCode)).Returns(new Caregiver());

        // Act
        var result = await _familyService.AssignPatientToCaregiver(token, caregiverCode);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Patient Assigned to Caregiver succesfully", result.message);
    }

    [Fact]
    public async Task AssignPatientToCaregiver_ShouldReturnError_WhenCaregiverCodeIsInvalid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var caregiverCode = "caregiverCode";
        var family = new Family { Id = familyId, PatientId = "patientId" };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);
        _caregiverRepositoryMock.Setup(x => x.GetById(caregiverCode)).Returns((Caregiver)null);

        // Act
        var result = await _familyService.AssignPatientToCaregiver(token, caregiverCode);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("Caregiver Code invaild", result.message);
    }

    [Fact]
    public async Task GetPatientLocationsTodayAsync_ShouldReturnLocations_WhenFamilyIdIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var family = new Family { Id = familyId, PatientId = patientId };
        var locations = new List<Location>
        {
            new Location { Latitude = 10.0, Longitude = 20.0, Timestamp = DateTime.Now },
            new Location { Latitude = 30.0, Longitude = 40.0, Timestamp = DateTime.Now }
        }.AsQueryable();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);
        _locationRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Location, bool>>>())).Returns(locations);

        // Act
        var result = await _familyService.GetPatientLocationsTodayAsync(token);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPatientLocationsTodayAsync_ShouldReturnEmpty_WhenFamilyIdIsInvalid()
    {
        // Arrange
        var token = "ineyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = (string)null;

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);

        // Act
        var result = await _familyService.GetPatientLocationsTodayAsync(token);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPatientReportsAsync_ShouldReturnReports_WhenFamilyIdIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var family = new Family { Id = familyId, PatientId = patientId };
        var reports = new List<Report>
        {
            new Report { ReportId = "1", PatientId = patientId, FromDate = DateTime.Now.AddDays(-10), ToDate = DateTime.Now, ReportContent = "Content 1" },
            new Report { ReportId = "2", PatientId = patientId, FromDate = DateTime.Now.AddDays(-5), ToDate = DateTime.Now, ReportContent = "Content 2" }
        }.AsQueryable();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);
        _reportRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Report, bool>>>())).Returns(reports);

        // Act
        var result = await _familyService.GetPatientReportsAsync(token);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPatientReportsAsync_ShouldReturnEmpty_WhenFamilyIdIsInvalid()
    {
        // Arrange
        var token = "ineyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = (string)null;

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);

        // Act
        var result = await _familyService.GetPatientReportsAsync(token);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FamilyNeedATraining_ShouldReturnNeedTrainingStatus_WhenFamilyAndPatientIdAreValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var family = new Family { Id = familyId, PatientId = patientId, NumberOfTrainingImage = 3 };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);

        // Act
        var result = await _familyService.FamilyNeedATraining(token);

        // Assert
        Assert.False(result.GlobalResponse.HasError);
        Assert.True(result.NeedATraining);
    }

    [Fact]
    public async Task FamilyNeedATraining_ShouldReturnError_WhenFamilyDoesNotHavePatient()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "familyId";
        var family = new Family { Id = familyId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);

        // Act
        var result = await _familyService.FamilyNeedATraining(token);

        // Assert
        Assert.True(result.GlobalResponse.HasError);
        Assert.False(result.NeedATraining);
        Assert.Equal("This Family doesn't have patient yet", result.GlobalResponse.message);
    }

    [Fact]
    public async Task TrainingImage_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var family = new Family { Id = familyId, PatientId = patientId, NumberOfTrainingImage = 3 };
        var addTrainingImageDto = new AddTrainingImageDto { TrainingImages = new List<IFormFile> { Mock.Of<IFormFile>(), Mock.Of<IFormFile>() } };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);

        // Act
        var result = await _familyService.TrainingImage(token, addTrainingImageDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Training image successfully", result.message);
    }

    [Fact]
    public async Task TrainingImage_ShouldReturnError_WhenFamilyDoesNotHavePatient()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var addTrainingImageDto = new AddTrainingImageDto { TrainingImages = new List<IFormFile> { Mock.Of<IFormFile>(), Mock.Of<IFormFile>() } };
        var family = new Family { Id = familyId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);

        // Act
        var result = await _familyService.TrainingImage(token, addTrainingImageDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("This Family doesn't have patient yet", result.message);
    }

    [Fact]
    public async Task AddPersonWithoutAccount_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var addPersonWithoutAccountDto = new AddPersonWithoutAccountDto
        {
            FullName = "Person Name",
            PhoneNumber = "1234567890",
            TraningImage = new List<IFormFile> { Mock.Of<IFormFile>(), Mock.Of<IFormFile>(), Mock.Of<IFormFile>(), Mock.Of<IFormFile>(), Mock.Of<IFormFile>() },
            MainLongitude = 30.0,
            MainLatitude = 50.0,
            Relationility = "Relative",
            DescriptionForPatient = "Description"
        };
        var family = new Family { Id = familyId, PatientId = patientId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);

        // Act
        var result = await _familyService.AddPersonWithoutAccount(token, addPersonWithoutAccountDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Person Added Succesfully, and training image successfully", result.message);
    }

    [Fact]
    public async Task AddPersonWithoutAccount_ShouldReturnError_WhenFamilyDoesNotHavePatient()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var addPersonWithoutAccountDto = new AddPersonWithoutAccountDto
        {
            FullName = "Person Name",
            PhoneNumber = "1234567890",
            TraningImage = new List<IFormFile> { Mock.Of<IFormFile>(), Mock.Of<IFormFile>(), Mock.Of<IFormFile>(), Mock.Of<IFormFile>(), Mock.Of<IFormFile>() },
            MainLongitude = 30.0,
            MainLatitude = 50.0,
            Relationility = "Relative",
            DescriptionForPatient = "Description"
        };
        var family = new Family { Id = familyId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);

        // Act
        var result = await _familyService.AddPersonWithoutAccount(token, addPersonWithoutAccountDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("This Family doesn't have patient yet", result.message);
    }

    [Fact]
    public async Task AddPersonWithoutAccount_ShouldReturnError_WhenLessThan5ImagesProvided()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "35da2914-ab42-407b-9275-bf3c43003008";
        var addPersonWithoutAccountDto = new AddPersonWithoutAccountDto
        {
            FullName = "Person Name",
            PhoneNumber = "1234567890",
            TraningImage = new List<IFormFile> { Mock.Of<IFormFile>(), Mock.Of<IFormFile>(), Mock.Of<IFormFile>() },
            MainLongitude = 30.0,
            MainLatitude = 50.0,
            Relationility = "Relative",
            DescriptionForPatient = "Description"
        };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);

        // Act
        var result = await _familyService.AddPersonWithoutAccount(token, addPersonWithoutAccountDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("Please send at least 5 image for training with Different angles like face samples", result.message);
    }
}
