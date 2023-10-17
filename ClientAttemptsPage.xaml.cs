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
using crm_library;

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
        protected List<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT> listOfLeads;
        protected List<REAL_STATE_MACROS.PROPERTY_STRUCT> listOfProperties;

        protected List<REAL_STATE_MACROS.CLIENT_ATTEMPT_STRUCT> attemptsList;
        protected List<REAL_STATE_MACROS.CLIENT_ATTEMPT_STRUCT> filteredAttempts;

        public ClientAttemptsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            commonQueriesObject = new CommonQueries();
            CommonFunctionsObject = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            listOfLeads = new List<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT>();

            attemptsList = new List<REAL_STATE_MACROS.CLIENT_ATTEMPT_STRUCT>();
            filteredAttempts = new List<REAL_STATE_MACROS.CLIENT_ATTEMPT_STRUCT>();

            loggedInUser = mLoggedInUser;
            currentDateTime = DateTime.Now;

            InitializeYearComboBox();
            InitializeMonthComboBox();

            if (!InitializeEmployeeComboBox())
                return;

            SetDateDefaultValues();
            SetDefaultSettings();
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

        private void InitializeAttemptsStackPanel()
        {
            ClientAttemptsStackPanel.Children.Clear();
            filteredAttempts.Clear();

            for (int i = 0; i < attemptsList.Count; i++)
            {
                if (yearCheckBox.IsChecked == true && attemptsList[i].attempt_date.Year != filterOptions.selectedYear)
                    continue;

                if (monthCheckBox.IsChecked == true && attemptsList[i].attempt_date.Month != filterOptions.selectedMonth)
                    continue;

                if (dayCheckBox.IsChecked == true && attemptsList[i].attempt_date.Day != filterOptions.selectedDay)
                    continue;

                if (employeeCheckBox.IsChecked == true && attemptsList[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                if (leadCheckBox.IsChecked == true && attemptsList[i].lead_id != listOfLeads[leadComboBox.SelectedIndex].contact.contact_id)
                    continue;

                filteredAttempts.Add(attemptsList[i]);

                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;

                Label companyAndLeadLabel = new Label();
                companyAndLeadLabel.Content = attemptsList[i].lead_name;
                companyAndLeadLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label salesPersonNameLabel = new Label();
                salesPersonNameLabel.Content = attemptsList[i].sales_person_name;
                salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label dateOfVisitLabel = new Label();
                dateOfVisitLabel.Content = attemptsList[i].attempt_date;
                dateOfVisitLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label attemptNotesLabel = new Label();
                attemptNotesLabel.Content = attemptsList[i].attempt_notes;
                attemptNotesLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label lineLabel = new Label();
                lineLabel.Content = "";
                lineLabel.Style = (Style)FindResource("stackPanelItemBody");

                currentStackPanel.Children.Add(companyAndLeadLabel);
                currentStackPanel.Children.Add(salesPersonNameLabel);
                currentStackPanel.Children.Add(dateOfVisitLabel);
                currentStackPanel.Children.Add(attemptNotesLabel);

                currentStackPanel.Children.Add(lineLabel);

                Grid newGrid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();

                newGrid.ColumnDefinitions.Add(column1);
                newGrid.MouseLeftButtonDown += OnBtnClickVisitItem;

                Grid.SetColumn(currentStackPanel, 0);

                newGrid.Children.Add(currentStackPanel);
                ClientAttemptsStackPanel.Children.Add(newGrid);

            }
        }
        private bool InitializeAttemptsGrid()
        {

            clientAttemptsGrid.Children.Clear();
            clientAttemptsGrid.RowDefinitions.Clear();
            clientAttemptsGrid.ColumnDefinitions.Clear();

            filteredAttempts.Clear();

            Label salesPersonHeader = new Label();
            salesPersonHeader.Content = "Sales Person";
            salesPersonHeader.Style = (Style)FindResource("tableHeaderItem");

            Label dateOfVisitHeader = new Label();
            dateOfVisitHeader.Content = "Attempt Date";
            dateOfVisitHeader.Style = (Style)FindResource("tableHeaderItem");

            Label contactInfoHeader = new Label();
            contactInfoHeader.Content = "Lead";
            contactInfoHeader.Style = (Style)FindResource("tableHeaderItem");

            Label contactNotesHeader = new Label();
            contactNotesHeader.Content = "Attempt Notes";
            contactNotesHeader.Style = (Style)FindResource("tableHeaderItem");

            clientAttemptsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientAttemptsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientAttemptsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            clientAttemptsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            clientAttemptsGrid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(salesPersonHeader, 0);
            Grid.SetColumn(salesPersonHeader, 0);
            clientAttemptsGrid.Children.Add(salesPersonHeader);

            Grid.SetRow(dateOfVisitHeader, 0);
            Grid.SetColumn(dateOfVisitHeader, 1);
            clientAttemptsGrid.Children.Add(dateOfVisitHeader);

            Grid.SetRow(contactInfoHeader, 0);
            Grid.SetColumn(contactInfoHeader, 2);
            clientAttemptsGrid.Children.Add(contactInfoHeader);

            Grid.SetRow(contactNotesHeader, 0);
            Grid.SetColumn(contactNotesHeader, 3);
            clientAttemptsGrid.Children.Add(contactNotesHeader);

            int currentRowNumber = 1;

            for (int i = 0; i < attemptsList.Count; i++)
            {
                if (yearCheckBox.IsChecked == true && attemptsList[i].attempt_date.Year != filterOptions.selectedYear)
                    continue;

                if (monthCheckBox.IsChecked == true && attemptsList[i].attempt_date.Month != filterOptions.selectedMonth)
                    continue;

                if (employeeCheckBox.IsChecked == true && attemptsList[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                    continue;

                filteredAttempts.Add(attemptsList[i]);


                RowDefinition currentRow = new RowDefinition();

                clientAttemptsGrid.RowDefinitions.Add(currentRow);

                Label salesPersonLabel = new Label();
                salesPersonLabel.Content = attemptsList[i].sales_person_name;
                salesPersonLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(salesPersonLabel, currentRowNumber);
                Grid.SetColumn(salesPersonLabel, 0);
                clientAttemptsGrid.Children.Add(salesPersonLabel);


                Label dateOfVisitLabel = new Label();
                dateOfVisitLabel.Content = attemptsList[i].attempt_date;
                dateOfVisitLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(dateOfVisitLabel, currentRowNumber);
                Grid.SetColumn(dateOfVisitLabel, 1);
                clientAttemptsGrid.Children.Add(dateOfVisitLabel);


                Label contactInfoLabel = new Label();
                contactInfoLabel.Content = attemptsList[i].lead_name;
                contactInfoLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(contactInfoLabel, currentRowNumber);
                Grid.SetColumn(contactInfoLabel, 2);
                clientAttemptsGrid.Children.Add(contactInfoLabel);

                Label attemptNotesLabel = new Label();
                attemptNotesLabel.Content = attemptsList[i].attempt_notes;
                attemptNotesLabel.Style = (Style)FindResource("tableSubItemLabel");

                Grid.SetRow(attemptNotesLabel, currentRowNumber);
                Grid.SetColumn(attemptNotesLabel, 3);
                clientAttemptsGrid.Children.Add(attemptNotesLabel);

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
            ExcelExport excelExport = new ExcelExport(clientAttemptsGrid);
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

            if (!GetAttemptsReport())
                return;

            InitializeAttemptsStackPanel();
            InitializeAttemptsGrid();

            dayCheckBox.IsChecked = false;
        }
        private void OnSelChangedDayCombo(object sender, RoutedEventArgs e)
        {
            filterOptions.selectedDay = (dayComboBox.SelectedIndex == -1 ? 0 : dayComboBox.SelectedIndex) + 1;

            UpdateDateValues();

            if (!GetAttemptsReport())
                return;

            InitializeAttemptsStackPanel();
            InitializeAttemptsGrid();
        }

        private void OnSelChangedEmployeeCombo(object sender, RoutedEventArgs e)
        {
            if (employeeCheckBox.IsChecked == true)
                filterOptions.selectedEmployee = listOfEmployees[employeeComboBox.SelectedIndex].employee_id;
            else
                filterOptions.selectedEmployee = 0;

            leadCheckBox.IsChecked = false;

            InitializeLeadComboBox();
            InitializeAttemptsStackPanel();
            InitializeAttemptsGrid();
        }

        private void OnSelChangedLeadCombo(object sender, RoutedEventArgs e)
        {
            if (leadCheckBox.IsChecked == true)
                filterOptions.selectedLead = listOfLeads[leadComboBox.SelectedIndex].contact.contact_id;
            else
                filterOptions.selectedLead = 0;

            InitializeAttemptsStackPanel();
            InitializeAttemptsGrid();
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


        private void OnClosedAddAttemptWindow(object sender, EventArgs e)
        {
            GetAttemptsReport();
            InitializeAttemptsStackPanel();
            InitializeAttemptsGrid();
        }

    }
}
