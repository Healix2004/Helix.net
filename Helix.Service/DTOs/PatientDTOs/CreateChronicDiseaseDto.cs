using Helix.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class CreateChronicDiseaseDto
    {
        public DateTime DiagnosisDate { get; set; }
        public string ChronicDiseaseCatalogCode { get; set; }
        public ChronicDiseaseCatalog ChronicDiseaseCatalog { get; set; }
        public Guid PatientId { get; set; }
    }
}