using Newtonsoft.Json;
using Plugin.Clipboard;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;

namespace Picturelate
{
    [DesignTimeVisible(false)]
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            listViewHave.ItemsSource = new string[]
            {
                "de " + "- Deutsch",
                "en " + "- Englisch",
                "it " + "- Italienisch",
                "fr " + "- Französisch",
                "ru " + "- Russian",
                "tr " + "- Türkisch",
                "sp " + "- Spanisch",
                "zh-Hant " + "- Chinesisch Traditionel",
                "pt-br " + "- Portugisisch",
                "ja " + "- Japanisch",
                "cs " + "- Tschechisch",
                "el " + "- Grichisch",
                "hu " + "- Ungarisch",
                "ga " + "- Irisch",
                "nb " + "- Norwegisch",
                "pl " + "- Polisch",
                "sv " + "- Schwedisch"
            };
            listViewWant.ItemsSource = new string[]
            {
                "de " + "- Deutsch",
                "en " + "- Englisch",
                "it " + "- Italienisch",
                "fr " + "- Französisch",
                "ru " + "- Russian",
                "tr " + "- Türkisch",
                "sp " + "- Spanisch",
                "zh-Hant " + "- Chinesisch Traditionel",
                "pt-br " + "- Portugisisch",
                "ja " + "- Japanisch",
                "cs " + "- Tschechisch",
                "el " + "- Grichisch",
                "hu " + "- Ungarisch",
                "ga " + "- Irisch",
                "nb " + "- Norwegisch",
                "pl " + "- Polisch",
                "sv " + "- Schwedisch"
            };
        }

        private async void TakeAPicture_Clicked(object sender, EventArgs e)
        {
            if (LanguageHave.Text != "")
            {
                if (LanguageWant.Text != "")
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsCameraAvailable && !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("No Camera", "No Camera available", "OK");
                        return;
                    }

                    var file = await CrossMedia.Current.TakePhotoAsync(
                        new StoreCameraMediaOptions
                        {
                            SaveToAlbum = true,
                            PhotoSize = PhotoSize.Small
                        }
                        );


                    if (file == null)
                        return;

                    MemoryStream memoryStream = new MemoryStream();
                    file.GetStream().CopyTo(memoryStream);
                    byte[] newfile = memoryStream.ToArray();

                    var requestContent = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(newfile);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                    requestContent.Add(imageContent, "image", "image.jpg");
                    using (var client = new HttpClient())
                    {

                      //  LanguageWant.Text = LanguageWant.Text.Split('-')[0];
                      //  LanguageHave.Text = LanguageHave.Text.Split('-')[0];
                      //  LanguageWant.Text = LanguageWant.Text.Replace(" ", "");
                      //  LanguageHave.Text = LanguageHave.Text.Replace(" ", "");
                        string LanguageYouHave = LanguageHave.Text;
                        string LanguageYouWant = LanguageWant.Text;

                        client.BaseAddress = new Uri("http://picturelate.bbs-rohrbach.at");
                        var result = client.PostAsync("/api/analyse", requestContent).Result;

                        string textToTranslate = result.Content.ReadAsStringAsync().Result.Replace("\\n", " ");
                        erneut.Text = textToTranslate;
                        

                        Translate data = new Translate()
                        {
                            SourceLanguage = LanguageYouHave,
                            DestinationLanguage = LanguageYouWant,
                            Text = textToTranslate
                        };
                        erneutlan.Text = LanguageYouHave;
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync("/api/translate", content);
                        if (response.IsSuccessStatusCode)
                        {
                            string EndText = await response.Content.ReadAsStringAsync();
                            EndText = EndText.Remove(0, 16);
                            EndText = EndText.Replace("\\", "");
                            EndText = EndText.Replace('"', '.');
                            EndText = EndText.Replace(".", "");

                            EndTextShow.Text = EndText;
                            CopyText.Text = EndText;
                            ImageShow.Source = file.Path;
                            Copy.IsVisible = true;
                            file.Dispose();
                            erneutButton.IsVisible = true;
                        }
                        else
                        {
                            EndTextShow.Text = "Leider war die Übersetzung nicht erfolgreich";
                            ImageShow.Source = file.Path;
                            file.Dispose();
                        }

                    }
                }
                else
                {
                    EndTextShow.Text = "In welche Sprache soll den übersetzt werden?";
                }
            }
            else
            {
                EndTextShow.Text = "Sie müssen eine Eingabesprache eingeben!";
            }


        }

        private async void PickaPicture_Clicked(object sender, EventArgs e)
        {
            if (LanguageHave.Text != "")
            {
                if (LanguageWant.Text != "")
                {
                    await CrossMedia.Current.Initialize();
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await DisplayAlert("Opsskskskksk", "Pick Picture is not supportet!", "OK");
                        return;
                    }

                    var file = await CrossMedia.Current.PickPhotoAsync(
                        new PickMediaOptions
                        {
                            PhotoSize = PhotoSize.Medium
                        });

                    if (file == null)
                        return;

                    MemoryStream memoryStream = new MemoryStream();
                    file.GetStream().CopyTo(memoryStream);
                    byte[] newfile = memoryStream.ToArray();

                    var requestContent = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(newfile);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                    requestContent.Add(imageContent, "image", "image.jpg");
                    using (var client = new HttpClient())
                    {
                        LanguageWant.Text = LanguageWant.Text.Split('-')[0];
                        LanguageHave.Text = LanguageHave.Text.Split('-')[0];
                        string LanguageYouHave = LanguageHave.Text;
                        string LanguageYouWant = LanguageWant.Text;

                        client.BaseAddress = new Uri("http://picturelate.bbs-rohrbach.at");
                        var result = client.PostAsync("/api/analyse", requestContent).Result;

                        string textToTranslate = result.Content.ReadAsStringAsync().Result.Replace("\\n", " ");
                        erneut.Text = textToTranslate;

                        Translate data = new Translate()
                        {
                            SourceLanguage = LanguageYouHave,
                            DestinationLanguage = LanguageYouWant,
                            Text = textToTranslate
                        };
                        erneutlan.Text = LanguageYouHave;

                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync("api/translate", content);
                        if (response.IsSuccessStatusCode)
                        {
                            string EndText = await response.Content.ReadAsStringAsync();
                            EndText = EndText.Remove(0, 16);
                            EndText = EndText.Replace("\\", "");
                            EndText = EndText.Replace('"', '.');
                            EndText = EndText.Replace(".", "");
                            EndTextShow.Text = EndText;
                            CopyText.Text = EndText;
                            ImageShow.Source = file.Path;
                            Copy.IsVisible = true;
                            file.Dispose();
                            erneutButton.IsVisible = true;
                        }
                        else
                        {
                            EndTextShow.Text = "Leider war die Übersetzung nicht erfolgreich";
                            ImageShow.Source = file.Path;
                            file.Dispose();
                        }
                    }
                }
                else
                {
                    EndTextShow.Text = "In welche Sprache soll den übersetzt werden?";
                }
            }
            else
            {
                EndTextShow.Text = "Sie müssen eine Eingabesprache eingeben!";
            }
        }

        private async void Erneut_Clicked(object sender, EventArgs e)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://picturelate.bbs-rohrbach.at");
                string textToTranslate = erneut.Text;
                string LanguageYouHave = LanguageHave.Text;
                string LanguageYouWant = LanguageWant.Text;
                if (erneutlan.Text == LanguageHave.Text)
                {
                    Translate data = new Translate()
                    {
                        SourceLanguage = LanguageYouHave,
                        DestinationLanguage = LanguageYouWant,
                        Text = textToTranslate
                    };

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("/api/translate", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string EndText = await response.Content.ReadAsStringAsync();
                        EndText = EndText.Remove(0, 16);
                        EndText = EndText.Replace("\\", "");
                        EndText = EndText.Replace('"', '.');
                        EndText = EndText.Replace(".", "");
                        EndTextShow.Text = EndText;
                    }
                    else
                    {
                        EndTextShow.Text = "Leider war die Übersetzung nicht erfolgreich";
                    }
                }
                else
                {
                    EndTextShow.Text = "Sie haben die Eingabesprache verändert, leider funktioniert das Programm nun nicht richtig";
                }
            }
        }

        private void Copy_Clicked(object sender, EventArgs e)
        {
            if (CopyText.Text != "")
            {
                string TextToCopy = CopyText.Text;
                CrossClipboard.Current.SetText(TextToCopy);
            }
            else
            {
                EndTextShow.Text = "Kein Text zum kopieren";
            }
        }

        private void ListViewHave_Tapped(object sender, ItemTappedEventArgs e)
        {

            listViewHave.SelectedItem = e.Item;

            LanguageHave.Text = listViewHave.SelectedItem.ToString();

            LanguageHave.Text = LanguageHave.Text.Split('-')[0];
            LanguageHave.Text = LanguageHave.Text.Replace(" ", "");
        }
        private void ListViewWant_Tapped(object sender, ItemTappedEventArgs e)
        {

            listViewWant.SelectedItem = e.Item;

            LanguageWant.Text = listViewWant.SelectedItem.ToString();

            LanguageWant.Text = LanguageWant.Text.Split('-')[0];
            LanguageWant.Text = LanguageWant.Text.Replace(" ", "");
        }

        public class Translate
        {
            public string SourceLanguage { get; set; }
            public string DestinationLanguage { get; set; }
            public string Text { get; set; }
        }
    }
}
