using BLL.Helper;
using BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BLL.Hubs
{
    public class MedicineReminderHub:Hub
    {
        public MedicineReminderHub(IDecodeJwt decodeJwt)
        {
            _decodeJwt = decodeJwt;
        }

        public IDecodeJwt _decodeJwt { get; }

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
