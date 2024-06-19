using Xunit;
using Moq;
using System.Threading.Tasks;
using BLL.Services;
using BLL.DTOs;
using BLL.DTOs.CaregiverDto;
using BLL.DTOs.FamilyDto;
using BLL.DTOs.PatientDto;
using BLL.Helper;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using BLL.Hubs;
using System.Linq.Expressions;

public class PatientServiceTests
{
    private readonly Mock<IHubContext<MedicineReminderHub>> _hubContextMock;
    private readonly Mock<IBaseRepository<Patient>> _patientRepositoryMock;
    private readonly Mock<IBaseRepository<Medication_Reminders>> _medicinesRepositoryMock;
    private readonly Mock<IBaseRepository<Mark_Medicine_Reminder>> _markMedicineReminderRepositoryMock;
    private readonly Mock<IBaseRepository<Appointment>> _appointmentsRepositoryMock;
    private readonly Mock<IBaseRepository<Family>> _familyRepositoryMock;
    private readonly Mock<IBaseRepository<PersonWithoutAccount>> _personRepositoryMock;
    private readonly Mock<IDecodeJwt> _jwtDecodeMock;
    private readonly Mock<IBaseRepository<Media>> _mediaRepositoryMock;
    private readonly Mock<IBaseRepository<SecretAndImportantFile>> _secretRepositoryMock;
    private readonly Mock<IBaseRepository<GameScore>> _gameScoreRepositoryMock;
    private readonly Mock<IMailService> _mailServiceMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly IOptions<Mail> _mailOptions;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _hubContextMock = new Mock<IHubContext<MedicineReminderHub>>();
        _patientRepositoryMock = new Mock<IBaseRepository<Patient>>();
        _medicinesRepositoryMock = new Mock<IBaseRepository<Medication_Reminders>>();
        _markMedicineReminderRepositoryMock = new Mock<IBaseRepository<Mark_Medicine_Reminder>>();
        _appointmentsRepositoryMock = new Mock<IBaseRepository<Appointment>>();
        _familyRepositoryMock = new Mock<IBaseRepository<Family>>();
        _personRepositoryMock = new Mock<IBaseRepository<PersonWithoutAccount>>();
        _jwtDecodeMock = new Mock<IDecodeJwt>();
        _mediaRepositoryMock = new Mock<IBaseRepository<Media>>();
        _secretRepositoryMock = new Mock<IBaseRepository<SecretAndImportantFile>>();
        _gameScoreRepositoryMock = new Mock<IBaseRepository<GameScore>>();
        _mailServiceMock = new Mock<IMailService>();
        _envMock = new Mock<IWebHostEnvironment>();

        _mailOptions = Options.Create(new Mail { ServerLink = "http://localhost", FromMail = "test@example.com", Password = "password" });

        _patientService = new PatientService(
            _hubContextMock.Object,
            _jwtDecodeMock.Object,
            _patientRepositoryMock.Object,
            _medicinesRepositoryMock.Object,
            _appointmentsRepositoryMock.Object,
            _familyRepositoryMock.Object,
            _mediaRepositoryMock.Object,
            _envMock.Object,
            _mailOptions,
            _mailServiceMock.Object,
            _secretRepositoryMock.Object,
            _gameScoreRepositoryMock.Object,
            _markMedicineReminderRepositoryMock.Object,
            _personRepositoryMock.Object
        );
    }
    [Fact]
    public async Task GetPatientProfileAsync_ShouldReturnProfile_WhenPatientExists()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var patient = new Patient { Id = patientId, FullName = "John Doe", Age = 70, PhoneNumber = "1234567890", DiagnosisDate = DateTime.Now };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetPatientProfileAsync(token);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Welcome To Your Profile", result.Message);
        Assert.Equal(patientId, result.PatientId);
        Assert.Equal(patient.FullName, result.FullName);
        Assert.Equal(patient.Age, result.Age);
        Assert.Equal(patient.PhoneNumber, result.PhoneNumber);
        Assert.Equal(patient.DiagnosisDate, result.DiagnosisDate);
    }

    [Fact]
    public async Task GetPatientProfileAsync_ShouldReturnError_WhenPatientDoesNotExist()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _patientService.GetPatientProfileAsync(token);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("Invalid Patient ID", result.Message);
    }

    [Fact]
    public async Task GetAppointmentAsync_ShouldReturnAppointments_WhenPatientExists()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var patient = new Patient { Id = patientId };
        var appointments = new List<Appointment>
    {
        new Appointment { AppointmentId = "1", PatientId = patientId, Date = DateTime.Now, Location = "Location 1", Notes = "Notes 1", family = new Family { FullName = "Family 1" } },
        new Appointment { AppointmentId = "2", PatientId = patientId, Date = DateTime.Now, Location = "Location 2", Notes = "Notes 2", family = new Family { FullName = "Family 2" } }
    }.AsQueryable();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _appointmentsRepositoryMock.Setup(x => x.Include(It.IsAny<Expression<Func<Appointment, object>>>())).Returns(appointments);

        // Act
        var result = await _patientService.GetAppointmentAsync(token);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("1", result.First().AppointmentId);
        Assert.Equal("Location 1", result.First().Location);
        Assert.Equal("Notes 1", result.First().Notes);
        Assert.Equal("Family 1", result.First().FamilyNameWhoCreatedAppointemnt);
    }


    [Fact]
    public async Task GetAppointmentAsync_ShouldReturnEmpty_WhenPatientDoesNotExist()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _patientService.GetAppointmentAsync(token);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMedicationRemindersAsync_ShouldReturnReminders_WhenPatientExists()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var medicines = new List<Medication_Reminders>
        {
            new Medication_Reminders { Reminder_ID = "1", Patient_Id = patientId, Medication_Name = "Med 1", Dosage = "Dosage 1", StartDate = DateTime.Now, EndDate = DateTime.Now, Medcine_Type = MedcineType.Pill, Repeater = RepeatType.Once },
            new Medication_Reminders { Reminder_ID = "2", Patient_Id = patientId, Medication_Name = "Med 2", Dosage = "Dosage 2", StartDate = DateTime.Now, EndDate = DateTime.Now, Medcine_Type = MedcineType.Syringe, Repeater = RepeatType.Four_Times }
        }.AsQueryable();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _medicinesRepositoryMock.Setup(x => x.WhereAsync(It.IsAny<Expression<Func<Medication_Reminders, bool>>>())).ReturnsAsync(medicines);

        // Act
        var result = await _patientService.GetMedicationRemindersAsync(token);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("1", result.First().ReminderId);
        Assert.Equal("Med 1", result.First().Medication_Name);
        Assert.Equal("Dosage 1", result.First().Dosage);
        Assert.Equal(MedcineType.Pill, result.First().MedcineType);
        Assert.Equal(RepeatType.Four_Times, result.First().Repeater);
    }

    [Fact]
    public async Task GetMedicationRemindersAsync_ShouldReturnEmpty_WhenPatientDoesNotExist()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _medicinesRepositoryMock.Setup(x => x.WhereAsync(It.IsAny< Expression<Func<Medication_Reminders, bool>>>())).ReturnsAsync((IQueryable<Medication_Reminders>)null);

        // Act
        var result = await _patientService.GetMedicationRemindersAsync(token);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldReturnSuccess_WhenPatientExists()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var patient = new Patient { Id = patientId };
        var updateDto = new UpdateMyProfileDto { Age = 71, PhoneNumber = "0987654321" };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);

        // Act
        var result = await _patientService.UpdateProfileAsync(token, updateDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Profile Updated Successfully :D", result.message);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldReturnError_WhenPatientDoesNotExist()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJkOTdjZWQ2My1hMjA5LTQ3MGMtYmFkYy0zZjA2MTYyNWM0NDMiLCJlbWFpbCI6ImVzbGFtYWhtZWR3b3JrNEBnbWFpbC5jb20iLCJGdWxsTmFtZSI6Itin2YTZhdix2YrYtiAvINiz2YrYryDYp9mE2YXZh9iv2YogIiwiUGhvbmVOdW1iZXIiOiIwMTI4MzQzOTIwMiIsInVpZCI6IjIxMjYwZjQ1LWViOTUtNDZiMy05M2Q0LTU5NGNjODkwMjUwOCIsIlVzZXJBdmF0YXIiOiJodHRwczovL2VsZWN0cm9uaWNtaW5kb2ZhbHpoZWltZXJwYXRpZW50cy5henVyZXdlYnNpdGVzLm5ldC9Vc2VyIEF2YXRhci8yMTI2MGY0NS1lYjk1LTQ2YjMtOTNkNC01OTRjYzg5MDI1MDhfMzE1MTBkZmUtYWEzMC00YmZkLTkyYWItYmRjOWRlNmYyYzJjLmpwZyIsIk1haW5MYXRpdHVkZSI6IjMwLjAwMTkxMiIsIk1haW5Mb25naXR1ZGUiOiIzMS4zMzQ3MzciLCJyb2xlcyI6IlBhdGllbnQiLCJNYXhEaXN0YW5jZSI6IjI1IiwiZXhwIjoxNzI2NTM0MjY0LCJpc3MiOiJBcnRPZkNvZGluZyIsImF1ZCI6IkFsemhlaW1hckFwcCJ9.3M3Su5kB3VpZQdKunxsQ89A9qI4cxKKuwoxWUjA0H1U";
        var patientId = "21260f45-eb95-46b3-93d4-594cc8902508";
        var updateDto = new UpdateMyProfileDto { Age = 71, PhoneNumber = "0987654321" };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _patientService.UpdateProfileAsync(token, updateDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("No Patient With this ID!", result.message);
    }

    [Fact]
    public async Task GetMediaAsync_ShouldReturnMedia_WhenPatientExists()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var patient = new Patient { Id = patientId };
        var media = new List<Media>
        {
            new Media { Media_Id = "1", PatientId = patientId, Caption = "Caption 1", Image_Path = "Path 1", Upload_Date = DateTime.Now, Extension = ".jpg", family = new Family { FullName = "Family 1" } },
            new Media { Media_Id = "2", PatientId = patientId, Caption = "Caption 2", Image_Path = "Path 2", Upload_Date = DateTime.Now, Extension = ".png", family = new Family { FullName = "Family 2" } }
        }.AsQueryable();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _mediaRepositoryMock.Setup(x => x.Include(It.IsAny< Expression<Func<Media, object>>>())).Returns(media);

        // Act
        var result = await _patientService.GetMediaAsync(token);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("1", result.First().MediaId);
        Assert.Equal("Caption 1", result.First().Caption);
        Assert.Equal("Path 1", result.First().MediaUrl);
        Assert.Equal(".jpg", result.First().MediaExtension);
        Assert.Equal("Family 1", result.First().FamilyNameWhoUpload);
    }

    [Fact]
    public async Task GetMediaAsync_ShouldReturnEmpty_WhenPatientDoesNotExist()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _patientService.GetMediaAsync(token);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddGameScoreAsync_ShouldReturnSuccess_WhenPatientExists()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var patient = new Patient { Id = patientId, CurrentScore = 100, MaximumScore = 200 };
        var gameScoreDto = new PostGameScoreDto { DifficultyGame = Difficulty.Meduim, PatientScore = 7 };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);

        // Act
        var result = await _patientService.AddGameScoreAsync(token, gameScoreDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Contains("Your score is", result.message);
    }

    [Fact]
    public async Task AddGameScoreAsync_ShouldReturnError_WhenPatientDoesNotExist()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var gameScoreDto = new PostGameScoreDto { DifficultyGame = Difficulty.Meduim, PatientScore = 7 };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _patientService.AddGameScoreAsync(token, gameScoreDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("No Patient With this ID!", result.message);
    }

    [Fact]
    public async Task GetRecommendedScoreAsync_ShouldReturnScores_WhenPatientExists()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var patient = new Patient { Id = patientId, CurrentScore = 250, MaximumScore = 500 };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetRecommendedScoreAsync(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(250, result.Score.CurrentScore);
        Assert.Equal(500, result.Score.MaxScore);
        Assert.Equal(1, result.RecommendedGameDifficulty);
    }

    [Fact]
    public async Task GetRecommendedScoreAsync_ShouldReturnNull_WhenPatientDoesNotExist()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _patientService.GetRecommendedScoreAsync(token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetGameScoresAsync_ShouldReturnScores_WhenPatientExists()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var patient = new Patient { Id = patientId, CurrentScore = 250 };
        var gameScores = new List<GameScore>
        {
            new GameScore { GameScoreId = "1", PatientId = patientId, DifficultyGame = Difficulty.Easy, PatientScore = 5, GameDate = DateTime.Now },
            new GameScore { GameScoreId = "2", PatientId = patientId, DifficultyGame = Difficulty.Meduim, PatientScore = 6, GameDate = DateTime.Now }
        }.AsQueryable();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _gameScoreRepositoryMock.Setup(x => x.Where(It.IsAny< Expression<Func<GameScore, bool>>>())).Returns(gameScores);

        // Act
        var result = await _patientService.GetGameScoresAsync(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.GameScore.Count);
        Assert.Equal(1, result.RecomendationDifficulty);
    }

    [Fact]
    public async Task GetGameScoresAsync_ShouldReturnNull_WhenPatientDoesNotExist()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _patientService.GetGameScoresAsync(token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddSecretFileAsync_ShouldReturnSuccess_WhenFileIsAdded()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var patient = new Patient { Id = patientId };
        var file = new Mock<IFormFile>();
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        writer.Write("Test file content");
        writer.Flush();
        memoryStream.Position = 0;
        file.Setup(f => f.OpenReadStream()).Returns(memoryStream);
        file.Setup(f => f.FileName).Returns("test.txt");
        file.Setup(f => f.Length).Returns(memoryStream.Length);
        var secretFileDto = new PostSecretFileDto { File = file.Object, FileName = "Secret File", File_Description = "Description" };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _envMock.Setup(e => e.WebRootPath).Returns("wwwroot");

        // Act
        var result = await _patientService.AddSecretFileAsync(token, secretFileDto);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Secret file added successfully", result.message);
    }

    [Fact]
    public async Task AddSecretFileAsync_ShouldReturnError_WhenPatientIdIsInvalid()
    {
        // Arrange
        var token = "invalidToken";
        var secretFileDto = new PostSecretFileDto { File = null, FileName = "Secret File", File_Description = "Description" };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns((string)null);

        // Act
        var result = await _patientService.AddSecretFileAsync(token, secretFileDto);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("Invalid patient ID", result.message);
    }

    [Fact]
    public async Task AskToViewSecretFileAsync_ShouldReturnSuccess_WhenVideoIsUploaded()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var patient = new Patient { Id = patientId, FullName = "John Doe" };
        var videoFile = new Mock<IFormFile>();
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        writer.Write("Test video content");
        writer.Flush();
        memoryStream.Position = 0;
        videoFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
        videoFile.Setup(f => f.FileName).Returns("video.mp4");
        videoFile.Setup(f => f.Length).Returns(memoryStream.Length);

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _patientRepositoryMock.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _envMock.Setup(e => e.WebRootPath).Returns("wwwroot");
        _mailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _patientService.AskToViewSecretFileAsync(token, videoFile.Object);

        // Assert
        Assert.False(result.HasError);
        Assert.Equal("Video uploaded successfully for review", result.message);
    }

    [Fact]
    public async Task AskToViewSecretFileAsync_ShouldReturnError_WhenVideoFileIsEmpty()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var videoFile = new Mock<IFormFile>();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        videoFile.Setup(f => f.Length).Returns(0);

        // Act
        var result = await _patientService.AskToViewSecretFileAsync(token, videoFile.Object);

        // Assert
        Assert.True(result.HasError);
        Assert.Equal("Video file is empty", result.message);
    }

    [Fact]
    public async Task GetPatientRelatedMembersAsync_ShouldReturnMembers_WhenMembersExist()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var families = new List<Family>
        {
            new Family { Id = "1", PatientId = patientId, FullName = "Family 1", Relationility = "Relation 1", imageUrl = "Path 1", DescriptionForPatient = "Description 1" }
        }.AsQueryable();
        var persons = new List<PersonWithoutAccount>
        {
            new PersonWithoutAccount { Id = "2", PatientId = patientId, FullName = "Person 1", Relationility = "Relation 2", ImageUrl = "Path 2", DescriptionForPatient = "Description 2" }
        }.AsQueryable();

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _familyRepositoryMock.Setup(x => x.WhereAsync(It.IsAny<Expression<Func<Family, bool>>>())).ReturnsAsync(families);
        _personRepositoryMock.Setup(x => x.WhereAsync(It.IsAny<Expression<Func<PersonWithoutAccount, bool>>>())).ReturnsAsync(persons);

        // Act
        var result = await _patientService.GetPatientRelatedMembersAsync(token);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPatientRelatedMembersAsync_ShouldReturnEmpty_WhenMembersDoNotExist()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _familyRepositoryMock.Setup(x => x.WhereAsync(It.IsAny<Expression<Func<Family, bool>>>())).ReturnsAsync((IQueryable<Family>)null);
        _personRepositoryMock.Setup(x => x.WhereAsync(It.IsAny<Expression<Func<PersonWithoutAccount, bool>>>())).ReturnsAsync((IQueryable<PersonWithoutAccount>)null);

        // Act
        var result = await _patientService.GetPatientRelatedMembersAsync(token);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFamilyLocation_ShouldReturnLocation_WhenFamilyExists()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var familyId = "familyId";
        var family = new Family { Id = familyId, PatientId = patientId, MainLatitude = 10.0, MainLongitude = 20.0 };

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync(family);

        // Act
        var result = await _patientService.GetFamilyLocation(token, familyId);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, result.Code);
        Assert.Equal("Family Location Found", result.Message);
        Assert.Equal(10.0, result.Latitude);
        Assert.Equal(20.0, result.Longitude);
    }

    [Fact]
    public async Task GetFamilyLocation_ShouldReturnError_WhenFamilyDoesNotExist()
    {
        // Arrange
        var token = "validToken";
        var patientId = "patientId";
        var familyId = "familyId";

        _jwtDecodeMock.Setup(x => x.GetUserIdFromToken(token)).Returns(patientId);
        _familyRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync((Family)null);
        _personRepositoryMock.Setup(x => x.GetByIdAsync(familyId)).ReturnsAsync((PersonWithoutAccount)null);

        // Act
        var result = await _patientService.GetFamilyLocation(token, familyId);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.Code);
        Assert.Equal("Invalid Family Id", result.Message);
    }

    // Add more tests for remaining methods and scenarios
}


