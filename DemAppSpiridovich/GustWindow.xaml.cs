using System.Linq;
using System.Windows;

namespace DemAppSpiridovich
{
    public partial class GustWindow : Window
    {
        DemAppEntities db = new DemAppEntities();

        public GustWindow() { InitializeComponent(); }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgProducts.ItemsSource = db.Tovar.OrderBy(x => x.Photo).ToList();
        }

        private void btLog_Click(object sender, RoutedEventArgs e)
        {
            MainWindow f = new MainWindow();
            f.Show();
            this.Close();
        }
    }
}