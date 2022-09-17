using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tea;
using Tea.Utils;

namespace AlibabaCloud.SDK.SMS
{
    public class SMSTool
    {
        public static AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config();
            config.AccessKeyId = accessKeyId;
            config.AccessKeySecret = accessKeySecret;
            config.Endpoint = "dysmsapi.aliyuncs.com";
            return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
        }

        public static void Main(string[] args)
        {
            AlibabaCloud.SDK.Dysmsapi20170525.Client client = CreateClient("LTAI5t9fCtK1zKPtuFKuHGyy", "yrqV2feDyemScJFcmfXZ71hQnVOpkX");
            AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest sendReq = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest
            {
                PhoneNumbers = args[0],
                SignName = args[1],
                TemplateCode = args[2],
                TemplateParam = "{\"name\":\"" + args[3] + "\"}",
                //PhoneNumbers = "18013031202",
                //SignName = "小盒子科技",
                //TemplateCode = "SMS_251132221",


            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                client.SendSmsWithOptions(sendReq, runtime);
                Console.WriteLine("send sms success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("send sms error:" + ex.Message);
            }
        }
    }
}
