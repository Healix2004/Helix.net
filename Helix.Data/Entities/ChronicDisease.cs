namespace Helix.Data.Entities
{
    // --- Standard Entities (These will become their own tables linked to Patient) ---

    public class ChronicDisease : BaseEntity
    {
        public string ChronicDiseaseCatalogCode { get; set; }
        public ChronicDiseaseCatalog ChronicDiseaseCatalog { get; set; }
        public DateTime DiagnosisDate { get; set; }

        // Foreign Key
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
    }
}