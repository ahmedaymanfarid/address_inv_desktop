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
    /// Interaction logic for PropertyMatchedLeeds.xaml
    /// </summary>
    public partial class PropertyMatchedLeeds : Page
    {
        Employee loggedInUser;
        Property currentProperty;

        CommonQueries commonQueries;

        private StackPanel previousSelectedLeadItem;
        private StackPanel currentSelectedLeadItem;

        private List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT> listOfEmployees;
        private List<BASIC_STRUCTS.LEAD_MATCHING_STRUCT> leads;

        private List<KeyValuePair<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT, List<BASIC_STRUCTS.CONTACT_LIST_STRUCT>>> employeesLeads;

        private List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT> budgetRanges;

        private int viewAddCondition;

        public PropertyMatchedLeeds(ref Employee mLoggedInUser, ref Property mCurrentProperty, ref int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            currentProperty = mCurrentProperty;
            viewAddCondition = mViewAddCondition;
            commonQueries = new CommonQueries();

            InitializeComponent();
            employeesLeads = new List<KeyValuePair<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT, List<BASIC_STRUCTS.CONTACT_LIST_STRUCT>>>();
            listOfEmployees = new List<COMPANY_ORGANISATION_MACROS.EMPLOYEE_STRUCT>();
            leads = new List<BASIC_STRUCTS.LEAD_MATCHING_STRUCT>();
            budgetRanges = new List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT>();

            if (!InitializeEmployeesList())
                return;

            if (!GetLeads())
                return;

            if (!GetBudgetRanges())
                return;

            GetAllLeadsMatch();


            InitializeLeedsGrid();

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

        private void InitializeLeedsGrid()
        {

            for (int i = 0; i < leads.Count; i++)
            {
                matchedLeedsGrid.RowDefinitions.Add(new RowDefinition());

                BrushConverter brush = new BrushConverter();

                Label leadNameLabel = new Label();
                leadNameLabel.Content = leads[i].lead_name;
                leadNameLabel.Style = (Style)FindResource("stackPanelItemHeader");
                leadNameLabel.HorizontalAlignment = HorizontalAlignment.Center;
                leadNameLabel.VerticalAlignment = VerticalAlignment.Center;
                leadNameLabel.FontSize = 18;
                leadNameLabel.Background = (Brush)brush.ConvertFrom("#EDEDED");

                matchedLeedsGrid.Children.Add(leadNameLabel);
                Grid.SetRow(leadNameLabel, i + 1);
                Grid.SetColumn(leadNameLabel, 0);

                StackPanel detailsStackPanel = new StackPanel();
                detailsStackPanel.Orientation = Orientation.Vertical;
                detailsStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                detailsStackPanel.Margin = new Thickness(24);

                for (int k = 0; k < leads[i].lead_mobiles.Count; k++)
                {
                    if (leads[i].lead_mobiles[k] != "")
                    {
                        Label leadPhoneLabel = new Label();
                        leadPhoneLabel.Content = leads[i].lead_mobiles[k];
                        leadPhoneLabel.Style = (Style)FindResource("stackPanelItemHeader");
                        leadPhoneLabel.HorizontalAlignment = HorizontalAlignment.Center;
                        leadPhoneLabel.VerticalAlignment = VerticalAlignment.Center;
                        leadPhoneLabel.FontSize = 18;
                        leadPhoneLabel.Background = (Brush)brush.ConvertFrom("#EDEDED");

                        detailsStackPanel.Children.Add(leadPhoneLabel);
                    }
                }

                Label leadStatusLabel = new Label();
                leadStatusLabel.Content = leads[i].lead_status.lead_status;
                leadStatusLabel.Style = (Style)FindResource("stackPanelItemHeader");
                leadStatusLabel.HorizontalAlignment = HorizontalAlignment.Center;
                leadStatusLabel.VerticalAlignment = VerticalAlignment.Center;
                leadStatusLabel.FontSize = 18;
                leadStatusLabel.Background = (Brush)brush.ConvertFrom("#EDEDED");

                detailsStackPanel.Children.Add(leadStatusLabel);

                Label leadBudgetLabel = new Label();
                leadBudgetLabel.Content = leads[i].budget_range.budget_range;
                leadBudgetLabel.Style = (Style)FindResource("stackPanelItemHeader");
                leadBudgetLabel.HorizontalAlignment = HorizontalAlignment.Center;
                leadBudgetLabel.VerticalAlignment = VerticalAlignment.Center;
                leadBudgetLabel.FontSize = 18;
                leadBudgetLabel.Background = (Brush)brush.ConvertFrom("#EDEDED");

                detailsStackPanel.Children.Add(leadBudgetLabel);

                matchedLeedsGrid.Children.Add(detailsStackPanel);
                Grid.SetRow(detailsStackPanel, i + 1);
                Grid.SetColumn(detailsStackPanel, 1);

                Border percentageBorder = new Border();
                percentageBorder.CornerRadius = new CornerRadius(100);
                percentageBorder.Width = 80;
                percentageBorder.Height = 80;
                percentageBorder.BorderThickness = new Thickness(10);
                percentageBorder.HorizontalAlignment = HorizontalAlignment.Center;
                percentageBorder.VerticalAlignment = VerticalAlignment.Center;
                percentageBorder.Margin = new Thickness(10);


                Label matchPercentageLabel = new Label();
                matchPercentageLabel.Content = leads[i].match_percentage.ToString() + "%";
                matchPercentageLabel.FontSize = 20;
                matchPercentageLabel.HorizontalAlignment = HorizontalAlignment.Center;
                matchPercentageLabel.VerticalAlignment = VerticalAlignment.Center;
               
                if (leads[i].match_percentage >= 85)
                    percentageBorder.BorderBrush = Brushes.Green;
                else if (leads[i].match_percentage >= 70)
                    percentageBorder.BorderBrush = Brushes.Yellow;
                else
                    percentageBorder.BorderBrush = Brushes.Red;

                percentageBorder.Child = matchPercentageLabel;

                matchedLeedsGrid.Children.Add(percentageBorder);
                Grid.SetRow(percentageBorder, i + 1);
                Grid.SetColumn(percentageBorder, 2);


                Label viewLabel = new Label();
                viewLabel.Content = "View";
                viewLabel.MouseEnter += OnMouseEnterViewLabel;
                viewLabel.MouseLeave += OnMouseLeaveLabel;
                viewLabel.PreviewMouseLeftButtonDown += OnClickViewLabel;
                viewLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                viewLabel.VerticalContentAlignment = VerticalAlignment.Center;
                viewLabel.Style = (Style)FindResource("labelStyle");
                viewLabel.Tag = i;
                viewLabel.VerticalAlignment = VerticalAlignment.Center;
                viewLabel.HorizontalAlignment = HorizontalAlignment.Center;

                matchedLeedsGrid.Children.Add(viewLabel);
                Grid.SetRow(viewLabel, i + 1);
                Grid.SetColumn(viewLabel, 3);
            }

        }

        private void GetAllLeadsMatch()
        {
            for (int i = 0; i < leads.Count; i++)
            {
                BASIC_STRUCTS.LEAD_MATCHING_STRUCT tmp = new BASIC_STRUCTS.LEAD_MATCHING_STRUCT();
                tmp = leads[i];
                tmp.match_percentage = GetMatchedPercentage(currentProperty, leads[i]);
                leads[i] = tmp;
            }

            leads.Sort((s1, s2) => s2.match_percentage.CompareTo(s1.match_percentage));
        }


        private int GetMatchedPercentage(Property mProperty, BASIC_STRUCTS.LEAD_MATCHING_STRUCT mLead)
        {
            int matchPercentage = 0;
            int budget = 30;
            int areaOfInterest = 25;
            int paymentMethod = 25;
            int tags = 20;
            bool isInterestArea = false;

            bool tagFound = false;

            for (int i = 0; i < mLead.interested_property_tags.Count; i++)
            {
                int ratio = 20 / mLead.interested_property_tags.Count;

                if (mLead.interested_property_tags.Contains(currentProperty.GetPropertySerial()))
                {
                    //tags -= ratio;
                    tagFound = true;
                }
            }

            if (!tagFound)
                tags = 0;

            int budgetRangeIndex = budgetRanges.IndexOf(mLead.budget_range);

            bool rangeFound = false;
            for(int i = 0; i < budgetRanges.Count - budgetRangeIndex - 1; i++)
            {
                if (mProperty.GetPrice() >= budgetRanges[budgetRangeIndex].budget_min && mProperty.GetPrice() <= budgetRanges[budgetRangeIndex].budget_max)
                {
                    rangeFound = true;
                    break;
                }
                else
                    budget -= 3;
            }

            if (rangeFound == false)
            {
                budget = 30;

                for (int i = 0; i < budgetRangeIndex - 0; i++)
                {
                    if (mProperty.GetPrice() >= budgetRanges[i].budget_min && mProperty.GetPrice() <= budgetRanges[i].budget_max)
                        break;
                    else
                        budget -= 3;
                }
            }

            if (rangeFound == false)
                budget = 0;

            if(mProperty.GetDistrictId() != mLead.district_id)
            {
                areaOfInterest = 15;

                if (mProperty.GetCityId() != mLead.city_id)
                    areaOfInterest = 0;
            }

            if (mProperty.GetPaymentMethodId() != mLead.payment_method.method_id)
                paymentMethod = 13;

            matchPercentage += budget;
            matchPercentage += areaOfInterest;
            matchPercentage += paymentMethod;
            matchPercentage += tags;

            return matchPercentage;
        }

        private bool GetLeads()
        {
            leads.Clear();

            if (!commonQueries.GetPropertyMatchingLeads(ref leads))
                return false;

            return true;
        }

        private bool GetAllLeads()
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

        private bool GetBudgetRanges()
        {
            budgetRanges.Clear();

            if (!commonQueries.GetBudgetRanges(ref budgetRanges))
                return false;

            return true;
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

        private void OnMouseEnterViewLabel(object sender, MouseEventArgs e)
        {
            BrushConverter brush = new BrushConverter();
            Label currentLabel = (Label)sender;
            currentLabel.Background = (Brush)brush.ConvertFrom("#000000");
            currentLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
        }


        private void OnMouseLeaveLabel(object sender, MouseEventArgs e)
        {
            BrushConverter brush = new BrushConverter();
            Label currentLabel = (Label)sender;
            currentLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
            currentLabel.Background = (Brush)brush.ConvertFrom("#EDEDED");
        }

        private void OnClickViewLabel(object sender, MouseButtonEventArgs e)
        {
            Label currentLabel = (Label)sender;
            Lead currentLead = new Lead();
            Employee salesPerson = new Employee();
            salesPerson.InitializeEmployeeInfo(leads[(int)currentLabel.Tag].sales_person_id);
            currentLead.InitializeLeadInfo(salesPerson, leads[(int)currentLabel.Tag].lead_id);

            ViewLeadWindow viewLeadWindow = new ViewLeadWindow(ref loggedInUser, ref currentLead);
            viewLeadWindow.Show();
        }
        public void ResizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        private void OnClickPropertyInfo(object sender, MouseButtonEventArgs e)
        {
            PropertyInfoPage propertyInfoPage = new PropertyInfoPage(ref loggedInUser, ref currentProperty, viewAddCondition);
            NavigationService.Navigate(propertyInfoPage);
        }

        private void OnClickPropertyFiles(object sender, MouseButtonEventArgs e)
        {
            PropertyUploadFilesPage propertyUploadFilesPage = new PropertyUploadFilesPage(ref loggedInUser, ref currentProperty, viewAddCondition);
            NavigationService.Navigate(propertyUploadFilesPage);
        }
    }
}
