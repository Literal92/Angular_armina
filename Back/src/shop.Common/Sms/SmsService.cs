using AlmasSms;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Common.Sms
{
    public static class SmsService
    {

        public static bool SendSms(string mobile, string message)
        {
            long[] mids;
            var ws = new AlmasSmsSoapClient(AlmasSmsSoapClient.EndpointConfiguration.AlmasSmsSoap);


            var result1 = ws.Send1SmsAsync(new Send1SmsRequest
            {
                message = message,
                mobile = mobile,
                pUsername = "nbwa15556",
                pPassword = "F@rhang! 1001"
            });

            if (result1.Result.Send1SmsResult < 0)
            {
                //litResult.Text = "";
                //foreach (long item in mids)
                //{
                //    if (item > 1000)
                //    {
                //        litResult.Text = litResult.Text + "Message Id:" + item + "<br/>";
                //        TXT_status.Text = item.ToString();
                //    }
                //    else
                //        litResult.Text = litResult.Text + "Error:" + messages.MAGFA_errors(item) + "<br/>";
                //}
            }
            else
            {
                //litResult.Text = messages.METHOD_errors(result);
            }

            return true;
        }
    }
}
