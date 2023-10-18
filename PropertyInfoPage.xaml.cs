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
    /// Interaction logic for PropertyInfoPage.xaml
    /// </summary>
    public partial class PropertyInfoPage : Page
    {
        private Employee loggedInUser;
        private CommonQueries commonQueries;
        private IntegrityChecks integrityChecks;

        private List<REAL_STATE_MACROS.PROPERTY_TYPE_STRUCT> propertyTypes;
        private List<REAL_STATE_MACROS.PAYMENT_METHOD_STRUCT> paymentMethods;
        private List<REAL_STATE_MACROS.PROPERTY_FINISHING_STRUCT> finishingTypes;
        private List<REAL_STATE_MACROS.PROPERTY_VIEW_STRUCT> propertyViews;
        private List<REAL_STATE_MACROS.PROPERTY_STATUS_STRUCT> propertyStatuses;

        private List<BASIC_STRUCTS.COUNTRY_STRUCT> countries;
        private List<BASIC_STRUCTS.STATE_STRUCT> states;
        private List<BASIC_STRUCTS.CITY_STRUCT> cities;
        private List<BASIC_STRUCTS.DISTRICT_STRUCT> districts;
        private List<BASIC_STRUCTS.STREET_STRUCT> streets;

        private List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT> listOfTags;
        private List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT> listOfSelectedTags;

        private Property currentProperty;

        private int viewAddCondition;

        public PropertyInfoPage(ref Employee mLoggedInUser, ref Property mCurrentProperty, int mViewAddCondition)
        {
            currentProperty = mCurrentProperty;
            loggedInUser = mLoggedInUser;
            viewAddCondition = mViewAddCondition;
            commonQueries = new CommonQueries();
            integrityChecks = new IntegrityChecks();

            propertyTypes = new List<REAL_STATE_MACROS.PROPERTY_TYPE_STRUCT>();
            paymentMethods = new List<REAL_STATE_MACROS.PAYMENT_METHOD_STRUCT>();
            finishingTypes = new List<REAL_STATE_MACROS.PROPERTY_FINISHING_STRUCT>();
            propertyViews = new List<REAL_STATE_MACROS.PROPERTY_VIEW_STRUCT>();
            propertyStatuses = new List<REAL_STATE_MACROS.PROPERTY_STATUS_STRUCT>();

            countries = new List<BASIC_STRUCTS.COUNTRY_STRUCT>();
            states = new List<BASIC_STRUCTS.STATE_STRUCT>();
            cities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            districts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();
            streets = new List<BASIC_STRUCTS.STREET_STRUCT>();

            listOfTags = new List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT>();
            listOfSelectedTags = new List<REAL_STATE_MACROS.PROPERTY_TAG_STRUCT>();

            InitializeComponent();

            if (viewAddCondition == REAL_STATE_MACROS.VIEW_CONDITION)
            {
                propertyDetailsHeader.Content = "VIEW PROPERTY DETAILS";
                SetUIElementsForView();
                SetLabelValues();
                saveChangesButton.IsEnabled = false;

            }
            else
            {
                InitializeSeaDirectionComboBox();
                InitializeParkingComboBox();

                if (!InitializePropertyTypeComboBox())
                    return;
                if (!InitializePaymentTypeComboBox())
                    return;
                if (!InitializeFinishingTypeComboBox())
                    return;
                if (!InitializePropertyViewComboBox())
                    return;
                if (!InitializePropertyStatusComboBox())
                    return;
                if (!InitializeCountryComboBox())
                    return;


                currentProperty.InitializePropertySales(loggedInUser.GetEmployeeId());

                saveChangesButton.IsEnabled = true;
            }

            InitializeTagsStackPanel();


        }

        private void SetUIElementsForView()
        {
            countryComboBox.Visibility = Visibility.Collapsed;
            stateComboBox.Visibility = Visibility.Collapsed;
            cityComboBox.Visibility = Visibility.Collapsed;
            districtComboBox.Visibility = Visibility.Collapsed;
            streetComboBox.Visibility = Visibility.Collapsed;
            priceTextBox.Visibility = Visibility.Collapsed;
            areaTextBox.Visibility = Visibility.Collapsed;
            floorNumberTextBox.Visibility = Visibility.Collapsed;
            buildingFloorsTextBox.Visibility = Visibility.Collapsed;
            flatsPerFloorsTextBox.Visibility = Visibility.Collapsed;
            numberOfToiletsTextBox.Visibility = Visibility.Collapsed;
            numberOfRoomsTextBox.Visibility = Visibility.Collapsed;
            numberOfEscalatorsTextBox.Visibility = Visibility.Collapsed;
            numberOfBalconiesTextBox.Visibility = Visibility.Collapsed;
            seaDirectionComboBox.Visibility = Visibility.Collapsed;
            parkingComboBox.Visibility = Visibility.Collapsed;
            finishingComboBox.Visibility = Visibility.Collapsed;
            propertyViewComboBox.Visibility = Visibility.Collapsed;
            paymentMethodComboBox.Visibility = Visibility.Collapsed;
            propertyTypeComboBox.Visibility = Visibility.Collapsed;
            propertyStatusComboBox.Visibility = Visibility.Collapsed;

            countryLabel.Visibility = Visibility.Visible;
            stateLabel.Visibility = Visibility.Visible;
            cityLabel.Visibility = Visibility.Visible;
            districtLabel.Visibility = Visibility.Visible;
            streetLabel.Visibility = Visibility.Visible;
            priceLabel.Visibility = Visibility.Visible;
            areaLabel.Visibility = Visibility.Visible;
            floorNumberLabel.Visibility = Visibility.Visible;
            buildingFloorsLabel.Visibility = Visibility.Visible;
            flatsPerFloorLabel.Visibility = Visibility.Visible;
            bathroomsLabel.Visibility = Visibility.Visible;
            roomsLabel.Visibility = Visibility.Visible;
            escalatorsLabel.Visibility = Visibility.Visible;
            balconiesLabel.Visibility = Visibility.Visible;
            seaDirectionLabel.Visibility = Visibility.Visible;
            parkingLabel.Visibility = Visibility.Visible;
            finishingLabel.Visibility = Visibility.Visible;
            propertyViewLabel.Visibility = Visibility.Visible;
            paymentMethodLabel.Visibility = Visibility.Visible;
            propertyTypeLabel.Visibility = Visibility.Visible;
            propertyStatusLabel.Visibility = Visibility.Visible;

        }

        private void SetLabelValues()
        {
            countryLabel.Content = currentProperty.GetCountry();
            stateLabel.Content = currentProperty.GetState();
            cityLabel.Content = currentProperty.GetCity();
            districtLabel.Content = currentProperty.GetDistrict();
            streetLabel.Content = currentProperty.GetLocation();
            priceLabel.Content = currentProperty.GetPrice().ToString();
            areaLabel.Content = currentProperty.GetArea().ToString();
            floorNumberLabel.Content = currentProperty.GetFloorNumber().ToString();
            buildingFloorsLabel.Content = currentProperty.GetBuildingFloors().ToString();
            flatsPerFloorLabel.Content = currentProperty.GetFlatsPerFloor().ToString();
            bathroomsLabel.Content = currentProperty.GetBathrooms().ToString();
            roomsLabel.Content = currentProperty.GetRooms().ToString();
            escalatorsLabel.Content = currentProperty.GetEscalatorNumber();
            balconiesLabel.Content = currentProperty.GetBalconies();
            seaDirectionLabel.Content = currentProperty.GetSeaDirection().ToString();
            parkingLabel.Content = currentProperty.GetHasParking().ToString();
            finishingLabel.Content = currentProperty.GetPropertyFinishing();
            propertyViewLabel.Content = currentProperty.GetPropertyView();
            paymentMethodLabel.Content = currentProperty.GetPaymentMethod();
            propertyTypeLabel.Content = currentProperty.GetPropertyType();
            propertyStatusLabel.Content = currentProperty.GetPropertyStatus();
        }

        //////////////////////////////////////////////////////////////////////////////////
        ///INITIALIZE FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////

        private bool InitializePropertyTypeComboBox()
        {
            if (!commonQueries.GetPropertyTypes(ref propertyTypes))
                return false;

            for (int i = 0; i < propertyTypes.Count; i++)
                propertyTypeComboBox.Items.Add(propertyTypes[i].property_type);

            return true;
        }

        private bool InitializePaymentTypeComboBox()
        {
            if (!commonQueries.GetPaymentMethods(ref paymentMethods))
                return false;

            for (int i = 0; i < paymentMethods.Count; i++)
                paymentMethodComboBox.Items.Add(paymentMethods[i].payment_method);

            return true;
        }

        private bool InitializeFinishingTypeComboBox()
        {
            if (!commonQueries.GetPropertyFinishings(ref finishingTypes))
                return false;

            for (int i = 0; i < finishingTypes.Count; i++)
                finishingComboBox.Items.Add(finishingTypes[i].finishing_type);

            return true;
        }
        private bool InitializePropertyViewComboBox()
        {
            if (!commonQueries.GetPropertyViews(ref propertyViews))
                return false;

            for (int i = 0; i < propertyViews.Count; i++)
                propertyViewComboBox.Items.Add(propertyViews[i].property_view);

            return true;
        }

        private bool InitializePropertyStatusComboBox()
        {
            if (!commonQueries.GetPropertiesStatus(ref propertyStatuses))
                return false;

            for (int i = 0; i < propertyStatuses.Count; i++)
                propertyStatusComboBox.Items.Add(propertyStatuses[i].property_status);

            return true;
        }

        private bool InitializeCountryComboBox()
        {
            countries.Clear();
            countryComboBox.Items.Clear();
            countryComboBox.SelectedIndex = -1;

            if (!commonQueries.GetAllCountries(ref countries))
                return false;

            for (int i = 0; i < countries.Count; i++)
                countryComboBox.Items.Add(countries[i].country_name);

            countryComboBox.SelectedItem = "Egypt";

            return true;
        }

        private bool InitializeStateComboBox()
        {
            states.Clear();
            stateComboBox.Items.Clear();
            stateComboBox.SelectedIndex = -1;

            if (!commonQueries.GetAllCountryStates(countries[countryComboBox.SelectedIndex].country_id, ref states))
                return false;

            for (int i = 0; i < states.Count; i++)
                stateComboBox.Items.Add(states[i].state_name);

            return true;
        }

        private bool InitializeCityComboBox()
        {
            cities.Clear();
            cityComboBox.Items.Clear();
            cityComboBox.SelectedIndex = -1;


            if (!commonQueries.GetAllStateCities(states[stateComboBox.SelectedIndex].state_id, ref cities))
                return false;

            for (int i = 0; i < cities.Count; i++)
                cityComboBox.Items.Add(cities[i].city_name);

            return true;
        }

        private bool InitializeDistrictComboBox()
        {
            districts.Clear();
            districtComboBox.Items.Clear();
            districtComboBox.SelectedIndex = -1;

            if (!commonQueries.GetAllCityDistricts(cities[cityComboBox.SelectedIndex].city_id, ref districts))
                return false;

            for (int i = 0; i < districts.Count; i++)
                districtComboBox.Items.Add(districts[i].district_name);

            return true;
        }

        private bool InitializeStreetComboBox()
        {
            streets.Clear();
            streetComboBox.Items.Clear();
            streetComboBox.SelectedIndex = -1;

            if (!commonQueries.GetAllDistrictStreets(districts[districtComboBox.SelectedIndex].district_id, ref streets))
                return false;

            for (int i = 0; i < streets.Count; i++)
                streetComboBox.Items.Add(streets[i].street_name);

            return true;
        }

        private bool InitializeSeaDirectionComboBox()
        {
            seaDirectionComboBox.Items.Clear();
            seaDirectionComboBox.SelectedIndex = -1;

            seaDirectionComboBox.Items.Add("True");
            seaDirectionComboBox.Items.Add("False");

            return true;
        }

        private bool InitializeParkingComboBox()
        {
            parkingComboBox.Items.Clear();
            parkingComboBox.SelectedIndex = -1;

            parkingComboBox.Items.Add("Available");
            parkingComboBox.Items.Add("Not Available");

            return true;
        }

        private bool InitializeTagsStackPanel()
        {
            TagsStackPanel.Children.Clear();

            if (!commonQueries.GetPropertyTags(ref listOfTags))
                return false;

            listOfSelectedTags.Clear();

            for (int i = 0; i < currentProperty.GetPropertyTags().Count; i++)
            {
                listOfSelectedTags.Add(currentProperty.GetPropertyTags()[i]);
            }

            for (int i = 0; i < listOfTags.Count(); i++)
            {
                Label currentTagLabel = new Label();
                currentTagLabel.Style = (Style)FindResource("BorderIconTextLabel");
                currentTagLabel.Content = listOfTags[i].property_tag;

                Border currentBorder = new Border();
                currentBorder.Style = (Style)FindResource("BorderIcon");
                currentBorder.Child = currentTagLabel;
                currentBorder.MouseDown += OnMouseDownBorderIcon;

                if (listOfSelectedTags.Contains(listOfTags[i]))
                {
                    BrushConverter brush = new BrushConverter();

                    currentBorder.Background = (Brush)brush.ConvertFrom("#EDEDED");
                    currentTagLabel.Foreground = (Brush)brush.ConvertFrom("#000000");
                }

                TagsStackPanel.Children.Add(currentBorder);
            }

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////
        ///SELECTION/TEXT CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedCountryComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (countryComboBox.SelectedIndex != -1)
            {
                stateComboBox.IsEnabled = true;
                if (!InitializeStateComboBox())
                    return;
                currentProperty.SetCountryId(countries[countryComboBox.SelectedIndex].country_id);
                currentProperty.SetCountry(countryComboBox.SelectedItem.ToString());
            }
            else
            {
                stateComboBox.IsEnabled = false;
                stateComboBox.SelectedIndex = -1;
                currentProperty.SetCountryId(0);
                currentProperty.SetCountry("");
            }
        }

        private void OnSelChangedStateComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (stateComboBox.SelectedIndex != -1)
            {
                cityComboBox.IsEnabled = true;
                if (!InitializeCityComboBox())
                    return;
                currentProperty.SetStateId(states[stateComboBox.SelectedIndex].state_id);
                currentProperty.SetState(stateComboBox.SelectedItem.ToString());
            }
            else
            {
                cityComboBox.IsEnabled = false;
                cityComboBox.SelectedIndex = -1;
                currentProperty.SetStateId(0);
                currentProperty.SetState("");
            }
        }

        private void OnSelChangedCityComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (cityComboBox.SelectedIndex != -1)
            {
                districtComboBox.IsEnabled = true;
                if (!InitializeDistrictComboBox())
                    return;
                currentProperty.SetCityId(cities[cityComboBox.SelectedIndex].city_id);
                currentProperty.SetCity(cityComboBox.SelectedItem.ToString());
            }
            else
            {
                districtComboBox.IsEnabled = false;
                districtComboBox.SelectedIndex = -1;
                currentProperty.SetCityId(0);
                currentProperty.SetCity("");
            }
        }

        private void OnSelChangedDistrictComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (districtComboBox.SelectedIndex != -1)
            {
                streetComboBox.IsEnabled = true;
                if (!InitializeStreetComboBox())
                    return;
                currentProperty.SetDistrictId(districts[districtComboBox.SelectedIndex].district_id);
                currentProperty.SetDistrict(districtComboBox.SelectedItem.ToString());
            }
            else
            {
                streetComboBox.IsEnabled = false;
                streetComboBox.SelectedIndex = -1;
                currentProperty.SetDistrictId(0);
                currentProperty.SetDistrict("");
            }
        }

        private void OnSelChangedStreetComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (streetComboBox.SelectedIndex != -1)
            {
                currentProperty.SetLocationId(streets[streetComboBox.SelectedIndex].street_id);
                currentProperty.SetLocation(streetComboBox.SelectedItem.ToString());
            }
            else
            {
                currentProperty.SetLocationId(0);
                currentProperty.SetLocation("");
            }
        }

        private void OnTextChangedPriceTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(priceTextBox.Text, BASIC_MACROS.PHONE_STRING) && priceTextBox.Text != "")
            {
                currentProperty.SetPrice(Decimal.Parse(priceTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < priceTextBox.Text.Length - 1; i++)
                {
                    temp += priceTextBox.Text[i].ToString();
                }
                priceTextBox.Text = temp;
                priceTextBox.Select(priceTextBox.Text.Length, 0);
            }
            
        }

        private void OnTextChangedAreaTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(areaTextBox.Text, BASIC_MACROS.PHONE_STRING) && areaTextBox.Text != "")
            {
                currentProperty.SetArea(Decimal.Parse(areaTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < areaTextBox.Text.Length - 1; i++)
                {
                    temp += areaTextBox.Text[i].ToString();
                }
                areaTextBox.Text = temp;
                areaTextBox.Select(areaTextBox.Text.Length, 0);
            }
            
        }

        private void OnTextChangedFloorNumberTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(floorNumberTextBox.Text, BASIC_MACROS.PHONE_STRING) && floorNumberTextBox.Text != "")
            {
                currentProperty.SetFloorNumber(int.Parse(floorNumberTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < floorNumberTextBox.Text.Length - 1; i++)
                {
                    temp += floorNumberTextBox.Text[i].ToString();
                }
                floorNumberTextBox.Text = temp;
                floorNumberTextBox.Select(floorNumberTextBox.Text.Length, 0);
            }
            
        }

        private void OnTextChangedBuildingFloorsNumberTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(buildingFloorsTextBox.Text, BASIC_MACROS.PHONE_STRING) && buildingFloorsTextBox.Text != "")
            {
                currentProperty.SetBuildingFloors(int.Parse(buildingFloorsTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < buildingFloorsTextBox.Text.Length - 1; i++)
                {
                    temp += buildingFloorsTextBox.Text[i].ToString();
                }
                buildingFloorsTextBox.Text = temp;
                buildingFloorsTextBox.Select(buildingFloorsTextBox.Text.Length, 0);
            }
            
        }

        private void OnTextChangedFlatsPerFloorTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(flatsPerFloorsTextBox.Text, BASIC_MACROS.PHONE_STRING) && flatsPerFloorsTextBox.Text != "")
            {
                currentProperty.SetFlatsPerFloor(int.Parse(flatsPerFloorsTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < flatsPerFloorsTextBox.Text.Length - 1; i++)
                {
                    temp += flatsPerFloorsTextBox.Text[i].ToString();
                }
                flatsPerFloorsTextBox.Text = temp;
                flatsPerFloorsTextBox.Select(flatsPerFloorsTextBox.Text.Length, 0);
            }
            
        }

        private void OnTextChangedNumberOfToiletsTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(numberOfToiletsTextBox.Text, BASIC_MACROS.PHONE_STRING) && numberOfToiletsTextBox.Text != "")
            {
                currentProperty.SetBathrooms(int.Parse(numberOfToiletsTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < numberOfToiletsTextBox.Text.Length - 1; i++)
                {
                    temp += numberOfToiletsTextBox.Text[i].ToString();
                }
                numberOfToiletsTextBox.Text = temp;
                numberOfToiletsTextBox.Select(numberOfToiletsTextBox.Text.Length, 0);
            }
            
        }

        private void OnTextChangedNumberOfRoomsTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(numberOfRoomsTextBox.Text, BASIC_MACROS.PHONE_STRING) && numberOfRoomsTextBox.Text != "")
            {
                currentProperty.SetRooms(int.Parse(numberOfRoomsTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < numberOfRoomsTextBox.Text.Length - 1; i++)
                {
                    temp += numberOfRoomsTextBox.Text[i].ToString();
                }
                numberOfRoomsTextBox.Text = temp;
                numberOfRoomsTextBox.Select(numberOfRoomsTextBox.Text.Length, 0);
            }
            
        }

        private void OnTextChangedNumberOfEscalatorsTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(numberOfEscalatorsTextBox.Text, BASIC_MACROS.PHONE_STRING) && numberOfEscalatorsTextBox.Text != "")
            {
                currentProperty.SetEscalatorNumber(int.Parse(numberOfEscalatorsTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < numberOfEscalatorsTextBox.Text.Length - 1; i++)
                {
                    temp += numberOfEscalatorsTextBox.Text[i].ToString();
                }
                numberOfEscalatorsTextBox.Text = temp;
                numberOfEscalatorsTextBox.Select(numberOfEscalatorsTextBox.Text.Length, 0);
            }
            
        }

        private void OnTextChangedNumberOfBalconiesTextBox(object sender, TextChangedEventArgs e)
        {
            if (integrityChecks.CheckInvalidCharacters(numberOfBalconiesTextBox.Text, BASIC_MACROS.PHONE_STRING) && numberOfBalconiesTextBox.Text != "")
            {
                currentProperty.SetBalconies(int.Parse(numberOfBalconiesTextBox.Text));
            }
            else
            {
                string temp = string.Empty;
                for (int i = 0; i < numberOfBalconiesTextBox.Text.Length - 1; i++)
                {
                    temp += numberOfBalconiesTextBox.Text[i].ToString();
                }
                numberOfBalconiesTextBox.Text = temp;
                numberOfBalconiesTextBox.Select(numberOfBalconiesTextBox.Text.Length, 0);
            }
            
        }

        private void OnSelChangedSeaDirectionComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (seaDirectionComboBox.SelectedIndex != -1)
            {
                if (seaDirectionComboBox.SelectedIndex == 0)
                    currentProperty.SetSeaDirection(true);
                else
                    currentProperty.SetSeaDirection(false);
            }
        }

        private void OnSelChangedParkingComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (parkingComboBox.SelectedIndex != -1)
            {
                if (parkingComboBox.SelectedIndex == 0)
                    currentProperty.SetHasParking(true);
                else
                    currentProperty.SetHasParking(false);
            }
        }

        private void OnSelChangedFinishingComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (finishingComboBox.SelectedIndex != -1)
            {
                currentProperty.SetPropertyFinishingId(finishingTypes[finishingComboBox.SelectedIndex].finishing_id);
                currentProperty.SetPropertyFinishing(finishingComboBox.SelectedItem.ToString());
            }
            else
            {
                currentProperty.SetPropertyFinishingId(0);
                currentProperty.SetPropertyFinishing("");
            }
        }

        private void OnSelChangedPropertyViewComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (propertyViewComboBox.SelectedIndex != -1)
            {
                currentProperty.SetPropertyViewId(propertyViews[propertyViewComboBox.SelectedIndex].view_id);
                currentProperty.SetPropertyView(propertyViewComboBox.SelectedItem.ToString());
            }
            else
            {
                currentProperty.SetPropertyViewId(0);
                currentProperty.SetPropertyView("");
            }
        }

        private void OnSelChangedPaymentMethodComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (paymentMethodComboBox.SelectedIndex != -1)
            {
                currentProperty.SetPaymentMethodId(paymentMethods[paymentMethodComboBox.SelectedIndex].method_id);
                currentProperty.SetPaymentMethod(paymentMethodComboBox.SelectedItem.ToString());
            }
            else
            {
                currentProperty.SetPaymentMethodId(0);
                currentProperty.SetPaymentMethod("");
            }
        }

        private void OnSelChangedPropertyTypeComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (propertyTypeComboBox.SelectedIndex != -1)
            {
                currentProperty.SetPropertyTypeId(propertyTypes[propertyTypeComboBox.SelectedIndex].type_id);
                currentProperty.SetPropertyType(propertyTypeComboBox.SelectedItem.ToString());
            }
            else
            {
                currentProperty.SetPropertyTypeId(0);
                currentProperty.SetPropertyType("");
            }
        }

        private void OnSelChangedPropertyStatusComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (propertyStatusComboBox.SelectedIndex != -1)
            {
                currentProperty.SetPropertyStatusId(propertyStatuses[propertyStatusComboBox.SelectedIndex].status_id);
                currentProperty.SetPropertyStatus(propertyStatusComboBox.SelectedItem.ToString());
            }
            else
            {
                currentProperty.SetPropertyStatusId(propertyStatuses[propertyStatusComboBox.SelectedIndex].status_id);
                currentProperty.SetPropertyStatus(propertyStatusComboBox.SelectedItem.ToString());
            }
        }

        //////////////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICK HANDLERS
        //////////////////////////////////////////////////////////////////////////////////

        private void OnMouseDownBorderIcon(object sender, RoutedEventArgs e)
        {
            saveChangesButton.IsEnabled = true;

            Border currentSelectedIcon = (Border)sender;
            Label currentSelectedLabel = (Label)currentSelectedIcon.Child;

            BrushConverter brush = new BrushConverter();

            int currentSelectedIndex = TagsStackPanel.Children.IndexOf(currentSelectedIcon);
            REAL_STATE_MACROS.PROPERTY_TAG_STRUCT currentSelectedTag = listOfTags[currentSelectedIndex];

            if (!listOfSelectedTags.Exists(tag_item => tag_item.tag_id == currentSelectedTag.tag_id))
            {
                currentSelectedIcon.Background = (Brush)brush.ConvertFrom("#EDEDED");
                currentSelectedLabel.Foreground = (Brush)brush.ConvertFrom("#000000");

                listOfSelectedTags.Add(currentSelectedTag);
            }

            else
            {
                currentSelectedIcon.Background = (Brush)brush.ConvertFrom("#000000");
                currentSelectedLabel.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
                listOfSelectedTags.Remove(listOfSelectedTags.Find(tag_item => tag_item.tag_id == currentSelectedTag.tag_id));
            }

        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition == REAL_STATE_MACROS.ADD_CONDITION)
            {
                currentProperty.SetPropertyTags(listOfSelectedTags);

                if (!currentProperty.IssueNewProperty())
                    return;

                viewAddCondition = REAL_STATE_MACROS.VIEW_CONDITION;

                PropertyUploadFilesPage propertyUploadFilesPage = new PropertyUploadFilesPage(ref loggedInUser, ref currentProperty, viewAddCondition);
                NavigationService.Navigate(propertyUploadFilesPage);
            }
            else
            {
                if(currentProperty.GetPropertyTags() != listOfSelectedTags)
                {
                    currentProperty.SetPropertyTags(listOfSelectedTags);

                    if (!currentProperty.UpdatePropertyTagsSelected())
                        return;
                }

                saveChangesButton.IsEnabled = false;
            }
        }

        private void OnClickUploadFiles(object sender, MouseButtonEventArgs e)
        {
            if(viewAddCondition == REAL_STATE_MACROS.VIEW_CONDITION)
            {
                PropertyUploadFilesPage propertyUploadFilesPage = new PropertyUploadFilesPage(ref loggedInUser, ref currentProperty, viewAddCondition);
                NavigationService.Navigate(propertyUploadFilesPage);
            }
        }

        private void OnClickMatchedLeeds(object sender, MouseButtonEventArgs e)
        {
            if (viewAddCondition == REAL_STATE_MACROS.VIEW_CONDITION)
            {
                PropertyMatchedLeeds propertyMatchedLeeds = new PropertyMatchedLeeds(ref loggedInUser, ref currentProperty, ref viewAddCondition);
                NavigationService.Navigate(propertyMatchedLeeds);
            }
        }






    }
}

