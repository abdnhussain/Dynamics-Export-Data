using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Query;

namespace Retrieve_Record
{
    class Program
    {
        static IOrganizationService _service;
        private static void ConnectToMSCRM(string UserName, string Password, string SoapOrgServiceUri)
        {
            try
            {
                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = UserName;
                credentials.UserName.Password = Password;
                Uri serviceUri = new Uri(SoapOrgServiceUri);
                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                _service = (IOrganizationService)proxy;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
            }
        }

        private static EntityCollection GetEntityCollection(IOrganizationService service, string entityName, string attributeName, string attributeValue, ColumnSet cols)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = cols,
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = attributeName,
                            Operator = ConditionOperator.Equal,
                            Values = { attributeValue }
                        }
                    }
                }
            };
            return service.RetrieveMultiple(query);
        }

        static void Main(string[] args)
        {
            EntityCollection ec = null;
            ConnectToMSCRM("abs@walksd365.onmicrosoft.com", " Swim.135", "https://walksd365.api.crm.dynamics.com/XRMServices/2011/Organization.svc");
            Guid userid = ((WhoAmIResponse)_service.Execute(new WhoAmIRequest())).UserId;

            if (userid == Guid.Empty)
            {
                return; //Check for CRM Connection Establishment. If Not return, other wise will proceed to next step
            }
            
            ec = GetEntityCollection(service, "Contact", "fullname", "Dan Hij", new ColumnSet("firstname", "lastname"));

                Console.WriteLine(ec.Entities.Count);

            foreach (var item in ec.Entities)
            {
                //lname
                if (item.Attributes.Contains("firstname"))
                { //Check for fullname value exists or not in Entity Collection
                    output += "Full Name : " + item.Attributes["firstname"] + "\n";
                    Console.WriteLine(output);
                    Console.ReadKey();
                }
                //fname
                if (item.Attributes.Contains("lastname"))
                { //Check for parentcustomerid exists or not in Entity Collection
                    output += "Company : " + item.Attributes["lastname"] + "\n";
                    Console.WriteLine(output);
                    Console.ReadKey();

                }
            }
        }
    }
}