using BLL.Helper;
using BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace BLL.Hubs
{
    /// <summary>
    /// Medicine Reminder Hub like appointments it's used for sending Medicines to the patient when his Caregiver adds it
    /// You can listen on this 'ReceiveMedicineReminder' and this have 2 arguments
    /// First one is 'Medication Added' or 'Medication Deleted' if caregiver adds or deleted and second one is 'String of json' with the Medication data
    /// </summary>
    [SignalRHub]
    
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
