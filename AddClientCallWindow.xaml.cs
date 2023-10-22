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
using address_inv_library;
using real_estate_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for AddClientCallWindow.xaml
    /// </summary>
    public partial class AddClientCallWindow : Window
    {
        CommonQueries commonQueries;
        Employee loggedInUser;
        ClientCall clientCall;

        List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> leads;
        public AddClientCallWindow(ref Employee mloggedInUser)
        {
            InitializeComponent();
            loggedInUser = mloggedInUser;


            CalendarDateRange lastCallDateRange = new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue);
            CallDatePicker.BlackoutDates.Add(lastCallDateRange);

            clientCall = new ClientCall(ref loggedInUser);
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
                System.Windows.Forms.MessageBox.Show("Lead must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private bool CheckCallDatePicker()
        {
            if (CallDatePicker.SelectedDate == null)
            {
                System.Windows.Forms.MessageBox.Show("Call Date must be specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            clientCall.SetCallDate(DateTime.Parse(CallDatePicker.SelectedDate.ToString()));

            return true;
        }
 
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedLead(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedCallDate(object sender, SelectionChangedEventArgs e)
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
            if (!CheckCallDatePicker())
                return;

            clientCall.InitializeSalesPersonInfo(loggedInUser.GetEmployeeId());
            clientCall.InitializeLeadInfo(leads[leadComboBox.SelectedIndex].contact.contact_id);

            clientCall.SetCallDate(Convert.ToDateTime(CallDatePicker.Text));

            clientCall.SetCallNotes(additionalDescriptionTextBox.Text.ToString());

            clientCall.IssueNewCall();

            this.Close();
        }
    }
}
