using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.RxNavTerminology.Quieres.Models
{
    public class GetRxNavDisplayQuery : IRequest<Response<string>>
    {
        public string Code { get; set; }
        public string System { get; set; }

        public GetRxNavDisplayQuery(string code, string system)
        {
            Code = code;
            System = system;
        }
    }
}
