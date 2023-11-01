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
    /// Interaction logic for ClientAttempts.xaml
    /// </summary>
    public partial class ClientAttemptsPage : Page
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

        protected List<REAL_STATE_MACROS.CLIENT_ATTEMPT_STRUCT> attemptsList;
        protected List<REAL_STATE_MACROS.CLIENT_ATTEMPT_STRUCT> filteredAttempts;

        public ClientAttemptsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            commonQueriesObject = new CommonQueries();
            CommonFunctionsObject = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            listOfLeads = new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();

            attemptsList = new List<REAL_STATE_MACROS.CLIENT_ATTEMPT_STRUCT>();
            filteredAttempts = new List<REAL_STATE_MACROS.CLIENT_ATTEMPT_STRUCT>();

            loggedInUser = mLoggedInUser;
            currentDateTime = DateTime.Now;

        }
        private bool GetAttemptsReport()
        {
            if (!commonQueriesObject.GetClientAttempts(queryStartDateTime, queryEndDateTime, ref attemptsList))
                return false;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        //////////////////////////////////////////////////////////
        /// DEFAULT SETTINGS
        //////////////////////////////////////////////////////////
        
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

        //private void OnClickListView(object sender, MouseButtonEventArgs e)
        //{
        //    listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");
        //    tableViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

        //    stackPanelScrollViewer.Visibility = Visibility.Visible;
        //    gridScrollViewer.Visibility = Visibility.Collapsed;
        //}

        //private void OnClickTableView(object sender, MouseButtonEventArgs e)
        //{
        //    listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
        //    tableViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

        //    stackPanelScrollViewer.Visibility = Visibility.Collapsed;
        //    gridScrollViewer.Visibility = Visibility.Visible;
        //}

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
        private void OnButtonClickedContacts(object sender, RoutedEventArgs e)
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
            AddClientAttemptWindow addClientAttemptWindow = new AddClientAttemptWindow(ref loggedInUser);
            addClientAttemptWindow.Closed += OnClosedAddAttemptWindow;
            addClientAttemptWindow.Show();
        }
        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            ClientVisit selectedVisit = new ClientVisit();
            selectedVisit.InitializeClientVisitInfo(filteredAttempts[ClientAttemptsStackPanel.Children.IndexOf(currentSelectedVisitItem)].attempt_serial, filteredAttempts[ClientAttemptsStackPanel.Children.IndexOf(currentSelectedVisitItem)].sales_person_id);

            ViewClientVisitWindow viewClientVisitWindow = new ViewClientVisitWindow(ref selectedVisit);
            viewClientVisitWindow.Show();
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            //ExcelExport excelExport = new ExcelExport(clientAttemptsGrid);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void OnSelChangedYearCombo(object sender, RoutedEventArgs e)
        //{
        //    filterOptions.selectedYear = BASIC_MACROS.CRM_START_YEAR + (yearComboBox.SelectedIndex == -1 ? 0 : yearComboBox.SelectedIndex);

        //    InitializeMonthComboBox();
        //}
        //private void OnSelChangedMonthCombo(object sender, RoutedEventArgs e)
        //{
        //    filterOptions.selectedMonth = (monthComboBox.SelectedIndex == -1 ? 0 : monthComboBox.SelectedIndex) + 1;

        //    UpdateDateValues();

        //    if (!GetAttemptsReport())
        //        return;

        //    InitializeAttemptsStackPanel();
        //    InitializeAttemptsGrid();

        //    dayCheckBox.IsChecked = false;
        //}
        //private void OnSelChangedDayCombo(object sender, RoutedEventArgs e)
        //{
        //    filterOptions.selectedDay = (dayComboBox.SelectedIndex == -1 ? 0 : dayComboBox.SelectedIndex) + 1;

        //    UpdateDateValues();

        //    if (!GetAttemptsReport())
        //        return;

        //    InitializeAttemptsStackPanel();
        //    InitializeAttemptsGrid();
        //}

        //private void OnSelChangedEmployeeCombo(object sender, RoutedEventArgs e)
        //{
        //    if (employeeCheckBox.IsChecked == true)
        //        filterOptions.selectedEmployee = listOfEmployees[employeeComboBox.SelectedIndex].employee_id;
        //    else
        //        filterOptions.selectedEmployee = 0;

        //    leadCheckBox.IsChecked = false;

        //    InitializeLeadComboBox();
        //    InitializeAttemptsStackPanel();
        //    InitializeAttemptsGrid();
        //}

        //private void OnSelChangedLeadCombo(object sender, RoutedEventArgs e)
        //{
        //    if (leadCheckBox.IsChecked == true)
        //        filterOptions.selectedLead = listOfLeads[leadComboBox.SelectedIndex].contact.contact_id;
        //    else
        //        filterOptions.selectedLead = 0;

        //    InitializeAttemptsStackPanel();
        //    InitializeAttemptsGrid();
        //}
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void OnCheckYearCheckBox(object sender, RoutedEventArgs e)
        //{
        //    filterOptions.isYearSelected = true;

        //    yearComboBox.IsEnabled = true;
        //}
        //private void OnCheckMonthCheckBox(object sender, RoutedEventArgs e)
        //{
        //    filterOptions.isMonthSelected = true;

        //    monthComboBox.IsEnabled = true;
        //    dayCheckBox.IsEnabled = true;

        //    InitializeMonthComboBox();
        //}
        //private void OnCheckDayCheckBox(object sender, RoutedEventArgs e)
        //{
        //    filterOptions.isDaySelected = true;

        //    dayComboBox.IsEnabled = true;

        //    InitializeDayComboBox();
        //}

        //private void OnCheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        //{
        //    filterOptions.isEmployeeSelected = true;

        //    employeeComboBox.SelectedIndex = 0;

        //    for (int i = 0; i < listOfEmployees.Count; i++)
        //        if (loggedInUser.GetEmployeeId() == listOfEmployees[i].employee_id)
        //            employeeComboBox.SelectedIndex = i;

        //    employeeComboBox.IsEnabled = true;
        //    leadCheckBox.IsEnabled = true;
        //}
        //private void OnCheckLeadCheckBox(object sender, RoutedEventArgs e)
        //{
        //    filterOptions.isLeadSelected = true;

        //    leadComboBox.SelectedIndex = 0;

        //    leadComboBox.IsEnabled = true;
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       


        private void OnClosedAddAttemptWindow(object sender, EventArgs e)
        {
            GetAttemptsReport();
        }

        private void OnButtonClickedNewLeads(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
