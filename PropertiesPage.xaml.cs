using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for PropertiesPage.xaml
    /// </summary>
    
    public partial class PropertiesPage : Page
    {
        private Employee loggedInUser;
        private CommonQueries commonQueries;
        private FTPServer ftpServer;
        IntegrityChecks integrityChecks;

        private List<REAL_STATE_MACROS.PROPERTY_STRUCT> propertiesList;

        private List<REAL_STATE_MACROS.PROPERTY_TYPE_STRUCT> propertyTypes;
        private List<REAL_STATE_MACROS.PAYMENT_METHOD_STRUCT> paymentMethods;
        private List<REAL_STATE_MACROS.PROPERTY_FINISHING_STRUCT> finishingTypes;
        private List<REAL_STATE_MACROS.PROPERTY_VIEW_STRUCT> propertyViews;
        private List<REAL_STATE_MACROS.PROPERTY_STATUS_STRUCT> propertyStatuses;

        private List<string> propertyPhotoNames;
        private List<KeyValuePair<int,List<Image>>> propertyPhotos;
        private List<MediaElement> propertyVideos;
        private List<Border> propertyStackPanelValue;
        protected BackgroundWorker downloadBackground;
        private Grid selectedPropertyGrid;
        private Grid previousSelectedPropertyGrid;

        Popup popUp;

        public PropertiesPage(ref Employee mLoggedInUser)
        {
            InitializeComponent();

            loggedInUser = mLoggedInUser;

            commonQueries = new CommonQueries();
            ftpServer = new FTPServer();
            integrityChecks = new IntegrityChecks();

            //popUp = new Popup();

            propertiesList = new List<REAL_STATE_MACROS.PROPERTY_STRUCT>();

            propertyTypes = new List<REAL_STATE_MACROS.PROPERTY_TYPE_STRUCT>();
            paymentMethods = new List<REAL_STATE_MACROS.PAYMENT_METHOD_STRUCT>();
            finishingTypes = new List<REAL_STATE_MACROS.PROPERTY_FINISHING_STRUCT>();
            propertyViews = new List<REAL_STATE_MACROS.PROPERTY_VIEW_STRUCT>();
            propertyStatuses = new List<REAL_STATE_MACROS.PROPERTY_STATUS_STRUCT>();

            propertyStackPanelValue = new List<Border>();

            propertyPhotoNames = new List<string>();
            propertyPhotos = new List<KeyValuePair<int, List<Image>>>();

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

            if (!GetProperties())
                return;
            if (!GetPropertyPhotos())
                return;

            InitializePropertiesStackPanel();

            
        }

        private bool GetProperties()
        {
            if (!commonQueries.GetProperties(ref propertiesList))
                return false;

            return true;
        }

        private bool GetPropertyPhotos()
        {
            propertyPhotos.Clear();

            for (int i = 0; i < propertiesList.Count; i++)
            {
                String folderName = propertiesList[i].property_serial.ToString();
                String localFolderPath = Directory.GetCurrentDirectory() + "\\" + folderName;
                if (!CheckFolderExists(localFolderPath))
                    CreateFolder(localFolderPath);

                string serverFolderPath = BASIC_MACROS.PROPERTIES_MEDIA_PATH + propertiesList[i].property_serial + "/";
                DateTime ftpFolderTimeStamp = new DateTime();

                if (!ftpServer.GetFtpFolderTimeStamp(serverFolderPath, ref ftpFolderTimeStamp))
                    return false;

                DateTime folderTimeStamp = new DateTime();

                //if (!commonQueries.GetPropertyTimeStamp(loggedInUser.GetEmployeeId(), propertiesList[i].property_serial, ref folderTimeStamp))
                //    return false;

                folderTimeStamp = propertiesList[i].property_timestamp;

                if (folderTimeStamp != DateTime.MinValue && folderTimeStamp.Day == ftpFolderTimeStamp.Day && folderTimeStamp.Month == ftpFolderTimeStamp.Month)
                {
                    DirectoryInfo directory = new DirectoryInfo(localFolderPath);
                    FileInfo[] files = directory.GetFiles();
                    List<Image> imagesList = new List<Image>();
                    foreach(FileInfo file in files)
                    {
                        ////ONLY GET PHOTOS AND NOT VIDEOS TO BE ADDED IN STACKPANEL AS PER AHMED AYMANS REQUEST
                        try
                        {
                            Image currentImage = new Image();
                            currentImage.Source = new BitmapImage(new Uri(file.FullName, UriKind.Absolute));
                            imagesList.Add(currentImage);
                        }
                        catch
                        {

                        }
                    }

                    propertyPhotos.Add(new KeyValuePair<int, List<Image>>(propertiesList[i].property_serial, imagesList));

                }
                else
                {
                    propertyPhotoNames.Clear();


                    if (ftpServer.ListFilesInFolder(serverFolderPath, ref propertyPhotoNames))
                    {

                        List<Image> imagesList = new List<Image>();

                        List<KeyValuePair<int, String>> keyValuePairs = new List<KeyValuePair<int, string>>();

                        for(int k = 0; k < propertyPhotoNames.Count; k++)
                        {
                            keyValuePairs.Add( new KeyValuePair<int, string>(int.Parse(propertyPhotoNames[k].Split('.')[0]), propertyPhotoNames[k]));
                        }

                        keyValuePairs = keyValuePairs.OrderBy(x => x.Key).ToList();

                        for (int j = 0; j < propertyPhotoNames.Count; j++)
                        {
                            String photoName = keyValuePairs[j].Value;
                            String serverImagePath = serverFolderPath + photoName;
                            String localImagePath = localFolderPath + "\\" + photoName;
                            Image currentImage = new Image();

                            DateTime imageTimeStamp = new DateTime();

                            if (!ftpServer.GetFtpFileTimeStamp(serverImagePath, ref imageTimeStamp))
                                return false;

                            if (!photoName.Contains(".mp4") && propertiesList[i].property_photos_timestamps.Count != 0 && imageTimeStamp == propertiesList[i].property_photos_timestamps.Find(x => x.photo_serial == keyValuePairs[j].Key - 1000).last_modified_date)
                            {
                                currentImage.Source = new BitmapImage(new Uri(localImagePath, UriKind.Absolute));

                                if (!commonQueries.UpdatePropertyPhotoTimeStamp(loggedInUser.GetEmployeeId(), propertiesList[i].property_serial, propertiesList[i].property_photos_timestamps[propertiesList[i].property_photos_timestamps.Count - 1].photo_serial, imageTimeStamp))
                                    return false;
                            }
                            else
                            {
                                if (!photoName.Contains(".mp4"))
                                {
                                    if (!ftpServer.DownloadFile(serverImagePath, localImagePath))
                                        return false;
                                    currentImage.Source = new BitmapImage(new Uri(localImagePath, UriKind.Absolute));

                                    int newPhotoSerial = 0;
                                    if (propertiesList[i].property_photos_timestamps.Count == 0)
                                        newPhotoSerial = 1;
                                    else
                                        newPhotoSerial = propertiesList[i].property_photos_timestamps[propertiesList[i].property_photos_timestamps.Count - 1].photo_serial + 1;

                                    if (!commonQueries.InsertPropertyPhotoTimeStamp(loggedInUser.GetEmployeeId(), propertiesList[i].property_serial, newPhotoSerial, imageTimeStamp))
                                        return false;

                                    propertiesList[i].property_photos_timestamps.Add(new REAL_STATE_MACROS.PROPERTY_PHOTO_TIMESTAMP_STRUCT() { photo_serial = newPhotoSerial });

                                }
                            }

                            imagesList.Add(currentImage);
                        }

                        propertyPhotos.Add(new KeyValuePair<int, List<Image>>(propertiesList[i].property_serial, imagesList));

                        if (folderTimeStamp == DateTime.MinValue)
                        {
                            commonQueries.InsertPropertyTimeStamp(loggedInUser.GetEmployeeId(), propertiesList[i].property_serial, ftpFolderTimeStamp);
                        }
                        else
                        {
                            commonQueries.UpdatePropertyTimeStamp(loggedInUser.GetEmployeeId(), propertiesList[i].property_serial, ftpFolderTimeStamp);
                        }
                    }
                }
            }
            return true;
        }

        private void InitializePropertiesStackPanel()
        {
            propertiesStackPanel.Children.Clear();
            propertyStackPanelValue.Clear();

            for (int i = 0; i < propertiesList.Count; i++) 
            {
                if (typeComboBox.SelectedIndex != -1 && propertyTypes[typeComboBox.SelectedIndex].type_id != propertiesList[i].property_type.type_id)
                    continue;
                if (areaTextBox.Text != "" && areaTextBox.Text != propertiesList[i].area.ToString())
                    continue;
                if (priceTextBox.Text != "" && Decimal.Parse(priceTextBox.Text) != propertiesList[i].price)
                    continue;
                if (paymentComboBox.SelectedIndex != -1 && paymentMethods[paymentComboBox.SelectedIndex].method_id != propertiesList[i].payment_method.method_id)
                    continue;
                if (roomsTextBox.Text != "" && roomsTextBox.Text != propertiesList[i].rooms.ToString())
                    continue;
                if (areaTextBox.Text != "" && areaTextBox.Text != propertiesList[i].area.ToString())
                    continue;
                if (bathroomsTextBox.Text != "" && bathroomsTextBox.Text != propertiesList[i].bathrooms.ToString())
                    continue;
                if (liftsTextBox.Text != "" && liftsTextBox.Text != propertiesList[i].escalators.ToString())
                    continue;
                if (finishingComboBox.SelectedIndex != -1 && finishingTypes[finishingComboBox.SelectedIndex].finishing_id != propertiesList[i].property_finishing.finishing_id)
                    continue;
                if (buildingFloorsTextBox.Text != "" && buildingFloorsTextBox.Text != propertiesList[i].building_floors.ToString())
                    continue;
                if (floorTextBox.Text != "" && floorTextBox.Text != propertiesList[i].floor_no.ToString())
                    continue;
                if (floorUnitsTextBox.Text != "" && floorUnitsTextBox.Text != propertiesList[i].flats_per_floor.ToString())
                    continue;
                if (viewComboBox.SelectedIndex != -1 && propertyViews[viewComboBox.SelectedIndex].view_id != propertiesList[i].property_view.view_id)
                    continue;
                if (statusComboBox.SelectedIndex != -1 && propertyStatuses[statusComboBox.SelectedIndex].status_id != propertiesList[i].property_status.status_id)
                    continue;
                bool hasParking = new bool();
                if (parkingComboBox.SelectedIndex == 0)
                    hasParking = true;
                else
                    hasParking = false;
                if (parkingComboBox.SelectedIndex != -1 && hasParking != propertiesList[i].has_parking)
                    continue;

                BrushConverter brush = new BrushConverter();

                Grid currentPropertyGrid = new Grid();
                currentPropertyGrid.PreviewMouseLeftButtonDown += OnClickProperty;
                
                ColumnDefinition photoColumn = new ColumnDefinition();
                ColumnDefinition infoColumn = new ColumnDefinition();

                currentPropertyGrid.ColumnDefinitions.Add(photoColumn);
                currentPropertyGrid.ColumnDefinitions.Add(infoColumn);

                ScrollViewer photoScrollViewer = new ScrollViewer();
                photoScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                photoScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                photoScrollViewer.VerticalAlignment = VerticalAlignment.Stretch;
                //photoScrollViewer.Margin = new Thickness(0, 0, 0, 3);

                StackPanel photoStackPanel = new StackPanel();
                photoStackPanel.Orientation = Orientation.Horizontal;

                photoScrollViewer.Content = photoStackPanel;

                currentPropertyGrid.Children.Add(photoScrollViewer);
                Grid.SetColumn(photoScrollViewer, 0);

                AddPropertyPhotos(ref photoStackPanel, propertiesList[i].property_serial);

                AddPropertyInfoGrid(ref currentPropertyGrid, i);

                Border currentPropertyBorder = new Border();
                currentPropertyBorder.BorderBrush = (Brush)brush.ConvertFrom("#000000");
                currentPropertyBorder.BorderThickness = new Thickness(3);
                //currentPropertyBorder.Margin = new Thickness(20);
                currentPropertyBorder.Height = 200;
                currentPropertyBorder.Child = currentPropertyGrid;

                propertiesStackPanel.Children.Add(currentPropertyBorder);

                //propertyStackPanelValue.Add(currentPropertyBorder);
            }
        }

        private void RefreshPropertiesStackPanel()
        {
            propertiesStackPanel.Children.Clear();

            for (int i = 0; i < propertiesList.Count; i++)
            {
                if (typeComboBox.SelectedIndex != -1 && propertyTypes[typeComboBox.SelectedIndex].type_id != propertiesList[i].property_type.type_id)
                    continue;
                if (areaTextBox.Text != "" && areaTextBox.Text != propertiesList[i].area.ToString())
                    continue;
                if (priceTextBox.Text != "" && Decimal.Parse(priceTextBox.Text) != propertiesList[i].price)
                    continue;
                if (paymentComboBox.SelectedIndex != -1 && paymentMethods[paymentComboBox.SelectedIndex].method_id != propertiesList[i].payment_method.method_id)
                    continue;
                if (roomsTextBox.Text != "" && roomsTextBox.Text != propertiesList[i].rooms.ToString())
                    continue;
                if (areaTextBox.Text != "" && areaTextBox.Text != propertiesList[i].area.ToString())
                    continue;
                if (bathroomsTextBox.Text != "" && bathroomsTextBox.Text != propertiesList[i].bathrooms.ToString())
                    continue;
                if (liftsTextBox.Text != "" && liftsTextBox.Text != propertiesList[i].escalators.ToString())
                    continue;
                if (finishingComboBox.SelectedIndex != -1 && finishingTypes[finishingComboBox.SelectedIndex].finishing_id != propertiesList[i].property_finishing.finishing_id)
                    continue;
                if (buildingFloorsTextBox.Text != "" && buildingFloorsTextBox.Text != propertiesList[i].building_floors.ToString())
                    continue;
                if (floorTextBox.Text != "" && floorTextBox.Text != propertiesList[i].floor_no.ToString())
                    continue;
                if (floorUnitsTextBox.Text != "" && floorUnitsTextBox.Text != propertiesList[i].flats_per_floor.ToString())
                    continue;
                if (viewComboBox.SelectedIndex != -1 && propertyViews[viewComboBox.SelectedIndex].view_id != propertiesList[i].property_view.view_id)
                    continue;
                if (statusComboBox.SelectedIndex != -1 && propertyStatuses[statusComboBox.SelectedIndex].status_id != propertiesList[i].property_status.status_id)
                    continue;
                bool hasParking = new bool();
                if (parkingComboBox.SelectedIndex == 0)
                    hasParking = true;
                else
                    hasParking = false;
                if (parkingComboBox.SelectedIndex != -1 && hasParking != propertiesList[i].has_parking)
                    continue;

                propertiesStackPanel.Children.Add(propertyStackPanelValue[i]);
            }
        }



            private void AddPropertyPhotos(ref StackPanel photoStackPanel, int propertySerial)
        {
            List<Image> currentPropertyPhotos = new List<Image>();
            currentPropertyPhotos = propertyPhotos.Find(s1 => s1.Key == propertySerial).Value;

            for (int i = 0; i < currentPropertyPhotos.Count; i++)
            {
                Image propertPhoto = new Image();
                propertPhoto = currentPropertyPhotos[i];

                propertPhoto.HorizontalAlignment = HorizontalAlignment.Stretch;
                propertPhoto.VerticalAlignment = VerticalAlignment.Stretch;

                photoStackPanel.Children.Add(propertPhoto);

            }
        }

        private void AddPropertyInfoGrid(ref Grid propertyGrid, int propertyNumber)
        {
            ScrollViewer propertyScrollViewer = new ScrollViewer();
            propertyScrollViewer.Height = 200;
            propertyScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            propertyGrid.Children.Add(propertyScrollViewer);
            Grid.SetRow(propertyScrollViewer, 0);
            Grid.SetColumn(propertyScrollViewer, 1);

            Grid currentInfoGrid = new Grid();
            propertyScrollViewer.Content = currentInfoGrid;

            ColumnDefinition gridHeaderColumn = new ColumnDefinition();
            ColumnDefinition gridValueColumn = new ColumnDefinition();
            currentInfoGrid.ColumnDefinitions.Add(gridHeaderColumn);
            currentInfoGrid.ColumnDefinitions.Add(gridValueColumn);

            for (int i = 0; i < REAL_STATE_MACROS.PROPERTY_INFO_PARAMETERS_NO; i++) 
            {
                RowDefinition currentGridRow = new RowDefinition();
                currentInfoGrid.RowDefinitions.Add(currentGridRow);
            }

            Label propertyTypeHeader = new Label();
            propertyTypeHeader.Content = "TYPE";
            propertyTypeHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyTypeHeader);
            Grid.SetRow(propertyTypeHeader, 0);
            Grid.SetColumn(propertyTypeHeader, 0);

            Label propertyTypeLabel = new Label();
            propertyTypeLabel.Content = propertiesList[propertyNumber].property_type.property_type;
            propertyTypeLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyTypeLabel);
            Grid.SetRow(propertyTypeLabel, 0);
            Grid.SetColumn(propertyTypeLabel, 1);

            
            Label propertyAreaHeader = new Label();
            propertyAreaHeader.Content = "AREA";
            propertyAreaHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyAreaHeader);
            Grid.SetRow(propertyAreaHeader, 1);
            Grid.SetColumn(propertyAreaHeader, 0);

            Label propertyAreaLabel = new Label();
            propertyAreaLabel.Content = propertiesList[propertyNumber].area;
            propertyAreaLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyAreaLabel);
            Grid.SetRow(propertyAreaLabel, 1);
            Grid.SetColumn(propertyAreaLabel, 1);

            
            Label propertyPriceHeader = new Label();
            propertyPriceHeader.Content = "PRICE";
            propertyPriceHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyPriceHeader);
            Grid.SetRow(propertyPriceHeader, 2);
            Grid.SetColumn(propertyPriceHeader, 0);
            
            Label propertyPriceLabel = new Label();
            propertyPriceLabel.Content = propertiesList[propertyNumber].price;
            propertyPriceLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyPriceLabel);
            Grid.SetRow(propertyPriceLabel, 2);
            Grid.SetColumn(propertyPriceLabel, 1);

            
            Label propertyPaymentHeader = new Label();
            propertyPaymentHeader.Content = "PAYMENT";
            propertyPaymentHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyPaymentHeader);
            Grid.SetRow(propertyPaymentHeader, 3);
            Grid.SetColumn(propertyPaymentHeader, 0);

            Label propertyPaymentLabel = new Label();
            propertyPaymentLabel.Content = propertiesList[propertyNumber].payment_method.payment_method;
            propertyPaymentLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyPaymentLabel);
            Grid.SetRow(propertyPaymentLabel, 3);
            Grid.SetColumn(propertyPaymentLabel, 1);

            Label propertyRoomsHeader = new Label();
            propertyRoomsHeader.Content = "ROOMS";
            propertyRoomsHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyRoomsHeader);
            Grid.SetRow(propertyRoomsHeader, 4);
            Grid.SetColumn(propertyRoomsHeader, 0);

            Label propertyRoomsLabel = new Label();
            propertyRoomsLabel.Content = propertiesList[propertyNumber].rooms;
            propertyRoomsLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyRoomsLabel);
            Grid.SetRow(propertyRoomsLabel, 4);
            Grid.SetColumn(propertyRoomsLabel, 1);

            Label propertyBathroomsHeader = new Label();
            propertyBathroomsHeader.Content = "BATHROOMS";
            propertyBathroomsHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyBathroomsHeader);
            Grid.SetRow(propertyBathroomsHeader, 5);
            Grid.SetColumn(propertyBathroomsHeader, 0);

            Label propertyBathroomsLabel = new Label();
            propertyBathroomsLabel.Content = propertiesList[propertyNumber].bathrooms;
            propertyBathroomsLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyBathroomsLabel);
            Grid.SetRow(propertyBathroomsLabel, 5);
            Grid.SetColumn(propertyBathroomsLabel, 1);

            Label propertyLiftsHeader = new Label();
            propertyLiftsHeader.Content = "LIFTS";
            propertyLiftsHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyLiftsHeader);
            Grid.SetRow(propertyLiftsHeader, 6);
            Grid.SetColumn(propertyLiftsHeader, 0);

            Label propertyLiftsLabel = new Label();
            propertyLiftsLabel.Content = propertiesList[propertyNumber].escalators;
            propertyLiftsLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyLiftsLabel);
            Grid.SetRow(propertyLiftsLabel, 6);
            Grid.SetColumn(propertyLiftsLabel, 1);

            Label propertyFinishingHeader = new Label();
            propertyFinishingHeader.Content = "FINISHING";
            propertyFinishingHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyFinishingHeader);
            Grid.SetRow(propertyFinishingHeader, 7);
            Grid.SetColumn(propertyFinishingHeader, 0);

            Label propertyFinishingLabel = new Label();
            propertyFinishingLabel.Content = propertiesList[propertyNumber].property_finishing.finishing_type;
            propertyFinishingLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyFinishingLabel);
            Grid.SetRow(propertyFinishingLabel, 7);
            Grid.SetColumn(propertyFinishingLabel, 1);

            Label propertyBuildingFloorsHeader = new Label();
            propertyBuildingFloorsHeader.Content = "BUILD. FLOORS";
            propertyBuildingFloorsHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyBuildingFloorsHeader);
            Grid.SetRow(propertyBuildingFloorsHeader, 8);
            Grid.SetColumn(propertyBuildingFloorsHeader, 0);

            Label propertyBuildingFloorsLabel = new Label();
            propertyBuildingFloorsLabel.Content = propertiesList[propertyNumber].building_floors;
            propertyBuildingFloorsLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyBuildingFloorsLabel);
            Grid.SetRow(propertyBuildingFloorsLabel, 8);
            Grid.SetColumn(propertyBuildingFloorsLabel, 1);

            Label propertyFloorNoHeader = new Label();
            propertyFloorNoHeader.Content = "FLOOR NO.";
            propertyFloorNoHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyFloorNoHeader);
            Grid.SetRow(propertyFloorNoHeader, 9);
            Grid.SetColumn(propertyFloorNoHeader, 0);

            Label propertyFloorNoLabel = new Label();
            propertyFloorNoLabel.Content = propertiesList[propertyNumber].floor_no;
            propertyFloorNoLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyFloorNoLabel);
            Grid.SetRow(propertyFloorNoLabel, 9);
            Grid.SetColumn(propertyFloorNoLabel, 1);

            Label propertyFloorUnitsHeader = new Label();
            propertyFloorUnitsHeader.Content = "FLATS/FLOOR";
            propertyFloorUnitsHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyFloorUnitsHeader);
            Grid.SetRow(propertyFloorUnitsHeader, 10);
            Grid.SetColumn(propertyFloorUnitsHeader, 0);

            Label propertyFloorUnitsLabel = new Label();
            propertyFloorUnitsLabel.Content = propertiesList[propertyNumber].flats_per_floor;
            propertyFloorUnitsLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyFloorUnitsLabel);
            Grid.SetRow(propertyFloorUnitsLabel, 10);
            Grid.SetColumn(propertyFloorUnitsLabel, 1);

            
            Label propertyViewHeader = new Label();
            propertyViewHeader.Content = "VIEW";
            propertyViewHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyViewHeader);
            Grid.SetRow(propertyViewHeader, 11);
            Grid.SetColumn(propertyViewHeader, 0);

            Label propertyViewLabel = new Label();
            propertyViewLabel.Content = propertiesList[propertyNumber].property_view.property_view;
            propertyViewLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyViewLabel);
            Grid.SetRow(propertyViewLabel, 11);
            Grid.SetColumn(propertyViewLabel, 1);

            Label propertyStatusHeader = new Label();
            propertyStatusHeader.Content = "STATUS";
            propertyStatusHeader.Style = (Style)FindResource("tableHeaderItem");
            currentInfoGrid.Children.Add(propertyStatusHeader);
            Grid.SetRow(propertyStatusHeader, 12);
            Grid.SetColumn(propertyStatusHeader, 0);

            Label propertyStatusLabel = new Label();
            propertyStatusLabel.Content = propertiesList[propertyNumber].property_status.property_status;
            propertyStatusLabel.Style = (Style)FindResource("tableSubItemLabel");
            currentInfoGrid.Children.Add(propertyStatusLabel);
            Grid.SetRow(propertyStatusLabel, 12);
            Grid.SetColumn(propertyStatusLabel, 1);
        }
        private bool InitializePropertyTypeComboBox()
        {
            if (!commonQueries.GetPropertyTypes(ref propertyTypes))
                return false;

            for (int i = 0; i < propertyTypes.Count; i++)
                typeComboBox.Items.Add(propertyTypes[i].property_type);

            return true; 
        }
        private bool InitializePaymentTypeComboBox()
        {
            if (!commonQueries.GetPaymentMethods(ref paymentMethods))
                return false;

            for (int i = 0; i < paymentMethods.Count; i++)
                paymentComboBox.Items.Add(paymentMethods[i].payment_method);

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
                viewComboBox.Items.Add(propertyViews[i].property_view);

            return true;
        }

        private bool InitializePropertyStatusComboBox()
        {
            if (!commonQueries.GetPropertiesStatus(ref propertyStatuses))
                return false;

            for (int i = 0; i < propertyStatuses.Count; i++)
                statusComboBox.Items.Add(propertyStatuses[i].property_status);

            return true;
        }

        private void InitializePropertyParkingComboBox()
        {
            parkingComboBox.Items.Add("Available");
            parkingComboBox.Items.Add("Not Available");
        }

        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SELECTION CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnSelChangedParameter(object sender, SelectionChangedEventArgs e)
        {
            ClearStackPanelChildren();
            InitializePropertiesStackPanel();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //TEXT CHANGED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnTextChangedParameter(object sender, TextChangedEventArgs e)
        {

            TextBox currentTextBox = (TextBox)sender;

            if (integrityChecks.CheckInvalidCharacters(currentTextBox.Text, BASIC_MACROS.PHONE_STRING))
            {
                ClearStackPanelChildren();
                InitializePropertiesStackPanel();
            }
            else
            {
                string temp = string.Empty;
                for(int i = 0; i < currentTextBox.Text.Length - 1; i++)
                {
                    temp += currentTextBox.Text[i].ToString();
                }
                currentTextBox.Text = temp;
                currentTextBox.Select(currentTextBox.Text.Length, 0);
            }

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //BUTTON CLICKED HANDLERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnBtnClickAdd(object sender, RoutedEventArgs e)
        {
            Property property = new Property();
            int viewAddCondition = REAL_STATE_MACROS.ADD_CONDITION;

            AddPropertyWindow addPropertyWindow = new AddPropertyWindow(ref loggedInUser, ref property, viewAddCondition);
            addPropertyWindow.Closed += OnClosedAddPropertyWindow;
            addPropertyWindow.Show();
        }

        

        private void OnBtnClickView(object sender, RoutedEventArgs e)
        {
            Property property = new Property();
            Border currentBorder = (Border)selectedPropertyGrid.Parent;
            property.InitializeProperty(propertiesList[propertiesStackPanel.Children.IndexOf(currentBorder)].property_serial);

            int viewAddCondition = REAL_STATE_MACROS.VIEW_CONDITION;

            AddPropertyWindow addPropertyWindow = new AddPropertyWindow(ref loggedInUser, ref property, viewAddCondition);
            addPropertyWindow.Closed += OnClosedAddPropertyWindow;
            addPropertyWindow.Show();
        }

        private void OnClickProperty(object sender, MouseButtonEventArgs e)
        {
            previousSelectedPropertyGrid = selectedPropertyGrid;
            selectedPropertyGrid = (Grid)sender;

            BrushConverter brush = new BrushConverter();

            if (previousSelectedPropertyGrid != null)
            {
                ScrollViewer infoScrollViewer = (ScrollViewer)previousSelectedPropertyGrid.Children[1];
                Grid infoGrid = (Grid)infoScrollViewer.Content;

                int prevHeader = 0;
                int prevValue = 1;

                Border prevBorder = (Border)previousSelectedPropertyGrid.Parent;
                prevBorder.BorderThickness = new Thickness(3);
                prevBorder.BorderBrush = (Brush)brush.ConvertFrom("#000000");

                for (int i = 0; i < infoGrid.Children.Count; i++)
                {
                    if (i == prevHeader)
                    {
                        Label currentHeader = (Label)infoGrid.Children[i];
                        currentHeader.Background = (Brush)brush.ConvertFrom("#000000");
                        currentHeader.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");

                        prevHeader += 2;
                    }
                    else
                    {
                        Label currentValue = (Label)infoGrid.Children[i];
                        currentValue.Background = (Brush)brush.ConvertFrom("#FFFFFF");
                        currentValue.Foreground = (Brush)brush.ConvertFrom("#000000");

                        prevValue += 2;
                    }
                }

                ScrollViewer photoScrollViewer = (ScrollViewer)previousSelectedPropertyGrid.Children[0];
                StackPanel photoStackPanel = (StackPanel)photoScrollViewer.Content;

                photoStackPanel.Background = (Brush)brush.ConvertFrom("#FFFFFF");


            }

            Border currentBorder = (Border)selectedPropertyGrid.Parent;
            currentBorder.BorderThickness = new Thickness(5);
            currentBorder.BorderBrush = (Brush)brush.ConvertFrom("#EDEDED");

            //ScrollViewer infoScrollViewerCurrent = (ScrollViewer)selectedPropertyGrid.Children[1];
            //Grid infoGridCurrent = (Grid)infoScrollViewerCurrent.Content;
            //
            //int header = 0;
            //int value = 1;
            //
            //for (int i = 0; i < infoGridCurrent.Children.Count; i++)
            //{
            //    if (i == header)
            //    {
            //        Label currentHeader = (Label)infoGridCurrent.Children[i];
            //        currentHeader.Background = (Brush)brush.ConvertFrom("#FFFFFF");
            //        currentHeader.Foreground = (Brush)brush.ConvertFrom("#000000");
            //
            //        header += 2;
            //    }
            //    else
            //    {
            //        Label currentValue = (Label)infoGridCurrent.Children[i];
            //        currentValue.Background = (Brush)brush.ConvertFrom("#000000");
            //        currentValue.Foreground = (Brush)brush.ConvertFrom("#FFFFFF");
            //
            //        value += 2;
            //    }
            //}
            //
            //ScrollViewer photoScrollViewerCurrent = (ScrollViewer)selectedPropertyGrid.Children[0];
            //StackPanel photoStackPanelCurrent = (StackPanel)photoScrollViewerCurrent.Content;
            //
            //photoStackPanelCurrent.Background = (Brush)brush.ConvertFrom("#000000");
            //
            //
            viewButton.IsEnabled = true;
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EXTERNAL TABS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnButtonClickedDashboard(object sender, RoutedEventArgs e)
        {
            DashboardPage dashboardPage = new DashboardPage(ref loggedInUser);
            this.NavigationService.Navigate(dashboardPage);
            //this.NavigationService.RemoveBackEntry();
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
        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        private void OnClickPropertyPhoto(object sender, MouseButtonEventArgs e)
        {
            if (popUp.IsOpen == false)
            {
                Image currentImage = (Image)sender;
                Image tempImage = new Image();
                tempImage.Source = new BitmapImage(new Uri(currentImage.Source.ToString(), UriKind.Absolute));
                tempImage.Stretch = Stretch.UniformToFill;
                //tempImage.Height = 500;
                //tempImage.Width = 500;

                popUp.PlacementTarget = mainGrid; 
                popUp.Placement = PlacementMode.Center;
                popUp.PreviewMouseLeftButtonDown += OnClickPopUp;
                popUp.Child = (Image)tempImage;

                popUp.IsOpen = true;
                propertiesStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void OnClickPopUp(object sender, MouseButtonEventArgs e)
        {
            popUp.IsOpen = false;
            propertiesStackPanel.Visibility = Visibility.Visible;
        }

        public void ResizeImage(ref Image imgToResize, int width, int height)
        {
            imgToResize.Width = width;
            imgToResize.Height = height;
        }

        private void OnClosedAddPropertyWindow(object sender, EventArgs e)
        {
            ClearStackPanelChildren();
            InitializePropertiesStackPanel();
        }

        private void ClearStackPanelChildren()
        {
            for (int i = 0; i < propertiesStackPanel.Children.Count; i++)
            {
                Border propertyBorder = (Border)propertiesStackPanel.Children[i];
                Grid propertyGrid = (Grid)propertyBorder.Child;

                ScrollViewer photoScrollViewer = (ScrollViewer)propertyGrid.Children[0];
                StackPanel photoStackPanel = (StackPanel)photoScrollViewer.Content;
                photoStackPanel.Children.Clear();

                //ScrollViewer infoScrollViewer = (ScrollViewer)propertyGrid.Children[1];
                //Grid infoGrid = (Grid)infoScrollViewer.Content;
                //infoGrid.Children.Clear();
            }
        }

        private void CreateFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
        }
        private bool CheckFolderExists(string folderPath)
        {
            if (Directory.Exists(folderPath))
                return true;
            else
                return false;
        }

        private bool CheckFileExists(string filePath)
        {
            if (Directory.Exists(filePath))
                return true;
            else
                return false;
        }

    }
}
