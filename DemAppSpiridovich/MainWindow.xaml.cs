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

namespace DemAppSpiridovich
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DemAppEntities db = new DemAppEntities();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btLog_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                
                var userObj = db.users.FirstOrDefault(x => x.Login == tblog.Text && x.Password == pbox.Password);
                if (userObj == null) return;

                Hide();
                switch (userObj.role)
                {
                    case "Администратор":
                        new AdminWindow().Show();
                        break;
                    case "Менеджер":
                        new ManagerWindow().Show();   
                        break;
                    case "Авторизированный клиент":
                        new UserWindow().Show();
                        break;

                    default:
                        MessageBox.Show("Неправильный логин или пароль:");
                        Show();
                        break;
                }
            }
            catch
            {
            }
        }

        private void btGust_Click(object sender, RoutedEventArgs e)
        {
            new GustWindow().Show();
        }
    }
}
