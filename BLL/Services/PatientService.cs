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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            IBaseRepository<Mark_Medicine_Reminder>Mark_Medicine_Reminder

            )
        {
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
        public async Task<GlobalResponse> UpdateProfileAsync(string token, UpdatePatientProfileDto updatePatientProfile)
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
            patient.DiagnosisDate = updatePatientProfile.DiagnosisDate.ToDateTime(TimeOnly.MinValue);
            patient.MaximumDistance = updatePatientProfile.MaximumDistance;
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
                PatientId = PatientId
            };
            await _gameScore.AddAsync(gamescore);
            int score = 0;
            if (gameScoreDto.DifficultyGame == Difficulty.Easy)
            {
               
                if (gameScoreDto.PatientScore >= 3)
                {
                    score = gameScoreDto.PatientScore*10;
                }
                else
                {
                    score = (3-gameScoreDto.PatientScore)*-10 ;
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
            patient.MaximumScore = (patient.CurrentScore > patient.MaximumScore) ? patient.CurrentScore : patient.MaximumScore;
            var message = $"Your score is {score} and your current score is {patient.CurrentScore} and your maximum score is {patient.MaximumScore}";
            return new GlobalResponse
            {
                HasError = false,
                message = message
            };
        }
        #region Old get game scores
        /*     public async Task<GetGameScoresDto> GetGameScoresAsync(string token)
             {
                 string? PatientId = _jwtDecode.GetUserIdFromToken(token);
                 if (PatientId == null)
                 {
                     return null;
                 }
                 var gamescores = _gameScore.Where(s => s.PatientId == PatientId).ToList();

                 if (gamescores == null)
                 {
                     return null;
                 }
                 Dictionary<Difficulty, (int wins, int losses)> difficultyState = new Dictionary<Difficulty, (int wins, int losses)>();


                 var groupedGamescores = gamescores.GroupBy(s => s.DifficultyGame);
                 int recommendedDifficulty = 0;

                 foreach ( var group in groupedGamescores)
                 {
                     var diff = group.Key;


                     int wins = 0;
                     int losses = 0;

                     foreach (var score in group)
                     {
                         if(score.PatientScore >= score.MaxScore / 2)
                         {
                             wins++;
                             losses = 0;
                         }
                         else
                         {
                             losses++;
                         }
                     }
                     difficultyState.Add(diff, (wins, losses));
                 }
                 if (difficultyState.ContainsKey(Difficulty.Easy))
                 {
                     if(difficultyState[Difficulty.Easy].wins > 0)
                     {
                         recommendedDifficulty = (int)Difficulty.Meduim;
                     }
                     else
                     {
                         recommendedDifficulty = (int)Difficulty.Easy;
                     }
                 }

                  if (difficultyState.ContainsKey(Difficulty.Meduim))
                 {
                     if (difficultyState[Difficulty.Meduim].wins > difficultyState[Difficulty.Meduim].losses && difficultyState[Difficulty.Meduim].wins > 0)
                     {
                         recommendedDifficulty = (int)Difficulty.Hard;

                     }
                     else if(difficultyState[Difficulty.Meduim].wins < difficultyState[Difficulty.Meduim].losses && difficultyState[Difficulty.Meduim].losses > 1)
                     {
                         recommendedDifficulty = (int)Difficulty.Easy;

                     }
                 }
                  if (difficultyState.ContainsKey(Difficulty.Hard))
                 {
                     if (difficultyState[Difficulty.Hard].wins > 0 && difficultyState[Difficulty.Hard].wins > difficultyState[Difficulty.Hard].losses)
                     {
                         recommendedDifficulty = (int)Difficulty.Hard;

                     }
                     if(difficultyState[Difficulty.Hard].losses > 1 && difficultyState[Difficulty.Hard].wins < difficultyState[Difficulty.Hard].losses)
                     {
                         recommendedDifficulty = (int)Difficulty.Meduim;


                     }

                 }
                 var gameScoresDto = gamescores.Select(s => new GameScoreDto
                 {
                     GameScoreId = s.GameScoreId,
                     GameScoreName = s.GameScoreName,
                     DifficultyGame = s.DifficultyGame,
                     PatientScore = s.PatientScore,
                     MaxScore = s.MaxScore,
                 }).ToList();
                 if(gameScoresDto == null)
                 {
                     return null;
                 }
                 return new GetGameScoresDto
                 {
                     GameScore = gameScoresDto,
                     RecomendationDifficulty = recommendedDifficulty
                 };
             }
             public async Task<GetGameScoresDto> GetGameScoresAsync2(string token)
             {
                 int recommendedDifficulty = 1;
                 string? PatientId = _jwtDecode.GetUserIdFromToken(token);
                 if (PatientId == null)
                 {
                     return null;
                 }
                 var gamescores = _gameScore.Where(s => s.PatientId == PatientId).ToList();

                 if (gamescores == null)
                 {
                     return null;
                 }
                 var gameScoresDto = gamescores.Select(s => new GameScoreDto
                 {
                     GameScoreId = s.GameScoreId,
                     GameScoreName = s.GameScoreName,
                     DifficultyGame = s.DifficultyGame,
                     PatientScore = s.PatientScore,
                     MaxScore = s.MaxScore,
                 }).ToList();
                 Dictionary<Difficulty, double> winRates = new Dictionary<Difficulty, double>
                 {
                             { Difficulty.Easy, CalculateWinRate(gameScoresDto, Difficulty.Easy) },
                             { Difficulty.Meduim, CalculateWinRate(gameScoresDto, Difficulty.Meduim) },
                             { Difficulty.Hard, CalculateWinRate(gameScoresDto, Difficulty.Hard) }
                 };
                 if (winRates[Difficulty.Easy] > 0.7)
                 {
                     recommendedDifficulty = (int)Difficulty.Meduim;
                 }
                  if (winRates[Difficulty.Meduim] > 0.6)
                 {
                    recommendedDifficulty = (int)Difficulty.Hard;
                 }
                  if (winRates[Difficulty.Hard] < 0.4)
                 {
                    recommendedDifficulty = (int)Difficulty.Meduim;
                 }
                  if (winRates[Difficulty.Meduim] < 0.4)
                 {
                     recommendedDifficulty = (int)Difficulty.Easy;
                 }
                 return new GetGameScoresDto
                 {
                     GameScore = gameScoresDto,
                     RecomendationDifficulty = recommendedDifficulty
                 };
             }

             private double CalculateWinRate(List<GameScoreDto> gameScoresDto, Difficulty difficulty)
             {

                 var filteredGameScores = gameScoresDto.Where(gs => gs.DifficultyGame == difficulty);      
                 int totalGamesPlayed = filteredGameScores.Count();     
                 int totalWins = filteredGameScores.Count(gs => gs.PatientScore > gs.MaxScore / 2);
                 double winRate = totalGamesPlayed > 0 ? (double)totalWins / totalGamesPlayed : 0;
                 return winRate;
             }*/
        #endregion

     
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
            }).ToList();
            if (Patient.CurrentScore >=200 && Patient.CurrentScore < 400)
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
                string filepath = Path.Combine( patientId, $"{patientId }_{fileID}{Path.GetExtension(secretFileDto.FileName)}");
                string directorypath = Path.Combine(_env.WebRootPath, patientId,"SecretFiles");
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
                    DocumentPath = Path.Combine(_env.WebRootPath, filepath),
                    permissionEndDate = DateTime.Now.AddDays(1),
                    hasPermission = true,
                    PatientId=patientId
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
                  var result =   await _mailService.SendEmailAsync("hazemzizo@gmail.com", _mail.FromMail, _mail.Password, $"Patient{patient.FullName} has sent a request to view his secret files", $"Video uploaded successfully for review. video Link: {GetMediaUrl(filepath)}");
                    if (result == true)
                    {
                        return new GlobalResponse
                        {
                            HasError = false,
                            message = $"Video uploaded successfully for review. File ID: {filepath}"
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

                string directoryPath = Path.Combine(_env.WebRootPath, patientId,"VideoForReview");

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
                secretFile.hasPermission = true;
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

        public async Task<IEnumerable<GetSecretFIleDTO>> GetSecretFilesAsync(string token)
        {
            try
            {
                
                if (string.IsNullOrEmpty(token))
                {
                    return Enumerable.Empty<GetSecretFIleDTO>();
                }

                string patientId = _jwtDecode.GetUserIdFromToken(token);

                if (string.IsNullOrEmpty(patientId))
                {
                    return Enumerable.Empty<GetSecretFIleDTO>();
                }

                var patient = await _patient.GetByIdAsync(patientId);

                if (patient == null)
                {
                    return Enumerable.Empty<GetSecretFIleDTO>();
                }

                var secretFiles = await _secret.WhereAsync(s => s.PatientId == patientId && s.hasPermission);

                if (secretFiles == null || !secretFiles.Any())
                {
                    return Enumerable.Empty<GetSecretFIleDTO>();
                }

                var result = secretFiles.Select(s => new GetSecretFIleDTO
                {
                    SecretId = s.File_Id,
                    FileName = s.FileName,
                    File_Description = s.File_Description,
                    DocumentUrl = GetMediaUrl(s.DocumentPath),
                    HasError = false
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetSecretFilesAsync: {ex.Message}");
                return Enumerable.Empty<GetSecretFIleDTO>();
            }
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

        public async Task<IEnumerable<GetFamiliesDto>> GetFamiliesAsync(string token)
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
            return families.Select(s => new GetFamiliesDto
            {
                FamilyId = s.Id,
                FamilyName = s.FullName,
                Relationility = s.Relationility,
                HisImageUrl = (s.imageUrl == null) ? "" : GetMediaUrl(s.imageUrl),
            }).ToList();
        }

        public async Task<GetFamilyLocationDto?> GetFamilyLocation(string token, string familyId)
        {
            string? PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return new GetFamilyLocationDto()
                {
                    Code= StatusCodes.Status400BadRequest,
                    Message = "Invalid Patient Id"
                };
            }
            var family = await _family.GetByIdAsync(familyId);
            if (family == null)
            {
                return new GetFamilyLocationDto()
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "Invalid Family Id"
                };
            }
            if (family.PatientId != PatientId)
            {
                return new GetFamilyLocationDto()
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "Invalid Family Id"
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
        private string GetMediaUrl(string imagePath)
        {
            // Assuming imagePath contains the relative path to the Media within the web root
            // Construct the URL based on your application's routing configuration
            string baseUrl = _mail.ServerLink; // Replace with your actual base URL
            string relativePath = imagePath.Replace(_env.WebRootPath, "").Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
        }


    }
}
