using BLL.Helper;
using BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Hubs
{
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
