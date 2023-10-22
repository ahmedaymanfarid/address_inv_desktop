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
using System.Windows.Shapes;
using address_inv_library;
using real_estate_library;

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for AddOwnerWindow.xaml
    /// </summary>
    public partial class AddOwnerWindow : Window
    {
        SQLServer sqlDatabase;
        Employee loggedInUser;
        CommonQueries commonQueries;

        REAL_STATE_MACROS.OWNER_STRUCT owner;
        List<REAL_STATE_MACROS.PROPERTY_LOCATION_STRUCT> properties;
        List<REAL_STATE_MACROS.PROPERTY_LOCATION_STRUCT> ownerProperties;
        List<COMPANY_ORGANISATION_MACROS.JOB_TITLE_STRUCT> jobTitles;

        String notes;
        int notesId;
        int phoneId;
        int viewAddCondition;
        bool addPropertyCondition;

        public AddOwnerWindow(ref Employee mLoggedInUser, ref REAL_STATE_MACROS.OWNER_STRUCT mOwner, int mViewAddCondition, bool mAddPropertyCondition)
        {
            sqlDatabase = new SQLServer();
            loggedInUser = mLoggedInUser;
            commonQueries = new CommonQueries(sqlDatabase);
            owner = mOwner;
            viewAddCondition = mViewAddCondition;
            addPropertyCondition = mAddPropertyCondition;

            properties = new List<REAL_STATE_MACROS.PROPERTY_LOCATION_STRUCT>();
            ownerProperties = new List<REAL_STATE_MACROS.PROPERTY_LOCATION_STRUCT>();
            jobTitles = new List<COMPANY_ORGANISATION_MACROS.JOB_TITLE_STRUCT>();

            InitializeComponent();

            if (viewAddCondition == REAL_STATE_MACROS.ADD_CONDITION && addPropertyCondition == false)
            {
                InitializeJobTitleCombo();
                genderCombo.Items.Add("Male");
                genderCombo.Items.Add("Female");
            }
            else if(viewAddCondition == REAL_STATE_MACROS.ADD_CONDITION)
            {
                ownerHeader.Content = "ADD PROPERTIES FOR OWNER";
                ownerDetailsWrapPanel.Visibility = Visibility.Collapsed;

                Grid.SetRow(propertiesScrollViewer, 0);

                addPhoneButton.Visibility = Visibility.Collapsed;

                Grid.SetColumnSpan(saveChangesButton, 3);
                saveChangesButton.HorizontalAlignment = HorizontalAlignment.Center;

                propertiesCheckBox.IsChecked = true;
            }
            else 
            {
                ownerHeader.Content = "VIEW OWNER";
                SetUpUIElementsForView();
                SetLabelContentsForView();
                SetExtraPhonesForView();
                SetExtraNotesForView();
                if (owner.propertyList.Count > 0)
                {
                    propertiesCheckBox.IsChecked = true;
                    propertiesCheckBox.IsEnabled = false;
                }
            }
            

            

            
        }
        ///////////////////////////////////////////////////////////////////////////
        ///INITIALIZE FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////
        private void InitializePropertiesStackPanel()
        {
            for(int i = propertiesStackPanel.Children.Count - 1; i > 0; i--)
            {
                propertiesStackPanel.Children.RemoveAt(i);
            }

            if (!commonQueries.GetPropertiesLocations(ref properties))
                return;

            for(int i = 0; i < properties.Count; i++)
            {
                CheckBox currentCheckBox = new CheckBox();
                currentCheckBox.Content = properties[i].property_id;
                currentCheckBox.Checked += OnCheckPropertyID;
                currentCheckBox.Unchecked += OnUncheckPropertyID;
                currentCheckBox.Tag = i;
                currentCheckBox.VerticalAlignment = VerticalAlignment.Center;

                if(viewAddCondition == REAL_STATE_MACROS.VIEW_CONDITION )
                {
                    currentCheckBox.IsEnabled = false;
                    if (owner.propertyList.Exists(s1 => s1.property_serial == properties[i].property_serial))
                        currentCheckBox.IsChecked = true;
                }
                else if(addPropertyCondition == true)
                {
                    if (owner.propertyList.Exists(s1 => s1.property_serial == properties[i].property_serial))
                        currentCheckBox.IsChecked = true;
                }

                WrapPanel wrapPanel = new WrapPanel();
                wrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
                wrapPanel.VerticalAlignment = VerticalAlignment.Center;

                Label label = new Label();
                label.Content = "View";
                label.MouseEnter += OnMouseEnterViewLabel;
                label.MouseLeave += OnMouseLeaveLabel;
                label.PreviewMouseLeftButtonDown += OnClickViewLabel;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.Style = (Style)FindResource("labelStyle");
                label.Tag = properties[i].property_serial;
                label.VerticalAlignment = VerticalAlignment.Center;

                if (viewAddCondition == REAL_STATE_MACROS.VIEW_CONDITION)
                {
                    if (owner.propertyList.Exists(s1 => s1.property_serial == properties[i].property_serial))
                    {
                        wrapPanel.Children.Add(currentCheckBox);
                        wrapPanel.Children.Add(label);
                        propertiesStackPanel.Children.Add(wrapPanel);
                    }
                }
                else
                {
                    wrapPanel.Children.Add(currentCheckBox);
                    wrapPanel.Children.Add(label);
                    propertiesStackPanel.Children.Add(wrapPanel);
                }

            }
        }

        private bool InitializeJobTitleCombo()
        {
            if (!commonQueries.GetJobTitles(ref jobTitles))
                return false;

            for(int i = 0; i < jobTitles.Count; i++)
            {
                jobTitleCombo.Items.Add(jobTitles[i].job_name);
            }

            return true;
        }

        private void FillOwnerPhonesList()
        {
            owner.owner_phones.Clear();
            for (int i = 0; i < ownerPhonesWrapPanel.Children.Count; i++)
            {
                WrapPanel wrapPanel = (WrapPanel)ownerPhonesWrapPanel.Children[i];
                TextBox phoneTextBox = (TextBox)wrapPanel.Children[1];
                if (phoneTextBox.Text != "")
                    owner.owner_phones.Add(phoneTextBox.Text);
            }
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
            currentLabel.Background = (Brush)brush.ConvertFrom("#FFFFFF");
        }


        ///////////////////////////////////////////////////////////////////////////
        ///CHECK/UNCHECK HANDLERS
        ///////////////////////////////////////////////////////////////////////////
        private void OnCheckPropertyID(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != REAL_STATE_MACROS.VIEW_CONDITION)
            {
                CheckBox currentCheckBox = (CheckBox)sender;
                ownerProperties.Add(properties[(int)currentCheckBox.Tag]);
            }
        }

        private void OnUncheckPropertyID(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != REAL_STATE_MACROS.VIEW_CONDITION)
            {
                CheckBox currentCheckBox = (CheckBox)sender;
                ownerProperties.RemoveAt(ownerProperties.FindIndex(s1 => s1.property_serial == (int)currentCheckBox.Tag + 1));
            }
        }

        private void OnCheckProperties(object sender, RoutedEventArgs e)
        {
            InitializePropertiesStackPanel();
        }

        private void OnUncheckProperties(object sender, RoutedEventArgs e)
        {
            ownerProperties.Clear();

            for (int i = propertiesStackPanel.Children.Count - 1; i > 0; i--)
            {
                propertiesStackPanel.Children.RemoveAt(i);
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        ///BUTTON CLICKED HANDLERS
        ///////////////////////////////////////////////////////////////////////////

        private void OnClickAddPhoneButton(object sender, RoutedEventArgs e)
        {
            WrapPanel wrapPanel = new WrapPanel();

            Label label = new Label();
            label.Content = "Owner Phone";
            label.Style = (Style)FindResource("tableItemLabel");
            wrapPanel.Children.Add(label);

            TextBox textBox = new TextBox();
            textBox.Style = (Style)FindResource("miniTextBoxStyle");
            wrapPanel.Children.Add(textBox);

            ownerPhonesWrapPanel.Children.Add(wrapPanel);
        }

        private void OnClickViewLabel(object sender, MouseButtonEventArgs e)
        {
            Property property = new Property();
            Label currentLabel = (Label)sender;
            property.InitializeProperty((int)currentLabel.Tag);
            int viewAddCondition = REAL_STATE_MACROS.VIEW_CONDITION;

            //AddPropertyWindow addPropertyWindow = new AddPropertyWindow(ref loggedInUser, ref property, viewAddCondition);
            //addPropertyWindow.Show();
        }

        private void OnBtnClkSaveChanges(object sender, RoutedEventArgs e)
        {
            if (addPropertyCondition == false)
            {
                FillOwnerPhonesList();

                GetNewOwnerID();
                InsertIntoOwnersInfo();

                GetNewOwnerTelephoneID();
                InsertIntoOwnerMobiles();

                GetNewOwnerNotesID();
                InsertIntoOwnerNotes();

                InsertIntoOwnerProperties();
            }
            else
            {
                DeleteOwnerProperties();
                InsertIntoOwnerProperties();
            }

            this.Close();
        }

        ///////////////////////////////////////////////////////////////////////////
        ///TEXT SELECTION CHANGED HANDLERS
        ///////////////////////////////////////////////////////////////////////////

        private void OnTextChangeOwnerName(object sender, TextChangedEventArgs e)
        {
            owner.owner_name = ownerTextBox.Text;
        }

        private void OnTextChangedEmailTextBox(object sender, TextChangedEventArgs e)
        {
            owner.email = emailTextBox.Text;
        }

        private void OnTextChangedNotes(object sender, TextChangedEventArgs e)
        {
            if (notesTextBox.Text.Length <= 150)
                notes = notesTextBox.Text;
            notesTextBox.Text = notes;
            notesTextBox.Select(notesTextBox.Text.Length, 0);
            counterLabel.Content = 150 - notesTextBox.Text.Length;
        }

        private void OnSelChangedGender(object sender, SelectionChangedEventArgs e)
        {
            if (genderCombo.SelectedIndex != -1)
                owner.gender = genderCombo.SelectedItem.ToString();
        }

        private void OnSelChangedJobTitle(object sender, SelectionChangedEventArgs e)
        {
            if(jobTitleCombo.SelectedIndex != -1)
            {
                owner.job_title = jobTitles[jobTitleCombo.SelectedIndex].job_name;
                owner.job_title_id = jobTitles[jobTitleCombo.SelectedIndex].job_id;
            }   
            else
            {
                owner.job_title = "";
                owner.job_title_id = 0;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        ///SET FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////

        private void SetUpUIElementsForView()
        {
            ownerTextBox.Visibility = Visibility.Collapsed;
            emailTextBox.Visibility = Visibility.Collapsed;
            genderCombo.Visibility = Visibility.Collapsed;
            jobTitleCombo.Visibility = Visibility.Collapsed;
            ownerPhoneTextBox.Visibility = Visibility.Collapsed;
            notesTextBox.Visibility = Visibility.Collapsed;
            counterLabel.Visibility = Visibility.Collapsed;

            ownerNameLabel.Visibility = Visibility.Visible;
            emailLabel.Visibility = Visibility.Visible;
            genderLabel.Visibility = Visibility.Visible;
            jobTitleLabel.Visibility = Visibility.Visible;
            ownerPhoneLabel.Visibility = Visibility.Visible;
            notesLabel.Visibility = Visibility.Visible;

            addPhoneButton.IsEnabled = false;
            saveChangesButton.IsEnabled = false;
        }

        private void SetExtraPhonesForView()
        {
            for (int i = 1; i < owner.owner_phones.Count; i++)
            {
                Label label = new Label();
                label.Content = "Owner Phone";
                label.Style = (Style)FindResource("tableItemLabel");

                Label phoneLabel = new Label();
                phoneLabel.Content = owner.owner_phones[i];
                phoneLabel.Width = 120;
                phoneLabel.Style = (Style)FindResource("labelStyle");

                WrapPanel wrapPanel = new WrapPanel();
                wrapPanel.Children.Add(label);
                wrapPanel.Children.Add(phoneLabel);

                ownerPhonesWrapPanel.Children.Add(wrapPanel);
            }
        }

        private void SetExtraNotesForView()
        {
            for (int i = 1; i < owner.notes.Count; i++)
            {
                Label label = new Label();
                label.Content = "Notes";
                label.Style = (Style)FindResource("tableItemLabel");

                Label notesLabel = new Label();
                notesLabel.Content = owner.notes[i];
                notesLabel.Width = 120;
                notesLabel.Style = (Style)FindResource("labelStyle");

                WrapPanel wrapPanel = new WrapPanel();
                wrapPanel.Children.Add(label);
                wrapPanel.Children.Add(notesLabel);

                ownerPhonesWrapPanel.Children.Add(wrapPanel);
            }
        }

        private void SetLabelContentsForView()
        {
            ownerNameLabel.Content = owner.owner_name;
            emailLabel.Content = owner.email;
            genderLabel.Content = owner.gender;
            jobTitleLabel.Content = owner.job_title;
            ownerPhoneLabel.Content = owner.owner_phones[0];
            notesLabel.Content = owner.notes[0];
        }

        ///////////////////////////////////////////////////////////////////////////
        ///DATABASE RELATED FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////

        private bool InsertIntoOwnersInfo()
        {
            String query = "insert into address_inv_desktop.dbo.owners_info values(";
            String queryPart2 = ",getdate());";
            String comma = ",";
            String apostropheComma = "',";
            String commaApostrophe = ",'";
            String apostropheCommaApostrophe = "','";

            query += owner.owner_id;
            query += comma;
            query += "N'" + owner.owner_name;
            query += apostropheCommaApostrophe;
            query += owner.email;
            query += apostropheCommaApostrophe;
            query += owner.gender;
            query += apostropheComma;
            query += owner.job_title_id;
            query += comma;
            query += loggedInUser.GetEmployeeId();
            query += queryPart2;

            if (!sqlDatabase.InsertRows(query))
                return false;

            return true;
        }

        private bool InsertIntoOwnerProperties()
        {
            String query = string.Empty;
            String queryPart1 = "insert into address_inv_desktop.dbo.owners_properties values(";
            String queryPart2 = ",getdate());";
            String comma = ",";
            String apostropheComma = "',";
            String commaApostrophe = ",'";
            String apostropheCommaApostrophe = "','";

            for (int i = 0; i < ownerProperties.Count; i++)
            {
                query = string.Empty;
                query += queryPart1;
                query += owner.owner_id;
                query += comma;
                query += ownerProperties[i].property_serial;
                query += comma;
                query += loggedInUser.GetEmployeeId();
                query += queryPart2;

                if (!sqlDatabase.InsertRows(query))
                    return false;
            }

            return true;
        }

        private bool DeleteOwnerProperties()
        {
            String query = string.Empty;
            String queryPart1 = "delete from address_inv_desktop.dbo.owners_properties where owner_id = ";

            query += queryPart1;
            query += owner.owner_id;

            if (!sqlDatabase.InsertRows(query))
                return false;


            return true;
        }

        private bool InsertIntoOwnerMobiles()
        {
            String query = string.Empty;
            String queryPart1 = "insert into address_inv_desktop.dbo.owner_mobile values(";
            String queryPart2 = "',getdate());";
            String comma = ",";
            String apostropheComma = "',";
            String commaApostrophe = ",'";
            String apostropheCommaApostrophe = "','";

            for (int i = 0; i < owner.owner_phones.Count; i++)
            {
                query = string.Empty;
                query += queryPart1;
                query += owner.owner_id;
                query += comma;
                query += phoneId;
                query += commaApostrophe;
                query += owner.owner_phones[i];
                query += queryPart2;

                if (!sqlDatabase.InsertRows(query))
                    return false;

                phoneId++;
            }

            return true;
        }

        private bool InsertIntoOwnerNotes()
        {
            String query = string.Empty;
            String queryPart1 = "insert into address_inv_desktop.dbo.owner_notes values(";
            String queryPart2 = "',getdate());";
            String comma = ",";
            String apostropheComma = "',";
            String commaApostrophe = ",'";
            String apostropheCommaApostrophe = "','";


            query = string.Empty;
            query += queryPart1;
            query += owner.owner_id;
            query += comma;
            query += notesId;
            query += commaApostrophe;
            query += notes;
            query += queryPart2;

            if (!sqlDatabase.InsertRows(query))
                return false;



            return true;
        }

        private bool GetNewOwnerID()
        {
            string query = @"select max(owner_id)
            from address_inv_desktop.dbo.owners_info";

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT sqlColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            sqlColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(query, sqlColumns))
                return false;

            owner.owner_id = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }

        private bool GetNewOwnerTelephoneID()
        {
            string query = @"select max(telephone_id)
            from address_inv_desktop.dbo.owner_mobile where owner_id =";

            query += owner.owner_id;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT sqlColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            sqlColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(query, sqlColumns))
                return false;

            phoneId = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }

        private bool GetNewOwnerNotesID()
        {
            string query = @"select max(note_id)
            from address_inv_desktop.dbo.owner_notes where owner_id =";

            query += owner.owner_id;

            BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT sqlColumns = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
            sqlColumns.sql_int = 1;

            if (!sqlDatabase.GetRows(query, sqlColumns))
                return false;

            notesId = sqlDatabase.rows[0].sql_int[0] + 1;

            return true;
        }


    }
}
