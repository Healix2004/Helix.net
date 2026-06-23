
namespace Helix.Service.DTOs.AuthDTOs
{
    public class AuthDto
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public List<string> Roles { get; set; }= new List<string>();
    }
}
