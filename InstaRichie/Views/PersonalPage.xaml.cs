// **************************************************************************
//Start Finance - An to manage your personal finances.
//Copyright(C) 2016  Jijo Bose

//Start Finance is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//Start Finance is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Start Finance.If not, see<http://www.gnu.org/licenses/>.
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;
using System.Globalization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalPage : Page
    {
        Personal p1 = new Personal();
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Personal>();

            /// Refresh Data
            var query = conn.Table<Personal>();
            PersonalInfoListView.ItemsSource = query.ToList();


            if (query.ToList().Count == 0)
            {
                EableInput();
            }
            else
            {
                DisableInput();

                tbxFirstName.Text = query.ToList()[0].FirstName;
                tbxLastName.Text = query.ToList()[0].LastName;
                tbxEmail.Text = query.ToList()[0].Email;
                tbxMobile.Text = query.ToList()[0].MobileNumber;
                cldDOB.Date = query.ToList()[0].DOB;
                //tbxID.Text = query.ToList()[0].PersonalID.ToString();
                p1 = (Personal)query.ToList()[0];
            }
        }

        private void AddData(object sender, RoutedEventArgs e)
        {
            SaveInformation();
        }

        private async void SaveInformation()
        {
            try
            {
                if (tbxFirstName.Text == "" || tbxLastName.Text == "" || tbxEmail.Text == "" || tbxMobile.Text == "")
                {
                    MessageDialog d = new MessageDialog("There are some empty boxes, please fill in");
                    await d.ShowAsync();
                }
                else if (cldDOB.Date == null)
                {
                    MessageDialog d = new MessageDialog("Birthday could not empty");
                    await d.ShowAsync();
                }
                else
                {
                    PersonalInsert();
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Value or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A Similar Asset Nane already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
            btnSave.Visibility = Visibility.Collapsed;
        }

        private async void DeleteAccout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string personSelection = ((Personal)PersonalInfoListView.SelectedItem).Email;
                if (personSelection == "")
                {
                    MessageDialog d = new MessageDialog("Please select the item");
                    await d.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Personal>();
                    var query1 = conn.Table<Personal>();
                    var query2 = conn.Query<Personal>("DELETE FROM Personal WHERE Email = '" + personSelection + "'");
                    PersonalInfoListView.ItemsSource = query1.ToList();
                }
                tbxFirstName.Text = "";
                tbxLastName.Text = "";
                tbxEmail.Text = "";
                tbxMobile.Text = "";
                cldDOB.Date = DateTime.Now;
                EableInput();
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void UpdateData(object sender, RoutedEventArgs e)
        {
            btnSave.Visibility = Visibility.Visible;
            btnAdd.Visibility = Visibility.Collapsed;
            EableInput();
        }

        private void SaveAccout_Click(object sender, RoutedEventArgs e)
        {
            //int id = int.Parse(tbxID.Text);
            CultureInfo provider = CultureInfo.InvariantCulture;
            string fName = tbxFirstName.Text;
            string lName = tbxLastName.Text;
            string mb = tbxMobile.Text;
            string em = tbxEmail.Text;
            // var dtt = DateTime.Parse(cldDOB.Date.Value.ToString(), "mm/dd/yyyy");

            var date = cldDOB.Date;
            DateTime dt = date.Value.DateTime;

            p1.FirstName = tbxFirstName.Text;
            p1.LastName = tbxLastName.Text;
            p1.DOB = dt;
            p1.Email = tbxEmail.Text;
            p1.MobileNumber = tbxMobile.Text;

            var qr = conn.Update(p1);
            var query = conn.Table<Personal>();



            PersonalInfoListView.ItemsSource = query.ToList();
            //tbxID.Text = query.ToList()[0].PersonalID.ToString();

            btnAdd.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Collapsed;
            DisableInput();
        }

        private void PersonalInsert()
        {
            conn.Insert(new Personal()
            {
                FirstName = tbxFirstName.Text,
                LastName = tbxLastName.Text,
                DOB = DateTime.Parse(cldDOB.Date.ToString()),
                Email = tbxEmail.Text,
                MobileNumber = tbxMobile.Text
            });
        }


        private void EableInput()
        {
            tbxFirstName.IsReadOnly = false;
            tbxLastName.IsReadOnly = false;
            tbxEmail.IsReadOnly = false;
            tbxMobile.IsReadOnly = false;
            cldDOB.IsEnabled = true;
        }

        private void DisableInput()
        {
            tbxFirstName.IsReadOnly = true;
            tbxLastName.IsReadOnly = true;
            tbxEmail.IsReadOnly = true;
            tbxMobile.IsReadOnly = true;
            cldDOB.IsEnabled = false;
        }
    }
}
