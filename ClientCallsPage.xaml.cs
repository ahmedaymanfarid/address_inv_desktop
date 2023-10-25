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
        protected List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> listOfLeads;
        protected List<REAL_STATE_MACROS.PROPERTY_STRUCT> listOfProperties;

        protected List<REAL_STATE_MACROS.CLIENT_CALL_STRUCT> callsList;
        protected List<REAL_STATE_MACROS.CLIENT_CALL_STRUCT> filteredCalls;

        public ClientCallsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            
            commonQueriesObject = new CommonQueries();
            CommonFunctionsObject = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            listOfLeads = new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();

            callsList = new List<REAL_STATE_MACROS.CLIENT_CALL_STRUCT>();
            filteredCalls = new List<REAL_STATE_MACROS.CLIENT_CALL_STRUCT>();

            loggedInUser = mLoggedInUser;
            currentDateTime = DateTime.Now;

            GetCallsReport();
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
     
        private void InitializeCallsStackPanel()
        {
            ClientCallsStackPanel.Children.Clear();
            filteredCalls.Clear();

            for (int i = 0; i < callsList.Count; i++)
            {
                //if (yearCheckBox.IsChecked == true && callsList[i].call_date.Year != filterOptions.selectedYear)
                //    continue;
                //
                //if (monthCheckBox.IsChecked == true && callsList[i].call_date.Month != filterOptions.selectedMonth)
                //    continue;
                //
                //if (dayCheckBox.IsChecked == true && callsList[i].call_date.Day != filterOptions.selectedDay)
                //    continue;

                //if (employeeCheckBox.IsChecked == true && callsList[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                //    continue;
                //
                //if (leadCheckBox.IsChecked == true && callsList[i].lead_id != listOfLeads[leadComboBox.SelectedIndex].contact.contact_id)
                //    continue;

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

            //clientCallsGrid.Children.Clear();
            //clientCallsGrid.RowDefinitions.Clear();
            //clientCallsGrid.ColumnDefinitions.Clear();
            //
            //filteredCalls.Clear();
            //
            //Label salesPersonHeader = new Label();
            //salesPersonHeader.Content = "Sales Person";
            //salesPersonHeader.Style = (Style)FindResource("tableHeaderItem");
            //
            //Label dateOfVisitHeader = new Label();
            //dateOfVisitHeader.Content = "Call Date";
            //dateOfVisitHeader.Style = (Style)FindResource("tableHeaderItem");
            //
            //Label contactInfoHeader = new Label();
            //contactInfoHeader.Content = "Lead";
            //contactInfoHeader.Style = (Style)FindResource("tableHeaderItem");
            //
            //Label contactNotesHeader = new Label();
            //contactNotesHeader.Content = "Call Notes";
            //contactNotesHeader.Style = (Style)FindResource("tableHeaderItem");
            //
            //clientCallsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //clientCallsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //clientCallsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //clientCallsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //
            //clientCallsGrid.RowDefinitions.Add(new RowDefinition());
            //
            //Grid.SetRow(salesPersonHeader, 0);
            //Grid.SetColumn(salesPersonHeader, 0);
            //clientCallsGrid.Children.Add(salesPersonHeader);
            //
            //Grid.SetRow(dateOfVisitHeader, 0);
            //Grid.SetColumn(dateOfVisitHeader, 1);
            //clientCallsGrid.Children.Add(dateOfVisitHeader);
            //
            //Grid.SetRow(contactInfoHeader, 0);
            //Grid.SetColumn(contactInfoHeader, 2);
            //clientCallsGrid.Children.Add(contactInfoHeader);
            //
            //Grid.SetRow(contactNotesHeader, 0);
            //Grid.SetColumn(contactNotesHeader, 3);
            //clientCallsGrid.Children.Add(contactNotesHeader);
            //
            //int currentRowNumber = 1;
            //
            //for (int i = 0; i < callsList.Count; i++)
            //{
            //    if (yearCheckBox.IsChecked == true && callsList[i].call_date.Year != filterOptions.selectedYear)
            //        continue;
            //
            //    if (monthCheckBox.IsChecked == true && callsList[i].call_date.Month != filterOptions.selectedMonth)
            //        continue;
            //
            //    if (employeeCheckBox.IsChecked == true && callsList[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
            //        continue;
            //
            //    filteredCalls.Add(callsList[i]);
            //
            //
            //    RowDefinition currentRow = new RowDefinition();
            //
            //    clientCallsGrid.RowDefinitions.Add(currentRow);
            //
            //    Label salesPersonLabel = new Label();
            //    salesPersonLabel.Content = callsList[i].sales_person_name;
            //    salesPersonLabel.Style = (Style)FindResource("tableSubItemLabel");
            //
            //    Grid.SetRow(salesPersonLabel, currentRowNumber);
            //    Grid.SetColumn(salesPersonLabel, 0);
            //    clientCallsGrid.Children.Add(salesPersonLabel);
            //
            //
            //    Label dateOfVisitLabel = new Label();
            //    dateOfVisitLabel.Content = callsList[i].call_date;
            //    dateOfVisitLabel.Style = (Style)FindResource("tableSubItemLabel");
            //
            //    Grid.SetRow(dateOfVisitLabel, currentRowNumber);
            //    Grid.SetColumn(dateOfVisitLabel, 1);
            //    clientCallsGrid.Children.Add(dateOfVisitLabel);
            //
            //
            //    Label contactInfoLabel = new Label();
            //    contactInfoLabel.Content = callsList[i].lead_name;
            //    contactInfoLabel.Style = (Style)FindResource("tableSubItemLabel");
            //
            //    Grid.SetRow(contactInfoLabel, currentRowNumber);
            //    Grid.SetColumn(contactInfoLabel, 2);
            //    clientCallsGrid.Children.Add(contactInfoLabel);
            //
            //    Label callNotesLabel = new Label();
            //    callNotesLabel.Content = callsList[i].call_notes;
            //    callNotesLabel.Style = (Style)FindResource("tableSubItemLabel");
            //
            //    Grid.SetRow(callNotesLabel, currentRowNumber);
            //    Grid.SetColumn(callNotesLabel, 3);
            //    clientCallsGrid.Children.Add(callNotesLabel);
            //
            //    //currentRow.MouseLeftButtonDown += OnBtnClickWorkOfferItem;
            //
            //    currentRowNumber++;
            //}

            return true;
        }

        //////////////////////////////////////////////////////////
        /// DEFAULT SETTINGS
        //////////////////////////////////////////////////////////
        private void SetDefaultSettings()
        {
            
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
            //ExcelExport excelExport = new ExcelExport(clientCallsGrid);
        }

  
        private void OnClosedAddCallWindow(object sender, EventArgs e)
        {
            GetCallsReport();
            InitializeCallsStackPanel();
            InitializeCallsGrid();
        }

    }
}
