using CollectionManagmentSystem.Views;
using CollectionManagmentSystem.Models;
using CollectionManagmentSystem.Controls;
using CollectionManagmentSystem.Services;
using System.Diagnostics;


namespace CollectionManagmentSystem.Views;

public partial class CollectionPage : ContentPage
{
    private List<CollectionItem> _collectionItems;
    private Collection _collection;
    private readonly string _dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Collections");


    public CollectionPage(Collection collection)
    {
        InitializeComponent();
        BindingContext = collection;
        _collection = collection;
        LoadCollectionItems();
    }

    private async void OnSummary_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CollectionSummaryPage(_collection));
    }

    private async void LoadCollectionItems()
    {
        var dataService = new FileDataService();
        _collectionItems = await dataService.LoadCollectionItems(_collection.CollectionName);
        collectionView.ItemsSource = _collectionItems;
       
    }

    private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is CollectionItem selectedItem)
            editButtonLayout.IsVisible = true;
        else
            editButtonLayout.IsVisible = false;
    }

    private async void OnEdit_Clicked(object sender, EventArgs e)
    {
        if (collectionView.SelectedItem is CollectionItem selectedItem)
        {
            string selectedProperty = await DisplayActionSheet("Select property to edit:", "Cancel", null, "Item Name", "Price", "Rating", "Status");
            if (selectedProperty == null || selectedProperty == "Cancel")
                return;

            switch (selectedProperty)
            {
                case "Item Name":
                    await EditItemName(selectedItem);
                    break;
                case "Price":
                    await EditPrice(selectedItem);
                    break;
                case "Rating":
                    await EditRating(selectedItem);
                    break;
                case "Status":
                    await EditStatus(selectedItem);
                    break;
            }

            var dataService = new FileDataService();
            dataService.SaveCollectionItems(_collection.CollectionName, _collectionItems);

            LoadCollectionItems();
        }
    }

    private async Task EditItemName(CollectionItem selectedItem)
    {
        string newValue = await DisplayPromptAsync("Edit Item Name", "Enter new item name:", "OK", "Cancel", selectedItem.ItemName);
        if (newValue != null)
            if (ItemNameAlreadyExists(newValue))
                await ConfirmAddingNewItem();
            else
                selectedItem.ItemName = newValue;
    }

    private async Task EditPrice(CollectionItem selectedItem)
    {
        string newValue = await DisplayPromptAsync("Edit Price", "Enter new price:", "OK", "Cancel", selectedItem.Price.ToString());
        if (newValue != null)
        {
            double newPrice;
            if (double.TryParse(newValue, out newPrice))
                selectedItem.Price = newPrice;
            else
                await DisplayAlert("Error", "Invalid price. Please enter a valid number.", "OK");
        }
    }

    private async Task EditRating(CollectionItem selectedItem)
    {
        string newValue = await DisplayPromptAsync("Edit Rating", "Enter new rating (1-10):", "OK", "Cancel", selectedItem.Rating.ToString());
        if (newValue != null)
        {
            int newRating;
            if (int.TryParse(newValue, out newRating) && newRating >= 1 && newRating <= 10)
                selectedItem.Rating = newRating;
            else
                await DisplayAlert("Error", "Invalid rating. Please enter a value between 1 and 10.", "OK");
        }
    }

    private async Task EditStatus(CollectionItem selectedItem)
    {
        string newValue = await GetStatusPrompt();
        if (newValue != null)
            selectedItem.Status = newValue;
    }

    private async void OnAdd_Clicked(object sender, EventArgs e)
    {
        string itemName = await PromptForItemName();
        if (itemName != null)
        {
            if (ItemNameAlreadyExists(itemName))
            {
                await ConfirmAddingNewItem();
                return; 
            }

            double price = await PromptForPrice();
            if (price != -1) 
            {
                int rating = await PromptForRating();
                if (rating != -1) 
                {
                    string status = await PromptForStatus();
                    if (status != null)
                        AddNewItem(itemName, price, rating, status);
                }
            }
        }
    }

    private async Task<string> PromptForItemName()
    {
        return await DisplayPromptAsync("New Item", "Enter your item's name: ");
    }

    private bool ItemNameAlreadyExists(string itemName)
    {
        return _collectionItems.Any(item => item.ItemName == itemName);
    }

    private async Task<bool> ConfirmAddingNewItem()
    {
        await DisplayAlert("Warning", "An item with provided name already exists. Please choose a different name.", "OK");
        return true;
    }

    private async Task<double> PromptForPrice()
    {
        double price;
        string priceText;
        do
        {
            priceText = await DisplayPromptAsync("New Item", "Enter your item's price (PLN): ");
            if (priceText == null)
                return -1; 

            if (!double.TryParse(priceText, out price))
                await DisplayAlert("Error", "Invalid price. Please enter a valid value", "OK");
        } while (!double.TryParse(priceText, out price));

        return price;
    }

    private async Task<int> PromptForRating()
    {
        int rating;
        string ratingText;
        do
        {
            ratingText = await DisplayPromptAsync("New Item", "Rate your item (1-10): ");
            if (ratingText == null)
                return -1;

            if (!int.TryParse(ratingText, out rating) || rating < 1 || rating > 10)
                await DisplayAlert("Error", "Invalid rating. Please enter a value between 1 and 10.", "OK");
        } while (!int.TryParse(ratingText, out rating) || rating < 1 || rating > 10);

        return rating;
    }

    private async Task<string> PromptForStatus()
    {
        return await GetStatusPrompt();
    }

    private void AddNewItem(string itemName, double price, int rating, string status)
    {
        int newItemId = _collectionItems.Count + 1;
        CollectionItem newItem = new CollectionItem
        {
            ItemId = newItemId,
            ItemName = itemName,
            Price = price,
            Rating = rating,
            Status = status
        };
        _collectionItems.Add(newItem);

        var dataService = new FileDataService();
        dataService.SaveCollectionItems(_collection.CollectionName, _collectionItems);
        LoadCollectionItems();
    }



    private void OnDelete_Clicked(object sender, EventArgs e)
    {
        if (collectionView.SelectedItem is CollectionItem selectedItem)
        {
            _collectionItems.Remove(selectedItem);
            var dataService = new FileDataService();
            dataService.SaveCollectionItems(_collection.CollectionName, _collectionItems);
            LoadCollectionItems();
        }
    }

    private async Task<string> GetStatusPrompt()
    {
        string[] statusOptions = { "New", "Used", "On sale", "Sold" };
        string selectedStatus = await DisplayActionSheet("Select status: ", "Cancel", null, statusOptions) ;

        if(selectedStatus == null)
        {
            return null;
        }
        return selectedStatus;
    }

}
