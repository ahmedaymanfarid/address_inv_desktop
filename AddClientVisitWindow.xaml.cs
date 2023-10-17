using address_inv_library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using real_estate_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for AddClientVisitWindow.xaml
    /// </summary>
    public partial class AddClientVisitWindow : Window
    {
        CommonQueries commonQueries;
        Employee loggedInUser;
        ClientVisit clientVisit;

        List<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT> leads;

        public AddClientVisitWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();

            loggedInUser = mloggedInUser;

            CalendarDateRange lastVisitDateRange = new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue);
            visitDatePicker.BlackoutDates.Add(lastVisitDateRange);

            clientVisit = new ClientVisit(loggedInUser);
            commonQueries = new CommonQueries();

            leads = new List<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT>();

            InitializeLeadsComboBox();

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool InitializeLeadsComboBox()
        {
            leadComboBox.Items.Clear();

            if (commonQueries.GetEmployeeLeads(loggedInUser.GetEmployeeId(), ref leads))
                return false;

            for (int i = 0; i < leads.Count; i++)
                leadComboBox.Items.Add(leads[i].contact.contact_name);

            if (leads.Count == 1)
                leadComboBox.SelectedIndex = 0;
            else
            {
                leadComboBox.IsEnabled = true;
                leadComboBox.SelectedIndex = 0;
            }

            return true;
        }




        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool CheckLeadNameComboBox()
        {
            if (leadComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Lead must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckVisitDatePicker()
        {
            if (visitDatePicker.SelectedDate == null)
            {
                System.Windows.Forms.MessageBox.Show("Visit Date must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            clientVisit.SetVisitDate(DateTime.Parse(visitDatePicker.SelectedDate.ToString()));

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedLead(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedVisitDate(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnTextChangeNotes(object sender, RoutedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckLeadNameComboBox())
                return;
            if (!CheckVisitDatePicker())
                return;
            
            clientVisit.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
            clientVisit.InitializeLeadInfo(leads[leadComboBox.SelectedIndex].contact.contact_id);
            
            clientVisit.SetVisitDate(Convert.ToDateTime(visitDatePicker.Text));

            clientVisit.SetVisitNotes(additionalDescriptionTextBox.Text.ToString());
            
            clientVisit.IssueNewVisit();

            
            this.Close();
        }
    }
}
