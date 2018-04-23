using System;
using System.Linq;
using System.Windows;
using System.Configuration;

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
                FillLoginDetails(ConnectionType.OnPremise);
            }
            else
            {
                gridOnLineLogin.Visibility = Visibility.Visible;
                FillLoginDetails(ConnectionType.Online);
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
                PBOnlineLogin.Visibility = Visibility.Visible;
                string strAuthType = "Office365";
                string strServerUrl = txtServerOnline.Text;

                AppendOrgnaisationName(ref strServerUrl);
                string strUserName = txtUserNameOnline.Text;
                string strPassword = txtPasswordOnline.Password.ToString();
                string strConnectionString = $"AuthType={strAuthType};Url={strServerUrl};Username={strUserName};Password={strPassword}";
                string strConStringKeyValue = ConfigurationManager.AppSettings["DynamicsConnectionStringOnline"];
                if(strConStringKeyValue!= strConnectionString)
                {
                    UpdateConnectionStringValue(strConnectionString, ConnectionType.Online);
                }
                string strErrorMessage;
                if (CrmHelper.ConnectUsingConnectionString(strConnectionString, out strErrorMessage))
                {
                    var parentWindow = Window.GetWindow(this) as MainWindow;
                    parentWindow.dockPanel.Children.Clear();
                    FaceIdentify cntrlFaceIdentify = new FaceIdentify();
                    cntrlFaceIdentify.CloseInitiated+= new Close(parentWindow.ClosePanel);
                    parentWindow.dockPanel.Children.Add(cntrlFaceIdentify);
                    parentWindow.dockPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Unable to login :" + "\r\n" + strErrorMessage);
                }
            }
            PBOnlineLogin.Visibility = Visibility.Hidden;
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
                PBOnpremiseLogin.Visibility = Visibility.Visible;
                string strAuthType = cmbAuthType.SelectedValue.ToString();
                string strServerUrl = txtServerOnPremise.Text;
                AppendOrgnaisationName(ref strServerUrl);
                string strDomain = txtDomainOnPremise.Text;
                string strUserName = txtUserNameOnPremise.Text;
                string strPassword = txtPasswordOnPremise.Password.ToString();
                string strConnectionString = $"AuthType={strAuthType};Url={strServerUrl};Domain={strDomain};Username={strUserName};Password={strPassword}";
                string strConStringKeyValue = ConfigurationManager.AppSettings["DynamicsConnectionStringOnPremise"];
                if (strConStringKeyValue != strConnectionString)
                {
                    UpdateConnectionStringValue(strConnectionString, ConnectionType.OnPremise);
                }
                string strErrorMessage;
                if (CrmHelper.ConnectUsingConnectionString(strConnectionString, out strErrorMessage))
                {
                    var parentWindow = Window.GetWindow(this) as MainWindow;
                    parentWindow.dockPanel.Children.Clear();
                    FaceIdentify cntrlFaceIdentify = new FaceIdentify();
                    cntrlFaceIdentify.CloseInitiated += new Close(parentWindow.ClosePanel);
                    parentWindow.dockPanel.Children.Add(cntrlFaceIdentify);
                    parentWindow.dockPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Unable to login :" + "\r\n" + strErrorMessage);
                }
            }
            PBOnpremiseLogin.Visibility = Visibility.Hidden;
        }
        private void UpdateConnectionStringValue(string strConnectionString, ConnectionType typeCon)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (typeCon.Equals(ConnectionType.Online))
                {
                    config.AppSettings.Settings["DynamicsConnectionStringOnline"].Value = strConnectionString;
                }
                if (typeCon.Equals(ConnectionType.OnPremise))
                {
                    config.AppSettings.Settings["DynamicsConnectionStringOnPremise"].Value = strConnectionString;
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillLoginDetails(ConnectionType typeCon)
        {
            try
            {
                string strConnectionString;
                if(typeCon.Equals(ConnectionType.Online))
                {
                    strConnectionString = ConfigurationManager.AppSettings["DynamicsConnectionStringOnline"];
                    if(string.IsNullOrEmpty(strConnectionString))
                    {
                        return;
                    }
                    string[] strParams = strConnectionString.Split(';');
                    foreach (string strParam in strParams)
                    {
                        string strKey = strParam.Split('=')[0];
                        string strValue = strParam.Split('=')[1];
                        if (strKey.Equals("Url"))
                        {
                            if(strValue.LastIndexOf('/')!=0)
                            {
                                txtServerOnline.Text = strValue.Substring(0, strValue.LastIndexOf('/'));
                            }else
                            {
                                txtServerOnline.Text = strValue;
                            }
                        }else
                        {
                            if (strKey.Equals("Username"))
                            {
                                txtUserNameOnline.Text = strValue;
                            }else
                            {
                                if (strKey.Equals("Password"))
                                {
                                    txtPasswordOnline.Password = strValue;
                                }
                            }
                        }
                    }
                }
                if (typeCon.Equals(ConnectionType.OnPremise))
                {
                    strConnectionString = ConfigurationManager.AppSettings["DynamicsConnectionStringOnPremise"];
                    if (string.IsNullOrEmpty(strConnectionString))
                    {
                        return;
                    }
                    string[] strParams = strConnectionString.Split(';');
                    foreach (string strParam in strParams)
                    {
                        string strKey = strParam.Split('=')[0];
                        string strValue = strParam.Split('=')[1];
                        strValue = strValue.Substring(strValue.IndexOf('{') + 1, strValue.LastIndexOf('}') - 1);
                        if (strKey.Equals("Url"))
                        {
                            if (strValue.LastIndexOf('/') != 0)
                            {
                                txtServerOnPremise.Text = strValue.Substring(0, strValue.LastIndexOf('/'));
                            }else
                            {
                                txtServerOnPremise.Text = strValue;
                            }
                        }
                        else
                        {
                            if (strKey.Equals("Username"))
                            {
                                txtUserNameOnPremise.Text = strValue;
                            }
                            else
                            {
                                if (strKey.Equals("Password"))
                                {
                                    txtPasswordOnPremise.Password = strValue;
                                }else
                                {
                                    if(strKey.Equals("Domain"))
                                    {
                                        txtDomainOnPremise.Text = strValue;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {

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
                CrmHelper._organisationUrl = serverUrl;
                serverUrl += serverUrl.Substring(8).Split('.').First();
            }
            else
            {
                if (serverUrl.Contains("http://"))
                {
                    CrmHelper._organisationUrl = serverUrl;
                    serverUrl += serverUrl.Substring(7).Split('.').First();
                }
                else
                {
                    serverUrl = "https://" + serverUrl + "/";
                    CrmHelper._organisationUrl = serverUrl;
                    serverUrl += serverUrl.Substring(8).Split('.').First();
                }
            }
        }
        #endregion
    }
    public enum ConnectionType
    {
        Online=0,
        OnPremise=1
    }
}
