using CollectionManagmentSystem.Models;
using CollectionManagmentSystem.Services;
using CollectionManagmentSystem.Views;
using CollectionManagmentSystem.Controls;

namespace CollectionManagmentSystem.Views;

public partial class CollectionSummaryPage : ContentPage
{
    private readonly string _dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Collections");
    private Collection _collection;

    public CollectionSummaryPage(Collection collection)
	{
		InitializeComponent();
        _collection = collection;
        LoadCollectionSummary();
	}

    private async void LoadCollectionSummary()
    {
        int totalItems = 0;
        int itemsForSale = 0;
        int itemsSold = 0;

        var filePath = Path.Combine(_dataFolderPath, $"{_collection.CollectionName}.txt");
        if(File.Exists(filePath))
        {
            using (StreamReader sr = File.OpenText(filePath))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    string[] parts = line.Split(',');
                    if(parts.Length == 5 ) {
                        totalItems++;
                        if (parts[4] == "Sold")
                        {
                            itemsSold++;
                        }else if (parts[4] == "On sale")
                        {
                            itemsForSale++;
                        }
                    }
                }
            }

            totalItemsLabel.Text = totalItems.ToString();
            itemsSoldLabel.Text = itemsSold.ToString();
            itemsForSaleLabel.Text = itemsForSale.ToString();
        }
    }
}