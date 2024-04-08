using CollectionManagmentSystem.Views;
using CollectionManagmentSystem.Models;
using CollectionManagmentSystem.Controls;
using CollectionManagmentSystem.Services;

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
        {
            editButtonLayout.IsVisible = true;
        }
        else
        {
            editButtonLayout.IsVisible = false;
        }
    }

    private async void OnEdit_Clicked(object sender, EventArgs e)
    {
        if (collectionView.SelectedItem is CollectionItem selectedItem)
        {
            
            string selectedProperty = await DisplayActionSheet("Select property to edit:", "Cancel", null, "Item Name", "Price", "Rating", "Status");
            if (selectedProperty == null || selectedProperty == "Cancel")
                return; 

            string newValue;
            switch (selectedProperty)
            {
                case "Item Name":
                    newValue = await DisplayPromptAsync("Edit Item Name", "Enter new item name:", "OK", "Cancel", selectedItem.ItemName);
                    if (newValue == null) {
                        return;
                    }
                        
                    selectedItem.ItemName = newValue;
                    break;
                case "Price":
                    newValue = await DisplayPromptAsync("Edit Price", "Enter new price:", "OK", "Cancel", selectedItem.Price.ToString());
                    if (newValue == null)
                    {
                        return;
                    }
                        
                    double newPrice;
                    if (!double.TryParse(newValue, out newPrice))
                    {
                        await DisplayAlert("Error", "Invalid price. Please enter a valid number.", "OK");
                        return;
                    }

                    selectedItem.Price = newPrice;
                    break;
                case "Rating":
                    newValue = await DisplayPromptAsync("Edit Rating", "Enter new rating(1-10):", "OK", "Cancel", selectedItem.Rating.ToString());
                    if (newValue == null)
                    {
                        return;
                    }
                        
                    int newRating;
                    if (!int.TryParse(newValue, out newRating))
                    {
                        await DisplayAlert("Error", "Invalid rating. Please enter a valid number.", "OK");
                        return;
                    }
                    selectedItem.Rating = newRating;
                    break;
                case "Status":
                    newValue = await GetStatusPrompt();
                    if (newValue == null)
                    {
                        return;
                    }
                         
                    selectedItem.Status = newValue;
                    break;
            }

            var dataService = new FileDataService();
            dataService.SaveCollectionItems(_collection.CollectionName, _collectionItems);

            LoadCollectionItems();
        }
    }

    private async void OnAdd_Clicked(object sender, EventArgs e)
    {
        string itemName = await DisplayPromptAsync("New Item", "Enter your item's name: ");
        if (itemName == null)
        {
            return;
        }

        if (_collectionItems.Any(item => item.ItemName == itemName))
        {
            var confirm = await DisplayAlert("Warn", "An item with provided name already exists. Do you want to add new one?", "Yes", "No");
            if (!confirm)
            {
                return;
            }
        }

        double price;
        while (true)
        {
            string priceText = await DisplayPromptAsync("New Item", "Enter your item's price(PLN): ");
            if (priceText == null)
            {
                return;
            }

            if (double.TryParse(priceText, out price))
            {
                break;
            }

            await DisplayAlert("Error", "Invalid price. Please enter valid value", "OK");
        }

        int rating;
        while (true)
        {
            string ratingText = await DisplayPromptAsync("New Item", "Rate your item(1-10): ");
            if (ratingText == null)
            {
                return;
            }

            if (int.TryParse(ratingText, out rating))
            {
                break;
            }
                
            await DisplayAlert("Error", "Invalid rating. Please enter valid value", "OK");
        }

        string status = await GetStatusPrompt();
        if(status == null)
        {
            return;
        }


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