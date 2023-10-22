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
    /// Interaction logic for AddLeadDetailsWindow.xaml
    /// </summary>
    public partial class AddLeadDetailsWindow : Window
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;
        protected IntegrityChecks integrityChecker;

        protected Lead lead;
        public ClientCall lastCall;
        public ClientAttempt lastAttempt;
        public ClientFollowUp followUp;

        protected List<BASIC_STRUCTS.STATE_STRUCT> listOfStates;
        protected List<BASIC_STRUCTS.CITY_STRUCT> listOfCities;
        protected List<BASIC_STRUCTS.DISTRICT_STRUCT> listOfDistricts;
        protected List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT> listOfTags;

        protected List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT> listOfSelectedTags;

        protected int phonesCount;
        protected int emailsCount;

        protected String returnMessage;
        public AddLeadDetailsWindow(ref Employee mLoggedInUser, ref Lead mLead)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            lead = mLead;

            lastCall = new ClientCall(ref mLoggedInUser);
            lastAttempt = new ClientAttempt(ref mLoggedInUser);
            followUp = new ClientFollowUp(ref mLoggedInUser);

            commonQueries = new CommonQueries();

            listOfStates = new List<BASIC_STRUCTS.STATE_STRUCT>();
            listOfCities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            listOfDistricts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();
            listOfTags = new List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT>();
            listOfSelectedTags = new List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT>();

            integrityChecker = new IntegrityChecks();

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
        private bool InitializeStateComboBox()
        {
            if (!commonQueries.GetAllCountryStates(BASIC_MACROS.EGYPT_ID, ref listOfStates))
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

            if(!commonQueries.GetPropertyTags(ref listOfTags))
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
        /// TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnTextChangedNotes(object sender, TextChangedEventArgs e)
        {

        }

        private void OnTextChangedTelephone(object sender, TextChangedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SEL CHEANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
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
        /// BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (!CheckLeadPhoneEditBox())
                return;

            if (!CheckCallDatePicker())
                return;
            if (!CheckAttemptDatePicker())
                return;
            if (!CheckFollowUpDatePicker())
                return;

            if (telephoneTextBox.Text != String.Empty)
                if (!lead.AddNewLeadPhone(telephoneTextBox.Text.ToString()))
                    return;

            if (stateComboBox.SelectedIndex != -1 && cityComboBox.SelectedIndex != -1 && districtComboBox.SelectedIndex != -1)
                if (!lead.AddNewInterestedArea(listOfDistricts[districtComboBox.SelectedIndex].district_id, listOfDistricts[districtComboBox.SelectedIndex].district_name))
                    return;

            if (notesTextBox.Text != String.Empty)
            {
                COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT employeeItem = new COMPANY_ORGANISATION_MACROS.EMPLOYEE_MIN_STRUCT();
                employeeItem.employee_id = loggedInUser.GetEmployeeId();
                employeeItem.employee_name = loggedInUser.GetEmployeeName();

                if (!lead.AddNewLeadNote(COMPANY_ORGANISATION_MACROS.LEAD_NOTE_TYPE.GENERAL_NOTE, employeeItem, notesTextBox.Text.ToString()))
                    return;
            }

            for (int i = 0; i < listOfSelectedTags.Count; i++)
            {
                if (!lead.AddNewTag(listOfSelectedTags[i].tag_id, listOfSelectedTags[i].property_tag))
                    return;
            }
            
            this.Close();
        }

        
        private bool CheckLeadPhoneEditBox()
        {
            String inputString = telephoneTextBox.Text;
            String outputString = telephoneTextBox.Text;

            if (!integrityChecker.CheckLeadPhoneEditBox(inputString, ref outputString, false, ref returnMessage))
                return false;

            //lead.AddCompanyPhone(outputString);
            // lead.GetNumberOfSavedCompanyPhones();
            telephoneTextBox.Text = outputString;

            return true;
        }

        private bool CheckCallDatePicker()
        {
            if (callDatePicker.SelectedDate != null)
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
    }
}
