namespace Helix.Service.DTOs.DrugDTOs
{
    public class CreateDrugDto
    {
        public string Name { get; set; }
    }
    public class UpdateDrugDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
