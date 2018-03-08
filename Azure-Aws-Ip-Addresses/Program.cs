using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureAndAwsIpAddresses
{
    /// <summary>
    /// SOURCE DATA FILES:
    /// Azure IP addresses: https://www.microsoft.com/en-nz/download/details.aspx?id=41653
    /// AWS IP addresses: https://ip-ranges.amazonaws.com/ip-ranges.json
    /// 
    /// INSTRUCTIONS:
    /// 1. Grab both those files and put them into the bin/Debug of this solution
    /// 2. Rename the Azure one to: Azure-Public-Ips.xml
    /// 3. In the Azure one, move USWEST and USWEST2 and JAPAN ones at the top
    /// 4. Run this, and the files will be created in the bin/Debug folder: Azure-Ip-List.txt, Aws-Ip-List.txt
    /// 5. Use the output and put the content in the config settings in the Wip solution: MicrosoftIpAddresses and AwsEc2IpAddresses
    /// </summary>
    internal class Program
    {
        private static void Main()
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

            //AZURE
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

            //AWS
            using (var file = File.OpenText(@"ip-ranges.json")) 
            using (var reader = new JsonTextReader(file))
            {
                var rawJson = (JObject)JToken.ReadFrom(reader);
                var items = (JArray) rawJson["prefixes"];
                
                foreach (var item in items)
                {
                    if ( !((string)item["region"]).Contains("gov")

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