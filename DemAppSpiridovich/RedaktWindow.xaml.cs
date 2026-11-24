using System;
using System.Linq;
using System.Windows;

namespace DemAppSpiridovich
{
    public partial class RedaktWindow : Window
    {
        DemAppEntities db = new DemAppEntities();
        private string currentArticle;

        public RedaktWindow(string article)
        {
            InitializeComponent();
            currentArticle = article;

            if (currentArticle != null) 
            {
                Tovar product = db.Tovar.FirstOrDefault(x => x.Article == currentArticle);
                if (product != null)
                {
                    tbArticle.Text = product.Article;
                    tbArticle.IsReadOnly = true; 
                    tbName.Text = product.Product_name;
                    tbUnit.Text = product.Unit;
                    tbPrice.Text = product.Price.ToString();
                    tbSupplier.Text = product.Supplier;
                    tbManufacturer.Text = product.Manufacturer;
                    tbCategory.Text = product.Category;
                    tbDiscount.Text = product.Discount.ToString();
                    tbQuantity.Text = product.Quantity.ToString();
                    tbDescription.Text = product.Description;
                    tbPhoto.Text = product.Photo;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(tbArticle.Text) || string.IsNullOrWhiteSpace(tbName.Text) || string.IsNullOrWhiteSpace(tbPrice.Text))
                {
                    MessageBox.Show("Заполните Артикул, Наименование и Цену!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                

                if (!decimal.TryParse(tbPrice.Text, out decimal priceVal))
                {
                    MessageBox.Show("Цена должна быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!byte.TryParse(tbDiscount.Text, out byte discountVal))
                {
                    discountVal = 0; 
                }

                if (!byte.TryParse(tbQuantity.Text, out byte quantityVal))
                {
                    quantityVal = 0; 
                }

                
                if (currentArticle == null) 
                {
                    if (db.Tovar.Any(x => x.Article == tbArticle.Text))
                    {
                        MessageBox.Show("Товар с таким артикулом уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    Tovar newProduct = new Tovar();
                    newProduct.Article = tbArticle.Text;
                    newProduct.Product_name = tbName.Text;
                    newProduct.Unit = tbUnit.Text;
                    newProduct.Price = (short)priceVal; 
                    newProduct.Supplier = tbSupplier.Text;
                    newProduct.Manufacturer = tbManufacturer.Text;
                    newProduct.Category = tbCategory.Text;
                    newProduct.Discount = discountVal; 
                    newProduct.Quantity = quantityVal; 
                    newProduct.Description = tbDescription.Text;
                    newProduct.Photo = tbPhoto.Text;

                    db.Tovar.Add(newProduct);
                }
                else
                {
                    Tovar product = db.Tovar.FirstOrDefault(x => x.Article == currentArticle);
                    product.Product_name = tbName.Text;
                    product.Unit = tbUnit.Text;
                    product.Price = (short)priceVal;
                    product.Supplier = tbSupplier.Text;
                    product.Manufacturer = tbManufacturer.Text;
                    product.Category = tbCategory.Text;
                    product.Discount = discountVal;
                    product.Quantity = quantityVal;
                    product.Description = tbDescription.Text;
                    product.Photo = tbPhoto.Text;
                }

                db.SaveChanges();
                MessageBox.Show("Сохранено успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                AdminWindow q = new AdminWindow();
                q.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow q = new AdminWindow();
            q.Show();
            Close();
        }
    }
}