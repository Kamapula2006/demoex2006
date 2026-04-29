
using Microsoft.EntityFrameworkCore;
using Shoeshop2;
using Shoeshop2.DbContexts;
using ShoeShop2;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShoeShop
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = emailtb.Text.Trim();
            string passw = passwordtb.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(passw))
            {
                MessageBox.Show("Введите email и пароль");
                return;
            }

            using var db = new ShoeshopContext();

            var user = db.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email && u.Passw == passw);

            if (user == null)
            {
                MessageBox.Show("Неверный email или пароль");
                return;
            }

            CurrentUser.Id = user.IdUser;
            CurrentUser.FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}";
            CurrentUser.Role = user.Role.RoleName;

            OpenProdWin();

        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CurrentUser.Id = 0;
            CurrentUser.Role = "Гость";
            CurrentUser.FullName = "Гость";
            OpenProdWin();
        }

        private void OpenProdWin()
        {
            Catalog mn = new();
            mn.Show();
            Close();
        }
    }

}