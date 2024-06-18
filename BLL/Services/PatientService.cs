using BLL.DTOs;
using BLL.DTOs.CaregiverDto;
using BLL.DTOs.FamilyDto;
using BLL.DTOs.PatientDto;
using BLL.Helper;
using BLL.Hubs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        public IHubContext<MedicineReminderHub> _hubContext { get; }
        private readonly IBaseRepository<Patient> _patient;
        private readonly IBaseRepository<Medication_Reminders> _medicines;
        private readonly IBaseRepository<Mark_Medicine_Reminder> _Mark_Medicine_Reminder;
        private readonly IBaseRepository<Appointment> _appointments;
        private readonly IBaseRepository<Family> _family;
        private readonly IBaseRepository<PersonWithoutAccount> _person;
        private readonly IDecodeJwt _jwtDecode;
        private readonly IBaseRepository<Media> _media;
        private readonly Mail _mail;
        private readonly IWebHostEnvironment _env;

        private readonly IBaseRepository<SecretAndImportantFile> _secret;
        private readonly IMailService _mailService;

        private readonly IBaseRepository<GameScore> _gameScore;

        public PatientService
            (
            IHubContext<MedicineReminderHub> hubContext, IDecodeJwt jwtDecode,
            IBaseRepository<Patient> patient,
            IBaseRepository<Medication_Reminders> medicines,
            IBaseRepository<Appointment> appointments,
            IBaseRepository<Family> family,
            IBaseRepository<Media> media,
            IWebHostEnvironment env,
            IOptions<Mail> Mail,
            IMailService mailService,
            IBaseRepository<SecretAndImportantFile> secret,
            IBaseRepository<GameScore> gameScore,
            IBaseRepository<Mark_Medicine_Reminder> Mark_Medicine_Reminder,
            IBaseRepository<PersonWithoutAccount> person

            )
        {
            _person = person;
            _hubContext = hubContext;
            _jwtDecode = jwtDecode;
            _patient = patient;
            _medicines = medicines;
            _appointments = appointments;
            _family = family;
            _media = media;
            _mail = Mail.Value;
            _env = env;

            _secret = secret;
            _mailService = mailService;

            _gameScore = gameScore;
            _Mark_Medicine_Reminder = Mark_Medicine_Reminder;

        }


        public async Task<GetPatientProfileDto> GetPatientProfileAsync(string token)
        {
            string? patientid = _jwtDecode.GetUserIdFromToken(token);

            if (patientid == null)
            {
                return new GetPatientProfileDto
                {
                    Message = "Invalid Patient ID",
                    HasError = true
                };
            }
            var patinet = await _patient.GetByIdAsync(patientid);
            if (patinet == null)
            {
                return new GetPatientProfileDto
                {
                    Message = "Invalid Patient ID",
                    HasError = true
                };
            }
            return new GetPatientProfileDto
            {
                PatientId = patientid,
                FullName = patinet.FullName,
                Age = patinet.Age,
                DiagnosisDate = patinet.DiagnosisDate,
                PhoneNumber = patinet.PhoneNumber,
                Message = "Welcome To Your Profile",
                HasError = false
            };
        }
        public async Task<IEnumerable<GetAppointmentDto>> GetAppointmentAsync(string token)
        {
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if (patient == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            var appointments = _appointments.Include(s => s.family).Where(s => s.PatientId == PatientId);
            if (appointments == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            return appointments.Select(s => new GetAppointmentDto
            {
                AppointmentId = s.AppointmentId,
                Date = s.Date,
                Location = s.Location,
                Notes = s.Notes,
                FamilyNameWhoCreatedAppointemnt = s.family.FullName,
            }).ToList();
        }
        public async Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token)
        {
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<MedicationReminderGetDto>();
            }
            var medicines = await _medicines.WhereAsync(s => s.Patient_Id == PatientId);
            if (medicines == null)
            {
                return Enumerable.Empty<MedicationReminderGetDto>();
            }
            return medicines.Select(s => new MedicationReminderGetDto
            {
                Medication_Name = s.Medication_Name,
                ReminderId = s.Reminder_ID,
                StartDate = s.StartDate,
                Dosage = s.Dosage,
                Repeater = s.Repeater,
                EndDate = s.EndDate,
                MedcineType = s.Medcine_Type
            }).ToList();
        }
        public async Task<GlobalResponse> UpdateProfileAsync(string token, UpdateMyProfileDto updatePatientProfile)
        {
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Invalid patient Id !"
                };
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if (patient == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "No Patient With this ID!"
                };
            }
            patient.Age = updatePatientProfile.Age;
            patient.PhoneNumber = updatePatientProfile.PhoneNumber;

            await _patient.UpdateAsync(patient);
            return new GlobalResponse
            {
                HasError = false,
                message = "Profile Updated Successfully :D"
            };
        }
        public async Task<IEnumerable<GetMediaforPatientDto>> GetMediaAsync(string token)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<GetMediaforPatientDto>();
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if (patient == null)
            {
                return Enumerable.Empty<GetMediaforPatientDto>();
            }
            var media = await _media.Include(s => s.patient).Include(s => s.family).Where(s => s.PatientId == PatientId).ToListAsync();
            var res = media.Select(s => new GetMediaforPatientDto
            {
                Caption = s.Caption,
                MediaUrl = GetMediaUrl(s.Image_Path),
                MediaId = s.Media_Id,
                Uploaded_date = s.Upload_Date,
                MediaExtension = s.Extension,
                FamilyNameWhoUpload = s.family.FullName
            }).ToList();
            return res;
        }

        public async Task<GlobalResponse> AddGameScoreAsync(string token, PostGameScoreDto gameScoreDto)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "Invalid patient Id !"
                };
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if (patient == null)
            {
                return new GlobalResponse
                {
                    HasError = true,
                    message = "No Patient With this ID!"
                };
            }
            var gamescore = new GameScore
            {
                DifficultyGame = gameScoreDto.DifficultyGame,
                PatientScore = gameScoreDto.PatientScore,
                PatientId = PatientId,
                GameDate = DateTime.UtcNow.AddHours(2)
            };
            await _gameScore.AddAsync(gamescore);
            int score = 0;
            if (gameScoreDto.DifficultyGame == Difficulty.Easy)
            {

                if (gameScoreDto.PatientScore >= 3)
                {
                    score = gameScoreDto.PatientScore * 10;
                }
                else
                {
                    score = (3 - gameScoreDto.PatientScore) * -10;

                }


            }
            else if (gameScoreDto.DifficultyGame == Difficulty.Meduim)
            {
                if (gameScoreDto.PatientScore >= 6)
                {
                    score = gameScoreDto.PatientScore * 10;
                }
                else
                {
                    score = (6 - gameScoreDto.PatientScore) * -10;

                }

            }
            else if (gameScoreDto.DifficultyGame == Difficulty.Hard)
            {
                if (gameScoreDto.PatientScore >= 9)
                {
                    score = gameScoreDto.PatientScore * 10;
                }
                else
                {
                    score = (9 - gameScoreDto.PatientScore) * -10;


                }
            }
            patient.CurrentScore += score;
            if (patient.CurrentScore < 0)
            {
                patient.CurrentScore = 0;
            }
            patient.MaximumScore = (patient.CurrentScore > patient.MaximumScore) ? patient.CurrentScore : patient.MaximumScore;
            await _patient.UpdateAsync(patient);
            var message = $"Your score is {score} and your current score is {patient.CurrentScore} and your maximum score is {patient.MaximumScore}";
            return new GlobalResponse
            {
                HasError = false,
                message = message
            };
        }


        public async Task<CurrentAndMaxScoreDto?> GetRecommendedScoreAsync(string token)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return null;
            }
            var Patient = await _patient.GetByIdAsync(PatientId);
            if (Patient == null)
            {
                return null;
            }

            var gameScoresDto = new ScoreDto
            {
                CurrentScore = Patient.CurrentScore,
                MaxScore = Patient.MaximumScore,
            };
            if (gameScoresDto == null)
            {
                return null;
            }
            if (Patient.CurrentScore >= 200 && Patient.CurrentScore < 400)
            {
                return new CurrentAndMaxScoreDto
                {
                    Score = gameScoresDto,
                    RecommendedGameDifficulty = 1
                };
            }
            else if (Patient.CurrentScore >= 400)
            {
                return new CurrentAndMaxScoreDto
                {
                    Score = gameScoresDto,
                    RecommendedGameDifficulty = 2
                };
            }
            else
            {
                return new CurrentAndMaxScoreDto
                {
                    Score = gameScoresDto,
                    RecommendedGameDifficulty = 0
                };
            }

        }

        public async Task<GetGameScoresDto?> GetGameScoresAsync(string token)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return null;
            }
            var Patient = await _patient.GetByIdAsync(PatientId);
            if (Patient == null)
            {
                return null;
            }
            var gamescores = _gameScore.Where(s => s.PatientId == PatientId).Select(s => new GameScoreDto
            {
                GameScoreId = s.GameScoreId,
                DifficultyGame = s.DifficultyGame,
                PatientScore = s.PatientScore,
                GameDate = s.GameDate,
            }).ToList();
            if (Patient.CurrentScore >= 200 && Patient.CurrentScore < 400)
            {
                return new GetGameScoresDto
                {
                    GameScore = gamescores,
                    RecomendationDifficulty = 1
                };
            }
            else if (Patient.CurrentScore >= 400)
            {
                return new GetGameScoresDto()
                {
                    GameScore = gamescores,
                    RecomendationDifficulty = 2
                };
            }
            else
            {
                return new GetGameScoresDto
                {
                    GameScore = gamescores,
                    RecomendationDifficulty = 0
                };
            }
        }


        public async Task<GlobalResponse> AddSecretFileAsync(string token, PostSecretFileDto secretFileDto)

        {
            try
            {
                string patientId = _jwtDecode.GetUserIdFromToken(token);
                if (patientId == null)
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Invalid patient ID"
                    };
                }
                string fileID = Guid.NewGuid().ToString();
                string filepath = Path.Combine(patientId, $"{patientId}_{fileID}{Path.GetExtension(secretFileDto.File.FileName)}");
                string directorypath = Path.Combine(_env.WebRootPath, patientId, "SecretFiles");
                if (!Directory.Exists(directorypath))
                {
                    Directory.CreateDirectory(directorypath);
                }
                using (FileStream filestream = File.Create(Path.Combine(_env.WebRootPath, filepath)))
                {
                    await secretFileDto.File.CopyToAsync(filestream);
                    filestream.Flush();
                }


                var secretFile = new SecretAndImportantFile
                {
                    File_Id = fileID,
                    FileName = secretFileDto.FileName,
                    File_Description = secretFileDto.File_Description,
                    DocumentPath = filepath,
                    DocumentExtension = Path.GetExtension(secretFileDto.File.FileName),
                    permissionEndDate = DateTime.Now.AddDays(1),

                    PatientId = patientId
                };


                await _secret.AddAsync(secretFile);

                return new GlobalResponse
                {
                    HasError = false,
                    message = "Secret file added successfully"
                };
            }
            catch
            {

                return new GlobalResponse
                {
                    HasError = true,
                    message = "An error occurred while adding the secret file",

                };
            }
        }

        public async Task<GlobalResponse> AskToViewSecretFileAsync(string token, IFormFile videoFile)
        {
            try
            {
                string patientId = _jwtDecode.GetUserIdFromToken(token);

                if (patientId == null)
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Invalid patient ID"
                    };
                }

                // Check if the video file is null or empty
                if (videoFile == null || videoFile.Length == 0)
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Video file is empty"
                    };
                }

                // Save the video file and get the File_Id
                string filepath = await SaveVideoAsync(patientId, videoFile);
                var patient = await _patient.GetByIdAsync(patientId);
                if (filepath != null)
                {
                    // Video saved successfully, return the File_Id for further processing
                    var result = await _mailService.SendEmailAsync("hazemzizo@gmail.com", _mail.FromMail, _mail.Password, $"Patient{patient.FullName} has sent a request to view his secret files", $"Video uploaded successfully for review. video Link: {GetMediaUrl(filepath)}");
                    if (result == true)
                    {
                        return new GlobalResponse
                        {
                            HasError = false,
                            message = $"Video uploaded successfully for review"
                        };
                    }
                    else
                    {
                        return new GlobalResponse
                        {
                            HasError = true,
                            message = "Failed to save the video, Try again later"
                        };
                    }

                }
                else
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Failed to save the video"
                    };
                }
            }
            catch
            {
                // Handle exceptions
                return new GlobalResponse
                {
                    HasError = true,
                    message = "An error occurred "
                };
            }
        }


        private async Task<string> SaveVideoAsync(string patientId, IFormFile videoFile)
        {
            try
            {
                string fileID = Guid.NewGuid().ToString();
                string fileName = $"{patientId}_{fileID}{Path.GetExtension(videoFile.FileName)}";

                string directoryPath = Path.Combine(_env.WebRootPath, patientId, "VideoForReview");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                    stream.Flush();
                }
                return filePath;
            }
            catch
            {
                return "Error saving video file";
            }
        }
        public async Task<GlobalResponse> ApproveVideoAsync(string fileId)
        {
            try
            {
                var secretFile = await _secret.FindAsync(s => s.File_Id == fileId);
                if (secretFile == null)
                {
                    return new GlobalResponse
                    {
                        HasError = true,
                        message = "Secret file not found"
                    };
                }

                secretFile.permissionEndDate = DateTime.Now.AddDays(1);

                await _secret.UpdateAsync(secretFile);

                return new GlobalResponse
                {
                    HasError = false,
                    message = "Secret file permission updated successfully"
                };
            }
            catch
            {

                return new GlobalResponse
                {
                    HasError = true,
                    message = "there is an error in approving the secret file"
                };
            }
        }

        public async Task<GetAllSecretFileDto?> GetSecretFilesAsync(string token)
        {

            if (string.IsNullOrEmpty(token))
            {
                return new GetAllSecretFileDto
                {
                    Code = StatusCodes.Status400BadRequest,
                };
            }

            string patientId = _jwtDecode.GetUserIdFromToken(token);

            if (string.IsNullOrEmpty(patientId))
            {
                return new GetAllSecretFileDto
                {
                    Code = StatusCodes.Status400BadRequest,
                };
            }

            var patient = await _patient.GetByIdAsync(patientId);

            if (patient == null)
            {
                return new GetAllSecretFileDto
                {
                    Code = StatusCodes.Status400BadRequest,
                };
            }

            var secretFiles = await _secret.WhereAsync(s => s.PatientId == patientId);

            if (secretFiles == null || !secretFiles.Any())
            {
                return new GetAllSecretFileDto
                {
                    Code = StatusCodes.Status404NotFound,
                };
            }


            var result = secretFiles.Select(s => new GetSecretFileDto
            {
                SecretId = s.File_Id,
                FileName = s.FileName,
                File_Description = s.File_Description,
                NeedToConfirm = !s.hasPermission,
                DocumentUrl = s.hasPermission == true ? GetMediaUrl(s.DocumentPath) : null,
                DocumentExtension = s.hasPermission == true ? s.DocumentExtension : null,

            }).ToList();

            return new GetAllSecretFileDto
            {
                Code = StatusCodes.Status200OK,
                SecretFiles = result,
            };

        }




        public async Task<GlobalResponse> MarkMedicationReminderAsync(string token, MarkMedictaionDto markMedictaionDto)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return new GlobalResponse()
                {
                    HasError = true,
                    message = "Invalid Patient Id"
                };
            }
            var reminder = await _medicines.FindAsync(i => i.Reminder_ID == markMedictaionDto.MedictaionId && i.Patient_Id == PatientId);
            if (reminder == null)
            {
                return new GlobalResponse()
                {
                    HasError = true,
                    message = "Medication Reminder with This Id is Not found "
                };
            }

            var MarkMedctaion = new Mark_Medicine_Reminder
            {

                MedicationReminderId = markMedictaionDto.MedictaionId,
                IsTaken = markMedictaionDto.IsTaken,
                MarkTime = DateTime.Now,

            };

            await _Mark_Medicine_Reminder.AddAsync(MarkMedctaion);

            return new GlobalResponse()
            {
                HasError = false,
                message = "Medication Reminder Marked Successfully ."
            };


        }

        public async Task<IEnumerable<GetFamiliesDto>> GetPatientRelatedMembersAsync(string token)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<GetFamiliesDto>();
            }
            var families = await _family.WhereAsync(s => s.PatientId == PatientId);
            if (families == null || families.Count() == 0)
            {
                return Enumerable.Empty<GetFamiliesDto>();
            }
            List<GetFamiliesDto> result = new List<GetFamiliesDto>();
            var Families = families.Select(s => new GetFamiliesDto
            {
                FamilyId = s.Id,
                FamilyName = s.FullName,
                Relationility = s.Relationility,
                HisImageUrl = (s.imageUrl == null) ? "" : GetMediaUrl(s.imageUrl),
                FamilyDescriptionForPatient = s.DescriptionForPatient

            }).ToList();
            result.AddRange(Families);
            var person = await _person.WhereAsync(i => i.PatientId == PatientId);
            if (person != null)
            {
                var persons = person.Select(s => new GetFamiliesDto()
                {
                    FamilyId = s.Id,
                    FamilyName = s.FullName,
                    Relationility = s.Relationility,
                    HisImageUrl = (s.ImageUrl == null) ? "" : GetMediaUrl(s.ImageUrl),
                    FamilyDescriptionForPatient = s.DescriptionForPatient
                }).ToList();
                result.AddRange(persons);
            }
            return result;
        }

        public async Task<GetFamilyLocationDto?> GetFamilyLocation(string token, string familyId)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return new GetFamilyLocationDto()
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "Invalid Patient Id"
                };
            }
            var family = await _family.GetByIdAsync(familyId);
            if (family == null)
            {
                var person = await _person.GetByIdAsync(familyId);
                if (person == null)
                {
                    return new GetFamilyLocationDto()
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Invalid Family Id"
                    };
                }

                if (person.PatientId != PatientId)
                {
                    return new GetFamilyLocationDto()
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Patient Id Not Found"
                    };
                }
                return new GetFamilyLocationDto
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Family Location Found",
                    Latitude = person.MainLatitude,
                    Longitude = person.MainLongitude
                };

            }
            if (family.PatientId != PatientId)
            {
                return new GetFamilyLocationDto()
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "Patient Id Not Found"
                };
            }
            if (family.MainLatitude == null || family.MainLongitude == null)
            {
                return new GetFamilyLocationDto()
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "Family Location Not Found"
                };
            }
            return new GetFamilyLocationDto
            {
                Code = StatusCodes.Status200OK,
                Message = "Family Location Found",
                Latitude = family.MainLatitude,
                Longitude = family.MainLongitude
            };

        }



        public async Task<RecognitionResult> ImageRecognition(PostImageRecognitionDto postImageRecognitionDto, string token)
        {
            var response = await RecognizeImage(postImageRecognitionDto, _jwtDecode.GetUserIdFromToken(token));
            var jsonObject = JObject.Parse(response);
            var recognitionResults = (JArray)jsonObject["recognition_results"];
            var recognitionResult = new RecognitionResult();

            if (postImageRecognitionDto.Image != null && postImageRecognitionDto.Image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    postImageRecognitionDto.Image.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    using (var image = new Bitmap(memoryStream))
                    using (var graphics = Graphics.FromImage(image))
                    {
                        var font = new Font("Arial", 30, FontStyle.Bold);
                        var brush = new SolidBrush(Color.Cyan);
                        var personsInImage = new List<PersonInImage>();
                        var format = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };

                        foreach (JObject result in recognitionResults)
                        {
                            var identifiedName = result.Value<string>("identified_name");
                            var realName = GetRealName(identifiedName, out bool isPersonWithoutAccount, out Family familyMember, out PersonWithoutAccount person);

                            var faceLocation = (JArray)result["face_location"];
                            var (left, top, width, height) = GetRectangleCoordinates(faceLocation);

                            DrawFaceRectangle(graphics, left, top, width, height);

                            var x = (faceLocation[1].Value<int>() + faceLocation[3].Value<int>()) / 2;
                            var y = faceLocation[2].Value<int>() + 40;
                            font = AdjustFontSize(graphics, realName, font, width);

                            DrawName(graphics, realName, font, brush, x, y, format);
                            personsInImage.Add(CreatePersonInImage(isPersonWithoutAccount, realName, familyMember, person));
                        }

                        recognitionResult.ImageAfterResultUrl = SaveImage(image, "PhotoAfterRecognition", 35L);
                        recognitionResult.PersonsInImage = personsInImage;
                        recognitionResult.GlobalResponse = new GlobalResponse { HasError = false, message = "Image recognition successful" };
                    }
                }
            }
            return recognitionResult;
        }

        private string GetRealName(string identifiedName, out bool isPersonWithoutAccount, out Family familyMember, out PersonWithoutAccount person)
        {
            isPersonWithoutAccount = false;
            familyMember = null;
            person = null;

            if (identifiedName == "Unknown")
            {
                return "Unknown";
            }

            familyMember = _family.Find(i => i.Id == identifiedName);
            if (familyMember == null)
            {
                person = _person.Find(i => i.Id == identifiedName);
                if (person != null)
                {
                    isPersonWithoutAccount = true;
                    return person.FullName;
                }
            }
            else
            {
                return familyMember.FullName;
            }
            return "Unknown";
        }

        private (int left, int top, int width, int height) GetRectangleCoordinates(JArray faceLocation)
        {
            int left = faceLocation[3].Value<int>();
            int top = faceLocation[0].Value<int>();
            int width = faceLocation[2].Value<int>() - faceLocation[0].Value<int>();
            int height = faceLocation[1].Value<int>() - faceLocation[3].Value<int>();
            return (left, top, width, height);
        }

        private void DrawFaceRectangle(Graphics graphics, int left, int top, int width, int height)
        {
            var fillColor = Color.FromArgb(30, Color.Cyan);
            var fillBrush = new SolidBrush(fillColor);
            var borderPen = new Pen(Color.Cyan, 1.5f) { DashStyle = DashStyle.Dash };

            graphics.FillRectangle(fillBrush, left, top, width, height);
            graphics.DrawRectangle(borderPen, left, top, width, height);
        }

        private void DrawName(Graphics graphics, string realName, Font font, SolidBrush brush, int x, int y, StringFormat format)
        {
            graphics.DrawString(realName, font, brush, x, y, format);
        }

        private Font AdjustFontSize(Graphics graphics, string realName, Font font, int width)
        {
            float fontSize = FitFontSize(graphics, realName, font, width);
            return new Font("Arial", fontSize, FontStyle.Bold);
        }

        private PersonInImage CreatePersonInImage(bool isPersonWithoutAccount, string realName, Family familyMember, PersonWithoutAccount person)
        {
            if (isPersonWithoutAccount)
            {
                return new PersonInImage
                {
                    FamilyName = person?.FullName ?? "Unknown",
                    FamilyLatitude = person?.MainLatitude,
                    FamilyLongitude = person?.MainLongitude,
                    FamilyPhoneNumber = person?.PhoneNumber ?? "Unknown",
                    RelationalityOfThisPatient = person?.Relationility ?? "Unknown",
                    FamilyAvatarUrl = GetMediaUrl(person?.ImageUrl),
                    DescriptionForPatient = person?.DescriptionForPatient
                };
            }
            else
            {
                return new PersonInImage
                {
                    FamilyName = familyMember?.FullName ?? "Unknown",
                    FamilyLatitude = familyMember?.MainLatitude,
                    FamilyLongitude = familyMember?.MainLongitude,
                    FamilyPhoneNumber = familyMember?.PhoneNumber ?? "Unknown",
                    RelationalityOfThisPatient = familyMember?.Relationility ?? "Unknown",
                    FamilyAvatarUrl = GetMediaUrl(familyMember?.imageUrl),
                    DescriptionForPatient = familyMember?.DescriptionForPatient
                };
            }
        }

        private string SaveImage(Bitmap image, string folder, long quality)
        {
            string mediaId = Guid.NewGuid().ToString();
            string filePath = Path.Combine(folder, $"{mediaId}.jpg");
            string directoryPath = Path.Combine(_env.WebRootPath, folder);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var jpegCodec = GetEncoderInfo(ImageFormat.Jpeg);
            var encoderParameters = new EncoderParameters(1)
            {
                Param = { [0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality) }
            };

            using (var fileStream = File.Create(Path.Combine(_env.WebRootPath, filePath)))
            {
                image.Save(fileStream, jpegCodec, encoderParameters);
            }

            return GetMediaUrl(filePath);
        }

        private async Task<string> RecognizeImage(PostImageRecognitionDto postImageRecognitionDto, string PatinetId)
        {
            string endpoint = "https://excited-hound-vastly.ngrok-free.app/recognize_faces";

            using (HttpClient httpClient = new HttpClient())
            {

                var multipartContent = new MultipartFormDataContent();


                var queryParameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "patient_id", PatinetId },

                };


                multipartContent.Add(new StreamContent(postImageRecognitionDto.Image.OpenReadStream()), "image", "image.jpg");


                var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                foreach (var param in queryParameters)
                {
                    queryString[param.Key] = param.Value;
                }

                var fullUrl = endpoint + "?" + queryString;


                var response = await httpClient.PostAsync(fullUrl, multipartContent);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {

                    return responseBody;
                }
                else
                {

                    return responseBody;
                }
            }
        }
        private string GetMediaUrl(string imagePath)
        {

            string baseUrl = _mail.ServerLink;
            string relativePath = imagePath.Replace(_env.WebRootPath, "").Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
        }
        private float FitFontSize(Graphics graphics, string text, Font font, int width)
        {
            SizeF textSize = graphics.MeasureString(text, font);
            float ratio = width / textSize.Width;
            float newSize = (font.Size * ratio) + 8;
            return newSize;
        }
        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
