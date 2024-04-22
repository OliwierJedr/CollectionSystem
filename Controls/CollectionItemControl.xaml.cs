namespace CollectionManagmentSystem.Controls;

public partial class CollectionItemControl : ContentView
{
	public static readonly BindableProperty ItemIdProperty =
		BindableProperty.Create(nameof(ItemId), typeof(int), typeof(CollectionItemControl), null);
	public static readonly BindableProperty ItemNameProperty =
		BindableProperty.Create(nameof(ItemName), typeof(string), typeof(CollectionItemControl), string.Empty);
	public static readonly BindableProperty PriceProperty =
		BindableProperty.Create(nameof(Price), typeof(double), typeof(CollectionItemControl), null);
	public static readonly BindableProperty RatingProperty =
		BindableProperty.Create(nameof(Rating), typeof(int), typeof(CollectionItemControl), null);
	public static readonly BindableProperty StatusProperty =
		BindableProperty.Create(nameof(Status), typeof(string), typeof(CollectionItemControl), string.Empty);

    public int ItemId
	{
		get => (int)GetValue(ItemIdProperty);
		set => SetValue(ItemIdProperty, value);
	}
    public string ItemName
    {
        get => (string)GetValue(ItemIdProperty);
        set => SetValue(ItemIdProperty, value);
    }
    public double Price
    {
        get => (double)GetValue(PriceProperty);
        set => SetValue(PriceProperty, value);
    }
	public int Rating
	{
		get => (int)GetValue(RatingProperty);
		set => SetValue(RatingProperty, value);
	}
	public string Status
	{
		get => (string)GetValue(StatusProperty);
		set => SetValue(StatusProperty, value);
	}

    public CollectionItemControl()
	{
		InitializeComponent();
	}
    public event EventHandler EditItemClicked;
    private void EditItem_Clicked(object sender, EventArgs e)
	{
		EditItemClicked?.Invoke(this, EventArgs.Empty);
	}
}
