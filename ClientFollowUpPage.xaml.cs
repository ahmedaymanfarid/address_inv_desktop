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
using address_inv_library;
using real_estate_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for CleitnFollowUpPage.xaml
    /// </summary>
    public partial class ClientFollowUpPage : Page
    {
        private Employee loggedInUser;

        protected CommonQueries commonQueriesObject;
        protected CommonFunctions CommonFunctionsObject;

        private Grid previousSelectedVisitItem;
        private Grid currentSelectedVisitItem;

        private DateTime queryStartDateTime;
        private DateTime queryEndDateTime;
        private DateTime currentDateTime;

        private BASIC_STRUCTS.FILTER_STRUCT filterOptions;

        private List<int> listOfYears = new List<int>();
        private List<String> listOfMonths = new List<string>();

        protected List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;
        protected List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> listOfLeads;
        protected List<REAL_STATE_MACROS.PROPERTY_STRUCT> listOfProperties;

        protected List<REAL_STATE_MACROS.CLIENT_FOLLOWUP_STRUCT> follow_upsList;
        protected List<REAL_STATE_MACROS.CLIENT_FOLLOWUP_STRUCT> filteredFollowUp;

        public ClientFollowUpPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            commonQueriesObject = new CommonQueries();
            CommonFunctionsObject = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            listOfLeads = new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();

            follow_upsList = new List<REAL_STATE_MACROS.CLIENT_FOLLOWUP_STRUCT>();
            filteredFollowUp = new List<REAL_STATE_MACROS.CLIENT_FOLLOWUP_STRUCT>();

            loggedInUser = mLoggedInUser;
            currentDateTime = DateTime.Now;

            SetDateDefaultValues();
            SetDefaultSettings();
        }
        private bool GetFollowUpReport()
        {
            if (!commonQueriesObject.GetClientFollowUps(queryStartDateTime, queryEndDateTime, ref follow_upsList))
                return false;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitializeFollowUpStackPanel()
        {
            ClientFollowUpStackPanel.Children.Clear();
            filteredFollowUp.Clear();

            for (int i = 0; i < follow_upsList.Count; i++)
            {
                //if (yearCheckBox.IsChecked == true && follow_upsList[i].follow_up_date.Year != filterOptions.selectedYear)
                //    continue;
                //
                //if (monthCheckBox.IsChecked == true && follow_upsList[i].follow_up_date.Month != filterOptions.selectedMonth)
                //    continue;
                //
                //if (dayCheckBox.IsChecked == true && follow_upsList[i].follow_up_date.Day != filterOptions.selectedDay)
                //    continue;
                //
                //if (employeeCheckBox.IsChecked == true && follow_upsList[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                //    continue;
                //
                //if (leadCheckBox.IsChecked == true && follow_upsList[i].lead_id != listOfLeads[leadComboBox.SelectedIndex].contact.contact_id)
                //    continue;

                filteredFollowUp.Add(follow_upsList[i]);

                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;

                Label companyAndLeadLabel = new Label();
                companyAndLeadLabel.Content = follow_upsList[i].lead_name;
                companyAndLeadLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesPersonNameLabel = new Label();
                salesPersonNameLabel.Content = follow_upsList[i].sales_person_name;
                salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label dateOfVisitLabel = new Label();
                dateOfVisitLabel.Content = follow_upsList[i].follow_up_date;
                dateOfVisitLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label follow_upNotesLabel = new Label();
                follow_upNotesLabel.Content = follow_upsList[i].followup_notes;
                follow_upNotesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label lineLabel = new Label();
                lineLabel.Content = "";
                lineLabel.Style = (Style)FindResource("stackPanelItemBody");

                currentStackPanel.Children.Add(companyAndLeadLabel);
                currentStackPanel.Children.Add(salesPersonNameLabel);
                currentStackPanel.Children.Add(dateOfVisitLabel);
                currentStackPanel.Children.Add(follow_upNotesLabel);

                currentStackPanel.Children.Add(lineLabel);

                Grid newGrid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();

                newGrid.ColumnDefinitions.Add(column1);
                newGrid.MouseLeftButtonDown += OnBtnClickVisitItem;

                Grid.SetColumn(currentStackPanel, 0);

                newGrid.Children.Add(currentStackPanel);
                ClientFollowUpStackPanel.Children.Add(newGrid);

            }
        }
  
        //////////////////////////////////////////////////////////
        /// DEFAULT SETTINGS
        //////////////////////////////////////////////////////////
        private void SetDefaultSettings()
        {
            //yearCheckBox.IsChecked = true;
            //monthCheckBox.IsChecked = true;
            //
            //yearCheckBox.IsEnabled = false;
            //monthCheckBox.IsEnabled = false;
            //
            //dayComboBox.IsEnabled = false;
            //
            //if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            //{
            //    employeeCheckBox.IsChecked = false;
            //    employeeCheckBox.IsEnabled = true;
            //    employeeComboBox.IsEnabled = false;
            //
            //    leadCheckBox.IsChecked = false;
            //    leadCheckBox.IsEnabled = false;
            //    leadComboBox.IsEnabled = false;
            //}
            //else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.SENIOR_POSTION)
            //{
            //    employeeCheckBox.IsChecked = false;
            //    employeeCheckBox.IsEnabled = true;
            //    employeeComboBox.IsEnabled = false;
            //
            //    leadCheckBox.IsChecked = false;
            //    leadCheckBox.IsEnabled = false;
            //    leadComboBox.IsEnabled = false;
            //}
            //else
            //{
            //    employeeCheckBox.IsChecked = true;
            //    employeeCheckBox.IsEnabled = false;
            //    employeeComboBox.IsEnabled = false;
            //
            //    leadCheckBox.IsChecked = false;
            //    leadCheckBox.IsEnabled = true;
            //    leadComboBox.IsEnabled = false;
            //
            //}
        }
        private void SetDateDefaultValues()
        {
            filterOptions.selectedYear = currentDateTime.Year;
            filterOptions.selectedMonth = currentDateTime.Month;
            filterOptions.selectedDay = currentDateTime.Day;

            queryStartDateTime = new DateTime(currentDateTime.Year, 1, 1);
            queryEndDateTime = new DateTime(currentDateTime.Year, 12, 31);
        }

        //////////////////////////////////////////////////////////
        /// UPDATE FUNCTIONS
        //////////////////////////////////////////////////////////
        private void UpdateDateValues()
        {
            queryStartDateTime = new DateTime(filterOptions.selectedYear, 1, 1);
            queryEndDateTime = new DateTime(filterOptions.selectedYear, 12, 31);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //VIEWING TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickVisitItem(object sender, RoutedEventArgs e)
        {
            previousSelectedVisitItem = currentSelectedVisitItem;
            currentSelectedVisitItem = (Grid)sender;
            BrushConverter brush = new BrushConverter();

            if (previousSelectedVisitItem != null)
            {
                previousSelectedVisitItem.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                StackPanel previousSelectedStackPanel = (StackPanel)previousSelectedVisitItem.Children[0];

                foreach (Label childLabel in previousSelectedStackPanel.Children)
                    childLabel.Foreground = (Brush)brush.ConvertFrom("#000000");

            }

            currentSelectedVisitItem.Background = (Brush)brush.ConvertFrom("#000000");

            StackPanel currentSelectedStackPanel = (StackPanel)currentSelectedVisitItem.Children[0];

            foreach (Label childLabel in currentSelectedStackPanel.Children)
                childLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickedDashboard(object sender, RoutedEventArgs e)
        {
            DashboardPage dashboardPage = new DashboardPage(ref loggedInUser);
            this.NavigationService.Navigate(dashboardPage);
        }
        private void OnButtonClickedLeads(object sender, RoutedEventArgs e)
        {
            LeadsPage leadsPage = new LeadsPage(ref loggedInUser);
            this.NavigationService.Navigate(leadsPage);
        }
        private void OnButtonClickedOwners(object sender, MouseButtonEventArgs e)
        {
            OwnersPage ownersPage = new OwnersPage(ref loggedInUser);
            this.NavigationService.Navigate(ownersPage);
        }
        private void OnButtonClickedProperties(object sender, RoutedEventArgs e)
        {
            //PropertiesPage propertiesPage = new PropertiesPage(ref loggedInUser);
            //this.NavigationService.Navigate(propertiesPage);
        }
        private void OnButtonClickedVisits(object sender, RoutedEventArgs e)
        {
            ClientVisitsPage clientVisitsPage = new ClientVisitsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientVisitsPage);
        }
        private void OnButtonClickedCalls(object sender, RoutedEventArgs e)
        {
            ClientCallsPage clientCallsPage = new ClientCallsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientCallsPage);
        }
        private void OnButtonClickedAttempts(object sender, RoutedEventArgs e)
        {
            ClientAttemptsPage clientAttemptsPage = new ClientAttemptsPage(ref loggedInUser);
            this.NavigationService.Navigate(clientAttemptsPage);
        }
        private void OnButtonClickedFollowUps(object sender, RoutedEventArgs e)
        {
            ClientFollowUpPage clientFollowUpPage = new ClientFollowUpPage(ref loggedInUser);
            this.NavigationService.Navigate(clientFollowUpPage);
        }
        private void OnButtonClickedAccounts(object sender, MouseButtonEventArgs e)
        {

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BTN CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            AddClientFollowUpWindow addClientFollowUpWindow = new AddClientFollowUpWindow(ref loggedInUser);
            addClientFollowUpWindow.Closed += OnClosedAddFollowUpWindow;
            addClientFollowUpWindow.Show();
        }
        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            ClientVisit selectedVisit = new ClientVisit();
            selectedVisit.InitializeClientVisitInfo(filteredFollowUp[ClientFollowUpStackPanel.Children.IndexOf(currentSelectedVisitItem)].follow_up_serial, filteredFollowUp[ClientFollowUpStackPanel.Children.IndexOf(currentSelectedVisitItem)].sales_person_id);

            ViewClientVisitWindow viewClientVisitWindow = new ViewClientVisitWindow(ref selectedVisit);
            viewClientVisitWindow.Show();
        }

      
     

        private void OnClosedAddFollowUpWindow(object sender, EventArgs e)
        {
            GetFollowUpReport();
            InitializeFollowUpStackPanel();
        }

    }
}
