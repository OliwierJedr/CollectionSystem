namespace CollectionManagmentSystem.Controls;

public partial class CollectionControl : ContentView
{
	public static readonly BindableProperty IdProperty = 
		BindableProperty.Create(nameof(Id), typeof(int), typeof(CollectionControl), null);
	public static readonly BindableProperty CollectionNameProperty =
		BindableProperty.Create(nameof(CollectionName), typeof(string), typeof(CollectionControl), string.Empty);

	public int Id
	{
		get => (int)GetValue(IdProperty);
		set => SetValue(IdProperty, value);
	}

	public string CollectionName
	{
		get => (string)GetValue(CollectionNameProperty);
		set => SetValue(CollectionNameProperty, value);
	}



	public CollectionControl()
	{
		InitializeComponent();
	}


}