using Microsoft.Win32;
using System.Windows;
using System.Xml.Linq;

namespace Pohovor
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        public MainWindow()
            {
            InitializeComponent();
            }

        private decimal prevedCenu(string cena)
            {
            string hodnota = cena.Replace(".", "").Replace(",-", "").Trim();

            return decimal.TryParse(hodnota, out decimal vysledek) ? vysledek : 0;
            }
        private void nactiXML(object sender, RoutedEventArgs e)
            {
                {
                try
                    {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "soubory XML (*.xml)|*.xml";
                    if (openFileDialog.ShowDialog() == true)
                        {
                        var auta = XElement.Load(openFileDialog.FileName)
                            .Elements("vuz")
                            .Select(vuz => new
                                {
                                Model = (string)vuz.Element("model"),
                                Datum = DateTime.Parse((string)vuz.Element("datum")),
                                Cena = prevedCenu((string)vuz.Element("cena")),
                                Dph = decimal.Parse((string)vuz.Element("dph"))
                                })
                            .Where(vuz => vuz.Datum.DayOfWeek == DayOfWeek.Saturday || vuz.Datum.DayOfWeek == DayOfWeek.Sunday)
                            .GroupBy(vuz => vuz.Model)
                            .Select(skupinaAut => new
                                {
                                Popis = $"{skupinaAut.Key}\n{skupinaAut.Sum(x => x.Cena):N02}",
                                CenaSDph = $"{skupinaAut.Sum(x => x.Cena * (1 + x.Dph / 100m)):N02}"
                                })
                            .ToList();

                        dataGrid.ItemsSource = auta;
                        }
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Chyba při načítání XML: " + ex.Message);
                    }
                }
            }

        }
    }