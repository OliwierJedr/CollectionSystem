using CollectionManagmentSystem.Models;
using System.Diagnostics;

namespace CollectionManagmentSystem.Services
{
    public class FileDataService
    {
        private readonly string _dataFolderPath;  

        public FileDataService()
        {
            _dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Collections");
            Directory.CreateDirectory(_dataFolderPath);
        }

        public async void SaveCollectionItems(string collectionName, List<CollectionItem> items)
        {
            string filePath = GetCollectionFilePath(collectionName);

            try
            {
                using StreamWriter writer = File.CreateText(filePath);
                foreach (var item in items)
                    await writer.WriteLineAsync($"{item.ItemId}|#|{item.ItemName.Replace(",", "|COMMA|")}|#|{item.Price}|#|{item.Rating}|#|{item.Status}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save {ex.Message}");
            }
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
            string formattedCollectionName = collectionName.Replace(" ", "_");
            return Path.Combine(_dataFolderPath, $"{formattedCollectionName}.txt");
        }
    }
}
