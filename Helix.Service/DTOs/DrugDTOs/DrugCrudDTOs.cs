namespace Helix.Service.DTOs.DrugDTOs
{
    /// <summary>
    /// DTO for creating a drug entry. Extend with additional fields as needed.
    /// </summary>
    public class CreateDrugDto
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// DTO for updating a drug entry. Extend with additional fields as needed.
    /// </summary>
    public class UpdateDrugDto
    {
        public string Name { get; set; }
    }
}
