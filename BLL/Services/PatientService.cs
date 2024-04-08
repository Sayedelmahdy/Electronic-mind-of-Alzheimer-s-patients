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
        private readonly IBaseRepository<Appointment> _appointments;
        private readonly IBaseRepository<Family> _family;
        private readonly IDecodeJwt _jwtDecode;
        private readonly IBaseRepository<Media> _media;
        private readonly Mail _mail;
        private readonly IWebHostEnvironment _env;
        private readonly IBaseRepository<GameScore> _gameScore;
        public PatientService
            (
            IHubContext<MedicineReminderHub> hubContext, IDecodeJwt jwtDecode,
            IBaseRepository<Patient>patient,
            IBaseRepository<Medication_Reminders>medicines ,
            IBaseRepository<Appointment>appointments,
            IBaseRepository<Family>family,
            IBaseRepository<Media>media,
             IWebHostEnvironment env,
              IOptions<Mail> Mail,
            IBaseRepository<GameScore> gameScore
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
            _gameScore = gameScore;
        }

       
        public async Task<GetPatientProfileDto> GetPatientProfileAsync(string token)
        {
            string? patientid = _jwtDecode.GetUserIdFromToken(token);

            if(patientid == null)
            {
                return new GetPatientProfileDto
                {
                    Message = "Invalid Patient ID",
                    HasError = true
                };
            }
            var patinet = await _patient.GetByIdAsync(patientid);
            if( patinet == null)
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
            if(PatientId == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            var patient = await _patient.GetByIdAsync(PatientId);
            if(patient == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
          var appointments =  _appointments.Include(s=>s.family).Where(s=>s.PatientId== PatientId);
            if(appointments == null)
            {
                return Enumerable.Empty<GetAppointmentDto>();
            }
            return appointments.Select(s=> new GetAppointmentDto
            {
                AppointmentId=s.AppointmentId,
                Date=s.Date,
                Location= s.Location,
                Notes=s.Notes,
                FamilyNameWhoCreatedAppointemnt=s.family.FullName,
            }).ToList();
        }
        public async Task<IEnumerable<MedicationReminderGetDto>> GetMedicationRemindersAsync(string token)
        {
            string PatientId = _jwtDecode.GetUserIdFromToken(token);
            if (PatientId == null)
            {
                return Enumerable.Empty<MedicationReminderGetDto>();
            }
            var medicines = await _medicines.WhereAsync(s=>s.Patient_Id== PatientId);
            if(medicines == null)
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
                Time_Period = s.Time_Period,
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
            var media =await _media.Include(s => s.patient).Include(s=>s.family).Where(s => s.PatientId == PatientId).ToListAsync();
            var res = media.Select(s => new GetMediaforPatientDto
            {
                Caption = s.Caption,
                MediaUrl = GetMediaUrl(s.Image_Path),
                MediaId = s.Media_Id,
                Uploaded_date = s.Upload_Date,
                MediaExtension = s.Extension,
                FamilyNameWhoUpload=s.family.FullName
            }).ToList();
            return res;
        }
        private string GetMediaUrl(string imagePath)
        {
            // Assuming imagePath contains the relative path to the Media within the web root
            // Construct the URL based on your application's routing configuration
            string baseUrl = _mail.ServerLink; // Replace with your actual base URL
            string relativePath = imagePath.Replace(_env.WebRootPath, "").Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
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
            var gamescore = new GameScore
            {
                GameScoreName = gameScoreDto.GameScoreName,
                DifficultyGame = gameScoreDto.DifficultyGame,
                PatientScore = gameScoreDto.PatientScore,
                MaxScore = gameScoreDto.MaxScore,
                PatientId = PatientId
            };
            await _gameScore.AddAsync(gamescore);
            string message = gameScoreDto.PatientScore >= gameScoreDto.MaxScore / 2 ? "Congratulations! You did a great job" : "Hard luck! But don't give up, try again ";
            return new GlobalResponse
            {
                HasError = false,
                message = message
            };
        }

        public async Task<GetGameScoresDto> GetGameScoresAsync(string token)
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
            else if (winRates[Difficulty.Meduim] > 0.6)
            {
               recommendedDifficulty = (int)Difficulty.Hard;
            }
            else if (winRates[Difficulty.Hard] < 0.4)
            {
               recommendedDifficulty = (int)Difficulty.Meduim;
            }
            else if (winRates[Difficulty.Meduim] < 0.4)
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
        }

        public Task<GlobalResponse> AddSecretFileAsync(string token, PostSecretFileDto secretFileDto)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse> AskToViewSecretFileAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<GetSecretFileDto> GetSecretFileAsync(string token)
        {
            throw new NotImplementedException();
        }
    }
}
