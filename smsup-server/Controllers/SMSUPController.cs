﻿using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace smsup_server.Controllers
{
    public class SMSUPController : ApiController
    {
        public HttpResponseMessage Post(dynamic jsonr)
        {
            string returnJson;
            string json = jsonr.ToString();
            JObject jObject = new JObject();
            try
            {
                if (!json.StartsWith("[") || !json.EndsWith("]"))
                {
                    json = "[" + json + "]";
                }
                JArray array = JArray.Parse(json);
                foreach (var item in array.Children())
                {
                    JObject jrow = JObject.Parse(item.ToString());
                    Data.Mongo.InsertUpMessage((string)jrow["phone_number"], (string)jrow["content"], (string)jrow["send_time"]);
                    Data.Mongo.UpdateReplyStatus((string)jrow["phone_number"], (string)jrow["send_time"]);
                }
                //returnJson = @"{""code"": 0, ""msg"": ""成功""}";
                jObject.Add("code", 0);
                jObject.Add("msg", "成功");
            }
            catch (Exception)
            {
                //returnJson = @"{""code"": 1,""msg"": ""失败""}";
                jObject.Add("code", 1);
                jObject.Add("msg", "失败");
            }

            returnJson = jObject.ToString().Replace("\r\n", "");
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(returnJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
    }
}
