using System;
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AWS_IP_address_ACL
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("Azure-Ip-List.txt"))
            {
                File.Delete("Azure-Ip-List.txt");
            }

            if (File.Exists("Aws-Ip-List.txt"))
            {
                File.Delete("Aws-Ip-List.txt");
            }

            var awsIps = new StringBuilder();
            var azureIps = new StringBuilder();

            using (var xmlReader = new XmlTextReader("Azure-Public-Ips.xml"))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "IpRange")
                    {
                        azureIps.AppendFormat("{0},", xmlReader.GetAttribute("Subnet"));
                    }
                }
            }

            using (var file = File.OpenText(@"ip-ranges.json")) 
            using (var reader = new JsonTextReader(file))
            {
                var rawJson = (JObject)JToken.ReadFrom(reader);

                var items = (JArray) rawJson["prefixes"];
                
                foreach (var item in items)
                {
                    if ( 
                        ( ((string)item["region"]).Equals("us-east-1") || ((string)item["region"]).Equals("us-west-2") || ((string)item["region"]).Equals("us-west-1") || ((string)item["region"]).Equals("eu-west-1") || ((string)item["region"]).Equals("ap-southeast-1") || ((string)item["region"]).Equals("ap-northeast-1") || ((string)item["region"]).Equals("ap-southeast-2") || ((string)item["region"]).Equals("sa-east-1"))
                        
                        &&
                        !((string)item["region"]).Contains("gov")

                        &&
                        !((string)item["region"]).Contains("eu")

                        &&

                        ((string)item["service"]).Equals("EC2"))
                    {
                        
                        awsIps.AppendFormat("{0},", (string)item["ip_prefix"]);
                    }
                }
            }

            File.AppendAllText(@"Aws-Ip-List.txt", awsIps.ToString());
            File.AppendAllText(@"Azure-Ip-List.txt", azureIps.ToString());
        }
    }
}