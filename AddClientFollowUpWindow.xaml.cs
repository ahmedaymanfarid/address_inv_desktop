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
using real_estate_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for AddClientFollowUpWindow.xaml
    /// </summary>
    public partial class AddClientFollowUpWindow : Window
    {
        CommonQueries commonQueries;
        Employee loggedInUser;
        ClientFollowUp clientFollowUp;

        List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> leads;
        public AddClientFollowUpWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();
            loggedInUser = mloggedInUser;

            CalendarDateRange followUpDateRange = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1));
            FollowUpDatePicker.BlackoutDates.Add(followUpDateRange);

            clientFollowUp = new ClientFollowUp(ref loggedInUser);
            commonQueries = new CommonQueries();

            leads = new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();

            if (!InitializeLeadsComboBox())
                return;
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool InitializeLeadsComboBox()
        {
            leadComboBox.Items.Clear();

            if (!commonQueries.GetEmployeeLeads(loggedInUser.GetEmployeeId(), ref leads))
                return false;

            for (int i = 0; i < leads.Count; i++)
                leadComboBox.Items.Add(leads[i].contact.contact_name);

            leadComboBox.SelectedIndex = 0;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CheckLeadNameComboBox()
        {
            if (leadComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Lead must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckFollowUpDatePicker()
        {
            if (FollowUpDatePicker.SelectedDate == null)
            {
                System.Windows.Forms.MessageBox.Show("FollowUp Date must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            clientFollowUp.SetFollowUpDate(DateTime.Parse(FollowUpDatePicker.SelectedDate.ToString()));

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedLead(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedFollowUpDate(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnTextChangeNotes(object sender, RoutedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckLeadNameComboBox())
                return;
            if (!CheckFollowUpDatePicker())
                return;

            clientFollowUp.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
            clientFollowUp.InitializeLeadInfo(leads[leadComboBox.SelectedIndex].contact.contact_id);

            clientFollowUp.SetFollowUpDate(Convert.ToDateTime(FollowUpDatePicker.Text));

            clientFollowUp.SetFollowUpNotes(additionalDescriptionTextBox.Text.ToString());

            clientFollowUp.IssueNewFollowUp();

            this.Close();
        }
    }
}
