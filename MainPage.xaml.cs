using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;


namespace CountryInfoApp
{
    public partial class MainPage : ContentPage
    {
        JObject jsonObject;
        List<JObject> tempList = new List<JObject>();

        ObservableCollection<CountriesData> countriesDatas = new ObservableCollection<CountriesData>();
        ObservableCollection<CountriesData> Changed = new ObservableCollection<CountriesData>()
        {
             new CountriesData()
                {

                    CountryName = "Empty!",
                    Flags = "street.jpg"

                }


        };


        public MainPage()
        {
            InitializeComponent();
            



            string fileName = "CountryInformation.json";
            Task.Run(async () =>
            {


                using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                using var reader = new StreamReader(stream);
                string content = await reader.ReadToEndAsync();
                JObject data = JObject.Parse(content);

                JArray? DataArray = data["DataCountry"] as JArray;


                //SelectionMode="None"

                for (int i = 0; i < DataArray.Count; i++)
                {

                    CountriesData temp = new CountriesData();

                    temp.CountryName = $"{data["DataCountry"][i]["CountryName"]}";
                    temp.OfficialName = $"{data["DataCountry"][i]["OfficialName"]}";
                    temp.Independence = $"{data["DataCountry"][i]["Independence"]}";
                    temp.Capital = $"{data["DataCountry"][i]["Capital"]}";
                    temp.Description = $"{data["DataCountry"][i]["Description"]}";
                    temp.UNMember = $"{data["DataCountry"][i]["UNMember"]}";
                    temp.GoogleMaps = $"{data["DataCountry"][i]["GoogleMaps"]}";
                    temp.OpenStreetMaps = $"{data["DataCountry"][i]["OpenStreetMaps"]}";
                    temp.Timezones = $"{data["DataCountry"][i]["Timezones"]}";
                    temp.Continents = $"{data["DataCountry"][i]["Continents"]}";
                    temp.Flags = $"{data["DataCountry"][i]["Flags"]}";
                    temp.CoatOfArms = $"{data["DataCountry"][i]["CoatOfArms"]}";
                    temp.Latitude = $"{data["DataCountry"][i]["Latitude"]}";
                    temp.Longitude = $"{data["DataCountry"][i]["Longitude"]}";
                    temp.Population = $"{data["DataCountry"][i]["Population"]}";
                    temp.Region = $"{data["DataCountry"][i]["Region"]}";
                    temp.subregion = $"{data["DataCountry"][i]["subregion"]}";
                    temp.abbreviations = $"{data["DataCountry"][i]["abbreviations"]}";

                    temp.languages = data["DataCountry"][i]["languages"].ToObject<List<string>>();

                    countriesDatas.Add(temp);
                }
            }).Wait();



            MyListCountry.ItemsSource = Changed;



            EntryName.TextChanged += EntryName_TextChanged;
            MyListCountry.ItemTapped += MyListCountry_ItemTapped;




        }

        private void MyListCountry_ItemTapped(object? sender, ItemTappedEventArgs e)
        {
            CountriesData? selected = e.Item as CountriesData;
            //Toast.Make(selected.OfficialName, ToastDuration.Short, 15).Show();


            InfoDisplayer infoDisplayer = new InfoDisplayer(selected);

            Navigation.PushAsync(infoDisplayer);
        }


        private bool CompareString(string entry, string compare)
        {

            for (int i = 0; i < entry.Length; i++)
            {

                if (entry.ToLower()[i] != compare.ToLower()[i])
                {
                    return false;
                }

            }
            return true;
        }
        

        private async void EntryName_TextChanged(object? sender, TextChangedEventArgs e)
        {
            Changed.Clear();

          

            if (EntryName.Text != "")
            {
                await Task.Run(() =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var entry in countriesDatas)
                        {

                            if (CompareString(EntryName.Text, entry.CountryName))
                            {
                                Changed.Add(entry);
                            }
                        }
                    });
                    
                

                });
                
                


                if (Changed.Count == 0)
                {

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Changed.Add(new CountriesData()
                        {

                            CountryName = "No Country Was Match!",
                            Flags = "no.png"

                        });

                    });

                }

            }
            else
            {



                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Changed.Add(new CountriesData()
                    {

                        CountryName = "Empty!",
                        Flags = "street.jpg"

                    });
                });
            }
            Countlabel.Text = $"Countries Found: {Changed.Count}";
        }


/*
        private async void UpdateData()
        {
            string fileName = "Country.json";

            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            string content = await reader.ReadToEndAsync();
            JArray jsonArray = JArray.Parse(content);


            for (int i = 0; i < jsonArray.Count; i++)
            {

                var data = JObject.Parse(jsonArray[i].ToString());



                string CountryName = $"{data["name"]["common"]}";
                string OfficialName = $"{data["name"]["official"]}";
                string Independence = $"{data["independent"]}";
                string unMember = $"{data["unMember"]}";
                string googleMaps = $"{data["maps"]["googleMaps"]}";
                string openStreetMaps = $"{data["maps"]["openStreetMaps"]}";


                JArray? CapitalArray = data["capital"] as JArray;
                string Capital = $"{((CapitalArray != null) ? CapitalArray[0] : "")}";

                JArray? timezonesArray = data["timezones"] as JArray;
                string timezones = $"{timezonesArray[0]}";

                JArray? continentsArray = data["continents"] as JArray;
                string continents = $"{continentsArray[0]}";


                string region = $"{data["region"]}";
                string subregion = $"{data["subregion"]}";
                string flags = $"{data["flags"]["png"]}";
                string coatOfArms = ($"{data["coatOfArms"]["png"]}" != null) ? $"{$"{data["coatOfArms"]["png"]}"}" : "";
                string Population = $"{data["population"]}";

                List<float> latlng = data["latlng"]
                            .Select(c => float.Parse(c.ToString()))
                            .ToList();

                string abbreviations = $"{data["cioc"]}";
                //string languages = $"{data["languages"]}";

                //Debug.WriteLine(data["languages"]);

                List<string> lan = new List<string>();
                if (data["languages"] != null) {

                    Dictionary<string, string> dict = new Dictionary<string, string>();


                    dict = data["languages"].ToObject<Dictionary<string, string>>();
         
                    lan = dict.Values.ToList();
                }
                
                

                //Debug.WriteLine(data["languages"].ToList());
                tempList.Add(new JObject(

                     new JProperty("CountryName", CountryName),
                     new JProperty("OfficialName", OfficialName),
                     new JProperty("Independence", Independence),
                     new JProperty("Capital", Capital),
                     new JProperty("Description", ""),
                     new JProperty("UNMember", unMember),
                     new JProperty("GoogleMaps", googleMaps),
                     new JProperty("OpenStreetMaps", openStreetMaps),
                     new JProperty("Timezones", timezones),
                     new JProperty("Continents", continents),
                     new JProperty("Flags", flags),
                     new JProperty("CoatOfArms", coatOfArms),
                     new JProperty("Latitude", latlng[0]),
                     new JProperty("Longitude", latlng[1]),
                     new JProperty("Population", Population),
                     new JProperty("Region", region),
                     new JProperty("subregion", subregion),
                     new JProperty("abbreviations", abbreviations),
                     new JProperty("languages", lan)
                    ));



               



            }
            jsonObject =
                  new JObject(new JProperty("DataCountry", tempList));

            using (StreamWriter write = new StreamWriter("C:/Users/Mikyo/OneDrive/Escritorio/CountryInformation.json"))
            {


                write.WriteLine(jsonObject);

            }

            Debug.WriteLine("Done");
        }
        */
    }


    public class CountriesData
    {

        public string CountryName { get; set; }
        public string OfficialName { get; set; }
        public string Independence { get; set; }
        public string Capital { get; set; }
        public string Description { get; set; }
        public string UNMember { get; set; }
        public string GoogleMaps { get; set; }
        public string OpenStreetMaps { get; set; }
        public string Timezones { get; set; }
        public string Continents { get; set; }
        public string Flags { get; set; }
        public string CoatOfArms { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Population { get; set; }
        public string Region { get; set; }
        public string subregion { get; set; }
        public string abbreviations { get; set; }
        public List<string> languages { get; set; }
        
    }

}
