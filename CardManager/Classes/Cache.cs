using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace CardManager
{
    public static class Cache
    {
        public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string saves = Path.Combine(appdata, "files_path.json");
        public static string images = Path.Combine(appdata, "images_path.json");

        public static void SaveFilesPath()
        {
            if (Globals.files_path != "" && Directory.Exists(Globals.files_path))
            {
                //Save the path to the files
                string json = JsonConvert.SerializeObject(Globals.files_path);
                File.WriteAllText(saves, json);
            }
        }

        public static string GetFilesPath()
        {
            if (File.Exists(saves))
            {
                string json = File.ReadAllText(saves);
                return JsonConvert.DeserializeObject<string>(json);
            }

            return "";
        }

        public static List<Card> LoadAllCards()
        {
            List<Card> cards = new List<Card>();

            if (Globals.files_path != "" && File.Exists(Path.Combine(Globals.files_path, "all_cards.json")))
            {
                string json = File.ReadAllText(Path.Combine(Globals.files_path, "all_cards.json"));
                cards = JsonConvert.DeserializeObject<List<Card>>(json);
            }

            return cards;
        }

        public static void SaveImagesPath()
        {
            if (Globals.images_path != "" && Directory.Exists(Globals.images_path))
            {
                //Save the path to the files
                string json = JsonConvert.SerializeObject(Globals.images_path);
                File.WriteAllText(images, json);
            }
        }

        public static string GetImagesPath()
        {
            if (File.Exists(images))
            {
                string json = File.ReadAllText(images);
                return JsonConvert.DeserializeObject<string>(json);
            }

            return "";
        }

        internal static List<BitmapImage> LoadAllImages()
        {
            List<BitmapImage> images = new List<BitmapImage>();

            if (Globals.images_path != "" && Directory.Exists(Globals.images_path))
            {
                foreach (string file in Directory.GetFiles(Globals.images_path))
                {
                    if (file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".bmp"))
                        images.Add(new BitmapImage(new Uri(file)));
                }
            }

            return images;
        }
    }
}
