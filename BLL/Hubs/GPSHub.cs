using BLL.Helper;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Model;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace BLL.Hubs
{
    /// <summary>
    /// gps hub for patient and family when patient location change it will send message to family
    /// but if the change is greater than the MaxDistance it will send message his family
    /// and if patient dissconnect it will remove from group and send message to family on 'ReceivePatientDisconnect'
    /// </summary>
    [SignalRHub]
    public class GPSHub : Hub
    {
        private readonly IDecodeJwt _decodeJwt;
        private readonly IBaseRepository<Patient> _patient;
        private readonly IBaseRepository<Family> _family;
        private readonly IBaseRepository<Location> _locationRepository;
        private readonly IMailService _mailService;
        public GPSHub(IDecodeJwt decodeJwt
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
        public async Task SendGPSToFamilies(double Latitude, double Longitude)
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
                        await Clients.Group(UserId).SendAsync("ReceivePatientDisconnect", "Patient Disconnected");
                    }

                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
