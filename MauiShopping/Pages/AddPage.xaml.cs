using MauiShopping.Models;
using Newtonsoft.Json;
using System.Text;

namespace MauiShopping.Pages;

public partial class AddPage : ContentPage
{

    string apikey;

	public AddPage()
	{
		InitializeComponent();

	}

    

    private async void AddBtn_Clicked(object sender, EventArgs e)
    {

        apikey = DateTime.UtcNow.ToString("dd") + "abc";

        Shoplist newItem = new Shoplist();
        
        newItem.Item = ItemField.Text;
        newItem.Amount = int.Parse(AmountField.Text);

        // Muutetaan em. data objekti Jsoniksi
        var input = JsonConvert.SerializeObject(newItem);

        HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");

        HttpClient client = new();
        client.BaseAddress = new Uri("https://shoppingrestapibackend.azurewebsites.net");
        HttpResponseMessage res = await client.PostAsync("/api/shoplist/" + apikey , content);
        
        if (res.StatusCode == System.Net.HttpStatusCode.OK)
        {
            ItemField.Text = "";
           

            await Navigation.PushAsync(new MainPage());
        }
        else
        {
            await DisplayAlert("Virhe ohjelmassa", "Ota yhteyttä kehittäjiin", "ok");
        }
    }
}
