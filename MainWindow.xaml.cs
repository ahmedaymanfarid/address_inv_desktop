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
using real_estate_library;
using address_inv_desktop;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            DashboardPage dashboard = new DashboardPage(ref mLoggedInUser);
            this.NavigationService.Navigate(dashboard);
        }
        public MainWindow()
        {
        }
    }
}
