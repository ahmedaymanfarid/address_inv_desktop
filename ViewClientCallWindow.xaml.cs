﻿using System;
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

namespace address_inv_desktop
{
    /// <summary>
    /// Interaction logic for ViewClientCallWindow.xaml
    /// </summary>
    public partial class ViewClientCallWindow : Window
    {
        ClientCall clientCall;
        public ViewClientCallWindow(ref ClientCall mClientCall)
        {
            InitializeComponent();

            clientCall = mClientCall;
            companyNameTextBox.IsEnabled = false;
            companyBranchTextBox.IsEnabled = false;

            contactNameTextBox.IsEnabled = false;

            CallDateTextBox.IsEnabled = false;
            CallPurposeTextBox.IsEnabled = false;
            CallResultTextBox.IsEnabled = false;

            additionalDescriptionTextBox.IsEnabled = false;
            contactNameTextBox.Text = clientCall.GetLeadName();

            CallDateTextBox.Text = clientCall.GetCallDate().ToString();

            additionalDescriptionTextBox.Text = clientCall.GetCallNotes();
        }
    }
}
