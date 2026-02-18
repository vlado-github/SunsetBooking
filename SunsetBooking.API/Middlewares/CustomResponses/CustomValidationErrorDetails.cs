using System.Net;
using FluentValidation.Results;
using SunsetBooking.Domain.Shared.Consts;

namespace SunsetBooking.API.Middlewares.CustomResponses
{
    public class CustomValidationErrorDetails : ErrorDetailsBase
    {
        public CustomValidationErrorDetails(string message)
            : base(Rfc.RfcBadRequestType, (int)HttpStatusCode.BadRequest, message)
        {
        }

        public CustomValidationErrorDetails(string message, IEnumerable<ValidationFailure> errors)
            : base(Rfc.RfcBadRequestType, (int)HttpStatusCode.BadRequest, message)
        {
            Errors = errors.Select(x => new KeyValuePair<string,string>(x.PropertyName, x.ErrorMessage)).ToList();
        }

        public IList<KeyValuePair<string, string>> Errors { get; private set; }
    }
}
