using Microsoft.EntityFrameworkCore;
using ShoeShop;
using Shoeshop2.DbContexts;
using Shoeshop2.Entities;
using ShoeShop2;
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
using System.Windows.Shapes;

namespace Shoeshop2
{
    public partial class Catalog : Window
    {
        private ShoeshopContext _context = new ShoeshopContext();
        private List<Product> _allProducts = new List<Product>();

        public Catalog()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _allProducts = _context.Products
                .Include(p => p.Cat)
                .Include(p => p.ProdNameNavigation)
                .Include(p => p.Manuf)
                .Include(p => p.Sup)
                .ToList();

            var suppliers = _context.Suppliers.ToList();
            suppliers.Insert(0, new Supplier { IdSup = 0, SupName = "Все поставщики" });
            SuppierCB.ItemsSource = suppliers;

            SuppierCB.SelectionChanged -= SuppierCB_SelectionChanged;
            SuppierCB.SelectedIndex = 0;
            SuppierCB.SelectionChanged += SuppierCB_SelectionChanged;


            UserNameTB.Text = CurrentUser.FullName;
            SortGrid.Visibility = CurrentUser.IsGuest ? Visibility.Collapsed : Visibility.Visible;

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (prodList == null || _allProducts == null) return;

            var filtered = _allProducts.AsEnumerable();

            string searchText = SearchTB.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(p =>
                    (p.ProdNameNavigation?.ProdName1?.ToLower().Contains(searchText) == true) ||
                    (p.Cat?.CatName?.ToLower().Contains(searchText) == true) ||
                    (p.Descrip?.ToLower().Contains(searchText) == true) ||
                    (p.Manuf?.ManufName?.ToLower().Contains(searchText) == true)
                );
            }

            if (SuppierCB.SelectedValue is int selectedSupplierId && selectedSupplierId != 0)
            {
                filtered = filtered.Where(p => p.SupId == selectedSupplierId);
            }

            if (SortCB.SelectedIndex == 1)
                filtered = filtered.OrderBy(p => p.Price * (1 - (decimal)p.Sale / 100));
            else if (SortCB.SelectedIndex == 2)
                filtered = filtered.OrderByDescending(p => p.Price * (1 - (decimal)p.Sale / 100));

            prodList.ItemsSource = filtered.ToList();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SuppierCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void exBtn_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}