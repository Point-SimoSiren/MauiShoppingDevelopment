using MauiShopping.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MauiShopping.Pages;

public partial class MainPage : ContentPage
{

    string apikey;

    public MainPage()
    {
        
        InitializeComponent();

        LoadDataFromRestAPI();

    }


    async void LoadDataFromRestAPI()
    {
        apikey = DateTime.UtcNow.ToString("dd") + "abc";
        DisplayAlert("a", apikey, "ok");
        itemList.ItemsSource = new List<string> { "Ladataan", "Ostoslista", "palvelimelta..." };
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://shoppingrestapibackend.azurewebsites.net");
        string json = await client.GetStringAsync("/api/shoplist/" + apikey);

        IEnumerable<Shoplist> ienumShoplists = JsonConvert.DeserializeObject<Shoplist[]>(json);

        ObservableCollection<Shoplist> Shoplists = new ObservableCollection<Shoplist>(ienumShoplists);

        itemList.ItemsSource = Shoplists;
    }



    private async void kerätty_nappi_Clicked(object sender, EventArgs e)
    {
        Shoplist? selected = itemList.SelectedItem as Shoplist;
        bool answer = await DisplayAlert("Menikö oikein?", selected.Item + " kerätty?", "Yess! Kyllä meni!", "Ei, Yritän uusiksi");
        if (answer == false)
        {
            return;
        }

        apikey = DateTime.UtcNow.ToString("dd") + "abc";

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://shoppingrestapibackend.azurewebsites.net");
        HttpResponseMessage res = await client.DeleteAsync("/api/shoplist/" + selected.Id +"/" + apikey);
        if (res.StatusCode == System.Net.HttpStatusCode.OK)
        {
            LoadDataFromRestAPI();
        }
        else
        {
            await DisplayAlert("Virhe ohjelmassa", "Ota yhteyttä kehittäjiin", "ok");
        }

    }

    private void addPageBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddPage());
    }

   
}