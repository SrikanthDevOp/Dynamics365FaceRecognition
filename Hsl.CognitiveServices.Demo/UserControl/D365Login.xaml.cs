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

namespace Hsl.CognitiveServices.Demo.UserControl
{
    /// <summary>
    /// Interaction logic for D365Login.xaml
    /// </summary>
    public partial class D365Login: ITabbed
    {
        public D365Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This event will be fired when user will click close button
        /// </summary>
        public event Close CloseInitiated;



        #region Events

        private void btnNextLogin_Click(object sender, RoutedEventArgs e)
        {
            if (rdoOnPremise.IsChecked.Equals(true))
            {
                gridOnPremiseLogin.Visibility = Visibility.Visible;
            }
            else
            {
                gridOnLineLogin.Visibility = Visibility.Visible;
            }
            btnNextLogin.Visibility = Visibility.Hidden;
            gridInstanceType.Visibility = Visibility.Hidden;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseInitiated?.Invoke(this, new EventArgs());
        }
        private void InstaceType_Checked(object sender, RoutedEventArgs e)
        {
            btnNextLogin.IsEnabled = true;
        }

        private void OnPremiseLoginBack_Click(object sender, RoutedEventArgs e)
        {
            gridOnPremiseLogin.Visibility = Visibility.Hidden;
            btnNextLogin.Visibility = Visibility.Visible;
            gridInstanceType.Visibility = Visibility.Visible;
        }
        private void OnlineLoginBack_Click(object sender, RoutedEventArgs e)
        {

            gridOnLineLogin.Visibility = Visibility.Hidden;
            btnNextLogin.Visibility = Visibility.Visible;
            gridInstanceType.Visibility = Visibility.Visible;
        }
        private void OnlineLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtServerOnline.Text.Equals(string.Empty)
               || txtUserNameOnline.Text.Equals(string.Empty)
               || txtPasswordOnline.Password.Equals(string.Empty))
            {
                MessageBox.Show("Unable to login, please provide the required details.");
            }
            else
            {
                string strAuthType = "Office365";
                string strServerUrl = txtServerOnline.Text;
                AppendOrgnaisationName(ref strServerUrl);
                string strUserName = txtUserNameOnline.Text;
                string strPassword = txtPasswordOnline.Password.ToString();
                string strConnectionString = $"AuthType={strAuthType};Url={strServerUrl}; Username={strUserName}; Password={strPassword}";
                string strErrorMessage;
                if (CrmHelper.ConnectUsingConnectionString(strConnectionString, out strErrorMessage))
                {

                }
                else
                {
                    MessageBox.Show("Unable to login :" + "\r\n" + strErrorMessage);
                }
            }
        }
        private void OnPremiseLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtServerOnPremise.Text.Equals(string.Empty)
                || txtDomainOnPremise.Text.Equals(string.Empty)
                || txtUserNameOnPremise.Text.Equals(string.Empty)
                || txtPasswordOnPremise.Password.Equals(string.Empty))
            {
                MessageBox.Show("Unable to login, please provide required details.");
            }
            else
            {
                string strAuthType = cmbAuthType.SelectedValue.ToString();
                string strServerUrl = txtServerOnPremise.Text;
                AppendOrgnaisationName(ref strServerUrl);
                string strDomain = txtDomainOnPremise.Text;
                string strUserName = txtUserNameOnPremise.Text;
                string strPassword = txtPasswordOnPremise.Password.ToString();
                string strConnectionString = $"AuthType={strAuthType};Url={strServerUrl}; Domain={strDomain}; Username={strUserName}; Password={strPassword}";
                string strErrorMessage;
                if (CrmHelper.ConnectUsingConnectionString(strConnectionString, out strErrorMessage))
                {

                }
                else
                {
                    MessageBox.Show("Unable to login :" + "\r\n" + strErrorMessage);
                }
            }
        }

        private void AppendOrgnaisationName(ref string serverUrl)
        {
            if (!serverUrl[serverUrl.Length - 1].Equals("/"))
            {
                serverUrl += "/";
            }
            if (serverUrl.Contains("https://"))
            {
                serverUrl += serverUrl.Substring(8).Split('.').First();
            }
            else
            {
                if (serverUrl.Contains("http://"))
                {
                    serverUrl += serverUrl.Substring(7).Split('.').First();
                }
                else
                {
                    serverUrl = "https://" + serverUrl + "/";
                    serverUrl += serverUrl.Substring(8).Split('.').First();
                }
            }
        }
    }
    #endregion
}
