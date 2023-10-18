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
using real_estate_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for AddClientAttemptWindow.xaml
    /// </summary>
    public partial class AddClientAttemptWindow : Window
    {
        CommonQueries commonQueries;
        Employee loggedInUser;
        ClientAttempt clientAttempt;

        List<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT> leads;
        public AddClientAttemptWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();
            loggedInUser = mloggedInUser;

            CalendarDateRange lastAttemptDateRange = new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue);
            AttemptDatePicker.BlackoutDates.Add(lastAttemptDateRange);

            clientAttempt = new ClientAttempt(ref loggedInUser);
            commonQueries = new CommonQueries();

            leads = new List<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT>();

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
        private bool CheckAttemptDatePicker()
        {
            if (AttemptDatePicker.SelectedDate == null)
            {
                System.Windows.Forms.MessageBox.Show("Attempt Date must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            clientAttempt.SetAttemptDate(DateTime.Parse(AttemptDatePicker.SelectedDate.ToString()));

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedLead(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedAttemptDate(object sender, SelectionChangedEventArgs e)
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
            if (!CheckAttemptDatePicker())
                return;

            clientAttempt.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
            clientAttempt.InitializeLeadInfo(leads[leadComboBox.SelectedIndex].contact.contact_id);

            clientAttempt.SetAttemptDate(Convert.ToDateTime(AttemptDatePicker.Text));

            clientAttempt.SetAttemptNotes(additionalDescriptionTextBox.Text.ToString());

            clientAttempt.IssueNewAttempt();

            this.Close();
        }
    }
}
