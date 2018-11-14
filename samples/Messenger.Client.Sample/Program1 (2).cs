//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.IO;
//using System.Net;
//using System.Security.Cryptography;
//using System.Text;
//using Jose;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace ChatBotSample
//{
//    internal class Program
//    {
//        private static string botAccountName = "@atom.uat.bot";
//        private static string apiKey = "aOgaKQ3PVoYUDWjzWcJnrPuH2qmTriIt";
//        private static string secureKey = "HF4BpnXFFJhXjdIHFhpfzkeuZVumkc1NB7gAtrSWTMoJU8Y2D2xpFuwzJzLuKD3oV5TkQJb1DP9rCcXdcZVNbksq5mklcfzu2StOfc4y32eJTK6Nuwn1HwxTcSQWVCAB";
//        private static string chatKey = "a4171219646dc70a9cbc4e6999b86139b655625e255e75f65fb253b0dc1d26d4";
//        private static int chatKeyVer = 1;
//        private static int botAccountID = 100012;

//        private static void Main(string[] args)
//        {
//            //Test Encode Decode
//            LetsalkClient client = new LetsalkClient(apiKey, botAccountName, secureKey, chatKey, chatKeyVer, botAccountID);
//            LetsalkClient.ReplyMarkup replyMarkup = new LetsalkClient.ReplyMarkup();
//            replyMarkup.type = "button";
//            LetsalkClient.Action action1 = new LetsalkClient.Action();
//            action1.label = "Savings";
//            action1.type = "postback";
//            action1.data = "action=buy&itemid=123";
//            LetsalkClient.Action action2 = new LetsalkClient.Action();
//            action2.label = "Credit Cards";
//            action2.type = "postback";
//            action2.data = "action=buy&itemid=125";
//            LetsalkClient.Action action3 = new LetsalkClient.Action();
//            action3.label = "Google";
//            action3.type = "uri";
//            action3.uri = "https://www.google.com";

//            replyMarkup.actions = new LetsalkClient.Action[3];
//            replyMarkup.actions[0] = action1;
//            replyMarkup.actions[1] = action2;
//            replyMarkup.actions[2] = action3;
//            client.SendTextMessageAndReplymarkupToUserID("starix_test233", "This is Button Reply message.\nHave uri and postback two type.", replyMarkup);
//        }
//    }

//    internal class LetsalkClient
//    {
//        public struct Action
//        {
//            public string type;
//            public string label;
//            public string data;
//            public string uri;
//        }

//        public struct ReplyMarkup
//        {
//            public string type;
//            public LetsalkClient.Action[] actions;
//        }

//        public string botAccountName;
//        public string apiKey;
//        public string secureKey;
//        public string chatKey;
//        public int chatKeyVer;
//        public int botAccountID;

//        private static string botPushURL = "https://message.biatalk.cc/bot/push";

//        //Init
//        public LetsalkClient(string apiKey, string botAccountName, string secureKey, string chatKey, int chatKeyVer, int botAccountID)
//        {
//            this.apiKey = apiKey;
//            this.botAccountName = botAccountName;
//            this.secureKey = secureKey;
//            this.chatKey = chatKey;
//            this.chatKeyVer = chatKeyVer;
//            this.botAccountID = botAccountID;
//        }

//        //Decode payload string
//        public string decodePayloadString(string payloadString)
//        {
//            string result = "";
//            JObject payloadObj = JObject.Parse(payloadString);
//            if (payloadObj != null)
//            {
//                string hashContent = (string)(payloadObj.ContainsKey("hash_content") == true ? payloadObj.GetValue("hash_content") : null);
//                int msgType = (int)(payloadObj.ContainsKey("type") == true ? payloadObj.GetValue("type") : -1); ;
//                int hashType = (int)(payloadObj.ContainsKey("hash_type") == true ? payloadObj.GetValue("hash_type") : -1); ;
//                string hashContentKey = (string)(payloadObj.ContainsKey("hash_content_key") == true ? payloadObj.GetValue("hash_content_key") : null); ;

//                if (hashContent != null && hashContentKey != null && hashType != -1 && msgType != -1)
//                {
//                    if (hashType == 5)
//                    {
//                        //decode msg
//                        //Get Key
//                        byte[] key = SecureUtil.ConvertHexStringToByte(chatKey);
//                        //Get IV
//                        byte[] iv = SecureUtil.GetIV(hashContentKey);

//                        string plainText = SecureUtil.DecryptAESMessage(key, iv, hashContent);
//                        result = plainText != null ? plainText : "";
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("parm fail :" + payloadString);
//                }
//            }
//            else
//            {
//                Console.WriteLine("jason parse fail");
//            }

//            return result;
//        }

//        public void SendTextMessageAndReplymarkupToUserID(string userID, string msg, ReplyMarkup replyMarkup)
//        {
//            JArray jarrayObj = new JArray();

//            foreach (LetsalkClient.Action action in replyMarkup.actions)
//            {
//                Console.Write("!" + action.type);
//                if (action.type.Equals("postback"))
//                {
//                    JObject actionObject = new JObject(new JProperty("type", action.type), new JProperty("label", action.label), new JProperty("data", action.data));
//                    jarrayObj.Add(actionObject);
//                }
//                else if (action.type.Equals("uri"))
//                {
//                    JObject actionObject = new JObject(new JProperty("type", action.type), new JProperty("label", action.label), new JProperty("uri", action.uri));
//                    jarrayObj.Add(actionObject);
//                }
//            }
//            JObject replyObject = new JObject(
//                new JProperty("type", "button"),
//                new JProperty("actions", jarrayObj));

//            JObject msgObject = new JObject(
//                new JProperty("msg", msg),
//                new JProperty("reply_markup", replyObject));

//            string plainHashContent = msgObject.ToString(Formatting.Indented);
//            Console.WriteLine("plainHashContent:" + plainHashContent);

//            //Encode msg
//            //Get Key
//            byte[] key = SecureUtil.ConvertHexStringToByte(chatKey);

//            //Get IV
//            UTF8Encoding utf8Encoding = new UTF8Encoding();
//            string hashContentKey = SecureUtil.ByteToSHA1(utf8Encoding.GetBytes(plainHashContent));
//            byte[] iv = SecureUtil.GetIV(hashContentKey);
//            string hashContent = SecureUtil.EncryptAESMessage(key, iv, plainHashContent);

//            //create payload jsonstring
//            long currentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
//            JObject payloadObject = new JObject(
//                            new JProperty("id", 100),
//                            new JProperty("time", currentTime),
//                            new JProperty("user_id", this.botAccountName),
//                            new JProperty("from", 2),
//                            new JProperty("type", 60),
//                            new JProperty("hash_type", 5),
//                            new JProperty("hash_id", this.chatKeyVer),
//                            new JProperty("hash_content", hashContent),
//                            new JProperty("hash_content_key", hashContentKey),
//                            new JProperty("send_time", currentTime),
//                            new JProperty("duedate", currentTime + 10000000) //30 Day msg will be delete
//                            );
//            string payloadJsonString = payloadObject.ToString(Formatting.None);
//            Console.WriteLine("payload string:" + payloadJsonString);
//            string testDecodeString = decodePayloadString(payloadJsonString);
//            Console.WriteLine("test Decode:" + testDecodeString);

//            //Create jwt body string
//            string[] receiver = new string[1] { userID };
//            //string[] receiver = new string[1] { userID};
//            var bodyObject = new Dictionary<string, object>
//            {
//                { "exp", currentTime + 5000 },
//                { "iat", currentTime },
//                { "receiver", receiver},
//                { "payload", payloadJsonString}
//            };

//            string jwtBodySring = JWTUtil.EnCodeJwt(bodyObject, this.secureKey);
//            string testString = JWTUtil.DeCodeJwt(jwtBodySring, this.secureKey);
//            Console.WriteLine("test :" + testString);
//            //do bot/push API
//            Action<Exception, string> callback = (err, response) =>
//            {
//                if (err == null)
//                {
//                    try
//                    {
//                        Console.WriteLine(response);
//                    }
//                    catch (Exception e)
//                    {
//                        Console.WriteLine(e);
//                    }
//                }
//                else
//                {
//                    Console.WriteLine(err);
//                    //onFail?.Invoke(err);
//                }
//            };
//            String url = botPushURL + "/" + this.botAccountID;
//            NetUtil netUtil = new NetUtil();
//            netUtil.PostString(null, url, jwtBodySring, SecureUtil.EncodeBase64(this.apiKey), callback);
//        }

//        public void SendTextMessageToUserID(string userID, string msg)
//        {
//            JObject msgObject = new JObject(
//                new JProperty("msg", msg));

//            string plainHashContent = msgObject.ToString(Formatting.None);
//            Console.WriteLine("plainHashContent:" + plainHashContent);

//            //Encode msg
//            //Get Key
//            byte[] key = SecureUtil.ConvertHexStringToByte(chatKey);

//            //Get IV
//            UTF8Encoding utf8Encoding = new UTF8Encoding();
//            string hashContentKey = SecureUtil.ByteToSHA1(utf8Encoding.GetBytes(plainHashContent));
//            byte[] iv = SecureUtil.GetIV(hashContentKey);
//            string hashContent = SecureUtil.EncryptAESMessage(key, iv, plainHashContent);

//            //create payload jsonstring
//            long currentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
//            JObject payloadObject = new JObject(
//                            new JProperty("id", 100),
//                            new JProperty("time", currentTime),
//                            new JProperty("user_id", this.botAccountName),
//                            new JProperty("from", 2),
//                            new JProperty("type", 60),
//                            new JProperty("hash_type", 5),
//                            new JProperty("hash_id", this.chatKeyVer),
//                            new JProperty("hash_content", hashContent),
//                            new JProperty("hash_content_key", hashContentKey),
//                            new JProperty("send_time", currentTime),
//                            new JProperty("duedate", currentTime + 10000000) //30 Day msg will be delete
//                            );
//            string payloadJsonString = payloadObject.ToString(Formatting.None);
//            Console.WriteLine("payload string:" + payloadJsonString);
//            string testDecodeString = decodePayloadString(payloadJsonString);
//            Console.WriteLine("test Decode:" + testDecodeString);

//            //Create jwt body string
//            string[] receiver = new string[1] { userID };
//            //string[] receiver = new string[1] { userID};
//            var bodyObject = new Dictionary<string, object>
//            {
//                { "exp", currentTime + 5000 },
//                { "iat", currentTime },
//                { "receiver", receiver},
//                { "payload", payloadJsonString}
//            };

//            string jwtBodySring = JWTUtil.EnCodeJwt(bodyObject, this.secureKey);
//            string testString = JWTUtil.DeCodeJwt(jwtBodySring, this.secureKey);
//            Console.WriteLine("test :" + testString);
//            //do bot/push API
//            Action<Exception, string> callback = (err, response) =>
//            {
//                if (err == null)
//                {
//                    try
//                    {
//                        Console.WriteLine(response);
//                    }
//                    catch (Exception e)
//                    {
//                        Console.WriteLine(e);
//                    }
//                }
//                else
//                {
//                    Console.WriteLine(err);
//                    //onFail?.Invoke(err);
//                }
//            };
//            String url = botPushURL + "/" + this.botAccountID;
//            NetUtil netUtil = new NetUtil();
//            netUtil.PostString(null, url, jwtBodySring, SecureUtil.EncodeBase64(this.apiKey), callback);
//        }

//        public void encodeJsonMessage(String jsonString, String key, String iv)
//        {
//            byte[] keyByte = SecureUtil.ConvertHexStringToByte(key);
//            byte[] ivByte = SecureUtil.GetIV(iv);
//            string hashContent = SecureUtil.EncryptAESMessage(keyByte, ivByte, jsonString);
//            Console.WriteLine(hashContent);
//        }
//    }

//    internal class JWTUtil
//    {
//        public static string EnCodeJwt(Dictionary<string, object> payload, string secureKey)
//        {
//            byte[] bytes = System.Text.Encoding.GetEncoding("utf-8").GetBytes(secureKey);
//            string token = Jose.JWT.Encode(payload, bytes, JwsAlgorithm.HS512);
//            Console.WriteLine(token);
//            return token;
//        }

//        public static string DeCodeJwt(string token, string secureKey)
//        {
//            byte[] bytes = System.Text.Encoding.GetEncoding("utf-8").GetBytes(secureKey);
//            string json = "";
//            try
//            {
//                json = Jose.JWT.Decode(token, bytes);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }

//            return json;
//        }
//    }

//    internal class NetUtil
//    {
//        public void PostJson(NameValueCollection headers, string url, JObject data, string authorization, Action<Exception, string> callback)
//        {
//            //var bytes = Encoding.Default.GetBytes(data);

//            using (var client = new WebClient())
//            {
//                ServicePointManager.Expect100Continue = false;
//                ServicePointManager.ServerCertificateValidationCallback =
//                    delegate { return true; };

//                client.Headers[HttpRequestHeader.ContentType] = "application/json;charset=utf-8";
//                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + authorization;
//                if (headers != null && headers.Count > 0)
//                {
//                    client.Headers.Add(headers);
//                }
//                client.Encoding = Encoding.UTF8;
//                try
//                {
//                    string result = client.UploadString(new Uri(url), "POST", JsonConvert.SerializeObject(data));
//                    callback(null, result);
//                }
//                catch (Exception e)
//                {
//                    callback(e, string.Empty);
//                }
//            }
//        }

//        public void PostString(NameValueCollection headers, string url, string body, string authorization, Action<Exception, string> callback)
//        {
//            //var bytes = Encoding.Default.GetBytes(data);

//            using (var client = new WebClient())
//            {
//                ServicePointManager.Expect100Continue = false;
//                ServicePointManager.ServerCertificateValidationCallback =
//                    delegate { return true; };

//                client.Headers[HttpRequestHeader.ContentType] = "text/plain";
//                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + authorization;
//                if (headers != null && headers.Count > 0)
//                {
//                    client.Headers.Add(headers);
//                }
//                //client.Encoding = Encoding.UTF8;
//                try
//                {
//                    string result = client.UploadString(new Uri(url), "POST", body);
//                    callback(null, result);
//                }
//                catch (Exception e)
//                {
//                    callback(e, string.Empty);
//                }
//            }
//        }
//    }

//    internal class SecureUtil
//    {
//        public static void TestEncodeAndDecode(string msg, string chatKey)
//        {
//            Console.WriteLine("Start testEncodeAndDecode msg " + msg + "chat_key" + chatKey);

//            //Get Key
//            byte[] key = SecureUtil.ConvertHexStringToByte(chatKey);

//            //Get IV
//            UTF8Encoding utf8Encoding = new UTF8Encoding();
//            string hashContentKey = SecureUtil.ByteToSHA1(utf8Encoding.GetBytes(msg));
//            byte[] iv = SecureUtil.GetIV(hashContentKey);

//            //Encode string
//            string hashContent = SecureUtil.EncryptAESMessage(key, iv, msg);
//            Console.WriteLine("encode string: " + hashContent);

//            string oriMsg = SecureUtil.DecryptAESMessage(key, iv, hashContent);
//            Console.WriteLine("decode string: " + oriMsg);

//            Console.WriteLine("End testEncodeAndDecode");
//        }

//        public static byte[] GetIV(string hashContentKey)
//        {
//            MD5 md5 = MD5.Create();
//            byte[] source = Encoding.Default.GetBytes(hashContentKey);
//            return md5.ComputeHash(source);
//        }

//        public static byte[] ConvertHexStringToByte(string hex)
//        {
//            int NumberChars = hex.Length;
//            byte[] bytes = new byte[NumberChars / 2];
//            for (int i = 0; i < NumberChars; i += 2)
//                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
//            return bytes;
//        }

//        public static string ByteToSHA1(byte[] data)
//        {
//            SHA1 sha = new SHA1CryptoServiceProvider();
//            byte[] result = sha.ComputeHash(data);
//            return ConvertHexByteToString(result);
//        }

//        public static string ConvertHexByteToString(byte[] data)
//        {
//            return BitConverter.ToString(data).Replace("-", "");
//        }

//        //input a plaint text return base64 text
//        public static string EncryptAESMessage(byte[] Key, byte[] IV, string plainText)
//        {
//            // Check arguments.
//            if (plainText == null || plainText.Length <= 0)
//                throw new ArgumentNullException("plainText");
//            if (Key == null || Key.Length <= 0)
//                throw new ArgumentNullException("Key");
//            if (IV == null || IV.Length <= 0)
//                throw new ArgumentNullException("IV");
//            byte[] encrypted;
//            // Create an RijndaelManaged object
//            // with the specified key and IV.
//            using (RijndaelManaged rijAlg = new RijndaelManaged())
//            {
//                rijAlg.Key = Key;
//                rijAlg.IV = IV;

//                // Create a decryptor to perform the stream transform.
//                System.Security.Cryptography.ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

//                // Create the streams used for encryption.
//                using (MemoryStream msEncrypt = new MemoryStream())
//                {
//                    using (System.Security.Cryptography.CryptoStream csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
//                    {
//                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
//                        {
//                            //Write all data to the stream.
//                            swEncrypt.Write(plainText);
//                        }
//                        encrypted = msEncrypt.ToArray();
//                    }
//                }
//            }

//            // Return the encrypted bytes from the memory stream.
//            return Convert.ToBase64String(encrypted);
//        }

//        public static string DecryptAESMessage(byte[] Key, byte[] IV, string cipherText)
//        {
//            // Check arguments.
//            if (cipherText == null || cipherText.Length <= 0)
//                throw new ArgumentNullException("cipherText");
//            if (Key == null || Key.Length <= 0)
//                throw new ArgumentNullException("Key");
//            if (IV == null || IV.Length <= 0)
//                throw new ArgumentNullException("IV");

//            // Declare the string used to hold
//            // the decrypted text.
//            string plaintext = null;

//            // Create an RijndaelManaged object
//            // with the specified key and IV.
//            using (RijndaelManaged rijAlg = new RijndaelManaged())
//            {
//                rijAlg.Key = Key;
//                rijAlg.IV = IV;

//                // Create a decrytor to perform the stream transform.
//                System.Security.Cryptography.ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

//                // Create the streams used for decryption.
//                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
//                {
//                    using (System.Security.Cryptography.CryptoStream csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
//                    {
//                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
//                        {
//                            // Read the decrypted bytes from the decrypting stream
//                            // and place them in a string.
//                            plaintext = srDecrypt.ReadToEnd();
//                        }
//                    }
//                }
//            }

//            return plaintext;
//        }

//        public static string EncodeBase64(string input)
//        {
//            byte[] bytes = Encoding.UTF8.GetBytes(input);
//            return Convert.ToBase64String(bytes);
//        }
//    }

//    //class
//}