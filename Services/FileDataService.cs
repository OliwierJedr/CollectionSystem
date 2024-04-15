using CollectionManagmentSystem.Models;

namespace CollectionManagmentSystem.Services
{
    public class FileDataService
    {
        private readonly string _dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Collections");

        public FileDataService()
        {
            Directory.CreateDirectory(_dataFolderPath);
        }

        public async void SaveCollectionItems(string collectionName, List<CollectionItem> items)
        {
            string filePath = GetCollectionFilePath(collectionName);

            using StreamWriter writer = File.CreateText(filePath);
            foreach (var item in items)
                await writer.WriteLineAsync($"{item.ItemId}|#|{item.ItemName.Replace(",", "|COMMA|")}|#|{item.Price}|#|{item.Rating}|#|{item.Status}");
        }

        public async Task<List<CollectionItem>> LoadCollectionItems(string collectionName)
        {
            string filePath = GetCollectionFilePath(collectionName);

            if (!File.Exists(filePath))
                return new List<CollectionItem>();


            using StreamReader reader = File.OpenText(filePath);
            return await ReadItemsFromStream(reader);
        }

        private async Task<List<CollectionItem>> ReadItemsFromStream(StreamReader reader)
        {
            List<CollectionItem> items = new List<CollectionItem>();
            string line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                CollectionItem item = ParseCollectionItem(line);
                if (item != null)
                    items.Add(item);
            }

            return items;
        }

        private CollectionItem ParseCollectionItem(string line)
        {
            string[] parts = line.Split(new string[] { "|#|" }, StringSplitOptions.None);
            if (parts.Length == 5)
            {
                return new CollectionItem
                {
                    ItemId = int.Parse(parts[0]),
                    ItemName = parts[1].Replace("|COMMA|", ","),
                    Price = double.Parse(parts[2]),
                    Rating = int.Parse(parts[3]),
                    Status = parts[4]
                };
            }
            return null;
        }

        private string GetCollectionFilePath(string collectionName)
        {
            return Path.Combine(_dataFolderPath, $"{collectionName}.txt");
        }
    }
}
