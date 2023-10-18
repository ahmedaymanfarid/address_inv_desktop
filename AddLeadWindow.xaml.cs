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
    /// Interaction logic for AddLeadWindow.xaml
    /// </summary>
    public partial class AddLeadWindow : Window
    {
        protected String sqlQuery;

        public Employee loggedInUser;
        public Lead lead;
        public ClientCall lastCall;
        public ClientAttempt lastAttempt;
        public ClientFollowUp followUp;

        protected SQLServer sqlServer;
        protected CommonQueries commonQueries;
        protected CommonFunctions commonFunctions;
        protected IntegrityChecks integrityChecker;

        String firstName;
        String lastName;

        protected List<COMPANY_ORGANISATION_MACROS.JOB_TITLE_STRUCT> jobTitles;
        protected List<BASIC_STRUCTS.LEAD_STATUS_STRUCT> leadsStatus;
        protected List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT> budgetRanges;
        protected List<REAL_STATE_MACROS.PAYMENT_METHOD_STRUCT> paymentMethods;
        protected List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT> listOfTags;
        protected List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT> listOfSelectedTags;


        protected List<BASIC_STRUCTS.STATE_STRUCT> listOfStates;
        protected List<BASIC_STRUCTS.CITY_STRUCT> listOfCities;
        protected List<BASIC_STRUCTS.DISTRICT_STRUCT> listOfDistricts;
        public AddLeadWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            sqlServer = new SQLServer();
            commonQueries = new CommonQueries();
            commonFunctions = new CommonFunctions();
            integrityChecker = new IntegrityChecks();

            lead = new Lead();

            lastCall = new ClientCall(ref mLoggedInUser);
            lastAttempt = new ClientAttempt(ref mLoggedInUser);
            followUp = new ClientFollowUp(ref mLoggedInUser);

            jobTitles = new List<COMPANY_ORGANISATION_MACROS.JOB_TITLE_STRUCT>();
            leadsStatus = new List<BASIC_STRUCTS.LEAD_STATUS_STRUCT>();
            budgetRanges = new List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT>();
            paymentMethods = new List<REAL_STATE_MACROS.PAYMENT_METHOD_STRUCT>();
            listOfTags = new List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT>();
            listOfSelectedTags = new List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT>();

            listOfStates = new List<BASIC_STRUCTS.STATE_STRUCT>();
            listOfCities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            listOfDistricts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();
            InitializeGenderComboBox();

            if (!InitializeJobTitlesComboBox())
                return;

            if (!InitializeLeadStatusComboBox())
                return;

            if (!InitializeBudgetRangeComboBox())
                return;

            if (!InitializePaymentMethodComboBox())
                return;

            if (!InitializeStateComboBox())
                return;

            if (!InitializeTagsStackPanel())
                return;

            DisableNecessaryItems();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void DisableNecessaryItems()
        {
            cityComboBox.IsEnabled = false;
            districtComboBox.IsEnabled = false;
        }
        
        private void InitializeGenderComboBox()
        {
            leadGenderComboBox.Items.Add("Male");
            leadGenderComboBox.Items.Add("Female");
        }
        private bool InitializeJobTitlesComboBox()
        {
            if (!commonQueries.GetJobTitles(ref jobTitles))
                return false;

            for (int i = 0; i < jobTitles.Count; i++)
                leadJobtitleComboBox.Items.Add(jobTitles[i].job_name);

            return true;
        }

        private bool InitializeLeadStatusComboBox()
        {
            if (!commonQueries.GetLeadsStatus(ref leadsStatus))
                return false;

            for (int i = 0; i < leadsStatus.Count; i++)
                leadStatusComboBox.Items.Add(leadsStatus[i].lead_status);

            return true;
        }

        private bool InitializeBudgetRangeComboBox()
        {
            if (!commonQueries.GetBudgetRanges(ref budgetRanges))
                return false;

            for (int i = 0; i < budgetRanges.Count; i++)
                leadBudgetComboBox.Items.Add(budgetRanges[i].budget_range);

            return true;
        }

        private bool InitializePaymentMethodComboBox()
        {
            if (!commonQueries.GetPaymentMethods(ref paymentMethods))
                return false;

            for (int i = 0; i < paymentMethods.Count; i++)
                leadPaymentComboBox.Items.Add(paymentMethods[i].payment_method);

            return true;
        }

        private bool InitializeStateComboBox()
        {
            if (!commonQueries.GetAllCountryStates(BASIC_MACROS.EGYPT_COUNTRY_ID, ref listOfStates))
                return false;

            for (int i = 0; i < listOfStates.Count(); i++)
                stateComboBox.Items.Add(listOfStates[i].state_name);

            return true;
        }
        private bool InitializeCityComboBox()
        {
            cityComboBox.Items.Clear();

            if (stateComboBox.SelectedIndex != -1)
                if (!commonQueries.GetAllStateCities(listOfStates[stateComboBox.SelectedIndex].state_id, ref listOfCities))
                    return false;

            for (int i = 0; i < listOfCities.Count(); i++)
                cityComboBox.Items.Add(listOfCities[i].city_name);

            return true;
        }
        private bool InitializeDistrictComboBox()
        {
            districtComboBox.Items.Clear();

            if (cityComboBox.SelectedIndex != -1)
                if (!commonQueries.GetAllCityDistricts(listOfCities[cityComboBox.SelectedIndex].city_id, ref listOfDistricts))
                    return false;

            for (int i = 0; i < listOfDistricts.Count(); i++)
                districtComboBox.Items.Add(listOfDistricts[i].district_name);

            return true;
        }

        private bool InitializeTagsStackPanel()
        {
            TagsStackPanel.Children.Clear();

            if (!commonQueries.GetPropertyTags(ref listOfTags))
                return false;

            for (int i = 0; i < listOfTags.Count(); i++)
            {
                if (lead.GetLeadInterests().Exists(tag_tem => tag_tem.tag_id == listOfTags[i].tag_id))
                    continue;

                Label currentTagLabel = new Label();
                currentTagLabel.Style = (Style)FindResource("BorderIconTextLabel");
                currentTagLabel.Content = listOfTags[i].property_tag;

                Border currentBorder = new Border();
                currentBorder.Style = (Style)FindResource("BorderIcon");
                currentBorder.Child = currentTagLabel;
                currentBorder.MouseDown += OnMouseDownBorderIcon;

                TagsStackPanel.Children.Add(currentBorder);
            }

            return true;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnSelChangedGender(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedJobTitle(object sender, SelectionChangedEventArgs e)
        {
        }
        private void OnSelChangeLeadStatus(object sender, SelectionChangedEventArgs e)
        {

        }
        private void OnSelChangedBudgetRange(object sender, SelectionChangedEventArgs e)
        {
        }
        private void OnSelChangedPaymentMethod(object sender, SelectionChangedEventArgs e)
        {
        }
        private void OnSelChangedInterestedArea(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelChangedState(object sender, SelectionChangedEventArgs e)
        {
            if (!InitializeCityComboBox())
                return;

            cityComboBox.IsEnabled = true;
            districtComboBox.IsEnabled = false;
        }
        private void OnSelChangedCity(object sender, SelectionChangedEventArgs e)
        {
            if (!InitializeDistrictComboBox())
                return;

            districtComboBox.IsEnabled = true;
        }
        private void OnSelChangedDistrict(object sender, SelectionChangedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON MOUSE DOWN HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseDownBorderIcon(object sender, RoutedEventArgs e)
        {
            Border currentSelectedIcon = (Border)sender;
            Label currentSelectedLabel = (Label)currentSelectedIcon.Child;

            BrushConverter brush = new BrushConverter();

            int currentSelectedIndex = TagsStackPanel.Children.IndexOf(currentSelectedIcon);
            REAL_STATE_MACROS.PROPERTY_TAG_STRUCT currentSelectedTag = listOfTags[currentSelectedIndex];

            if (!listOfSelectedTags.Exists(tag_item => tag_item.tag_id == currentSelectedTag.tag_id))
            {
                currentSelectedIcon.Background = (Brush)brush.ConvertFrom("#EDEDED");
                currentSelectedLabel.Foreground = (Brush)brush.ConvertFrom("#000000");

                listOfSelectedTags.Add(currentSelectedTag);
            }

            else
            {
                currentSelectedIcon.Background = (Brush)brush.ConvertFrom("#000000");
                currentSelectedLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
                listOfSelectedTags.Remove(listOfSelectedTags.Find(tag_item => tag_item.tag_id == currentSelectedTag.tag_id));
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void OnTextChangedFirstName(object sender, TextChangedEventArgs e)
        {

        }
        private void OnTextChangedLastName(object sender, TextChangedEventArgs e)
        {

        }
        private void OnTextChangedBusinessPhone(object sender, TextChangedEventArgs e)
        {
        }
        private void OnTextChangedPersonalPhone(object sender, TextChangedEventArgs e)
        {
        }

        private void OnTextChangedBusinessEmail(object sender, TextChangedEventArgs e)
        {
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CHECKER FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool CheckLeadFirstNameEditBox()
        {
            String inputString = leadFirstNameTextBox.Text;
            String outputString = leadFirstNameTextBox.Text;


            if (!integrityChecker.CheckLeadNameEditBox(inputString, ref outputString, true))
                return false;

            firstName = outputString;
            lead.SetLeadName(firstName + " " + lastName);
            leadFirstNameTextBox.Text = firstName;

            return true;
        }

        private bool CheckLeadLastNameEditBox()
        {
            String inputString = leadLastNameTextBox.Text;
            String outputString = leadLastNameTextBox.Text;

            if (!integrityChecker.CheckLeadNameEditBox(inputString, ref outputString, true))
                return false;

            lastName = outputString;
            lead.SetLeadName(firstName + " " + lastName);
            leadLastNameTextBox.Text = lastName;

            return true;
        }

        private bool CheckLeadGenderComboBox()
        {
            if (leadGenderComboBox.SelectedItem != null)
                lead.SetLeadGender(leadGenderComboBox.SelectedItem.ToString());

            return true;
        }

        private bool CheckJobTitleComboBox()
        {
            if (leadJobtitleComboBox.SelectedItem != null)
                lead.SetLeadJobTitle(jobTitles[leadJobtitleComboBox.SelectedIndex].job_id, leadJobtitleComboBox.SelectedItem.ToString());

            return true;
        }
        private bool CheckStatusComboBox()
        {
            if (leadStatusComboBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Lead status must be specified.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            lead.SetLeadStatus(leadsStatus[leadStatusComboBox.SelectedIndex].status_id, leadsStatus[leadStatusComboBox.SelectedIndex].lead_status);

            return true;
        }
        private bool CheckBudgetRangeComboBox()
        {
            if (leadBudgetComboBox.SelectedItem != null)
                lead.SetLeadBudgetRange(budgetRanges[leadBudgetComboBox.SelectedIndex]);

            return true;
        }
        
        private bool CheckPaymentMethodComboBox()
        {
            if (leadPaymentComboBox.SelectedItem != null)
                lead.SetLeadPaymentMethod(paymentMethods[leadPaymentComboBox.SelectedIndex]);

            return true;
        }
        private bool CheckLeadBusinessPhoneEditBox()
        {
            String inputString = leadBusinessPhoneTextBox.Text;
            String outputString = leadBusinessPhoneTextBox.Text;

            if (!integrityChecker.CheckLeadPhoneEditBox(inputString, ref outputString, true))
                return false;

            leadBusinessPhoneTextBox.Text = outputString;

            return true;
        } 
        
        private bool CheckLeadPersonalPhoneEditBox()
        {
            String inputString = leadPersonalPhoneTextBox.Text;
            String outputString = leadPersonalPhoneTextBox.Text;

            if (!integrityChecker.CheckLeadPhoneEditBox(inputString, ref outputString, false))
                return false;

            if(outputString!= string.Empty)
            {
               lead.AddNewLeadPhone(outputString);
               leadPersonalPhoneTextBox.Text = outputString;
            }

            return true;
        }

        private bool CheckCallDatePicker()
        {
            if(callDatePicker.SelectedDate != null)
            {
                lastCall.SetCallDate((DateTime)callDatePicker.SelectedDate);
                lastCall.SetCallLead(lead);

                if (!lastCall.IssueNewCall())
                    return false;
            }

            return true;
        }

        private bool CheckAttemptDatePicker()
        {
            if (attemptDatePicker.SelectedDate != null)
            {
                lastAttempt.SetAttemptDate((DateTime)attemptDatePicker.SelectedDate);
                lastAttempt.SetAttemptLead(lead);

                if (!lastAttempt.IssueNewAttempt())
                    return false;
            }

            return true;
        }

        private bool CheckFollowUpDatePicker()
        {
            if (followUpDatePicker.SelectedDate != null)
            {
                followUp.SetFollowUpDate((DateTime)followUpDatePicker.SelectedDate);
                followUp.SetFollowUpLead(lead);

                if (!followUp.IssueNewFollowUp())
                    return false;
            }

            return true;
        }

        private bool CheckInterestedAreas()
        {
            if (stateComboBox.SelectedIndex != -1 && cityComboBox.SelectedIndex != -1 && districtComboBox.SelectedIndex != -1)
                if (!lead.AddNewInterestedArea(listOfDistricts[districtComboBox.SelectedIndex].district_id, listOfDistricts[districtComboBox.SelectedIndex].district_name))
                    return false;

            return true;
        }

        private bool CheckNotes()
        {
            if (notesTextBox.Text != String.Empty)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT employeeItem = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT();
                employeeItem.employee_id = loggedInUser.GetEmployeeId();
                employeeItem.employee_name = loggedInUser.GetEmployeeName();

                if (!lead.AddNewLeadNote(BASIC_STRUCTS.LEAD_NOTE_TYPE.GENERAL_NOTE, employeeItem, notesTextBox.Text.ToString()))
                    return false;
            }

            return true;
        }

        private bool CheckInterestedTags()
        {
            for (int i = 0; i < listOfSelectedTags.Count; i++)
            {
                if (!lead.AddNewTag(listOfSelectedTags[i].tag_id, listOfSelectedTags[i].property_tag))
                    return false;
            }

            return true;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckLeadFirstNameEditBox())
                return;
            if (!CheckLeadLastNameEditBox())
                return;
            if (!CheckStatusComboBox())
                return;
            if (!CheckLeadBusinessPhoneEditBox())
                return;

            if (!CheckLeadGenderComboBox())
                return;
            if (!CheckJobTitleComboBox())
                return;
            
            if (!CheckBudgetRangeComboBox())
                return;
            if (!CheckPaymentMethodComboBox())
                return;
            
            if (!CheckLeadPersonalPhoneEditBox())
                return;

            lead.SetSalesPerson(loggedInUser);

            if (!lead.IssueNewLead())
                return;

            if (!CheckCallDatePicker())
                return;
            if (!CheckAttemptDatePicker())
                return;
            if (!CheckFollowUpDatePicker())
                return;

            if (!CheckInterestedAreas())
                return;

            if (!CheckInterestedTags())
                return;

            if (!CheckNotes())
                return;

            if (leadBusinessPhoneTextBox.Text != String.Empty)
                if (!lead.AddNewLeadPhone(leadPersonalPhoneTextBox.Text))
                    return;

            if (leadPersonalPhoneTextBox.Text != String.Empty)
                if (!lead.AddNewLeadPhone(leadPersonalPhoneTextBox.Text))
                    return;

            
            this.Close();
        }


    }
}
