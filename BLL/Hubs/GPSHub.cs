using BLL.Helper;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Hubs
{
    /// <summary>
    /// gps hub for patient and family when patient location change it will send message to family
    /// but if the change is greater than the MaxDistance it will send message his family
    /// and if patient dissconnect it will remove from group and send message to family on 'ReceivePatientDisconnect'
    /// </summary>
    [SignalRHub]
    public class GPSHub:Hub
    {
        private readonly IDecodeJwt _decodeJwt;
        private readonly IBaseRepository<Patient> _patient;
        private readonly IBaseRepository<Family> _family;
        private readonly IBaseRepository<Location> _locationRepository;
        private readonly IMailService _mailService;
        public GPSHub( IDecodeJwt decodeJwt
            , IBaseRepository<Patient> PatientRepository
            , IBaseRepository<Location> locationRepository
            , IBaseRepository<Family> FamilyRepository
            , IMailService mailService
            )
        {
            _mailService = mailService;
            _locationRepository = locationRepository;
            _patient = PatientRepository;
            _family = FamilyRepository;
            _decodeJwt = decodeJwt;
        }
        [SignalRMethod("SendGPSToFamilies")]
        public async Task SendGPSToFamilies( double Latitude, double Longitude)
        {
            var httpContext = Context.GetHttpContext();
            var token = HttpContextHelper.GetTokenHub(httpContext);
            if (token != null)
            {
                var UserId = _decodeJwt.GetUserIdFromToken(token);
                if (UserId != null)
                {
                    await _mailService.SendEmailAsync("sayed.work223@gmail.com", "electronicmind22@hotmail.com", "ASEHOM@#2392023", "Send GPS Successfully", $"{UserId} Send GPS Successfully Latitude : {Latitude} , Longitude : {Longitude}");
                    var patient = await _patient.FindAsync(f=>f.Id== UserId);
                    if (patient!=null)
                    {

                       
                           await Clients.Group(UserId).SendAsync("ReceiveGPSToFamilies", Latitude, Longitude);
                        
                        await _locationRepository.AddAsync(new Location
                        {
                            Latitude = Latitude,
                            Longitude = Longitude,
                            PatientId = patient.Id,

                        });
                    }
                   
                }
            }
          
        }

        public async override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var token = HttpContextHelper.GetTokenHub(httpContext);
            if (token != null)
            {
                
                var UserId = _decodeJwt.GetUserIdFromToken(token);
              await  _mailService.SendEmailAsync("sayed.work223@gmail.com", "electronicmind22@hotmail.com", "ASEHOM@#2392023", "Connected Successfully", $"{UserId} Connected Successfully");
                var Family = await _family.FindAsync(f => f.Id == UserId);

                if (Family != null)
                {
                    if (Family.PatientId != null)
                    await Groups.AddToGroupAsync(Context.ConnectionId, Family.PatientId);
                }
               else
                {
                    var patient = await _patient.FindAsync(f => f.Id == UserId);
                    if (patient != null)    
                    await Groups.AddToGroupAsync(Context.ConnectionId, patient.Id);
                }
            }
            await base.OnConnectedAsync();

        }
        /// <summary>
        /// when patient disconnect from hub it will remove from group 
        /// and send message to family on 'ReceivePatientDisconnect'
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        [SignalRMethod("ReceivePatientDisconnect")]
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var token = HttpContextHelper.GetTokenHub(httpContext);
            if (token != null)
            {
                var UserId = _decodeJwt.GetUserIdFromToken(token);
                if (UserId != null)
                {
                    var patient = await _patient.FindAsync(f => f.Id == UserId);
                    if (patient != null)
                    {
                       await Clients.Group(UserId).SendAsync("ReceivePatientDisconnect","Patient Disconnected");
                    }

                }
            }

                await base.OnDisconnectedAsync(exception);
        }
        private static double Haversine(double lat1, double lat2, double lon1, double lon2)
        {
            const double r = 6371e3; // meters
            var dlat = (lat2 - lat1) / 2;
            var dlon = (lon2 - lon1) / 2;

            var q = Math.Pow(Math.Sin(dlat), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(q), Math.Sqrt(1 - q));

            var d = r * c;
            return d; // return distance in meters
        }
    }
}
