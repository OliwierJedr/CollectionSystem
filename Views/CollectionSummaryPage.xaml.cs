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

    private void LoadCollectionSummary()
    {
        int totalItems = 0;
        int itemsForSale = 0;
        int itemsSold = 0;

        string filePath = GetCollectionFilePath(_collection.CollectionName);
        if (File.Exists(filePath))
        {
            using StreamReader sr = File.OpenText(filePath);
            ProcessCollectionData(sr, ref totalItems, ref itemsForSale, ref itemsSold);
            
            UpdateLabels(totalItems, itemsForSale, itemsSold);
        }
    }

    private string GetCollectionFilePath(string collectionName)
    {
        return Path.Combine(_dataFolderPath, $"{collectionName}.txt");
    }

    private void ProcessCollectionData(StreamReader sr, ref int totalItems, ref int itemsForSale, ref int itemsSold)
    {
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string[] parts = line.Split(new string[] { "|#|" }, StringSplitOptions.None);
            if (parts.Length == 5)
            {
                totalItems++;
                UpdateItemCount(parts[4], ref itemsForSale, ref itemsSold);
            }
        }
    }

    private void UpdateItemCount(string status, ref int itemsForSale, ref int itemsSold)
    {
        if (status == "Sold") itemsSold++;
        else if (status == "On sale") itemsForSale++;
    }

    private void UpdateLabels(int totalItems, int itemsForSale, int itemsSold)
    {
        totalItemsLabel.Text = totalItems.ToString();
        itemsSoldLabel.Text = itemsSold.ToString();
        itemsForSaleLabel.Text = itemsForSale.ToString();
    }
}
