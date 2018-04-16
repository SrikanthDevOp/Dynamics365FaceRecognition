using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Windows;


namespace Hsl.CognitiveServices.Demo.UserControl
{ 
   /// <summary>
    /// Interaction logic for FaceIdentify.xaml
    /// </summary>
    public partial class FaceIdentify :ITabbed
    {
        private string strSubscriptionKey = string.Empty;
        private string strEndpoint = string.Empty;
        public FaceIdentify()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This event will be fired when user will click close button
        /// </summary>
        public event Close CloseInitiated;

    #region Events
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseInitiated?.Invoke(this, new EventArgs());
        }
        private void BtnNextKeyManagement_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtSubscriptionKey.Text) || string.IsNullOrEmpty(txtEndpoint.Text))
            {

            }
            else
            {
                this.strSubscriptionKey = txtSubscriptionKey.Text;
                this.strEndpoint = txtEndpoint.Text;
            }
        }

        private void BtnTrain_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                List<Contact> lstContacts = new List<Contact>();
                // Get the Contacts Images from CRM.
                string strContactsImageQuery = @"<fetch mapping='logical'>
                          <entity name='contact'>
                            <attribute name='fullname'/>
                            <attribute name='contactid'/>
                            <attribute name='entityimage' />
                          </entity>
                        </fetch>";
                EntityCollection entColContacts = CrmHelper._serviceProxy.RetrieveMultiple(new FetchExpression(strContactsImageQuery));
                if(entColContacts==null || entColContacts.Entities.Count==0)
                {
                    return;
                }
                lstContacts = entColContacts.Entities
                    .Where(ent => (ent.Attributes.Contains("entityimage") && ent.Attributes["entityimage"] != null))
                    .Select(ent => new Contact
                     {
                         FullName = ent.Attributes["FullName"].ToString(),
                         ImageBytes = ent.Attributes["entityimage"] as byte[],
                         Id = ent.Attributes["contactid"].ToString()
                     }).ToList();


            }
            catch(Exception ex)
            {

            }


        }
        #endregion

        private async void TrainContact(string personGroupId, byte[] contactImgBytes)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string strEndpoint = this.strEndpoint;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.strSubscriptionKey);

            var uri = $"{strEndpoint}/persongroups/{personGroupId}/train?" + queryString;

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(contactImgBytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
    }

    public  class Contact
    {
        public string FullName { get; set; }
        public byte[] ImageBytes { get; set; }
        public string Id { get; set; }

    }

}
