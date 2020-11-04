using System;
using System.Configuration;
using System.Net;
using System.Text;
using AcousticConnections.DTOs;
using AcousticConnections.Enums;
using AcousticConnections.Interfaces;
using AcousticConnections.Services;
using Newtonsoft.Json;

namespace _DELETE_AcousticLibraryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("####### Starting test ##########");

            var myAcousticConfiguration = new MyAcousticConfiguration();
            var myAcousticLogger = new MyAcousticLogger();
            var myStorageRepository = new MyStorageRepository();
            var myJsonConverter = new MyJsonConverter();

            var acousticRestService = new AcousticRestService(myAcousticConfiguration, myAcousticLogger,
                myStorageRepository, myJsonConverter);

            var exampleApiCall = GetSentMailingsForOrg(1);
            var response = acousticRestService.PostAndReturnResponse(exampleApiCall, ApiCommand.GetSendMailingsForOrg);
            Console.WriteLine(response);

            Console.WriteLine("####### Finishing test ##########");
            Console.ReadLine();
        }

        public static string GetSentMailingsForOrg(int daysFromCurrentDate)
        {
            daysFromCurrentDate = daysFromCurrentDate * (-1);
            var sb = new StringBuilder();
            sb.Append("&xml=");
            sb.Append("<Envelope><Body> <GetSentMailingsForOrg>");
            sb.Append("<SHARED/>");
            sb.Append("<EXCLUDE_ZERO_SENT/>");
            sb.Append("<EXCLUDE_TEST_MAILINGS/>");
            
            sb.Append(" <INCLUDE_TAGS/> <DATE_START>");
            sb.Append(DateTime.Now.AddDays(daysFromCurrentDate).Month + "/" +
                      DateTime.Now.AddDays(daysFromCurrentDate).Day + "/" +
                      DateTime.Now.AddDays(daysFromCurrentDate).Year);
            sb.Append(" 00:00:00</DATE_START> <DATE_END>" + DateTime.Now.AddDays(daysFromCurrentDate).Month + "/" +
                      DateTime.Now.AddDays(daysFromCurrentDate).Day + "/" +
                      DateTime.Now.AddDays(daysFromCurrentDate).Year +
                      " 23:59:59</DATE_END> </GetSentMailingsForOrg> </Body></Envelope>");

            return sb.ToString();
        }
    }

    class MyJsonConverter : IJsonConverter
    {
        public AccessTokenReply DeserializeObject<T>(string unserializedString)
        {
            return JsonConvert.DeserializeObject<AccessTokenReply>(unserializedString);
        }
    }

    class MyStorageRepository : IAcousticStorage
    {
        public void InsertApiCallLog(string sessionId, string postData, string responseFromServer, ApiCommand apiCommand,
            int applicationId)
        {
            Console.WriteLine("Inserting API call to log...");
        }

        public void StoreAccessToken(DateTime? accessTokenExpirationDate, int expiresIn, string accessTokenValue, int organizationId)
        {
            Console.WriteLine("Storing access token to some persistence...");
        }

        public AccessToken GetAccessToken(int organizationId, int applicationId)
        {
            return new AccessToken
                {ExpirationDate = DateTime.Now.AddMinutes(5), Value = "xxxxxxxyyyyyyyyzzzzzznnnnnnnnn"};
        }
    }

    class MyAcousticLogger : IAcousticLogger
    {
        public void Fatal(string message)
        {
            Console.WriteLine("FATAL: "+message);
        }

        public void Error(string message)
        {
            Console.WriteLine("ERROR: "+message);
        }

        public void Fatal(string message, WebException webException)
        {
            Console.WriteLine("FATAL: "+message);
        }

        public void Fatal(string message, Exception exception)
        {
            Console.WriteLine("FATAL: "+message);
        }
    }

    class MyAcousticConfiguration : IAcousticConfiguration
    {
        public int OrganizationId => Convert.ToInt32(ConfigurationManager.AppSettings["OrganizationId"]);
        public int ApplicationId => Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationId"]);
        public string RequestUrl => ConfigurationManager.AppSettings["RequestUrl"];
        public string LoginRequestUrl => ConfigurationManager.AppSettings["LoginRequestUrl"];
        public string ClientId => ConfigurationManager.AppSettings["ClientId"];
        public string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];
        public string RefreshToken => ConfigurationManager.AppSettings["RefreshToken"];
    }
}
