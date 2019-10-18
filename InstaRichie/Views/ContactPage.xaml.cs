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
using SQLite.Net;
using StartFinance.Models;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        Contacts c1 = new Contacts();
        bool isUpdate = false;
        public ContactPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<Contacts>();            
        }

        public void Results()
        {
            conn.CreateTable<Contacts>();
            var query1 = conn.Table<Contacts>();
            ContactListView.ItemsSource = query1.ToList();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fNameTxtBox.Text == "" && lnameTxtBox.Text == "" && companyNameTxtBox.Text == "" && mobilePhoneTxtBox.Text == "")
                {
                    MessageDialog dialog = new MessageDialog("All name fields cannot be empty.");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new Contacts()
                    {
                        FirstName = fNameTxtBox.Text,
                        LastName = lnameTxtBox.Text,
                        CompanyName = companyNameTxtBox.Text,
                        MobilePhone = mobilePhoneTxtBox.Text
                    });
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Format Exception Caught, check that all fields are entered correctly.");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Format Exception Caught, check that all fields are entered correctly.");
                    await dialog.ShowAsync();
                }
                else
                {
                    //Nothing
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ContactSelection = ((Contacts)ContactListView.SelectedItem).ID.ToString();
                if (ContactSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("No item selected.");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Contacts>();
                    var query1 = conn.Table<Contacts>();
                    var query2 = conn.Query<Contacts>("DELETE FROM Contacts WHERE ID =" + ContactSelection);
                    isUpdate = true;
                    ContactListView.ItemsSource = query1.ToList();

                    saveAppBtn.IsEnabled = false;
                    addAppBtn.IsEnabled = true;
                    deleteAppBtn.IsEnabled = true;
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("No item selected.");
                await dialog.ShowAsync();
            }
        }
        private void saveAppBtn_Click(object sender, RoutedEventArgs e)
        {
            c1.FirstName = fNameTxtBox.Text;
            c1.LastName = lnameTxtBox.Text;
            c1.CompanyName = companyNameTxtBox.Text;
            c1.MobilePhone = mobilePhoneTxtBox.Text;

            isUpdate = true;

            var qr = conn.Update(c1);
            var query1 = conn.Table<Contacts>();

            ContactListView.ItemsSource = query1.ToList();

            saveAppBtn.IsEnabled = false;
            addAppBtn.IsEnabled = true;
            deleteAppBtn.IsEnabled = true;

            ContactListView.SelectedValue = -1;
        }

        private void ContactsListView_SelctionChanged(object sender, SelectionChangedEventArgs e)
        {
            saveAppBtn.IsEnabled = true;
            addAppBtn.IsEnabled = false;
            
            c1 = ContactListView.SelectedItem as Contacts;
            if (!isUpdate)
            {
                fNameTxtBox.Text = c1.FirstName;
                lnameTxtBox.Text = c1.LastName;
                companyNameTxtBox.Text = c1.CompanyName;
                mobilePhoneTxtBox.Text = c1.MobilePhone;
            }
            else
            {
                isUpdate = false;
            }
        }
    }
}
