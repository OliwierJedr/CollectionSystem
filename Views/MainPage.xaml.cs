using CollectionManagmentSystem.Models;
using CollectionManagmentSystem.Controls;
using CollectionManagmentSystem.Services;
using CollectionManagmentSystem.Views;

namespace CollectionManagmentSystem
{
    public partial class MainPage : ContentPage
    {
        private readonly FileDataService _fileDataService = new FileDataService();
        private readonly string _dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Collections");
        private List<Collection> _collections;

        public MainPage()
        {
            InitializeComponent();
            Directory.CreateDirectory(_dataFolderPath);
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
                var filePath = Path.Combine(_dataFolderPath, $"{collectionName}.txt");
                if (File.Exists(filePath))
                {
                    await DisplayAlert("Warning", "A collection with the same name already exists. Please choose a different name.", "OK");
                    return;
                }

                File.Create(filePath).Dispose();

                _collections.Add(new Collection { Id = _collections.Count + 1, CollectionName = collectionName });
                LoadCollections();
                RefreshCollectionListView();
            }
        }
        private void RefreshCollectionListView() => collectionListView.ItemsSource = _collections;


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
                    
                    RefreshCollectionListView();
                    LoadCollections();
                }
            }
            else
            {
                await DisplayAlert("Error", "Please select a collection to delete", "OK");
            }
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

        private string GetCollectionFilePath(string collectionName)
        {
            return Path.Combine(_dataFolderPath, $"{collectionName}.txt");
        }

        private async void OnViewCollection_Clicked(object sender, EventArgs e)
        {
            if(sender is Button button && button.BindingContext is Collection selectedCollection)
                await Navigation.PushAsync(new CollectionPage(selectedCollection));
        }

        private void ShowDataPath_Clicked(object sender, EventArgs e)
        {
            dataPathLabel.IsVisible = !dataPathLabel.IsVisible;

            if (dataPathLabel.IsVisible)
                dataPathLabel.Text = $"Data is stored in: {_dataFolderPath}";
        }

    }
}
