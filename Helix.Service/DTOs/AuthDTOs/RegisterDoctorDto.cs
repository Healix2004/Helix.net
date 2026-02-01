using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Service.DTOs.AuthDTOs
{
    public class RegisterDoctorDto : RegisterUserDto
    {
        public string Specialization { get; set; } = "Genarel";
    }
}
