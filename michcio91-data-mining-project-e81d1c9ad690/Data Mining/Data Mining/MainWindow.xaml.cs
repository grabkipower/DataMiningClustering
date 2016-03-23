using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.IO;
using Data_Mining.Classes;
using Data_Mining.Functions;
using System.Diagnostics;

namespace Data_Mining
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        List<Cluster> Clusters;
        string documenttext;
        int MaxDistance;

        long milisecs = 0;

        public MainWindow()
        {
      //      MessageBox.Show(Algorithm.CalculateLevenshtein("Sam", "Samantha").ToString());
            InitializeComponent();


        }

        private void AddDocument_Click(object sender, RoutedEventArgs e)
        {
     
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {

                string filename = dlg.FileName;
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(System.IO.File.ReadAllText(filename));
                documenttext = System.IO.File.ReadAllText(filename);
                FlowDocument document = new FlowDocument(paragraph);
                FlowDocReader.Document = document;
                Start.IsEnabled = true;

            } 
        }
    

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            int MaxDistance;
       
           bool result = Int32.TryParse(MinDistanceTextBox.Text, out MaxDistance);

           if (result)
           {


               var watch = Stopwatch.StartNew();
               Clusters = Algorithm.InitializeClusters(documenttext);
               Clusters = Algorithm.Cluster(Clusters, Int32.Parse( MinDistanceTextBox.Text)); 
               Clusters = Algorithm.KMeans(Clusters);
               watch.Stop();
               milisecs = watch.ElapsedMilliseconds;
               Time.Content = milisecs;
               dataGrid.ItemsSource = Clusters;
          }
              
              

           else
           {
               MessageBox.Show("Wrong number inserted");
           }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if( Clusters != null)
                if (Clusters.Count() != 0)
                {
                    string filename = "";
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.FileName = "Save File"; // Default file name
                    dlg.DefaultExt = ".txt"; // Default file extension
                    dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        filename = dlg.FileName;
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong.");
                        return;
                    }


                    System.IO.StreamWriter file = new System.IO.StreamWriter(filename);

                    string text = "";
                    foreach (var c in Clusters)
                    {
                        file.WriteLine(" ");
                        file.WriteLine("Centroid: " + c.Contents[0]);
                        for (int i = 0; i < c.Contents.Count(); i++)
                            if (i != c.Contents.Count() - 1)
                                file.Write(c.Contents[i] + ", ");
                            else
                                file.Write(c.Contents[i]);

                    }
                    file.Close();

                }
                else
                {
                    MessageBox.Show("Something went wrong.");
                }
        }
    }
}
