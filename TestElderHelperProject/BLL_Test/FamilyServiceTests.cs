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
using Microsoft.EntityFrameworkCore.Query;
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
        var familyId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var patientId = "35da2914-ab42-407b-9275-bf3c43003008";
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
        var familyId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var patientId = "35da2914-ab42-407b-9275-bf3c43003008";
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
    public async Task DeleteAppointmentAsync_ShouldReturnSuccess_WhenFamilyCreatedTheAppointment()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1pZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0YXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var appointmentId = "35da22914-ab42-407b-9275-bf3c43003008";
        var appointment = new Appointment { AppointmentId = appointmentId, FamilyId = familyId, PatientId = "patientId" };
        var family = new Family { Id = familyId, PatientId = "patientId" };

        var mockClientProxy = new Mock<IClientProxy>();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _appointmentsRepositoryMock.Setup(x => x.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);
        _appointmentsRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Appointment>())).Returns(Task.CompletedTask);
        _appointmentHubMock.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

        mockClientProxy.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                       .Returns(Task.CompletedTask);

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
        var familyId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var appointmentId = "35da29124-ab42-407b-9275-bf3c43003008";
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
    public async Task UploadMediaAsync_ShouldReturnError_WhenFamilyDoesNotHavePatient()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdW1iZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "21260f45-eb95-46b3-93d4-594cc8902508";
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
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYW1lIjoi2KfYs9iq2KfYsCAvINin2LPZhNin2YUg2KfYrdmF2K8gIiwiUGhvbmVOdWRiZXIiOiIwMTI4ODUxMzIyNSIsInVpZCI6IjM1ZGEyOTE0LWFiNDItNDA3Yi05Mjc1LWJmM2M0MzAwMzAwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8zNWRhMjkxNC1hYjQyLTQwN2ItOTI3NS1iZjNjNDMwMDMwMDhfOGY4ZDBkNDYtYmE5ZC00YjM3LTgwZWYtN2FhZDgyN2ZlZjY0LmpwZWciLCJNYWluTGF0aXR1ZGUiOiIzMC4wOTI5ODgiLCJNYWluTG9uZ2l0dWRlIjoiMzEuMjA0MzMxIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
        var familyId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var patientId = "35da2914-ab42-407b-9275-bf3c43003008";
        var caregiverCode = "35da2231914-ab42-407b-9275-bf3c43003008";
        var family = new Family { Id = familyId, PatientId = patientId };
        var patient = new Patient { Id = patientId };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(familyId);
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(family);
        _caregiverRepositoryMock.Setup(x => x.GetById(caregiverCode)).Returns(new Caregiver());
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);

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
        var familyId = "21260f45-eb95-46b3-93d4-594cc8902508v";
        var caregiverCode = "35da292114-ab42-407b-9275-bf3c43003008";
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
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZTBmNTM4OC1kZGMzLTQyZmYtYTQ1OC02YTljNTlhNDFiZGEiLCJlbWFpbCI6Im5kcmRjbHViQGdtYWlsLmNvbSIsIkZ1bGxOYWlIiwicm9sZXMiOiJGYW1pbHkiLCJleHAiOjE3MjY1MzQ0ODcsImlzcyI6IkFydE9mQ29kaW5nIiwiYXVkIjoiQWx6aGVpbWFyQXBwIn0.5CWOgvtRnRpfkpsQjhGwCWB8M4iqVKWFSlNtDhlR7So";
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
        _familyRepositoryMock.Setup(x => x.GetById(familyId)).Returns(new Family { Id = familyId, PatientId = "patientId" });

        // Act
        var result = await _familyService.AddPersonWithoutAccount(token, addPersonWithoutAccountDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("Please send at least 5 image for training with Different angles like face samples", result.message);
    }

}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public T Current => _inner.Current;
}

internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
    {
        return new TestAsyncEnumerable<TResult>(expression);
    }

    public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute<TResult>(expression));
    }

    TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
