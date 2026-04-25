using Helix.Core.Bases;
using Helix.Core.DTOs.Terminology;
using MediatR;

namespace Helix.Core.Features.RxNavTerminology.Quieres.Models
{
    public class SearchRxNavTerminologyQuery : IRequest<Response<List<CodingDto>>>
    {
        public string Text { get; set; }
        public string System { get; set; }

        public SearchRxNavTerminologyQuery(string text, string system)
        {
            Text = text;
            System = system;
        }
    }
}
