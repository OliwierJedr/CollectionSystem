using CollectionManagmentSystem.Models;
using CollectionManagmentSystem.Controls;
using CollectionManagmentSystem.Services;
using CollectionManagmentSystem.Views;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace CollectionManagmentSystem
{
    public partial class MainPage : ContentPage
    {
        private readonly string _dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Collections");
        private List<Collection> _collections;

        public MainPage()
        {
            InitializeComponent();
            Directory.CreateDirectory(_dataFolderPath);
            Debug.WriteLine("DATA STORED IN: " + _dataFolderPath + "\n");
            LoadCollections();
        }

        private void LoadCollections()
        {
            _collections = new List<Collection>();
            var collectionFiles = Directory.GetFiles(_dataFolderPath, "*.txt");

            foreach (var filePath in collectionFiles)
            {
                var collectionName = Path.GetFileNameWithoutExtension(filePath);
                _collections.Add(new Collection { Id = _collections.Count + 1, CollectionName = collectionName });
            }

            collectionListView.ItemsSource = _collections;
        }

        private async void OnAddCollection_Clicked(object sender, EventArgs e)
        {
            var collectionName = await DisplayPromptAsync("New Collection", "Enter collection name:");

            if (!string.IsNullOrWhiteSpace(collectionName))
            {
                if (!IsValidCollectionName(collectionName))
                {
                    await DisplayAlert("Error", "Collection name cannot contain special characters.", "OK");
                    return;
                }

                AddCollection(collectionName);
            }
        }

        private bool IsValidCollectionName(string collectionName)
        {
            return !ContainsSpecialCharacters(collectionName);
        }

        private async void AddCollection(string collectionName)
        {
            string formattedCollectionName = RemoveSpecialCharacters(collectionName);

            formattedCollectionName = formattedCollectionName.Replace(" ", "_");

            var fileName = $"{formattedCollectionName}.txt";
            var filePath = Path.Combine(_dataFolderPath, fileName);
            if (File.Exists(filePath))
            {
                await DisplayAlert("Warning", "A collection with the same name already exists. Please choose a different name.", "OK");
                return;
            }

            File.Create(filePath).Dispose();

            _collections.Add(new Collection { Id = _collections.Count + 1, CollectionName = collectionName });
            LoadCollections();
        }

        private async void OnDeleteCollection_Clicked(object sender, EventArgs e)
        {
            if (collectionListView.SelectedItem is Collection selectedCollection)
            {
                var confirm = await DisplayAlert("Delete Collection", $"Are you sure you want to delete collection: {selectedCollection.CollectionName}", "Yes", "No");
                if (confirm)
                {
                    var filePath = Path.Combine(_dataFolderPath, $"{selectedCollection.CollectionName}.txt");
                    File.Delete(filePath);
                    _collections.Remove(selectedCollection);

                    collectionListView.ItemsSource = _collections;
                    LoadCollections();
                }
            }
            else
                await DisplayAlert("Error", "Please select a collection to delete", "OK");
            
        }

        public async void OnEditCollection_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Collection selectedCollection)
            {
                string newName = await GetNewCollectionName(selectedCollection);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    UpdateCollectionFilePath(selectedCollection, newName);
                    UpdateCollectionName(selectedCollection, newName);
                    LoadCollections();
                }
            }
        }

        private async Task<string> GetNewCollectionName(Collection selectedCollection)
        {
            return await DisplayPromptAsync("Edit Collection Name", "Enter new name: ", "OK", "Cancel", selectedCollection.CollectionName);
        }

        private void UpdateCollectionFilePath(Collection selectedCollection, string newName)
        {
            string oldFilePath = GetCollectionFilePath(selectedCollection.CollectionName);
            string newFilePath = GetCollectionFilePath(newName);

            if (File.Exists(oldFilePath))
            {
                File.Move(oldFilePath, newFilePath);
                File.Delete(oldFilePath);
            }
        }

        private void UpdateCollectionName(Collection selectedCollection, string newName)
        {
            selectedCollection.CollectionName = newName;
        }

        private async void OnViewCollection_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Collection selectedCollection)
                await Navigation.PushAsync(new CollectionPage(selectedCollection));
        }

        private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Collection selectedCollection) ShowCollectionDescription(selectedCollection);
            else HideCollectionDescription();
        }

        private void ShowCollectionDescription(Collection selectedCollection)
        {
            descriptionLayout1.IsVisible = true;
            descriptionLayout1.BindingContext = selectedCollection;
            descriptionLayout2.IsVisible = true;
            descriptionLayout2.BindingContext = selectedCollection;
        }

        private void HideCollectionDescription()
        {
            descriptionLayout1.IsVisible = false;
            descriptionLayout2.IsVisible = false;
        }

        private string GetCollectionFilePath(string collectionName)
        {
            string formattedCollectionName = collectionName.Replace(" ", "_");
            return Path.Combine(_dataFolderPath, $"{formattedCollectionName}.txt");
        }

        private bool ContainsSpecialCharacters(string input)
        {
            string pattern = "[^a-zA-Z0-9 ]";
            return Regex.IsMatch(input, pattern);
        }

        private string RemoveSpecialCharacters(string input)
        {
            string pattern = "[^a-zA-Z0-9 ]";
            return Regex.Replace(input, pattern, "");
        }
    }
}
