namespace DealHawk.Application.DTOs
{
    public class StoreAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CheapSharkStoreId { get; set; }
        public bool IsActive { get; set; }
        public string LogoUrl { get; set; }
        public int GameCount { get; set; }
    }
}
