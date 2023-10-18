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

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for LeadInfoPage.xaml
    /// </summary>
    public partial class LeadInfoPage : Page
    {
        protected Employee loggedInUser;

        protected CommonQueries commonQueries;


        List<COMPANY_ORGANISATION_MACROS.JOB_TITLE_STRUCT> jobTitles;
        List<BASIC_STRUCTS.LEAD_STATUS_STRUCT> leadsStatus;
        List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT> budgetRanges;
        List<REAL_STATE_MACROS.PAYMENT_METHOD_STRUCT> paymentMethods;

        Lead lead;

        public LeadInfoPage(ref Employee mLoggedInUser, ref Lead mLead)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;
            lead = mLead;

            commonQueries = new CommonQueries();

            jobTitles = new List<COMPANY_ORGANISATION_MACROS.JOB_TITLE_STRUCT>();
            leadsStatus = new List<BASIC_STRUCTS.LEAD_STATUS_STRUCT>();
            budgetRanges = new List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT>();
            paymentMethods = new List<REAL_STATE_MACROS.PAYMENT_METHOD_STRUCT>();

            InitiazeLeadNameTextBox();

            InitializeGenderComboBox();

            if (!InitializeJobTitlesComboBox())
                return;

            if (!InitializeLeadStatusComboBox())
                return;

            if (!InitializeBudgetRangeComboBox())
                return;

            if (!InitializePaymentMethodComboBox())
                return;

            InitializeStackPanels();

            DisableNecessaryItems();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// UI FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void DisableNecessaryItems()
        {
            leadNameTextBox.IsEnabled = false;
            leadGenderComboBox.IsEnabled = false;
            leadJobtitleComboBox.IsEnabled = false;
            leadStatusComboBox.IsEnabled = false;
            leadBudgetComboBox.IsEnabled = false;
            leadPaymentComboBox.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

        private void InitializeStackPanels()
        {
            InitializeDistrictsStackPanel();
            InitializeTagsStackPanel();
            InitializePhonesStackPanel();
            InitializeNotesStackPanel();
        }
        private void InitiazeLeadNameTextBox()
        {
            leadNameTextBox.Text = lead.GetLeadName();
        }

        private void InitializeGenderComboBox()
        {
            leadGenderComboBox.Items.Add("Male");
            leadGenderComboBox.Items.Add("Female");

            leadGenderComboBox.SelectedItem = lead.GetLeadGender();
        }
        private bool InitializeJobTitlesComboBox()
        {
            if (!commonQueries.GetJobTitles(ref jobTitles))
                return false;

            for (int i = 0; i < jobTitles.Count; i++)
                leadJobtitleComboBox.Items.Add(jobTitles[i].job_name);

            leadJobtitleComboBox.SelectedIndex = jobTitles.FindIndex(job_item => job_item.job_id == lead.GetLeadJobTitleId());

            return true;
        }

        private bool InitializeLeadStatusComboBox()
        {
            if (!commonQueries.GetLeadsStatus(ref leadsStatus))
                return false;

            for (int i = 0; i < leadsStatus.Count; i++)
                leadStatusComboBox.Items.Add(leadsStatus[i].lead_status);

            leadStatusComboBox.SelectedIndex = leadsStatus.FindIndex(status_item => status_item.status_id == lead.GetLeadStatusId());

            return true;
        }

        private bool InitializeBudgetRangeComboBox()
        {
            if (!commonQueries.GetBudgetRanges(ref budgetRanges))
                return false;

            for (int i = 0; i < budgetRanges.Count; i++)
                leadBudgetComboBox.Items.Add(budgetRanges[i].budget_range);

            leadBudgetComboBox.SelectedIndex = budgetRanges.FindIndex(budget_item => budget_item.budget_id == lead.GetLeadBudgetId());

            return true;
        }

        private bool InitializePaymentMethodComboBox()
        {
            if (!commonQueries.GetPaymentMethods(ref paymentMethods))
                return false;

            for (int i = 0; i < paymentMethods.Count; i++)
                leadPaymentComboBox.Items.Add(paymentMethods[i].payment_method);

            leadPaymentComboBox.SelectedIndex = paymentMethods.FindIndex(method_item => method_item.method_id == lead.GetLeadPaymentMethodId());

            return true;
        }

        private void InitializeDistrictsStackPanel()
        {
            DistrictsStackPanel.Children.Clear();

            for (int i = 0; i < lead.GetLeadInterestedAreas().Count(); i++)
            {
                Label currentDistrictLabel = new Label();
                currentDistrictLabel.Style = (Style)FindResource("tableSubItemLabel");
                currentDistrictLabel.Content = lead.GetLeadInterestedAreas()[i].district_name;

                DistrictsStackPanel.Children.Add(currentDistrictLabel);
            }
        }

        private void InitializePhonesStackPanel()
        {
            PhonesStackPanel.Children.Clear();

            for (int i = 0; i < lead.GetLeadPhones().Count(); i++)
            {
                Label currentPhoneLabel = new Label();
                currentPhoneLabel.Style = (Style)FindResource("tableSubItemLabel");
                currentPhoneLabel.Content = lead.GetLeadPhones()[i];

                PhonesStackPanel.Children.Add(currentPhoneLabel);
            }
        }

        private void InitializeTagsStackPanel()
        {
            TagsStackPanel.Children.Clear();

            for (int i = 0; i < lead.GetLeadInterests().Count(); i++)
            {
                Label currentTagLabel = new Label();
                currentTagLabel.Style = (Style)FindResource("BorderIconTextLabel");
                currentTagLabel.Content = lead.GetLeadInterests()[i].property_tag;

                Border currentBorder = new Border();
                currentBorder.Style = (Style)FindResource("BorderIcon");
                currentBorder.Child = currentTagLabel;

                TagsStackPanel.Children.Add(currentBorder);
            }
        }

        private void InitializeNotesStackPanel()
        {
            NotesStackPanel.Children.Clear();

            for (int i = 0; i < lead.GetLeadNotes().Count(); i++)
            {
                StackPanel currentNoteStackPanel = new StackPanel();

                Label currentSalesLabel = new Label();
                currentSalesLabel.Style = (Style)FindResource("stackPanelItemHeader");
                currentSalesLabel.Content = lead.GetLeadNotes()[i].added_by.employee_name;

                Label currentNoteLabel = new Label();
                currentNoteLabel.Style = (Style)FindResource("stackPanelItemBody");
                currentNoteLabel.Content = lead.GetLeadNotes()[i].note;

                Label currentDateLabel = new Label();
                currentDateLabel.Style = (Style)FindResource("stackPanelItemBody");
                currentDateLabel.Content = lead.GetLeadNotes()[i].note_date;


                currentNoteStackPanel.Children.Add(currentSalesLabel);
                currentNoteStackPanel.Children.Add(currentNoteLabel);
                currentNoteStackPanel.Children.Add(currentDateLabel);

                NotesStackPanel.Children.Add(currentNoteStackPanel);
            }
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClkAddDetails(object sender, RoutedEventArgs e)
        {
            AddLeadDetailsWindow addLeadDetailsWindow = new AddLeadDetailsWindow(ref loggedInUser, ref lead);
            addLeadDetailsWindow.Closed += OnClosedAddLeadDetailsWindow;
            addLeadDetailsWindow.Show();
        }
        private void OnClosedAddLeadDetailsWindow(object sender, EventArgs e)
        {
            InitializeStackPanels();
        }

        private void OnClickMatchedProperties(object sender, MouseButtonEventArgs e)
        {
            LeadMatchedPropertiesPage leadMatchedPropertiesPage = new LeadMatchedPropertiesPage(ref loggedInUser, ref lead);
            NavigationService.Navigate(leadMatchedPropertiesPage);
        }
    }
}
