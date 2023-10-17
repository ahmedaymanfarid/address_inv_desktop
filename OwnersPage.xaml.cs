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
    /// Interaction logic for OwnersPage.xaml
    /// </summary>
    public partial class OwnersPage : Page
    {
        private SQLServer sqlDatabase;
        private Employee loggedInUser;
        private CommonQueries commonQueries;

        private List<REAL_STATE_MACROS.OWNER_STRUCT> ownersList;

        private List<BASIC_STRUCTS.STATE_STRUCT> listOfStates;
        private List<BASIC_STRUCTS.CITY_STRUCT> listOfCities;
        private List<BASIC_STRUCTS.DISTRICT_STRUCT> listOfDistricts;

        private List<KeyValuePair<int, TreeViewItem>> ownersTreeArray;

        private REAL_STATE_MACROS.OWNER_STRUCT owner;

        public OwnersPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            sqlDatabase = new SQLServer();
            commonQueries = new CommonQueries();
            loggedInUser = mLoggedInUser;

            owner = new REAL_STATE_MACROS.OWNER_STRUCT();
            owner.propertyList = new List<REAL_STATE_MACROS.PROPERTY_MIN_STRUCT>();
            owner.notes = new List<string>();
            owner.owner_phones = new List<string>();

            ownersList = new List<REAL_STATE_MACROS.OWNER_STRUCT>();
            

            ownersTreeArray = new List<KeyValuePair<int, TreeViewItem>>();

            listOfStates = new List<BASIC_STRUCTS.STATE_STRUCT>();
            listOfCities = new List<BASIC_STRUCTS.CITY_STRUCT>();
            listOfDistricts = new List<BASIC_STRUCTS.DISTRICT_STRUCT>();

            if (!GetOwnersList())
                return;

            InitializeOwnersTree();
        }

        public void DisableNecessaryItems()
        {
            ownerNameTextBox.IsEnabled = false;
            ownerPhoneTextBox.IsEnabled = false;

            propertyStateComboBox.IsEnabled = false;

            propertyCityCheckBox.IsEnabled = false;
            propertyCityComboBox.IsEnabled = false;

            propertyDistrictCheckBox.IsEnabled = false;
            propertyDistrictCheckBox.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// INITIALIZATION FUNCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public bool GetOwnersList()
        {
            if (!commonQueries.GetPropertyOwners(ref ownersList))
                return false;

            return true;
        }
        public void InitializeOwnersTree()
        {
            ownersTreeView.Items.Clear();

            ownersTreeArray.Clear();

            for (int j = 0; j < ownersList.Count(); j++)
            {
                if (ownerNameCheckBox.IsChecked == true && ownerNameTextBox.Text != "" && ownersList[j].owner_name.IndexOf(ownerNameTextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                //if (ownerPhoneCheckBox.IsChecked == true && ownerPhoneTextBox.Text != "" && !ownersList[j].owner_phones.Contains(ownerPhoneTextBox.Text))
                //    continue;

                

                TreeViewItem ParentItem = new TreeViewItem();

                ParentItem.Header = ownersList[j].owner_name;
                ParentItem.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                ParentItem.FontSize = 14;
                ParentItem.FontWeight = FontWeights.SemiBold;
                ParentItem.FontFamily = new FontFamily("Sans Serif");
                ParentItem.Tag = ownersList[j].owner_id;

                if (ownersList[j].propertyList != null)
                {
                    for (int i = 0; i < ownersList[j].propertyList.Count; i++)
                    {
                        int state = 0;
                        int city = 0;
                        int district = 0;
                        String temp = string.Empty;
                        String tempLocation = ownersList[j].propertyList[i].location.street_id.ToString();

                        for (int k = 0; k < tempLocation.Length; k++)
                        {
                            temp += tempLocation[k];

                            if (k == 3)
                                state = int.Parse(temp);
                            if (k == 5)
                                city = int.Parse(temp);
                            if (k == 7)
                                district = int.Parse(temp);
                        }

                        if (propertyStateCheckBox.IsChecked == true && propertyStateComboBox.SelectedIndex != -1 && listOfStates[propertyStateComboBox.SelectedIndex].state_id != state)
                            continue;
                        if (propertyCityCheckBox.IsChecked == true && propertyCityComboBox.SelectedIndex != -1 && listOfCities[propertyCityComboBox.SelectedIndex].city_id != city)
                            continue;
                        if (propertyDistrictCheckBox.IsChecked == true && propertyDistrictComboBox.SelectedIndex != -1 && listOfDistricts[propertyDistrictComboBox.SelectedIndex].district_id != district)
                            continue;

                        TreeViewItem propertyItem = new TreeViewItem();
                        propertyItem.Header = ownersList[j].propertyList[i].property_id;
                        propertyItem.Foreground = new SolidColorBrush(Color.FromRgb(16, 90, 151));
                        propertyItem.FontSize = 14;
                        propertyItem.FontWeight = FontWeights.SemiBold;
                        propertyItem.FontFamily = new FontFamily("Sans Serif");
                        propertyItem.Tag = ownersList[j].propertyList[i].property_serial;

                        ParentItem.Items.Add(propertyItem);
                    }
                }

                ownersTreeView.Items.Add(ParentItem);

                ownersTreeArray.Add(new KeyValuePair<int, TreeViewItem>(ownersList[j].owner_id, ParentItem));

            }
        }

        public bool InitializeStatesComboBox()
        {
            propertyStateComboBox.Items.Clear();

            if (!commonQueries.GetAllCountryStates(BASIC_MACROS.EGYPT_COUNTRY_ID, ref listOfStates))
                return false;

            for (int i = 0; i < listOfStates.Count; i++)
                propertyStateComboBox.Items.Add(listOfStates[i].state_name);

            propertyStateComboBox.SelectedIndex = 0;

            return true;
        }

        public bool InitializeCitiesComboBox()
        {
            propertyCityComboBox.Items.Clear();

            if (!commonQueries.GetAllStateCities(listOfStates[propertyStateComboBox.SelectedIndex].state_id, ref listOfCities))
                return false;

            for (int i = 0; i < listOfCities.Count; i++)
                propertyCityComboBox.Items.Add(listOfCities[i].city_name);

            propertyCityComboBox.SelectedIndex = 0;

            return true;
        }

        public bool InitializeDistrictsComboBox()
        {
            propertyDistrictComboBox.Items.Clear();

            if (!commonQueries.GetAllCityDistricts(listOfCities[propertyCityComboBox.SelectedIndex].city_id, ref listOfDistricts))
                return false;

            for (int i = 0; i < listOfDistricts.Count; i++)
                propertyDistrictComboBox.Items.Add(listOfDistricts[i].district_name);

            propertyDistrictComboBox.SelectedIndex = 0;

            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON TEXT CHANGED  HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnTextChangedOwnerName(object sender, TextChangedEventArgs e)
        {
            InitializeOwnersTree();
        }
        private void OnTextChangedOwnerPhone(object sender, TextChangedEventArgs e)
        {
            InitializeOwnersTree();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON CHECKED  HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnCheckedOwnerNameCheckBox(object sender, RoutedEventArgs e)
        {
            ownerNameTextBox.IsEnabled = true;
        }
        private void OnCheckedOwnerPhoneCheckBox(object sender, RoutedEventArgs e)
        {
            ownerPhoneTextBox.IsEnabled = true;
        }
        private void OnCheckedPropertyStateCheckBox(object sender, RoutedEventArgs e)
        {
            propertyStateComboBox.IsEnabled = true;

            if (!InitializeStatesComboBox())
                return;

        }
        private void OnCheckedPropertyCityCheckBox(object sender, RoutedEventArgs e)
        {
            propertyCityComboBox.IsEnabled = true;

            if (!InitializeCitiesComboBox())
                return;
        }
        private void OnCheckedPropertyDistrictCheckBox(object sender, RoutedEventArgs e)
        {
            propertyDistrictComboBox.IsEnabled = true;

            if (!InitializeDistrictsComboBox())
                return;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON UNCHECKED  HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnUncheckedOwnerNameCheckBox(object sender, RoutedEventArgs e)
        {
            ownerNameTextBox.IsEnabled = false;
            ownerNameTextBox.Text = null;

            viewButton.IsEnabled = false;
        }
        private void OnUncheckedOwnerPhoneCheckBox(object sender, RoutedEventArgs e)
        {
            ownerPhoneTextBox.IsEnabled = false;
            ownerPhoneTextBox.Text = null;

            viewButton.IsEnabled = false;
        }
        private void OnUncheckedPropertyStateCheckBox(object sender, RoutedEventArgs e)
        {
            propertyStateComboBox.IsEnabled = false;
            propertyStateComboBox.SelectedIndex = -1;

            viewButton.IsEnabled = false;
        }
        private void OnUncheckedPropertyCityCheckBox(object sender, RoutedEventArgs e)
        {
            propertyCityComboBox.IsEnabled = false;
            propertyCityComboBox.SelectedIndex = -1;

            viewButton.IsEnabled = false;

        }
        private void OnUncheckedPropertyDistrictCheckBox(object sender, RoutedEventArgs e)
        {
            propertyDistrictComboBox.IsEnabled = false;
            propertyDistrictComboBox.SelectedIndex = -1;

            viewButton.IsEnabled = false;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ON SELETION CHANGED HANDLERS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSelChangedPropertyStateComboBox(object sender, SelectionChangedEventArgs e)
        {
            if(propertyStateComboBox.SelectedIndex != -1)
                InitializeOwnersTree();
        }
        private void OnSelChangedPropertyCityComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (propertyCityComboBox.SelectedIndex != -1)
                InitializeOwnersTree();
        }
        private void OnSelChangedPropertyDistrictComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (propertyDistrictComboBox.SelectedIndex != -1)
                InitializeOwnersTree();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BUTTON CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            //REAL_STATE_MACROS.OWNER_STRUCT owner = new REAL_STATE_MACROS.OWNER_STRUCT();
            //owner.owner_phones = new List<string>();
            //owner.propertyList = new List<REAL_STATE_MACROS.PROPERTY_MIN_STRUCT>();
            //owner.notes = new List<string>();
            int viewAddCondition = REAL_STATE_MACROS.ADD_CONDITION;


            AddOwnerWindow addOwnerWindow = new AddOwnerWindow(ref loggedInUser, ref owner, viewAddCondition, false);
            addOwnerWindow.Closed += OnClosedAddOwnerWindow;
            addOwnerWindow.Show();
        }

        private void OnBtnClickAddProperty(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)ownersTreeView.SelectedItem;
            int viewAddCondition = REAL_STATE_MACROS.ADD_CONDITION;

            owner.notes.Clear();
            owner.propertyList.Clear();
            owner.owner_phones.Clear();
            InitializeOwner((int)selectedItem.Tag);
            AddOwnerWindow addOwnerWindow = new AddOwnerWindow(ref loggedInUser, ref owner, viewAddCondition, true);
            addOwnerWindow.Closed += OnClosedAddOwnerWindow;
            addOwnerWindow.Show();

        }

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)ownersTreeView.SelectedItem;
            int viewAddCondition = REAL_STATE_MACROS.VIEW_CONDITION;

            if(ownersTreeArray.Exists(s1 => s1.Value == selectedItem))
            {
                owner.notes.Clear();
                owner.propertyList.Clear();
                owner.owner_phones.Clear();
                InitializeOwner((int)selectedItem.Tag);
                AddOwnerWindow addOwnerWindow = new AddOwnerWindow(ref loggedInUser, ref owner, viewAddCondition, false);
                addOwnerWindow.Show();
            }
            else
            {
                Property property = new Property();
                property.InitializeProperty((int)selectedItem.Tag);

                AddPropertyWindow addPropertyWindow = new AddPropertyWindow(ref loggedInUser, ref property, viewAddCondition);
                addPropertyWindow.Show();
            }
        }

        private void OnClosedAddOwnerWindow(object sender, EventArgs e)
        {
            GetOwnersList();
            InitializeOwnersTree();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON SELECTED ITEM CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelectedItemChangedTreeViewItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            viewButton.IsEnabled = false;
            addPropertyButton.IsEnabled = false;
            TreeViewItem selectedItem = (TreeViewItem)ownersTreeView.SelectedItem;

            if (selectedItem != null)
                viewButton.IsEnabled = true;

            if (ownersTreeArray.Exists(s1 => s1.Value == selectedItem) && selectedItem != null)
            {
                addPropertyButton.IsEnabled = true;
            }
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
        //DATABASE RELATED
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private bool InitializeOwner(int ownerId)
        {
            String query = string.Empty;
            String queryPart1 = @"select owners_info.owner_id,
  owners_info.job_title,
  owners_info.added_by,
  owners_properties.property_serial,
  properties.property_status,
  properties.property_type,
  properties.finishing,
  owner_notes.note_id,
  owner_mobile.telephone_id,
  properties.location,
  properties.price,
  properties.area,
  employees_info.name,
  owners_info.name, 
  owners_info.email,
  owners_info.gender,
  job_titles.job_title,
  properties.property_id,
  streets.street,
  properties_status.property_status,
  property_types.property_type,
  finishing_types.finishing_type,
  owner_notes.notes,
  owner_mobile.mobile

  from address_inv_desktop.dbo.owners_info
  left join address_inv_desktop.dbo.job_titles
  on owners_info.job_title = job_titles.id
  left join address_inv_desktop.dbo.employees_info
  on owners_info.added_by = employees_info.employee_id
  left join address_inv_desktop.dbo.owners_properties
  on owners_info.owner_id = owners_properties.owner_id
  left join address_inv_desktop.dbo.properties
  on owners_properties.property_serial = properties.property_serial
  left join address_inv_desktop.dbo.streets
  on properties.location = streets.id
  left join address_inv_desktop.dbo.properties_status
  on properties.property_status = properties_status.id
  left join address_inv_desktop.dbo.property_types
  on properties.property_type = property_types.id
  left join address_inv_desktop.dbo.finishing_types
  on properties.finishing = finishing_types.id
  left join address_inv_desktop.dbo.owner_notes
  on owners_info.owner_id = owner_notes.owner_id
  left join address_inv_desktop.dbo.owner_mobile
  on owners_info.owner_id = owner_mobile.owner_id

  where owners_info.owner_id = ";

            query += queryPart1;
            query += ownerId;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT queryColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            queryColumns.sql_int = 9;
            queryColumns.sql_bigint = 1;
            queryColumns.sql_money = 1;
            queryColumns.sql_decimal = 1;
            queryColumns.sql_string = 12;

            if (!sqlDatabase.GetRows(query, queryColumns))
                return false;

            int telephoneId = 1;
            int noteId = 1;

            for (int i = 0; i < sqlDatabase.rows.Count; i++)
            {
                int numericCount = 0;
                int StringCount = 0;
                
                owner.owner_id = sqlDatabase.rows[i].sql_int[numericCount++];
                owner.job_title_id = sqlDatabase.rows[i].sql_int[numericCount++];
                owner.added_by_id = sqlDatabase.rows[i].sql_int[numericCount++];

                REAL_STATE_MACROS.PROPERTY_MIN_STRUCT tempProperty = new REAL_STATE_MACROS.PROPERTY_MIN_STRUCT();
                tempProperty.property_serial = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProperty.property_status.status_id = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProperty.property_type.type_id = sqlDatabase.rows[i].sql_int[numericCount++];
                tempProperty.property_finishing.finishing_id = sqlDatabase.rows[i].sql_int[numericCount++];
                
                /////iterate twice for note_id and telephone_id
                numericCount += 2;

                tempProperty.location.street_id = sqlDatabase.rows[i].sql_bigint[0];
                tempProperty.price = sqlDatabase.rows[i].sql_money[0];
                tempProperty.area = sqlDatabase.rows[i].sql_decimal[0];

                owner.added_by = sqlDatabase.rows[i].sql_string[StringCount++];
                owner.owner_name = sqlDatabase.rows[i].sql_string[StringCount++];
                owner.email = sqlDatabase.rows[i].sql_string[StringCount++];
                owner.gender = sqlDatabase.rows[i].sql_string[StringCount++];
                owner.job_title = sqlDatabase.rows[i].sql_string[StringCount++];

                tempProperty.property_id = sqlDatabase.rows[i].sql_string[StringCount++];
                tempProperty.location.street_name = sqlDatabase.rows[i].sql_string[StringCount++];
                tempProperty.property_status.property_status = sqlDatabase.rows[i].sql_string[StringCount++];
                tempProperty.property_type.property_type = sqlDatabase.rows[i].sql_string[StringCount++];
                tempProperty.property_finishing.finishing_type = sqlDatabase.rows[i].sql_string[StringCount++];

                if(i == 0)
                {
                    if(tempProperty.property_serial != 0)
                        owner.propertyList.Add(tempProperty);

                    owner.notes.Add(sqlDatabase.rows[i].sql_string[StringCount++]);
                    owner.owner_phones.Add(sqlDatabase.rows[i].sql_string[StringCount++]);
                }
                else
                {
                    if (sqlDatabase.rows[i].sql_int[7] > noteId)
                    {
                        noteId++;
                        owner.notes.Add(sqlDatabase.rows[i].sql_string[10]);
                    }

                    if (sqlDatabase.rows[i].sql_int[8] > telephoneId)
                    {
                        telephoneId++;
                        owner.owner_phones.Add(sqlDatabase.rows[i].sql_string[11]);
                    }

                    if (tempProperty.property_serial != owner.propertyList.Last().property_serial)
                    {
                        owner.propertyList.Add(tempProperty);
                    }
                }


            }

            return true;
        }

        
    }
}
