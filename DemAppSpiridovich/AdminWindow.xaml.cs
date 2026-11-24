using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DemAppSpiridovich
{
    public partial class AdminWindow : Window
    {
        DemAppEntities db = new DemAppEntities();

        public AdminWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            dgOrders.Visibility = Visibility.Collapsed;
            dgProducts.Visibility = Visibility.Visible;
            dgProducts.ItemsSource = db.Tovar.OrderBy(x => x.Photo).ToList();
        }

        private void btSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText)) { LoadProducts(); return; }

            dgProducts.ItemsSource = db.Tovar
                .Where(x => x.Product_name.ToLower().Contains(searchText))
                .OrderBy(x => x.Photo)
                .ToList();
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (Tovar)dgProducts.SelectedItem;
            if (item != null)
            {
                if (db.zakaz.Any(z => z.Article.Contains(item.Article)))
                {
                    MessageBox.Show("Нельзя удалить товар, так как он присутствует в заказах!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                db.Tovar.Remove(item);
                db.SaveChanges();
                LoadProducts();
            }
        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            Tovar item = (Tovar)dgProducts.SelectedItem;
            if (item == null) return;
            RedaktWindow q = new RedaktWindow(item.Article);
            q.Show();
            Hide();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            RedaktWindow q = new RedaktWindow(null);
            q.Show();
            Hide();
        }

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgProducts.Visibility = Visibility.Collapsed;
                dgOrders.Visibility = Visibility.Visible;

                var zakazy = db.zakaz.ToList();

                var orders = zakazy.Select(x => new
                {
                    Номер = x.Number,
                    Артикулы = x.Article,
                    Товар = GetProductName(x.Article),
                    Дата_заказа = x.Order_date,
                    Дата_выдачи = x.Delivery_date,
                    Клиент = db.users.FirstOrDefault(u => u.FIO == x.FIO_client)?.FIO ?? x.FIO_client,
                    Адрес_ПВЗ = GetPvzAddress(x.Address.ToString()),
                    Код = x.Code,
                    Статус = x.Status
                }).ToList();

                dgOrders.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке заказов: " + ex.Message);
            }
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

       
        private string GetProductName(string articleList)
        {
            if (string.IsNullOrWhiteSpace(articleList)) return "Неизвестно";

            string firstArticle = articleList.Split(',')[0].Trim();
            var tovar = db.Tovar.FirstOrDefault(t => t.Article == firstArticle);

            return tovar != null ? tovar.Product_name : articleList;
        }

      
        private string GetPvzAddress(string addressStr)
        {
            if (string.IsNullOrWhiteSpace(addressStr)) return "Неизвестно";

            if (int.TryParse(addressStr, out int pvzIndex))
            {
                var pvz = db.pvz.FirstOrDefault(p => p.index == pvzIndex);
                if (pvz != null)
                    return pvz.streat + ", " + pvz.home;
            }
            return addressStr;
        }

        private void btUnlog_Click(object sender, RoutedEventArgs e)
        {
            MainWindow f  = new MainWindow();   
            f.Show();
            this.Close();
        }
    }
}