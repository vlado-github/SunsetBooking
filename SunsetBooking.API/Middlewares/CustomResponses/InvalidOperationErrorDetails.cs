using System.Net;
using SunsetBooking.Domain.Shared.Consts;

namespace SunsetBooking.API.Middlewares.CustomResponses
{
    public class InvalidOperationErrorDetails : ErrorDetailsBase
    {
        public InvalidOperationErrorDetails(string message)
            : base(Rfc.RfcBadRequestType, (int)HttpStatusCode.BadRequest, message)
        {

        }
    }
}
