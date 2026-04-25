using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.RxNavTerminology.Quieres.Models
{
    public class ValidateRxNavCodeQuery : IRequest<Response<bool>>
    {
        public string Code { get; set; }
        public string System { get; set; }

        public ValidateRxNavCodeQuery(string code, string system)
        {
            Code = code;
            System = system;
        }
    }
}
