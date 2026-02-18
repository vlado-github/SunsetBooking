using System.Net;
using SunsetBooking.Domain.Shared.Consts;

namespace SunsetBooking.API.Middlewares.CustomResponses
{
    public class NotFoundErrorDetails : ErrorDetailsBase
    {
        public NotFoundErrorDetails(string message)
            : base(Rfc.RfcNotFoundType, (int)HttpStatusCode.NotFound, message)
        {

        }
    }
}
