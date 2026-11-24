using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DemAppSpiridovich
{
    public partial class ManagerWindow : Window
    {
        DemAppEntities db = new DemAppEntities();

        public ManagerWindow()
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

            var query = db.Tovar.AsQueryable();

            string search = tbSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Product_name.ToLower().Contains(search));

            if (cbSort.SelectedIndex == 1) query = query.OrderBy(x => x.Price);
            else if (cbSort.SelectedIndex == 2) query = query.OrderByDescending(x => x.Price);
            else if (cbSort.SelectedIndex == 3) query = query.OrderBy(x => x.Photo);

            dgProducts.ItemsSource = query.ToList();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        { 
            LoadProducts();
        }
        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { 
            if (dgProducts != null)
                LoadProducts();
        }
        private void btnProducts_Click(object sender, RoutedEventArgs e) 
        {
            LoadProducts();
        }

        // Кнопка Заказы
        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgProducts.Visibility = Visibility.Collapsed;
                dgOrders.Visibility = Visibility.Visible;

                var zakazy = db.zakaz.ToList();

                // Если заказов нет в базе
                if (zakazy.Count == 0)
                {
                    MessageBox.Show("В базе данных нет заказов!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

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
                MessageBox.Show("Ошибка при загрузке заказов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                if (pvz != null) return pvz.streat + ", " + pvz.home;
            }
            return addressStr;
        }

        private void btUnlog_Click(object sender, RoutedEventArgs e)
        {
            MainWindow f = new MainWindow();
            f.Show();
            this.Close();
        }
    }
}