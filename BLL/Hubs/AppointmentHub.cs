using BLL.Helper;
using BLL.Interfaces;
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
    /// AppointmentHub is used for send appointment to patient 
    /// when a new appointment created with his family 
    /// You can listen on this 'ReceiveAppointment' and this have 2 arguments 
    /// first one is 'Appointment Added' or 'Appointment deleted' if family created or deleted and second one is 'String of json' with the appointment data
    /// </summary>
    [SignalRHub]
    public class AppointmentHub :Hub
    {
        private readonly IDecodeJwt _decodeJwt;

        public AppointmentHub(IDecodeJwt decodeJwt)
        {
           _decodeJwt = decodeJwt;
        }
        public override async Task OnConnectedAsync()
        {

            var httpContext = Context.GetHttpContext();
            var token = HttpContextHelper.GetTokenHub(httpContext);
            if (token != null)
            {
                var UserId = _decodeJwt.GetUserIdFromToken(token);
                if (UserId != null)
                {

                    await Groups.AddToGroupAsync(Context.ConnectionId, UserId);
                }
            }


            await base.OnConnectedAsync();


        }


    }
}
