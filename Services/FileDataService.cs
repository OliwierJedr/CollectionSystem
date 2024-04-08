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
            using (StreamWriter writer = File.CreateText(filePath))
            {
                foreach(var item in items)
                {
                   await writer.WriteLineAsync($"{item.ItemId},{item.ItemName},{item.Price},{item.Rating},{item.Status}");
                }
            }
        }

        public async Task<List<CollectionItem>> LoadCollectionItems(string collectionName)
        {
            List<CollectionItem> items = new List<CollectionItem>();
            string filePath = GetCollectionFilePath(collectionName);

            if(File.Exists(filePath))
            {
                using (StreamReader reader = File.OpenText(filePath))
                {
                    string line;
                    while((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] parts = line.Split(',');
                        if(parts.Length == 5)
                        {
                            items.Add(new CollectionItem
                            {
                                ItemId = int.Parse(parts[0]),
                                ItemName = parts[1],
                                Price = double.Parse(parts[2]),
                                Rating = int.Parse(parts[3]),
                                Status = parts[4]
                            });
                        }
                    }
                }
            }
            return items;
        }

        private string GetCollectionFilePath(string collectionName)
        {
            return Path.Combine(_dataFolderPath, $"{collectionName}.txt");
        }
    }
}
