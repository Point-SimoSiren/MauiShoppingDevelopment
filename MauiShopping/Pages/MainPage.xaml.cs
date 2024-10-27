using MauiShopping.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;

namespace MauiShopping.Pages;

public partial class MainPage : ContentPage
{

    string apikey = "";

    public MainPage()
    {

        InitializeComponent();

    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        Task.Delay(2000);
        loadingAnouncement.IsAnimationPlaying = true;
        LoadDataFromRestAPI();
    }


    async void LoadDataFromRestAPI()
    {
        apikey = DateTime.UtcNow.ToString("dd") + "abc";

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://shoppingrestapibackend.azurewebsites.net");
        string json = await client.GetStringAsync("/api/shoplist/" + apikey);

        IEnumerable<Shoplist> ienumShoplists = JsonConvert.DeserializeObject<Shoplist[]>(json);
        ObservableCollection<Shoplist> Shoplists = new ObservableCollection<Shoplist>(ienumShoplists);

        itemList.ItemsSource = Shoplists;

        loadingAnouncement.IsVisible = false;
        addPageBtn.IsVisible = true;
        kerätty_label.IsVisible = true;
        kerätty_nappi.IsVisible = true;

    }


    private async void kerätty_nappi_Clicked(object sender, EventArgs e)
    {
        try
        {
            Shoplist? selected = itemList.SelectedItem as Shoplist;
            bool answer = await DisplayAlert("Menikö oikein?", selected.Item + " kerätty?", "Yess! Kyllä meni!", "Ei, Yritän uusiksi");
            if (answer == false)
            {
                return;
            }

            loadingAnouncement.IsVisible = true;
            apikey = DateTime.UtcNow.ToString("dd") + "abc";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://shoppingrestapibackend.azurewebsites.net");
            HttpResponseMessage res = await client.DeleteAsync("/api/shoplist/" + selected.Id + "/" + apikey);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                LoadDataFromRestAPI();
                loadingAnouncement.IsVisible = false;

            }
            else
            {
                await DisplayAlert("Virhe ohjelmassa", "Ota yhteyttä kehittäjiin", "ok");
            }
        }
        catch
        {
            await DisplayAlert("Error", "Error", "ok");
        }
    }

    // Kun halutaan avata lisäyslomake tuotteille:
    private void addPageBtn_Clicked(object sender, EventArgs e)
    {
        // Perustilassa piilotetut uuden lisäykseen tarvittavat elementit näytetään nyt kun ollaan lisäystilanteessa:
        ItemField.IsVisible = true;
        AmountField.IsVisible = true;
        AddBtn.IsVisible = true;

        // Perustilassa näytettävät elementit piilotetaan kun ollaan lisäystilanteessa:
        addPageBtn.IsVisible = false;
        kerätty_label.IsVisible = false;
        kerätty_nappi.IsVisible = false;
    }


    // Kun uusi tuote on syötetty ja painetaan "tallennus" nappia:
    private async void AddBtn_Clicked(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(ItemField.Text))
        {
            await DisplayAlert("Tieto puuttuu", "Valitse tuote", "Ok");
            return;
        }

        else if (String.IsNullOrEmpty(AmountField.Text.ToString()))
        {
            await DisplayAlert("Tieto puuttuu", "Anna lukumäärä", "Ok");
            return;
        }

        else
        {
            loadingAnouncement.IsVisible=true;

            apikey = DateTime.UtcNow.ToString("dd") + "abc";

            Shoplist newItem = new Shoplist();

            newItem.Item = ItemField.Text;
            newItem.Amount = int.Parse(AmountField.Text);

            // Muutetaan em. data objekti Jsoniksi
            var input = JsonConvert.SerializeObject(newItem);

            HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");

            HttpClient client = new();
            client.BaseAddress = new Uri("https://shoppingrestapibackend.azurewebsites.net");
            HttpResponseMessage res = await client.PostAsync("/api/shoplist/" + apikey, content);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ItemField.Text = "";
                AmountField.Text = "";

                // Palataan takaisin perustilaan jossa näytetään ostoslistan tuotteet jne., mutta ei lisäyksen elementtejä
                ItemField.IsVisible = false;
                AmountField.IsVisible = false;
                AddBtn.IsVisible = false;

                // Ladataan uusi tuotelistaus ja tässä metodissahan tarvittavat elementitkin otetaan taas näkyviin
                LoadDataFromRestAPI();

                // Lataus spinneri poistuu
                loadingAnouncement.IsVisible = false;

            }
            else
            {
                await DisplayAlert("Virhe ohjelmassa", "Ota yhteyttä kehittäjiin", "ok");
            }
        }
    }

    private void itemList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        Shoplist? selectedItem = itemList.SelectedItem as Shoplist;
        kerätty_nappi.Text = "Poimi " + selectedItem?.Item;
    }
}