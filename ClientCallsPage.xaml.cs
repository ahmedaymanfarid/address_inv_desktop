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
<<<<<<< HEAD
=======
using address_inv_library;
>>>>>>> f1056db924f05508e201e913f4f25c418687f515
using real_estate_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for ClientCallsPage.xaml
    /// </summary>
    public partial class ClientCallsPage : Page
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
        protected List<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT> listOfLeads;
        protected List<REAL_STATE_MACROS.PROPERTY_STRUCT> listOfProperties;

        protected List<REAL_STATE_MACROS.CLIENT_CALL_STRUCT> callsList;
        protected List<REAL_STATE_MACROS.CLIENT_CALL_STRUCT> filteredCalls;

        public ClientCallsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            
            commonQueriesObject = new CommonQueries();
            CommonFunctionsObject = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            listOfLeads = new List<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT>();

            callsList = new List<REAL_STATE_MACROS.CLIENT_CALL_STRUCT>();
            filteredCalls = new List<REAL_STATE_MACROS.CLIENT_CALL_STRUCT>();

            loggedInUser = mLoggedInUser;
            currentDateTime = DateTime.Now;

            InitializeYearComboBox();
            InitializeMonthComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            SetDateDefaultValues();
            SetDefaultSettings();
        }
        private bool GetCallsReport()
        {
            if (!commonQueriesObject.GetClientCalls(queryStartDateTime, queryEndDateTime, ref callsList))
                return false;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializeYearComboBox()
        {
            yearComboBox.Items.Clear();

            for (int i = BASIC_MACROS.CRM_START_YEAR; i <= currentDateTime.Year; i++)
            {
                listOfYears.Add(i);
                yearComboBox.Items.Add(i.ToString());
            }

            yearComboBox.SelectedIndex = currentDateTime.Year - BASIC_MACROS.CRM_START_YEAR;
        }
        private void InitializeMonthComboBox()
        {
            monthComboBox.Items.Clear();

            listOfMonths = CommonFunctionsObject.GetListOfMonths();

            for (int i = 0; (filterOptions.selectedYear != currentDateTime.Year && i < listOfMonths.Count) || (filterOptions.selectedYear == currentDateTime.Year && i < currentDateTime.Month); i++)
                monthComboBox.Items.Add(listOfMonths[i]);

            if (filterOptions.selectedYear == currentDateTime.Year)
                monthComboBox.SelectedIndex = currentDateTime.Month - 1;
            else
                monthComboBox.SelectedIndex = 0;
        }
        private void InitializeDayComboBox()
        {
            dayComboBox.Items.Clear();

            for (int i = 0; (filterOptions.selectedMonth != currentDateTime.Month && i < CommonFunctionsObject.GetMonthLength(filterOptions.selectedMonth, filterOptions.selectedYear)) || (filterOptions.selectedMonth == currentDateTime.Month && i < currentDateTime.Day); i++)
                dayComboBox.Items.Add(i + 1);

            if (filterOptions.selectedYear == currentDateTime.Year && filterOptions.selectedMonth == currentDateTime.Month)
                dayComboBox.SelectedIndex = currentDateTime.Day - 1;
            else
                dayComboBox.SelectedIndex = 0;
        }
        private bool InitializeEmployeeComboBox()
        {
            if (!commonQueriesObject.GetTeamEmployees(loggedInUser.GetEmployeeTeamId(), ref listOfEmployees))
                return false;

            for (int i = 0; i < listOfEmployees.Count; i++)
                employeeComboBox.Items.Add(listOfEmployees[i].employee_name);
            return true;
        }
        private bool InitializeLeadComboBox()
        {
            if (!commonQueriesObject.GetEmployeeLeads(filterOptions.selectedEmployee, ref listOfLeads))
                return false;

            for (int i = 0; i < listOfLeads.Count; i++)
                leadComboBox.Items.Add(listOfLeads[i].contact.contact_name);

            return true;
        }

        private void InitializeCallsStackPanel()
        {
            ClientCallsStackPanel.Children.Clear();
            filteredCalls.Clear();

            for (int i = 0; i < callsList.Count; i++)
            {
                if (yearCheckBox.IsChecked == true && callsList[i].call_date.Year != filterOptions.selectedYear)
                    continue;

                if (monthCheckBox.IsChecked == true && callsList[i].call_date.Month != filterOptions.selectedMonth)
                    continue;

                if (dayCheckBox.IsChecked == true && callsList[i].call_date.Day != filterOptions.selectedDay)
                    continue;

                if (employeeCheckBox.IsChecked == true && callsList[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                if (leadCheckBox.IsChecked == true && callsList[i].lead_id != listOfLeads[leadComboBox.SelectedIndex].contact.contact_id)
                    continue;

                filteredCalls.Add(callsList[i]);

                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;

                Label companyAndLeadLabel = new Label();
                companyAndLeadLabel.Content = callsList[i].lead_name;
                companyAndLeadLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesPersonNameLabel = new Label();
                salesPersonNameLabel.Content = callsList[i].sales_person_name;
                salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label dateOfVisitLabel = new Label();
                dateOfVisitLabel.Content = callsList[i].call_date;
                dateOfVisitLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label callNotesLabel = new Label();
                callNotesLabel.Content = callsList[i].call_notes;
                callNotesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label lineLabel = new Label();
                lineLabel.Content = "";
                lineLabel.Style = (Style)FindResource("stackPanelItemBody");

                currentStackPanel.Children.Add(companyAndLeadLabel);
                currentStackPanel.Children.Add(salesPersonNameLabel);
                currentStackPanel.Children.Add(dateOfVisitLabel);
                currentStackPanel.Children.Add(callNotesLabel);

                currentStackPanel.Children.Add(lineLabel);

                Grid newGrid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();

                newGrid.ColumnDefinitions.Add(column1);
                newGrid.MouseLeftButtonDown += OnBtnClickVisitItem;

                Grid.SetColumn(currentStackPanel, 0);

                newGrid.Children.Add(currentStackPanel);
                ClientCallsStackPanel.Children.Add(newGrid);

            }
        }
        private bool InitializeCallsGrid()
        {

            clientCallsGrid.Children.Clear();
            clientCallsGrid.RowDefinitions.Clear();
            clientCallsGrid.ColumnDefinitions.Clear();

            filteredCalls.Clear();

            Label salesPersonHeader = new Label();
            salesPersonHeader.Content = "Sales Person";
            salesPersonHeader.Style = (Style)FindResource("tableHeaderItem");

            Label dateOfVisitHeader = new Label();
            dateOfVisitHeader.Content = "Call Date";
            dateOfVisitHeader.Style = (Style)FindResource("tableHeaderItem");

            Label contactInfoHeader = new Label();
            contactInfoHeader.Content = "Lead";
            contactInfoHeader.Style = (Style)FindResource("tableHeaderItem");

            Label contactNotesHeader = new Label();
            contactNotesHeader.Content = "Call Notes";
            contactNotesHeader.Style = (Style)FindResource("tableHeaderItem");

            clientCallsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientCallsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientCallsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientCallsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            clientCallsGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(salesPersonHeader, 0);
            Grid.SetColumn(salesPersonHeader, 0);
            clientCallsGrid.Children.Add(salesPersonHeader);

            Grid.SetRow(dateOfVisitHeader, 0);
            Grid.SetColumn(dateOfVisitHeader, 1);
            clientCallsGrid.Children.Add(dateOfVisitHeader);

            Grid.SetRow(contactInfoHeader, 0);
            Grid.SetColumn(contactInfoHeader, 2);
            clientCallsGrid.Children.Add(contactInfoHeader);

            Grid.SetRow(contactNotesHeader, 0);
            Grid.SetColumn(contactNotesHeader, 3);
            clientCallsGrid.Children.Add(contactNotesHeader);

            int currentRowNumber = 1;

            for (int i = 0; i < callsList.Count; i++)
            {
                if (yearCheckBox.IsChecked == true && callsList[i].call_date.Year != filterOptions.selectedYear)
                    continue;

                if (monthCheckBox.IsChecked == true && callsList[i].call_date.Month != filterOptions.selectedMonth)
                    continue;

                if (employeeCheckBox.IsChecked == true && callsList[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                filteredCalls.Add(callsList[i]);


                RowDefinition currentRow = new RowDefinition();

                clientCallsGrid.RowDefinitions.Add(currentRow);

                Label salesPersonLabel = new Label();
                salesPersonLabel.Content = callsList[i].sales_person_name;
                salesPersonLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesPersonLabel, currentRowNumber);
                Grid.SetColumn(salesPersonLabel, 0);
                clientCallsGrid.Children.Add(salesPersonLabel);


                Label dateOfVisitLabel = new Label();
                dateOfVisitLabel.Content = callsList[i].call_date;
                dateOfVisitLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(dateOfVisitLabel, currentRowNumber);
                Grid.SetColumn(dateOfVisitLabel, 1);
                clientCallsGrid.Children.Add(dateOfVisitLabel);


                Label contactInfoLabel = new Label();
                contactInfoLabel.Content = callsList[i].lead_name;
                contactInfoLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(contactInfoLabel, currentRowNumber);
                Grid.SetColumn(contactInfoLabel, 2);
                clientCallsGrid.Children.Add(contactInfoLabel);

                Label callNotesLabel = new Label();
                callNotesLabel.Content = callsList[i].call_notes;
                callNotesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(callNotesLabel, currentRowNumber);
                Grid.SetColumn(callNotesLabel, 3);
                clientCallsGrid.Children.Add(callNotesLabel);

                //currentRow.MouseLeftButtonDown += OnBtnClickWorkOfferItem;

                currentRowNumber++;
            }

            return true;
        }

        //////////////////////////////////////////////////////////
        /// DEFAULT SETTINGS
        //////////////////////////////////////////////////////////
        private void SetDefaultSettings()
        {
            yearCheckBox.IsChecked = true;
            monthCheckBox.IsChecked = true;

            yearCheckBox.IsEnabled = false;
            monthCheckBox.IsEnabled = false;

            dayComboBox.IsEnabled = false;

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                employeeCheckBox.IsChecked = false;
                employeeCheckBox.IsEnabled = true;
                employeeComboBox.IsEnabled = false;

                leadCheckBox.IsChecked = false;
                leadCheckBox.IsEnabled = false;
                leadComboBox.IsEnabled = false;
            }
            else if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.SENIOR_POSTION)
            {
                employeeCheckBox.IsChecked = false;
                employeeCheckBox.IsEnabled = true;
                employeeComboBox.IsEnabled = false;

                leadCheckBox.IsChecked = false;
                leadCheckBox.IsEnabled = false;
                leadComboBox.IsEnabled = false;
            }
            else
            {
                employeeCheckBox.IsChecked = true;
                employeeCheckBox.IsEnabled = false;
                employeeComboBox.IsEnabled = false;

                leadCheckBox.IsChecked = false;
                leadCheckBox.IsEnabled = true;
                leadComboBox.IsEnabled = false;

            }
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

        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");
            tableViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            stackPanelScrollViewer.Visibility = Visibility.Visible;
            gridScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void OnClickTableView(object sender, MouseButtonEventArgs e)
        {
            listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
            tableViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            stackPanelScrollViewer.Visibility = Visibility.Collapsed;
            gridScrollViewer.Visibility = Visibility.Visible;
        }

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
            PropertiesPage propertiesPage = new PropertiesPage(ref loggedInUser);
            this.NavigationService.Navigate(propertiesPage);
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
            AddClientCallWindow addClientCallWindow = new AddClientCallWindow(ref loggedInUser);
            addClientCallWindow.Closed += OnClosedAddCallWindow;
            addClientCallWindow.Show();
        }
        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            ClientVisit selectedVisit = new ClientVisit();
            selectedVisit.InitializeClientVisitInfo(filteredCalls[ClientCallsStackPanel.Children.IndexOf(currentSelectedVisitItem)].call_serial, filteredCalls[ClientCallsStackPanel.Children.IndexOf(currentSelectedVisitItem)].sales_person_id);

            ViewClientVisitWindow viewClientVisitWindow = new ViewClientVisitWindow(ref selectedVisit);
            viewClientVisitWindow.Show();
        }

        private void OnBtnClickExport(object sender, RoutedEventArgs e)
        {
            ExcelExport excelExport = new ExcelExport(clientCallsGrid);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedYearCombo(object sender, RoutedEventArgs e)
        {
            filterOptions.selectedYear = BASIC_MACROS.CRM_START_YEAR + (yearComboBox.SelectedIndex == -1 ? 0 : yearComboBox.SelectedIndex);

            InitializeMonthComboBox();
        }
        private void OnSelChangedMonthCombo(object sender, RoutedEventArgs e)
        {
            filterOptions.selectedMonth = (monthComboBox.SelectedIndex == -1 ? 0 : monthComboBox.SelectedIndex) + 1;

            UpdateDateValues();

            if (!GetCallsReport())
                return;

            InitializeCallsStackPanel();
            InitializeCallsGrid();

            dayCheckBox.IsChecked = false;
        }
        private void OnSelChangedDayCombo(object sender, RoutedEventArgs e)
        {
            filterOptions.selectedDay = (dayComboBox.SelectedIndex == -1 ? 0 : dayComboBox.SelectedIndex) + 1;

            UpdateDateValues();

            if (!GetCallsReport())
                return;

            InitializeCallsStackPanel();
            InitializeCallsGrid();
        }

        private void OnSelChangedEmployeeCombo(object sender, RoutedEventArgs e)
        {
            if (employeeCheckBox.IsChecked == true)
                filterOptions.selectedEmployee = listOfEmployees[employeeComboBox.SelectedIndex].employee_id;
            else
                filterOptions.selectedEmployee = 0;

            leadCheckBox.IsChecked = false;

            InitializeLeadComboBox();
            InitializeCallsStackPanel();
            InitializeCallsGrid();
        }

        private void OnSelChangedLeadCombo(object sender, RoutedEventArgs e)
        {
            if (leadCheckBox.IsChecked == true)
                filterOptions.selectedLead = listOfLeads[leadComboBox.SelectedIndex].contact.contact_id;
            else
                filterOptions.selectedLead = 0;

            InitializeCallsStackPanel();
            InitializeCallsGrid();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isYearSelected = true;

            yearComboBox.IsEnabled = true;
        }
        private void OnCheckMonthCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isMonthSelected = true;

            monthComboBox.IsEnabled = true;
            dayCheckBox.IsEnabled = true;

            InitializeMonthComboBox();
        }
        private void OnCheckDayCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isDaySelected = true;

            dayComboBox.IsEnabled = true;

            InitializeDayComboBox();
        }

        private void OnCheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isEmployeeSelected = true;

            employeeComboBox.SelectedIndex = 0;

            for (int i = 0; i < listOfEmployees.Count; i++)
                if (loggedInUser.GetEmployeeId() == listOfEmployees[i].employee_id)
                    employeeComboBox.SelectedIndex = i;

            employeeComboBox.IsEnabled = true;
            leadCheckBox.IsEnabled = true;
        }
        private void OnCheckLeadCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isLeadSelected = true;

            leadComboBox.SelectedIndex = 0;

            leadComboBox.IsEnabled = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON UNCHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnUncheckYearCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isYearSelected = false;
            yearComboBox.SelectedIndex = -1;

            yearComboBox.IsEnabled = false;
        }
        private void OnUncheckMonthCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isMonthSelected = false;
            monthComboBox.SelectedIndex = -1;

            dayCheckBox.IsChecked = false;
            dayCheckBox.IsEnabled = false;

            monthComboBox.IsEnabled = false;
        }
        private void OnUncheckDayCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isDaySelected = false;
            dayComboBox.SelectedIndex = -1;

            dayComboBox.IsEnabled = false;
        }
        private void OnUncheckEmployeeCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isEmployeeSelected = false;
            employeeComboBox.SelectedIndex = -1;

            employeeComboBox.IsEnabled = false;
        }
        private void OnUncheckLeadCheckBox(object sender, RoutedEventArgs e)
        {
            filterOptions.isLeadSelected = false;
            leadComboBox.SelectedIndex = -1;

            leadComboBox.IsEnabled = false;
        }


        private void OnClosedAddCallWindow(object sender, EventArgs e)
        {
            GetCallsReport();
            InitializeCallsStackPanel();
            InitializeCallsGrid();
        }

    }
}
