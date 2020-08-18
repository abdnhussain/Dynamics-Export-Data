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
                Guid userid = ((WhoAmIResponse)_service.Execute(new WhoAmIRequest())).UserId;
                if (userid == Guid.Empty)
                {
                    return; //Check for CRM Connection Establishment. If Not return, other wise will proceed to next step
                }
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
            Console.WriteLine("Please enter your Dynamics Username:  "); string mscrmUserName = Console.ReadLine();
            Console.WriteLine("Please enter your Dynamics Password:  "); string mscrmPassWord = Console.ReadLine();

            ConnectToMSCRM(mscrmUserName, mscrmPassWord, "https://nttdbss1.api.crm11.dynamics.com/XRMServices/2011/Organization.svc");

            Console.WriteLine("Please enter the Entity that you would like to find the records for:  "); string EntityName = Console.ReadLine();
            EntityName.ToLower();

            Console.WriteLine("Please enter the first name of the person who you would like to find the records for:  ");  string userInputFirstName = Console.ReadLine();
            Console.WriteLine("Please enter the last name of the person who you would like to find the records for:  "); string userInputLastName = Console.ReadLine();
            string userInputFullName = userInputFirstName + " " + userInputLastName;

            Console.WriteLine("Please enter the field name that you would like to find info for:  "); string userFieldName = Console.ReadLine();

            EntityCollection ec = null;
            ec = GetEntityCollection(_service, EntityName, userFieldName, userInputFullName, new ColumnSet("firstname", "lastname"));

            string output = string.Empty;

            foreach (var item in ec.Entities)
            {
                //fname
                if (item.Attributes.Contains("firstname"))
                { //Check for fname value exists or not in Entity Collection
                    output += "First Name : " + item.Attributes["firstname"] + "\n";
                    Console.WriteLine(output);
                }
                //lname
                if (item.Attributes.Contains("lastname"))
                { //Check for lname exists or not in Entity Collection
                    output += "Last Name : " + item.Attributes["lastname"] + "\n";
                    Console.WriteLine(output);
                }
            }
        }
    }
}