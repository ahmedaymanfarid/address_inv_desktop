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
using address_inv_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for ViewClientVisitWindow.xaml
    /// </summary>
    public partial class ViewClientVisitWindow : Window
    {
        ClientVisit visitInfo;
        public ViewClientVisitWindow(ref ClientVisit mVisitInfo)
        {
            visitInfo = mVisitInfo;
            InitializeComponent();

            companyNameTextBox.IsEnabled = false;
            companyBranchTextBox.IsEnabled = false;

            contactNameTextBox.IsEnabled = false;

            visitDateTextBox.IsEnabled = false;
            visitPurposeTextBox.IsEnabled = false;
            visitResultTextBox.IsEnabled = false;
            
            additionalDescriptionTextBox.IsEnabled = false;

            contactNameTextBox.Text = visitInfo.GetLeadName();

            visitDateTextBox.Text = visitInfo.GetVisitDate().ToString();

            visitPurposeTextBox.Text = visitInfo.GetVisitPurpose();
            visitResultTextBox.Text = visitInfo.GetVisitResult();

            additionalDescriptionTextBox.Text = visitInfo.GetVisitNotes();

        }
      
    }
}
