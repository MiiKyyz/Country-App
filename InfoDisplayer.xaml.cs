
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace CountryInfoApp;

public partial class InfoDisplayer : ContentPage
{


    public InfoDisplayer(CountriesData countriesData)
    {
        InitializeComponent();

        CountryNameLabel.Text = countriesData.CountryName;

        OfficialNameLabel.Text = countriesData.OfficialName;

        FlagImage.Source = countriesData.Flags;



        bool ind = bool.Parse(countriesData.Independence.ToLower());


        IndependaceImage.Source = (ind) ? ImageSource.FromFile("yes.png") : ImageSource.FromFile("no.png");


        CapitalNameLabel.Text = $"The Capital is {countriesData.Capital}";




        bool UN = bool.Parse(countriesData.UNMember.ToLower());


        UNMemberImage.Source = (UN) ? ImageSource.FromFile("yes.png") : ImageSource.FromFile("no.png");



        ContinentsNameLabel.Text = $"This Country belongs to the Continent of {countriesData.Continents}";



        PopulationNameLabel.Text = $"The current population of {CountryNameLabel.Text} is {Number(countriesData.Population)}";



        RegionsNameLabel.Text = $"The region of this country is {countriesData.Region}";



        subregionLabel.Text = $"{((countriesData.subregion != "") ? $"The Subregion is {countriesData.subregion}" : "None")}";


        abbreviationsLabel.Text = $"{((countriesData.abbreviations != "") ? $"({countriesData.abbreviations})" : "()")}";


        ImageCoatOfArms.Source = (countriesData.CoatOfArms != "") ? countriesData.CoatOfArms : ImageSource.FromFile("nophoto.png");


        LatitudeLabel.Text = $"Latitude: {countriesData.Latitude}";

        LongitudeLabel.Text = $"Longitude: {countriesData.Longitude}";



        if (countriesData.languages.Count == 0)
        {


            LanguagesLabel.Text = "Antarctica does not have an official language, as it is not a country and has no permanent population. Instead, the languages spoken in Antarctica depend on the nationalities of the researchers and staff working at the various research stations on the continent.";

        }
        else if (countriesData.languages.Count == 1)
        {

            LanguagesLabel.Text = $"The only language people in {countriesData.CountryName} speaks is {countriesData.languages[0]}";

        }
        else
        {

            LanguagesLabel.Text = $"in {countriesData.CountryName} people speak multiple languages such as ";


            for (int i = 0; i < countriesData.languages.Count; i++)
            {

                if (i == 0)
                {

                    LanguagesLabel.Text += $"{countriesData.languages[i]}";
                }
                else if (i == (countriesData.languages.Count - 1))
                {

                    LanguagesLabel.Text += $", and {countriesData.languages[i]}";

                }
                else
                {
                    LanguagesLabel.Text += $", {countriesData.languages[i]}";

                }

            }
        }

        google = countriesData.GoogleMaps;
        openstreet = countriesData.OpenStreetMaps;
        CountryLabelNews.Text = $"News Of {countriesData.CountryName}";
        NewsCoutnry(countriesData.OfficialName);
        NewsList.ItemTapped += NewsList_ItemTapped;
    }

    private void NewsList_ItemTapped(object? sender, ItemTappedEventArgs e)
    {
        NewsCountry? ne = e.Item as NewsCountry;
        Launcher.OpenAsync(new Uri(ne.web_url));
    }

    List<NewsCountry> news = new List<NewsCountry>();
    public async  void NewsCoutnry(string country_name)
    {

        string url_api = $"https://api.nytimes.com/svc/search/v2/articlesearch.json?q={country_name}&api-key=oxBxSd3KApzUfnTLz2G97t5Mh9wD2dT3";

        var handler = new HttpClientHandler();

        HttpClient client = new HttpClient(handler);
        string All_Data = await client.GetStringAsync(url_api);

        var data_in_string = JObject.Parse(All_Data);
        CopyRightLabel.Text = $"{data_in_string["copyright"]}";


        for (int i = 0; i< data_in_string["response"]["docs"].ToList().Count; i++)
        {
            NewsCountry temp = new NewsCountry();


            temp.NewsHeadline = data_in_string["response"]["docs"][i]["headline"]["main"].ToString();
            temp.NewsSubHeadline = data_in_string["response"]["docs"][i]["abstract"].ToString();
            temp.NewsDescription = data_in_string["response"]["docs"][i]["lead_paragraph"].ToString();
            temp.ImageString = (data_in_string["response"]["docs"][i]["multimedia"].ToList().Count != 0) ? $"https://www.nytimes.com/{data_in_string["response"]["docs"][i]["multimedia"][0]["url"]}" : "nophoto.png";
            temp.web_url = data_in_string["response"]["docs"][i]["web_url"].ToString();


            DateTimeOffset dateTimeOffset = DateTimeOffset.Parse($"{data_in_string["response"]["docs"][i]["pub_date"]}");
            temp.Datetime = $"Publication date: {dateTimeOffset.DateTime}";


            temp.WrittenBy = $"{data_in_string["response"]["docs"][i]["byline"]["original"]}";

            news.Add(temp);



        }
        
        /*
        string key = "c2f7f9c576a44c6fb5dbb52bf88682e9";
        string url_api = $"https://newsapi.org/v2/everything?q={country_name}&apiKey={key}";

        var handler = new HttpClientHandler();

        HttpClient client = new HttpClient();


        string All_Data = await client.GetStringAsync(url_api);

        var data_in_string = JObject.Parse(All_Data);




        for (int i = 0; i < data_in_string["articles"].ToList().Count; i++)
        {

            NewsCountry temp = new NewsCountry();
            

            temp.NewsHeadline = data_in_string["articles"][i]["title"].ToString();
   
            temp.ImageString = (data_in_string["articles"][i]["urlToImage"] != null) ? $"{data_in_string["articles"][i]["urlToImage"]}" : "nophoto.png";


            DateTimeOffset dateTimeOffset = DateTimeOffset.Parse($"{data_in_string["articles"][i]["publishedAt"]}");
            temp.Datetime = $"Publication date: {dateTimeOffset.DateTime}";


            temp.WrittenBy = (data_in_string["articles"][i]["title"] != null)? $"{data_in_string["articles"][i]["title"]}":"NO FOUND!";
            
            news.Add(temp);

        }

        */
        NewsList.ItemsSource = news;

       
    }



    string google, openstreet;


    private async void OnOpenStreetMapsTapped(object? sender, EventArgs e)
    {
        await Launcher.OpenAsync(new Uri($"{openstreet}"));
    }

    private async void GoogleMapsButtonTapped(object? sender, EventArgs e)
    {

        await Launcher.OpenAsync(new Uri($"{google}"));
    }

    public static string Number(string number)
    {


        string new_number = "", decimals = "";
        int counter = 0;

        List<string> list = new List<string>();


        if (number.Contains("."))
        {
            list = number.Split('.').ToList();
            number = list[0];
            decimals = $".{list[1]}";
        }



        for (int i = number.Length - 1; i >= 0; i--)
        {
            //Console.WriteLine(number[i]);

            if (counter == 3)
            {
                new_number += ",";
                new_number += number[i];
                counter = 0;
            }
            else
            {
                new_number += number[i];

            }


            counter++;

        }
        number = "";
        for (int i = new_number.Length - 1; i >= 0; i--)
        {
            number += new_number[i];

        }

        number += $"{decimals}";

        return number;


    }




    class NewsCountry
    {

        public string NewsHeadline { get; set; }
        public string ImageString { get; set; }
        public string NewsDescription { get; set; }
        public string web_url { get; set; }
        public string NewsSubHeadline { get; set; }
        public string Datetime { get; set; }
        public string WrittenBy { get; set; }


    }
}