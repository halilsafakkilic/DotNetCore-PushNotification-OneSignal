using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace OneSignal
{
    class Program
    {
        static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();

            Start();
        }

        static void Start()
        {
            Console.Write("Please enter target Player ID: ");

            var messageInput = Console.ReadLine();
            if (string.IsNullOrEmpty(messageInput))
            {
                Console.WriteLine("Are u disco or cola?"); // Only those who watch "Cem Yilmaz" can understand. :)

                Start();
            }

            SendNotification(messageInput);

            Start();
        }

        static void SendNotification(string playerID)
        {
            dynamic body = new JObject();
            body.app_id = configuration.GetSection("OneSignal")["AppId"];
            body.include_player_ids = new JArray(playerID);
            body.contents = new JObject();
            body.contents.en = "Hi, everyone! Time: " + DateTime.Now.ToString();
            body.headings = new JObject();
            body.headings.en = "OneSignal Notification Test";

            try
            {
                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("authorization", "Basic " + configuration.GetSection("OneSignal")["ApiKey"]);

                byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using var response = request.GetResponse() as HttpWebResponse;
                using var reader = new StreamReader(response.GetResponseStream());
                var responseContent = reader.ReadToEnd();

                Console.WriteLine("Response: " + responseContent);
            }
            catch (WebException e)
            {
                Console.WriteLine($"EXCEPTION: {e.Message}");
                Console.WriteLine(new StreamReader(e.Response.GetResponseStream()).ReadToEnd());
            } catch (Exception e)
            {
                Console.WriteLine($"EXCEPTION: {e.Message}");
            }
        }
    }
}