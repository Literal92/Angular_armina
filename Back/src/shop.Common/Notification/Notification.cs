using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace shop.Common.Notification
{
    public static class Notification
    {
        private static string serverKey = "AAAA8fF1dA0:APA91bGbc5I7VmXoi-t5OmpbnP3t_Lc1Kkn6CBWi19qFmI4N-NI0vaejlKnOhMnbzaXJb4zXqUoYGrhD0jfsF2hBAjc02n0so5U11bcqeM1tdya4C6kypqTV5VzFEM43hrAUby7ZgRad";
        private static string senderId = "1039138124813";
        private static string webAddr = "https://fcm.googleapis.com/fcm/send";

        public static string Send(List<string> DeviceToken, string title, string msg, string id,string kind)
        {
            var result = "-1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            httpWebRequest.Method = "POST";
            var payload = new
            {
                to = (DeviceToken.Any() && DeviceToken.Count==1)? DeviceToken.FirstOrDefault():null,
                registration_ids= (DeviceToken.Any() && DeviceToken.Count > 1) ? DeviceToken : null,
                priority = "high",
                content_available = true,
                data = new
                {
                    body = msg,
                    title = title,
                    id = id,
                    kind =kind
                },
            };
            //  var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                string json = JsonConvert.SerializeObject(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }


        public static string CheckNull(this string str)
        {

            if (str==null ||string.IsNullOrEmpty(str)||string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            else
            {
                return str;
            }
        }


        public static object SetNullClass(this object response)
        {
            foreach (PropertyInfo propertyInfo in response.GetType().GetProperties())
            {


                if (propertyInfo.PropertyType == typeof(string))
                {
                    var propertyValue = propertyInfo.GetValue(response);
                    if (propertyValue == null)
                    {
                        propertyInfo.SetValue(response, "");
                    }
                }

                //if (propertyInfo.PropertyType.IsClass)
                //{
                //    NewMethod(propertyInfo.PropertyType);
                //}


            }
            return response;
        }
    }
}
