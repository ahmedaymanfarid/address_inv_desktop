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
    /// Interaction logic for LeadMatchedPropertiesPage.xaml
    /// </summary>
    public partial class LeadMatchedPropertiesPage : Page
    {
        

        Employee loggedInUser;
        Property currentProperty;

        CommonQueries commonQueries;

        Lead lead;

        private List<REAL_STATE_MACROS.PROPERTY_STRUCT> properties;
        private List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT> budgetRanges;


        private int viewAddCondition;

        public LeadMatchedPropertiesPage(ref Employee mLoggedInUser, ref Lead mCurrentLead)
        {
            loggedInUser = mLoggedInUser;
            lead = mCurrentLead;
            commonQueries = new CommonQueries();

            InitializeComponent();

            properties = new List<REAL_STATE_MACROS.PROPERTY_STRUCT>();
            budgetRanges = new List<REAL_STATE_MACROS.BUDGET_RANGE_STRUCT>();

            if (!GetProperties())
                return;
            if (!GetBudgetRanges())
                return;

            GetAllPropertiesMatch();

            InitializePropertiesGrid();

        }


        private void InitializePropertiesGrid()
        {

            for (int i = 0; i < properties.Count; i++)
            {
                matchedPropertiesGrid.RowDefinitions.Add(new RowDefinition());

                BrushConverter brush = new BrushConverter();

                Label propertyID = new Label();
                propertyID.Content = properties[i].property_id;
                propertyID.Style = (Style)FindResource("stackPanelItemHeader");
                propertyID.HorizontalAlignment = HorizontalAlignment.Center;
                propertyID.VerticalAlignment = VerticalAlignment.Center;
                propertyID.FontSize = 18;
                propertyID.Background = (Brush)brush.ConvertFrom("#EDEDED");

                matchedPropertiesGrid.Children.Add(propertyID);
                Grid.SetRow(propertyID, i + 1);
                Grid.SetColumn(propertyID, 0);


                Border percentageBorder = new Border();
                percentageBorder.CornerRadius = new CornerRadius(100);
                percentageBorder.Width = 80;
                percentageBorder.Height = 80;
                percentageBorder.BorderThickness = new Thickness(10);
                percentageBorder.HorizontalAlignment = HorizontalAlignment.Center;
                percentageBorder.VerticalAlignment = VerticalAlignment.Center;
                percentageBorder.Background = (Brush)brush.ConvertFrom("#EDEDED");
                percentageBorder.Margin = new Thickness(19);


                Label matchPercentageLabel = new Label();
                matchPercentageLabel.Content = properties[i].matched_percentage.ToString() + "%";
                matchPercentageLabel.FontSize = 20;
                matchPercentageLabel.HorizontalAlignment = HorizontalAlignment.Center;
                matchPercentageLabel.VerticalAlignment = VerticalAlignment.Center;

                if (properties[i].matched_percentage >= 85)
                    percentageBorder.BorderBrush = Brushes.Green;
                else if (properties[i].matched_percentage >= 70)
                    percentageBorder.BorderBrush = Brushes.Yellow;
                else
                    percentageBorder.BorderBrush = Brushes.Red;

                percentageBorder.Child = matchPercentageLabel;

                matchedPropertiesGrid.Children.Add(percentageBorder);
                Grid.SetRow(percentageBorder, i + 1);
                Grid.SetColumn(percentageBorder, 1);


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

                matchedPropertiesGrid.Children.Add(viewLabel);
                Grid.SetRow(viewLabel, i + 1);
                Grid.SetColumn(viewLabel, 2);
            }

        }

        private void GetAllPropertiesMatch()
        {
            for (int i = 0; i < properties.Count; i++)
            {
                REAL_STATE_MACROS.PROPERTY_STRUCT tmp = new REAL_STATE_MACROS.PROPERTY_STRUCT();
                tmp = properties[i];
                tmp.matched_percentage = GetMatchedPercentage(lead, properties[i]);
                properties[i] = tmp;
            }

            properties.Sort((s1, s2) => s2.matched_percentage.CompareTo(s1.matched_percentage));
        }


        private int GetMatchedPercentage(Lead mLead, REAL_STATE_MACROS.PROPERTY_STRUCT mProperty)
        {
            int matchPercentage = 0;
            int budget = 30;
            int areaOfInterest = 25;
            int paymentMethod = 25;
            int tags = 20;

            bool tagFound = false;

            for (int i = 0; i < mLead.GetNumberOfSavedTags(); i++)
            {
                int ratio = 20 / mLead.GetNumberOfSavedTags();

                if (mProperty.property_tags.Contains(mLead.GetLeadInterests()[i]))
                {
                    //tags -= ratio;
                    tagFound = true;
                }
            }

            if (!tagFound)
                tags = 0;

            int budgetRangeIndex = budgetRanges.FindIndex(s1 => s1.budget_id == mLead.GetLeadBudgetId());

            bool rangeFound = false;
            for (int i = 0; i < budgetRanges.Count - budgetRangeIndex - 1; i++)
            {
                if (mProperty.price >= budgetRanges[budgetRangeIndex].budget_min && mProperty.price <= budgetRanges[budgetRangeIndex].budget_max)
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
                    if (mProperty.price >= budgetRanges[i].budget_min && mProperty.price <= budgetRanges[i].budget_max)
                    {
                        rangeFound = true;
                        break;
                    }
                    else
                        budget -= 3;
                }
            }

            if(rangeFound == false)
                budget = 0;

            if (!mLead.GetLeadInterestedAreas().Contains(mProperty.district))
            {
                areaOfInterest = 15;

                for(int i = 0; i < mLead.GetLeadInterestedAreas().Count; i++)
                {
                    String temp = mLead.GetLeadInterestedAreas()[i].district_id.ToString();

                    if (mProperty.city.city_id.ToString().Contains(temp))
                    {
                        areaOfInterest = 15;
                        break;
                    }
                    else
                        areaOfInterest = 0;
                }
            }

            if (mLead.GetLeadPaymentMethodId() != mProperty.payment_method.method_id)
                paymentMethod = 13;

            matchPercentage += budget;
            matchPercentage += areaOfInterest;
            matchPercentage += paymentMethod;
            matchPercentage += tags;

            return matchPercentage;
        }

        private bool GetProperties()
        {
            properties.Clear();

            if (!commonQueries.GetProperties(ref properties))
                return false;

            return true;
        }

        private bool GetBudgetRanges()
        {
            budgetRanges.Clear();

            if (!commonQueries.GetBudgetRanges(ref budgetRanges))
                return false;

            return true;
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
            Property currentProperty = new Property();
            currentProperty.InitializeProperty(properties[(int)currentLabel.Tag].property_serial);

            int viewAddCondition = REAL_STATE_MACROS.VIEW_CONDITION;

            AddPropertyWindow addPropertyWindow = new AddPropertyWindow(ref loggedInUser, ref currentProperty, viewAddCondition);
            addPropertyWindow.Show();
        }
        public void ResizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        private void OnClickLeadInfo(object sender, MouseButtonEventArgs e)
        {
            LeadInfoPage leadInfoPage = new LeadInfoPage(ref loggedInUser, ref lead);
            NavigationService.Navigate(leadInfoPage);
        }

    }
}

