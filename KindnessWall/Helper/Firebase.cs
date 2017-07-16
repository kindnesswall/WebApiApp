using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace KindnessWall.Helper
{
    public static class Firebase
    {
        public static int AddToFirebaseGroup(string notification_key_name, string notification_key, string modelRegisterationId)
        {
            var data = new
            {
                operation = "add",
                notification_key_name = notification_key_name,
                notification_key = notification_key,
                registration_ids = new string[] { modelRegisterationId },
            };

            var result = CallFireBase("https://android.googleapis.com/gcm/notification", data);
            string error = result.error;
            if (string.IsNullOrEmpty(error))
            {
                notification_key = result.notification_key;
                if (notification_key != null) return 1;
            }

            if (error == "no valid registration ids") throw new Exception("no valid registration ids");
            if (!string.IsNullOrEmpty(error)) throw new Exception("unknown problem");

            return 0;

        }
        public static int RemoveFromFireBase(string notification_key_name, string notification_key, string[] oldRegisterationId)
        {
            var data = new
            {
                operation = "remove",
                notification_key_name = notification_key_name,
                notification_key = notification_key,
                registration_ids = oldRegisterationId,
            };

            var result = CallFireBase("https://android.googleapis.com/gcm/notification", data);
            string error = result.error;
            if (string.IsNullOrEmpty(error))
            {
                notification_key = result.notification_key;
                if (notification_key != null) return 1;
            }

            if (error == "notification_key not found") throw new Exception("notification_key not found");
            if (error == "no valid registration ids") throw new Exception("no valid registration ids");
            if (!string.IsNullOrEmpty(error)) throw new Exception("unknown problem");

            return 0;

        }
        public static Tuple<int, int> SendeMessageToGroup(string notification_key, MessageFireBase message)
        {

            var sendItem = new
            {
                to = notification_key,
                data = message
            };
            try
            {
                var result = CallFireBase("https://fcm.googleapis.com/fcm/send", sendItem);
                if (result.failure != null && result.success != null)
                    return new Tuple<int, int>((int)result.success, (int)result.failure);
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, 0);
            }

            return null;


        }
        public static string CreateFirebaseGroup(string notification_key_name, string modelRegisterationId)
        {

            var data = new
            {
                operation = "create",
                notification_key_name = notification_key_name,
                registration_ids = new string[] { modelRegisterationId },
            };


            var result = CallFireBase("https://android.googleapis.com/gcm/notification", data);
            string error = result.error;
            if (string.IsNullOrEmpty(error))
            {
                string notification_key = result.notification_key; //fill Or Null
                if (notification_key != null) return notification_key;
            }


            if (error == "notification_key already exists") throw new Exception("notification_key already exists");
            if (error == "no valid registration ids") throw new Exception("no valid registration ids");
            if (!string.IsNullOrEmpty(error)) throw new Exception("unknown problem");



            return null;
        }
        private static dynamic CallFireBase(string url, object data)
        {
            string sResponseFromServer = "";
            try
            {

                WebRequest tRequest = (HttpWebRequest)WebRequest.Create(url);
                string jsonss = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                tRequest.Method = "post";
                tRequest.Headers.Add("Authorization: key="); // add firebase key here
                tRequest.Headers.Add("project_id:000000000000"); // add firebase project id here
                tRequest.Headers.Add(string.Format("Sender: id={0}", 000000000000)); // add sender id here

                Byte[] byteArray = Encoding.UTF8.GetBytes(jsonss);
                tRequest.ContentLength = byteArray.Length;
                tRequest.ContentType = "application/json";

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    try
                    {
                        using (var tResponse = tRequest.GetResponse() as HttpWebResponse)
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    sResponseFromServer = tReader.ReadToEnd();

                                }
                            }
                        }
                    }
                    catch (WebException wex)
                    {

                        if (wex.Response != null && wex.Message.Contains("400"))
                        {

                            using (var errorResponse = (HttpWebResponse)wex.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    sResponseFromServer = reader.ReadToEnd();

                                }
                            }
                        }

                    }

                }
            }

            catch (Exception ex)
            {

                sResponseFromServer = (new { error = ex.Message }).ToString();
            }

            dynamic dynamicResult = JsonConvert.DeserializeObject<dynamic>(sResponseFromServer);
            return dynamicResult;
        }
        public static Tuple<int, int> SendeMessageToRegisterationKey(string registeration_key, MessageFireBase message)
        {

            var sendItem = new
            {
                to = registeration_key,
                data = message
            };
            try
            {
                var result = CallFireBase("https://fcm.googleapis.com/fcm/send", sendItem);

                if (result.failure != null && result.success != null)

                    return new Tuple<int, int>((int)result.success, (int)result.failure);

            }
            catch (Exception ex)
            {
                return Tuple.Create(0, 0);
            }

            return null;


        }
    }


    public class MessageFireBase
    {
        public object data { get; set; }

        public string userId { get; set; }

        public string method { get; set; }
    }



}
