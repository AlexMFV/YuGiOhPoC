using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace CardManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Card> cards;
        List<BitmapImage> images;

        public MainWindow()
        {
            InitializeComponent();
            Globals.files_path = Cache.GetFilesPath();
            Globals.images_path = Cache.GetImagesPath();

            while(Globals.files_path == "" || !Directory.Exists(Globals.files_path))
            {
                System.Windows.MessageBox.Show("Please select the path to the files");
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowDialog();
                Globals.files_path = dialog.SelectedPath;
                Cache.SaveFilesPath();
            }

            while(Globals.images_path == "" || !Directory.Exists(Globals.images_path))
            {
                System.Windows.MessageBox.Show("Please select the path to the card images");
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowDialog();
                Globals.images_path = dialog.SelectedPath;
                Cache.SaveImagesPath();
            }

            images = Cache.LoadAllImages();
            cards = Cache.LoadAllCards();
            FillList();
        }

        public void FillList()
        {
            dgCards.ItemsSource = cards;
            //dgCards.DataContext = cards;

            //foreach (Card card in cards)
            //{
            //    System.Windows.Controls.ListViewItem item = new System.Windows.Controls.ListViewItem();
            //    item.Content = card._name;
            //    item.Tag = card._id;
            //    //item.MouseDoubleClick += Item_MouseDoubleClick;
            //    lstCards.Items.Add(item);
            //}
            //
            //if(lstCards.Items.Count > 0)
            //    lstCards.SelectedIndex = 0;
        }

        private void lstCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Card card = (Card)dgCards.SelectedItem;

            if (card != null)
            {
                BitmapImage image = images.Where(x => x.UriSource.Segments.Last().Split('.')[0] == card._imageName).First();
                imgCard.Source = image;
            }
        }
    }
}
