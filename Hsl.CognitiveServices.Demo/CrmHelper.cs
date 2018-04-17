using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.ServiceModel;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Client;

namespace Hsl.CognitiveServices.Demo
{
    public static class CrmHelper
    {
        public static string  _organisationUrl{get;set;}

        public static OrganizationServiceProxy _serviceProxy;
        /// <summary>
        /// Retrieve Dynamics 365 Service Proxy using Dynamics 365 Connection String
        /// </summary>
        /// <param name="connectionString">Dynamics 365 Connection String</param>
        /// <returns>Service Proxy</returns>
        public static bool ConnectUsingConnectionString(string connectionString, out string connectionErrorMessage)
        {
            bool blnConnectionSuccess = false;
            connectionErrorMessage = string.Empty;
            try
            {
                using (CrmServiceClient crmSvc = new CrmServiceClient(connectionString))
                {
                    _serviceProxy = crmSvc.OrganizationServiceProxy;
                    blnConnectionSuccess = true;
                } 
            }
            catch(Exception ex)
            {
                connectionErrorMessage = ex.Message;
            }
            return blnConnectionSuccess;
        }
    }
}
