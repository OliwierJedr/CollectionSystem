namespace CollectionManagmentSystem.Models
{
    public class CollectionItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public int Rating { get; set; }
        public string Status { get; set; }
        public int CollectionId { get; set; }
    }
}
