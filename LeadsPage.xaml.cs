﻿using crm_library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for LeadsPage.xaml
    /// </summary>
    public partial class LeadsPage : Page
    {

        CommonQueries commonQueries;
        private Employee loggedInUser;
        private int selectedEmployee;

        private StackPanel previousSelectedLeadItem;
        private StackPanel currentSelectedLeadItem;

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;

        private List<BASIC_STRUCTS.LEAD_STATUS_STRUCT> leadStatusList;
        private List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT> budgetRangeList;
        
        private List<KeyValuePair<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT, List<BASIC_STRUCTS.CONTACT_LIST_STRUCT>>> employeesLeads;

        //private List<KeyValuePair<int, TreeViewItem>> salesTreeArray;
        //private List<KeyValuePair<int, StackPanel>> salesStackArray;
        private List<KeyValuePair<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT, TreeViewItem>> leadsTreeArray;
        private List<KeyValuePair<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT, StackPanel>> leadsStackArray;

        public LeadsPage(ref Employee mLoggedInUser)
        {
            employeesLeads = new List<KeyValuePair<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT, List<BASIC_STRUCTS.CONTACT_LIST_STRUCT>>>();
            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();

            leadStatusList = new List<BASIC_STRUCTS.LEAD_STATUS_STRUCT>();
            budgetRangeList = new List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT>();

            //salesTreeArray = new List<KeyValuePair<int, TreeViewItem>>();
            //salesStackArray = new List<KeyValuePair<int, StackPanel>>();

            leadsTreeArray = new List<KeyValuePair<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT, TreeViewItem>>();
            leadsStackArray = new List<KeyValuePair<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT, StackPanel>>();

            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;

            InitializeComponent();

            //CalendarDateRange lastVisitDateRange = new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue);
            //lastVisitDatePicker.BlackoutDates.Add(lastVisitDateRange);
            //
            //CalendarDateRange lastCallDateRange = new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue);
            //lastCallDatePicker.BlackoutDates.Add(lastCallDateRange);
            //
            //CalendarDateRange lastAttemptDateRange = new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue);
            //lastAttemptDatePicker.BlackoutDates.Add(lastAttemptDateRange);

            //if (!InitializeLeadStatusComboBox())
            //    return;
            //if (!InitializeBudgetRangeComboBox())
            //    return;
            //
            //if (!InitializeEmployeesList())
            //    return;
            //
            //if (!GetAllLeads())
            //    return;
            //
            //SetDefaultSettings();
            //
            //InitializeSalesPersonComboBox();
            //InitializeLeadsTree();
            //InitializeLeadsStackPanel();

        }
        private void SetDefaultSettings()
        {
            //if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION || loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.TEAM_LEAD_POSTION)
            //{
            //    salesPersonCheckBox.IsChecked = false;
            //    salesPersonCheckBox.IsEnabled = true;
            //    salesPersonComboBox.IsEnabled = false;
            //}
            //else
            //{
            //    salesPersonCheckBox.IsChecked = true;
            //    salesPersonCheckBox.IsEnabled = false;
            //    salesPersonComboBox.IsEnabled = false;
            //}
            //
            //contactNameTextBox.IsEnabled = false;
            //contactPhoneTextBox.IsEnabled = false;
            //leadStatusComboBox.IsEnabled = false;
            //budgetComboBox.IsEnabled = false;
            //
            //lastCallDatePicker.IsEnabled = false;
            //lastAttemptDatePicker.IsEnabled = false;
            //lastVisitDatePicker.IsEnabled = false;

            viewButton.IsEnabled = false;
        }
        
        public bool InitializeEmployeesList()
        {
            listOfEmployees.Clear();

            if (loggedInUser.GetEmployeePositionId() == COMPANY_ORGANISATION_MACROS.MANAGER_POSTION)
            {
                if (!commonQueries.GetDepartmentEmployees(loggedInUser.GetEmployeeDepartmentId(), ref listOfEmployees))
                    return false;
            }
            else
            {
                if (!commonQueries.GetTeamEmployees(loggedInUser.GetEmployeeTeamId(), ref listOfEmployees))
                    return false;
            }

            return true;
        }
        public bool GetAllLeads()
        {
            employeesLeads.Clear();

            for (int i = 0; i < listOfEmployees.Count; i++)
            {
                List<BASIC_STRUCTS.CONTACT_LIST_STRUCT> employeeLeadList = new List<BASIC_STRUCTS.CONTACT_LIST_STRUCT>();

                if (!commonQueries.GetEmployeeLeads(listOfEmployees[i].employee_id, ref employeeLeadList))
                    return false;

                 employeesLeads.Add(new KeyValuePair<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT, List<BASIC_STRUCTS.CONTACT_LIST_STRUCT>>(listOfEmployees[i], employeeLeadList));                
            }

            return true;
        }

        private void InitializeSalesPersonComboBox()
        {
            //salesPersonComboBox.Items.Clear();
            //
            //for (int i = 0; i < listOfEmployees.Count; i++)
            //    salesPersonComboBox.Items.Add(listOfEmployees[i].employee_name);
        }
        public bool InitializeLeadStatusComboBox()
        {
            //leadStatusComboBox.Items.Clear();
            //
            //if (!commonQueries.GetLeadsStatus(ref leadStatusList))
            //    return false;
            //
            //for (int i = 0; i < leadStatusList.Count; i++)
            //    leadStatusComboBox.Items.Add(leadStatusList[i].lead_status);
            //
            return true;
        }
        public bool InitializeBudgetRangeComboBox()
        {
            //budgetComboBox.Items.Clear();
            //
            //if (!commonQueries.GetBudgetRanges(ref budgetRangeList))
            //    return false;
            //
            //for (int i = 0; i < budgetRangeList.Count; i++)
            //    budgetComboBox.Items.Add(budgetRangeList[i].budget_range);
            //
            return true;
        }

        public bool InitializeLeadsTree()
        {
            contactTreeView.Items.Clear();

            leadsTreeArray.Clear();

            for (int i = 0; i < employeesLeads.Count(); i++)
            {
                //if (salesPersonComboBox.SelectedItem != null && listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id != employeesLeads[i].Key.employee_id)
                //    continue;

                TreeViewItem salesPersonItem = new TreeViewItem();

                salesPersonItem.Header = employeesLeads[i].Key.employee_name;
                salesPersonItem.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                salesPersonItem.FontSize = 14;
                salesPersonItem.FontWeight = FontWeights.SemiBold;
                salesPersonItem.FontFamily = new FontFamily("Sans Serif");
                salesPersonItem.Tag = employeesLeads[i].Key.employee_id;

                contactTreeView.Items.Add(salesPersonItem);

                for (int j = 0; j < employeesLeads[i].Value.Count; j++)
                {
                    //bool containsName = employeesLeads[i].Value[j].contact_name.IndexOf(contactNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    bool containsPhone = false;

                    //foreach (String contact_phone in employeesLeads[i].Value[j].contact_phones)
                    //    containsPhone |= contact_phone.IndexOf(contactPhoneTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    //
                    //if (contactNameCheckBox.IsChecked == true && contactNameTextBox.Text != null && !containsName)
                    //    continue;
                    //if (contactPhoneCheckBox.IsChecked == true && contactPhoneTextBox.Text != null && !containsPhone)
                    //    continue;
                    //if (leadStatusCheckBox.IsChecked == true && leadStatusList[leadStatusComboBox.SelectedIndex].status_id != employeesLeads[i].Value[j].lead_status.status_id)
                    //    continue;
                    //if (budgetCheckBox.IsChecked == true && budgetRangeList[budgetComboBox.SelectedIndex].budget_id != employeesLeads[i].Value[j].budget_range.budget_id)
                    //    continue;
                    //
                    //if (lastCallCheckBox.IsChecked == true && lastCallDatePicker.SelectedDate != employeesLeads[i].Value[j].last_call.Date)
                    //    continue;
                    //if (lastVisitCheckBox.IsChecked == true && lastVisitDatePicker.SelectedDate != employeesLeads[i].Value[j].last_visit.Date)
                    //    continue;
                    //if (lastAttemptCheckBox.IsChecked == true && lastAttemptDatePicker.SelectedDate != employeesLeads[i].Value[j].last_attempt.Date)
                    //    continue;

                    TreeViewItem leadTreeItem = new TreeViewItem();

                    leadTreeItem.Header = employeesLeads[i].Value[j].contact_name;
                    leadTreeItem.Tag = employeesLeads[i].Value[j].contact_id;
                    leadTreeItem.FontSize = 12;
                    leadTreeItem.FontWeight = FontWeights.Normal;

                    salesPersonItem.Items.Add(leadTreeItem);

                    BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT lead_item = new BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT();

                    lead_item.sales_person_id = employeesLeads[i].Value[j].sales_person_id;
                    lead_item.contact.contact_id = employeesLeads[i].Value[j].contact_id;
                    lead_item.contact.contact_name = employeesLeads[i].Value[j].contact_name;

                    leadsTreeArray.Add(new KeyValuePair<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT, TreeViewItem>(lead_item, leadTreeItem));

                }

            }

            return true;
        }

        public void InitializeLeadsStackPanel()
        {
            contactStackView.Children.Clear();

            currentSelectedLeadItem = null;
            previousSelectedLeadItem = null;

            leadsStackArray.Clear();
            
            for (int i = 0; i < listOfEmployees.Count(); i++)
            {
                //if (salesPersonComboBox.SelectedItem != null && listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id != listOfEmployees[i].employee_id)
                //    continue;

                StackPanel employeeStackPanel = new StackPanel();
                employeeStackPanel.Orientation = Orientation.Vertical;

                Label employeeNameLabel = new Label();

                employeeNameLabel.Content = listOfEmployees[i].employee_name;
                employeeNameLabel.Style = (Style)FindResource("stackPanelItemHeader");

                employeeStackPanel.Children.Add(employeeNameLabel);
                contactStackView.Children.Add(employeeStackPanel);

                for (int j = 0; j < employeesLeads[i].Value.Count; j++)
                {
                    //bool containsName = employeesLeads[i].Value[j].contact_name.IndexOf(contactNameTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    //bool containsPhone = false;

                    //foreach (String contact_phone in employeesLeads[i].Value[j].contact_phones)
                    //    containsPhone |= contact_phone.IndexOf(contactPhoneTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                    //if (contactNameCheckBox.IsChecked == true && contactNameTextBox.Text != null && !containsName)
                    //    continue;
                    //if (contactPhoneCheckBox.IsChecked == true && contactPhoneTextBox.Text != null && !containsPhone)
                    //    continue;
                    //if (leadStatusCheckBox.IsChecked == true && leadStatusList[leadStatusComboBox.SelectedIndex].status_id != employeesLeads[i].Value[j].lead_status.status_id)
                    //    continue;
                    //if (budgetCheckBox.IsChecked == true && budgetRangeList[budgetComboBox.SelectedIndex].budget_id != employeesLeads[i].Value[j].budget_range.budget_id)
                    //    continue;
                    //
                    //if (lastCallCheckBox.IsChecked == true && lastCallDatePicker.SelectedDate != employeesLeads[i].Value[j].last_call.Date)
                    //    continue;
                    //if (lastVisitCheckBox.IsChecked == true && lastVisitDatePicker.SelectedDate != employeesLeads[i].Value[j].last_visit.Date)
                    //    continue;
                    //if (lastAttemptCheckBox.IsChecked == true && lastAttemptDatePicker.SelectedDate != employeesLeads[i].Value[j].last_attempt.Date)
                    //    continue;

                    StackPanel leadDetailsStackPanel = new StackPanel();
                    leadDetailsStackPanel.Orientation = Orientation.Vertical;
                    leadDetailsStackPanel.MouseDown += OnMouseDownLeadStackPanel;


                    Grid leadDetailsGrid = new Grid();

                    ColumnDefinition gridIconColumn = new ColumnDefinition();
                    ColumnDefinition gridDetailColumn = new ColumnDefinition();

                    gridIconColumn.MaxWidth = 30;

                    leadDetailsGrid.ColumnDefinitions.Add(gridIconColumn);
                    leadDetailsGrid.ColumnDefinitions.Add(gridDetailColumn);

                    Label leadNameLabel = new Label();
                    leadNameLabel.Content = employeesLeads[i].Value[j].contact_name;
                    leadNameLabel.Style = (Style)FindResource("stackPanelItemHeader");
                    leadDetailsStackPanel.Children.Add(leadNameLabel);

                    for (int k = 0; k < employeesLeads[i].Value[j].contact_phones.Count; k++)
                        SetLeadPhoneRow(k, employeesLeads[i].Value[j].contact_phones[k], ref leadDetailsGrid);

                    if (employeesLeads[i].Value[j].lead_status.lead_status != "")
                        SetLeadStatusRow(employeesLeads[i].Value[j].contact_phones.Count, employeesLeads[i].Value[j].lead_status.lead_status, ref leadDetailsGrid);

                    if (employeesLeads[i].Value[j].budget_range.budget_range != "")
                        SetLeadBudgetRow(employeesLeads[i].Value[j].contact_phones.Count + 1, employeesLeads[i].Value[j].budget_range.budget_range, ref leadDetailsGrid);


                    leadDetailsStackPanel.Children.Add(leadDetailsGrid);

                    Grid leadGridItem = new Grid();

                    ColumnDefinition iconColumn = new ColumnDefinition();
                    ColumnDefinition stackColumn = new ColumnDefinition();

                    iconColumn.MaxWidth = 50;

                    leadGridItem.ColumnDefinitions.Add(iconColumn);
                    leadGridItem.ColumnDefinitions.Add(stackColumn);

                    Image contactIcon = new Image { Source = new BitmapImage(new Uri(@"icons\contact_icon.png", UriKind.Relative)) };
                    ResizeImage(ref contactIcon, 40, 40);
                    
                    leadGridItem.Children.Add(contactIcon);
                    Grid.SetRow(contactIcon, 0);

                    leadGridItem.Children.Add(leadDetailsStackPanel);
                    Grid.SetColumn(leadDetailsStackPanel, 1);

                    employeeStackPanel.Children.Add(leadGridItem);

                    BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT lead_item = new BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT();

                    lead_item.sales_person_id = employeesLeads[i].Value[j].sales_person_id;
                    lead_item.contact.contact_id = employeesLeads[i].Value[j].contact_id;
                    lead_item.contact.contact_name = employeesLeads[i].Value[j].contact_name;

                    leadsStackArray.Add(new KeyValuePair<BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT, StackPanel>(lead_item, leadDetailsStackPanel));

                }

            }
        }

        public void SetLeadPhoneRow(int row, String leadName, ref Grid leadGrid)
        {
            RowDefinition leadPhoneRow = new RowDefinition();
            leadGrid.RowDefinitions.Add(leadPhoneRow);


            Image phoneIcon = new Image { Source = new BitmapImage(new Uri(@"icons\phone_icon.png", UriKind.Relative)) };
            ResizeImage(ref phoneIcon, 25, 25);

            leadGrid.Children.Add(phoneIcon);
            Grid.SetRow(phoneIcon, row);
            Grid.SetColumn(phoneIcon, 0);


            Label leadPhoneLabel = new Label();
            leadPhoneLabel.Content = leadName;
            leadPhoneLabel.Style = (Style)FindResource("stackPanelItemBody");

            leadGrid.Children.Add(leadPhoneLabel);
            Grid.SetRow(leadPhoneLabel, row);
            Grid.SetColumn(leadPhoneLabel, 1);
        }
        
        public void SetLeadStatusRow(int row, String leadStatus, ref Grid leadGrid)
        {
            RowDefinition leadStatusRow = new RowDefinition();
            leadGrid.RowDefinitions.Add(leadStatusRow);


            Image statusIcon = new Image { Source = new BitmapImage(new Uri(@"icons\status_icon.png", UriKind.Relative)) };
            ResizeImage(ref statusIcon, 25, 25);

            leadGrid.Children.Add(statusIcon);
            Grid.SetRow(statusIcon, row);
            Grid.SetColumn(statusIcon, 0);



            Label leadStatusLabel = new Label();
            leadStatusLabel.Content = leadStatus;
            leadStatusLabel.Style = (Style)FindResource("stackPanelItemBody");

            leadGrid.Children.Add(leadStatusLabel);
            Grid.SetRow(leadStatusLabel, row);
            Grid.SetColumn(leadStatusLabel, 1);
        }
        public void SetLeadBudgetRow(int row, String leadBudget, ref Grid leadGrid)
        {
            RowDefinition leadStatusRow = new RowDefinition();
            leadGrid.RowDefinitions.Add(leadStatusRow);


            Image budgetIcon = new Image { Source = new BitmapImage(new Uri(@"icons\budget_icon.jpg", UriKind.Relative)) };
            ResizeImage(ref budgetIcon, 25, 25);

            leadGrid.Children.Add(budgetIcon);
            Grid.SetRow(budgetIcon, row);
            Grid.SetColumn(budgetIcon, 0);


            Label leadBudgeRangeLabel = new Label();
            leadBudgeRangeLabel.Content = leadBudget;
            leadBudgeRangeLabel.Style = (Style)FindResource("stackPanelItemBody");

            leadGrid.Children.Add(leadBudgeRangeLabel);
            Grid.SetRow(leadBudgeRangeLabel, row);
            Grid.SetColumn(leadBudgeRangeLabel, 1);
        }

        public void ResizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON CHECK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void OnCheckedLeadNameCheckBox(object sender, RoutedEventArgs e)
        //{
        //    contactNameTextBox.IsEnabled = true;
        //}
        //private void OnCheckedLeadPhoneCheckBox(object sender, RoutedEventArgs e)
        //{
        //    contactPhoneTextBox.IsEnabled = true;
        //}
        //private void OnCheckedLeadStatusCheckBox(object sender, RoutedEventArgs e)
        //{
        //    leadStatusComboBox.IsEnabled = true;
        //    leadStatusComboBox.SelectedIndex = 0;
        //}
        //private void OnCheckedBudgetCheckBox(object sender, RoutedEventArgs e)
        //{
        //    budgetComboBox.IsEnabled = true;
        //    budgetComboBox.SelectedIndex = 0;
        //}
        //private void OnCheckedLastCallCheckBox(object sender, RoutedEventArgs e)
        //{
        //    lastCallDatePicker.IsEnabled = true;
        //    lastCallDatePicker.SelectedDate = DateTime.Now;
        //}
        //private void OnCheckedLastAttemptCheckBox(object sender, RoutedEventArgs e)
        //{
        //    lastAttemptDatePicker.IsEnabled = true;
        //    lastAttemptDatePicker.SelectedDate = DateTime.Now;
        //}
        //private void OnCheckedLastVisitCheckBox(object sender, RoutedEventArgs e)
        //{
        //    lastVisitDatePicker.IsEnabled = true;
        //    lastVisitDatePicker.SelectedDate = DateTime.Now;
        //}
        //private void OnCheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        //{
        //    salesPersonComboBox.IsEnabled = true;
        //
        //    for (int i = 0; i < listOfEmployees.Count; i++)
        //        if (loggedInUser.GetEmployeeId() == listOfEmployees[i].employee_id)
        //            salesPersonComboBox.SelectedIndex = i;
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnTextChangedLeadName(object sender, TextChangedEventArgs e)
        {
            InitializeLeadsTree();
            InitializeLeadsStackPanel();
        }
        private void OnTextChangedLeadPhone(object sender, TextChangedEventArgs e)
        {
            InitializeLeadsTree();
            InitializeLeadsStackPanel();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELECTION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSelChangedLeadStatusComboBox(object sender, SelectionChangedEventArgs e)
        {
            viewButton.IsEnabled = false;

            InitializeLeadsTree();
            InitializeLeadsStackPanel();
        }
        private void OnSelChangedBudgetComboBox(object sender, SelectionChangedEventArgs e)
        {
            viewButton.IsEnabled = false;

            InitializeLeadsTree();
            InitializeLeadsStackPanel();
        }
        private void OnSelChangedLastCallDate(object sender, SelectionChangedEventArgs e)
        {
            viewButton.IsEnabled = false;

            InitializeLeadsTree();
            InitializeLeadsStackPanel();
        }
        private void OnSelChangedLastAttemptDate(object sender, SelectionChangedEventArgs e)
        {
            viewButton.IsEnabled = false;

            InitializeLeadsTree();
            InitializeLeadsStackPanel();
        }
        private void OnSelChangedLastVisitDate(object sender, SelectionChangedEventArgs e)
        {
            viewButton.IsEnabled = false;

            InitializeLeadsTree();
            InitializeLeadsStackPanel();
        }
        //private void OnSelChangedSalesPersonComboBox(object sender, SelectionChangedEventArgs e)
        //{
        //    viewButton.IsEnabled = false;
        //
        //    if (salesPersonCheckBox.IsChecked == true && salesPersonComboBox.SelectedItem != null)
        //        selectedEmployee = listOfEmployees[salesPersonComboBox.SelectedIndex].employee_id;
        //    else
        //        selectedEmployee = 0;
        //
        //    InitializeLeadsTree();
        //    InitializeLeadsStackPanel();
        //
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON UNCHECKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void OnUncheckedLeadNameCheckBox(object sender, RoutedEventArgs e)
        //{
        //    contactNameTextBox.IsEnabled = false;
        //    contactNameTextBox.Text = null;
        //
        //    viewButton.IsEnabled = false;
        //}
        //private void OnUncheckedLeadPhoneCheckBox(object sender, RoutedEventArgs e)
        //{
        //    contactPhoneTextBox.IsEnabled = false;
        //    contactPhoneTextBox.Text = null;
        //
        //    viewButton.IsEnabled = false;
        //}
        //private void OnUncheckedLeadStatusCheckBox(object sender, RoutedEventArgs e)
        //{
        //    leadStatusComboBox.IsEnabled = false;
        //    viewButton.IsEnabled = false;
        //
        //    leadStatusComboBox.SelectedItem = null;
        //}
        //private void OnUncheckedLastCallCheckBox(object sender, RoutedEventArgs e)
        //{
        //    lastCallDatePicker.IsEnabled = false;
        //    viewButton.IsEnabled = false;
        //
        //    lastCallDatePicker.SelectedDate = null;
        //}
        //private void OnUncheckedLastAttemptCheckBox(object sender, RoutedEventArgs e)
        //{
        //    lastAttemptDatePicker.IsEnabled = false;
        //    viewButton.IsEnabled = false;
        //
        //    lastAttemptDatePicker.SelectedDate = null;
        //}
        //private void OnUncheckedLastVisitCheckBox(object sender, RoutedEventArgs e)
        //{
        //    lastVisitDatePicker.IsEnabled = false;
        //    viewButton.IsEnabled = false;
        //
        //    lastVisitDatePicker.SelectedDate = null;
        //}
        //private void OnUncheckedBudgetCheckBox(object sender, RoutedEventArgs e)
        //{
        //    budgetComboBox.SelectedItem = null;
        //
        //    budgetComboBox.IsEnabled = false;
        //    viewButton.IsEnabled = false;
        //}
        //private void OnUncheckedSalesPersonCheckBox(object sender, RoutedEventArgs e)
        //{
        //    salesPersonComboBox.IsEnabled = false;
        //    salesPersonComboBox.SelectedItem = null;
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON BTN CLICKED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void OnBtnClickAddLead(object sender, RoutedEventArgs e)
        {
            viewButton.IsEnabled = false;

            AddLeadWindow addLeadWindow = new AddLeadWindow(ref loggedInUser);
            addLeadWindow.Closed += OnClosedAddLeadWindow;
            addLeadWindow.Show();
        }
        private void OnClosedAddLeadWindow(object sender, EventArgs e)
        {
            GetAllLeads();

            SetDefaultSettings();

            InitializeLeadsTree();
        }
        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            if(contactTreeScrollViewer.Visibility == Visibility.Visible)
            {
                TreeViewItem selectedItem = (TreeViewItem)contactTreeView.SelectedItem;

                if (!selectedItem.HasItems)
                {
                    BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT currentLeadStruct = new BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT();
                    currentLeadStruct = leadsTreeArray.Find(current_item => current_item.Value == selectedItem).Key;

                    Lead selectedLead = new Lead();
                    selectedLead.InitializeLeadInfo(currentLeadStruct.sales_person_id, currentLeadStruct.contact.contact_id);

                    ViewLeadWindow viewLeadWindow = new ViewLeadWindow(ref loggedInUser, ref selectedLead);
                    viewLeadWindow.Show();

                }
            }
            else if(contactStackScrollViewer.Visibility == Visibility.Visible)
            {
                BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT currentLeadStruct = new BASIC_STRUCTS.CONTACT_MIN_LIST_STRUCT();
                currentLeadStruct = leadsStackArray.Find(current_item => current_item.Value == currentSelectedLeadItem).Key;

                Lead selectedLead = new Lead();
                selectedLead.InitializeLeadInfo(currentLeadStruct.sales_person_id, currentLeadStruct.contact.contact_id);

                ViewLeadWindow viewLeadWindow = new ViewLeadWindow(ref loggedInUser, ref selectedLead);
                viewLeadWindow.Show();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON SELECTED ITEM CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnMouseDownLeadStackPanel(object sender, RoutedEventArgs e)
        {
            previousSelectedLeadItem = currentSelectedLeadItem;
            currentSelectedLeadItem = (StackPanel)sender;

            BrushConverter brush = new BrushConverter();

            if (previousSelectedLeadItem != null)
            {
                Grid previousParentGrid = (Grid)previousSelectedLeadItem.Parent;
                previousParentGrid.Background = (Brush)brush.ConvertFrom("#FFFFFF"); ;

                Image previousSontactIcon = (Image)previousParentGrid.Children[0];
                previousSontactIcon.Source = new BitmapImage(new Uri(@"icons\contact_icon.png", UriKind.Relative));

                Label previousSelectedLeadLabel = (Label)previousSelectedLeadItem.Children[0];
                previousSelectedLeadLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
                previousSelectedLeadLabel.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                Label previousleadNameLabel = (Label)previousSelectedLeadItem.Children[0];

                previousleadNameLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
                previousleadNameLabel.Background = (Brush)brush.ConvertFrom("#FFFFFF");

                Grid previousLeadDetailsGrid = (Grid)previousSelectedLeadItem.Children[1];


                for (int i = 1; i < previousLeadDetailsGrid.Children.Count; i += 2)
                {
                    Label currentLabel = (Label)previousLeadDetailsGrid.Children[i];

                    currentLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
                    currentLabel.Background = (Brush)brush.ConvertFrom("#FFFFFF");
                }

                for (int i = 0; i < previousLeadDetailsGrid.Children.Count; i += 2)
                {
                    Image currentIcon = (Image)previousLeadDetailsGrid.Children[i];

                    if (i == previousLeadDetailsGrid.Children.Count - 1)
                        currentIcon.Source = new BitmapImage(new Uri(@"icons\budget_icon.png", UriKind.Relative));
                    else if (i == previousLeadDetailsGrid.Children.Count - 2)
                        currentIcon.Source = new BitmapImage(new Uri(@"icons\status_icon.png", UriKind.Relative));
                    else
                        currentIcon.Source = new BitmapImage(new Uri(@"icons\phone_icon.png", UriKind.Relative));
                }
            }

            Grid parentGrid = (Grid)currentSelectedLeadItem.Parent;
            parentGrid.Background = (Brush)brush.ConvertFrom("#000000"); ;

            Image contactIcon = (Image)parentGrid.Children[0];
            contactIcon.Source = new BitmapImage(new Uri(@"icons\contact_icon_blue.png", UriKind.Relative));

            Label currentSelectedLeadLabel = (Label)currentSelectedLeadItem.Children[0];
            currentSelectedLeadLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            currentSelectedLeadLabel.Background = (Brush)brush.ConvertFrom("#000000");

            Label leadNameLabel = (Label)currentSelectedLeadItem.Children[0];

            leadNameLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            leadNameLabel.Background = (Brush)brush.ConvertFrom("#000000");

            Grid leadDetailsGrid = (Grid)currentSelectedLeadItem.Children[1];


            for (int i = 1; i < leadDetailsGrid.Children.Count; i+=2) 
            {
                Label currentLabel = (Label)leadDetailsGrid.Children[i];

                currentLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
                currentLabel.Background = (Brush)brush.ConvertFrom("#000000");
            }

            for (int i = 0; i < leadDetailsGrid.Children.Count; i += 2)
            {
                Image currentIcon = (Image)leadDetailsGrid.Children[i];

                if(i == leadDetailsGrid.Children.Count - 1)
                    currentIcon.Source = new BitmapImage(new Uri(@"icons\budget_icon_blue.png", UriKind.Relative));
                else if (i == leadDetailsGrid.Children.Count - 2)
                    currentIcon.Source = new BitmapImage(new Uri(@"icons\status_icon_blue.png", UriKind.Relative));
                else
                    currentIcon.Source = new BitmapImage(new Uri(@"icons\phone_icon_blue.png", UriKind.Relative));
            }

            viewButton.IsEnabled = true;
        }
        private void OnSelectedItemChangedTreeViewItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //VIEW BUTTON IS ENABLED ONCE ANY ITEM IS SELECTED
            viewButton.IsEnabled = false;
            TreeViewItem selectedItem = (TreeViewItem)contactTreeView.SelectedItem;

            if (selectedItem != null)
            {
                viewButton.IsEnabled = true;
            }

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //VIEWING TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickListView(object sender, MouseButtonEventArgs e)
        {
            //listViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");
            //treeViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");

            contactStackScrollViewer.Visibility = Visibility.Visible;
            contactTreeScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void OnClickTreeView(object sender, MouseButtonEventArgs e)
        {
            //listViewLabel.Style = (Style)FindResource("unselectedMainTabLabelItem");
            //treeViewLabel.Style = (Style)FindResource("selectedMainTabLabelItem");

            contactStackScrollViewer.Visibility = Visibility.Collapsed;
            contactTreeScrollViewer.Visibility = Visibility.Visible;
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
        
    }


}