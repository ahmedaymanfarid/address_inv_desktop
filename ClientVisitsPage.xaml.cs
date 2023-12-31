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
    /// Interaction logic for ClientVisitsPage.xaml
    /// </summary>
    public partial class ClientVisitsPage : Page
    {
        private Employee loggedInUser;

        protected CommonQueries commonQueriesObject;
        protected CommonFunctions commonFunctionsObject;

        protected List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;
        protected List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT> listOfLeads;
        protected List<REAL_STATE_MACROS.PROPERTY_STRUCT> listOfProperties;

        protected List<REAL_STATE_MACROS.CLIENT_VISIT_STRUCT> visitsInfo;
        protected List<REAL_STATE_MACROS.CLIENT_VISIT_STRUCT> filteredVisits;

        private Grid previousSelectedVisitItem;
        private Grid currentSelectedVisitItem;

        private int selectedYear;
        private int selectedMonth;
        private int selectedDay;

        private int selectedLead;
        private int selectedEmployee;
        public ClientVisitsPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();
            loggedInUser = mLoggedInUser;

            commonQueriesObject = new CommonQueries();
            commonFunctionsObject = new CommonFunctions();

            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            listOfLeads = new List<COMPANY_ORGANISATION_MACROS.CONTACT_MIN_LIST_STRUCT>();

            visitsInfo = new List<REAL_STATE_MACROS.CLIENT_VISIT_STRUCT>();
            filteredVisits = new List<REAL_STATE_MACROS.CLIENT_VISIT_STRUCT>();

            GetVisits();
            InitializeStackPanel();
            InitializeGrid();

            SetDefaultSettings();
        }
        private void GetVisits()
        {
            commonQueriesObject.GetClientVisits(ref visitsInfo);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONFIGURATION FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void SetDefaultSettings()
        {
            
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SET FUNCTIONS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      

        private void SetPropertyComboBox()
        {
            //propertyComboBox.SelectedIndex = 0;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
        private void InitializeStackPanel()
        {
            ClientVisitsStackPanel.Children.Clear();
            filteredVisits.Clear();

            for (int i = 0; i < visitsInfo.Count; i++)
            {
                DateTime currentVisitDate = visitsInfo[i].visit_date;

                //if (yearCheckBox.IsChecked == true && currentVisitDate.Year != selectedYear)
                //    continue;
                //
                //if (monthCheckBox.IsChecked == true && currentVisitDate.Month != selectedMonth)
                //    continue;
                //
                //if (employeeCheckBox.IsChecked == true && visitsInfo[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
                //    continue;

                filteredVisits.Add(visitsInfo[i]);

                StackPanel currentStackPanel = new StackPanel();
                currentStackPanel.Orientation = Orientation.Vertical;

                Label salesPersonNameLabel = new Label();
                salesPersonNameLabel.Content = visitsInfo[i].sales_person_name;
                salesPersonNameLabel.Style = (Style)FindResource("stackPanelItemHeader");

                Label dateOfVisitLabel = new Label();
                dateOfVisitLabel.Content = visitsInfo[i].visit_date;
                dateOfVisitLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label companyAndLeadLabel = new Label();
                companyAndLeadLabel.Content = visitsInfo[i].lead_name;
                companyAndLeadLabel.Style = (Style)FindResource("stackPanelItemBody");

                Label lineLabel = new Label();
                lineLabel.Content = "";
                lineLabel.Style = (Style)FindResource("stackPanelItemBody");

                currentStackPanel.Children.Add(salesPersonNameLabel);
                currentStackPanel.Children.Add(dateOfVisitLabel);
                
                currentStackPanel.Children.Add(companyAndLeadLabel);

                currentStackPanel.Children.Add(lineLabel);

                Grid newGrid = new Grid();
                ColumnDefinition column1 = new ColumnDefinition();

                newGrid.ColumnDefinitions.Add(column1);
                newGrid.MouseLeftButtonDown += OnBtnClickVisitItem;

                Grid.SetColumn(currentStackPanel, 0);

                newGrid.Children.Add(currentStackPanel);
                ClientVisitsStackPanel.Children.Add(newGrid);

            }
        }

        private bool InitializeGrid()
        {
             
            //clientVisitsGrid.Children.Clear();
            //clientVisitsGrid.RowDefinitions.Clear();
            //clientVisitsGrid.ColumnDefinitions.Clear();
            //
            //filteredVisits.Clear();
            //
            //Label salesPersonHeader = new Label();
            //salesPersonHeader.Content = "Sales Person";
            //salesPersonHeader.Style = (Style)FindResource("tableSubHeaderItem");
            //
            //Label dateOfVisitHeader = new Label();
            //dateOfVisitHeader.Content = "Visit Date";
            //dateOfVisitHeader.Style = (Style)FindResource("tableSubHeaderItem");
            //
            //Label contactInfoHeader = new Label();
            //contactInfoHeader.Content = "Lead Info";
            //contactInfoHeader.Style = (Style)FindResource("tableSubHeaderItem");
            //
            //Label purposeHeader = new Label();
            //purposeHeader.Content = "Visit Purpose";
            //purposeHeader.Style = (Style)FindResource("tableSubHeaderItem");
            //
            //Label resultHeader = new Label();
            //resultHeader.Content = "Visit Result";
            //resultHeader.Style = (Style)FindResource("tableSubHeaderItem");
            //
            //clientVisitsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //clientVisitsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //clientVisitsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //
            //clientVisitsGrid.RowDefinitions.Add(new RowDefinition());
            //
            //
            //
            //Grid.SetRow(salesPersonHeader, 0);
            //Grid.SetColumn(salesPersonHeader, 0);
            //clientVisitsGrid.Children.Add(salesPersonHeader);
            //
            //Grid.SetRow(dateOfVisitHeader, 0);
            //Grid.SetColumn(dateOfVisitHeader, 1);
            //clientVisitsGrid.Children.Add(dateOfVisitHeader);
            //
            //Grid.SetRow(contactInfoHeader, 0);
            //Grid.SetColumn(contactInfoHeader, 2);
            //clientVisitsGrid.Children.Add(contactInfoHeader);
            //
            //Grid.SetRow(purposeHeader, 0);
            //Grid.SetColumn(purposeHeader, 3);
            //clientVisitsGrid.Children.Add(purposeHeader);
            //
            //Grid.SetRow(resultHeader, 0);
            //Grid.SetColumn(resultHeader, 4);
            //clientVisitsGrid.Children.Add(resultHeader);
            //
            //int currentRowNumber = 1;
            //
            //for (int i = 0; i < visitsInfo.Count; i++)
            //{
            //    DateTime currentVisitDate = visitsInfo[i].visit_date;
            //
            //    if (yearCheckBox.IsChecked == true && currentVisitDate.Year != selectedYear)
            //        continue;
            //
            //    if (monthCheckBox.IsChecked == true && currentVisitDate.Month != selectedMonth)
            //        continue;
            //
            //    if (employeeCheckBox.IsChecked == true && visitsInfo[i].sales_person_id != listOfEmployees[employeeComboBox.SelectedIndex].employee_id)
            //        continue;
            //
            //    filteredVisits.Add(visitsInfo[i]);
            //
            //
            //    RowDefinition currentRow = new RowDefinition();
            //
            //    clientVisitsGrid.RowDefinitions.Add(currentRow);
            //
            //    Label salesPersonLabel = new Label();
            //    salesPersonLabel.Content = visitsInfo[i].sales_person_name;
            //    salesPersonLabel.Style = (Style)FindResource("tableSubItemLabel");
            //
            //    Grid.SetRow(salesPersonLabel, currentRowNumber);
            //    Grid.SetColumn(salesPersonLabel, 0);
            //    clientVisitsGrid.Children.Add(salesPersonLabel);
            //
            //
            //    Label dateOfVisitLabel = new Label();
            //    dateOfVisitLabel.Content = visitsInfo[i].visit_date;
            //    dateOfVisitLabel.Style = (Style)FindResource("tableSubItemLabel");
            //
            //    Grid.SetRow(dateOfVisitLabel, currentRowNumber);
            //    Grid.SetColumn(dateOfVisitLabel, 1);
            //    clientVisitsGrid.Children.Add(dateOfVisitLabel);
            //
            //
            //    Label contactInfoLabel = new Label();
            //    contactInfoLabel.Content = visitsInfo[i].lead_name;
            //    contactInfoLabel.Style = (Style)FindResource("tableSubItemLabel");
            //
            //    Grid.SetRow(contactInfoLabel, currentRowNumber);
            //    Grid.SetColumn(contactInfoLabel, 2);
            //    clientVisitsGrid.Children.Add(contactInfoLabel);
            //
            //    //currentRow.MouseLeftButtonDown += OnBtnClickWorkOfferItem;
            //
            //    currentRowNumber++;
            //}
            //
            return true;
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
            AddClientVisitWindow addClientVisitWindow = new AddClientVisitWindow(ref loggedInUser);
            addClientVisitWindow.Closed += OnClosedAddVisitWindow;
            addClientVisitWindow.Show();
        }
        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            ClientVisit selectedVisit = new ClientVisit();
            selectedVisit.InitializeClientVisitInfo(filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].visit_serial, filteredVisits[ClientVisitsStackPanel.Children.IndexOf(currentSelectedVisitItem)].sales_person_id);

            ViewClientVisitWindow viewClientVisitWindow = new ViewClientVisitWindow(ref selectedVisit);
            viewClientVisitWindow.Show();
        }

   
        private void OnClosedAddVisitWindow(object sender, EventArgs e)
        {
            GetVisits();
            InitializeStackPanel();
            InitializeGrid();
        }

        private void OnButtonClickedCalls(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnButtonClickedNewLeads(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
