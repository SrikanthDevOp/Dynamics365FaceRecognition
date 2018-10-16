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
using System.Net;

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
        public static async Task<bool> ConnectUsingConnectionStringAsync(string connectionString, Tuple<string> errorMessage)
        {
            bool blnConnectionSuccess = false;
            string connectionErrorMessage = string.Empty;
            errorMessage = new Tuple<string>(string.Empty);
            try
            {
                blnConnectionSuccess = await MakeConnectionRequestAsync(connectionString);
                if (!blnConnectionSuccess)
                {
                    errorMessage = new Tuple<string>("Please try again.");
                }
            }
            catch (Exception ex)
            {
                errorMessage = new Tuple<string>(Task.FromException(ex).Exception.Message);
            }
            return blnConnectionSuccess;
        }

        public static Task<bool> MakeConnectionRequestAsync(string connectionString)
        {
          Task<bool> taskConnection= Task.Run(()=>
            {
                bool blnConnectionSuccess = false;
                try
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    using (CrmServiceClient crmSvc = new CrmServiceClient(connectionString))
                    {
                        _serviceProxy = crmSvc.OrganizationServiceProxy;
                        if (_serviceProxy != null)
                        {
                            blnConnectionSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
                return  blnConnectionSuccess;
            });
            return taskConnection;
        }
    }
}
