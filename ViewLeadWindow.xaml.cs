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
using crm_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for ViewLeadWindow.xaml
    /// </summary>
    public partial class ViewLeadWindow : NavigationWindow
    {

        public ViewLeadWindow(ref Employee mLoggedInUser, ref Lead mLead)
        {
            InitializeComponent();

            LeadInfoPage leadInfoPage = new LeadInfoPage(ref mLoggedInUser, ref mLead);
            NavigationService.Navigate(leadInfoPage);
        }

    }
}
