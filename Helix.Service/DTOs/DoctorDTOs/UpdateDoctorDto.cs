using System;

namespace Helix.Service.DTOs.DoctorDTOs
{
    public class UpdateDoctorDto
    {
        public string Specialty { get; set; }
        public decimal ConsultationFee { get; set; }
        public string Bio { get; set; }
        public string SyndicateNumber { get; set; }
    }
}
