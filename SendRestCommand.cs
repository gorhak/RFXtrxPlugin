using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace SwitchKing.Server.Plugins.RFXtrx
{
    public class SendRestCommand
    {
        public string url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }
        public string user
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }
        public string pass
        {
            get
            {
                return _pass;
            }
            set
            {
                _pass = value;
            }
        }
        
        // State information used in the task.
        protected string _url;
        protected string _user;
        protected string _pass;

        // The constructor obtains the state information.
        public SendRestCommand(string Url)
        {
            _url = Url;
        }

        // The constructor obtains the state information.
        public SendRestCommand(string Url, string User, string Pass)
        {
            _url = Url;
            _user = User;
            _pass = Pass;
        }

        public void SendString()
        {
            XDocument xDoc;

            try
            {
                int i = 0;
                do
                {
                    XmlUrlResolver xmlResolver = new XmlUrlResolver();
                    System.Net.NetworkCredential myCred;
                    myCred = new System.Net.NetworkCredential(user, pass);
                    xmlResolver.Credentials = myCred;
                    XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                    xmlReaderSettings.XmlResolver = xmlResolver;

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(XmlReader.Create(url, xmlReaderSettings));
                    xmlDoc.InnerXml = FixNameSpaces(xmlDoc.InnerXml);

                    using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc))
                    {
                        // the reader must be in the Interactive state in order to
                        // create a LINQ to XML tree from it.
                        nodeReader.MoveToContent();

                        xDoc = XDocument.Load(nodeReader);
                    }

                    i++;
                }
                while (((xDoc.ToString().Contains("Root") && !xDoc.Root.Value.Equals("OK"))
                     || (xDoc.ToString().Contains("REST") && !xDoc.Element("RESTOperationResult").Element("Successfull").Value.Equals("true")))
                     && (i < 6));
            }
            catch
            { }
        }

        private HttpWebRequest GetWebRequest(string url)
        {
            CookieContainer myContainer = new CookieContainer();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Credentials = new NetworkCredential(user, pass);
            //req.Credentials = new NetworkCredential("user", "pass");
            req.CookieContainer = myContainer;
            req.PreAuthenticate = true;

            return req;
        }

        private HttpWebRequest GetWebRequest(string url, string user, string pass)
        {
            CookieContainer myContainer = new CookieContainer();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Credentials = new NetworkCredential(user, pass);
            req.CookieContainer = myContainer;
            req.PreAuthenticate = true;

            return req;
        }

        private string Post(string url, object data)
        {
            string vystup = null;

            try
            {
                MemoryStream stream = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(data.GetType());
                serializer.WriteObject(stream, data);
                stream.Position = 0;

                //Our postvars
                byte[] buffer = stream.GetBuffer();
                //Initialisation, we use localhost, change if appliable
                HttpWebRequest request = GetWebRequest(url);
                //Our method is post, otherwise the buffer (postvars) would be useless
                request.Method = "POST";
                //We use form contentType, for the postvars.
                request.ContentType = "application/x-www-form-urlencoded";
                //The length of the buffer (postvars) is used as contentlength.
                request.ContentLength = buffer.Length;
                //We open a stream for writing the postvars
                Stream PostData = request.GetRequestStream();
                //Now we write, and afterwards, we close. Closing is always important!
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
                //Get the response handle, we have no true response yet!
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Now, we read the response (the string), and output it.
                Stream Answer = response.GetResponseStream();
                StreamReader _Answer = new StreamReader(Answer);
                vystup = _Answer.ReadToEnd();
                stream.Close();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return vystup;
        }

        private byte[] Get(string url)
        {
            byte[] vystup = null;

            //Initialisation, we use localhost, change if appliable
            HttpWebRequest request = GetWebRequest(url);
            //Our method is post, otherwise the buffer (postvars) would be useless
            request.Method = "GET";
            //Get the response handle, we have no true response yet!
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format.
            StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);

            var data = readStream.ReadToEnd();

            vystup = System.Text.Encoding.UTF8.GetBytes(data);

            response.Close();
            readStream.Close();

            return vystup;
            //return data;
        }

        private string SendString(string url)
        {
            //Initialisation, we use localhost, change if appliable
            HttpWebRequest request = GetWebRequest(url);
            //Our method is post, otherwise the buffer (postvars) would be useless
            request.Method = "GET";
            //Get the response handle, we have no true response yet!
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format.
            StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);

            var data = readStream.ReadToEnd();

            response.Close();
            readStream.Close();

            return data;
        }

        public static string FixNameSpaces(string xmlString)
        {
            while (xmlString.Contains("xmlns"))
            {
                int i1 = 0;
                int i2 = 0;
                int i3 = 0;
                i1 = xmlString.IndexOf(" xmlns");
                string string1 = xmlString.Substring(i1, xmlString.Length - i1);
                i2 = string1.IndexOf("\"");
                string string2 = string1.Substring(i2 + 1, string1.Length - (i2 + 1));
                i3 = string2.IndexOf("\"");
                string string3 = string2.Substring(i3 + 1, string2.Length - (i3 + 1));
                xmlString = xmlString.Substring(0, i1) + string3;
            }

            while (xmlString.Contains(" i:nil"))
            {
                int i = 0;
                int j = 0;
                i = xmlString.IndexOf(" i:nil");
                string string1 = xmlString.Substring(0, i);
                while (string1.Contains("<"))
                {
                    j = string1.IndexOf("<");
                    string1 = string1.Substring(j + 1, string1.Length - (j + 1));
                }
                j = xmlString.Substring(i, xmlString.Length - i).IndexOf("<");
                string string2 = xmlString.Substring(i + j, xmlString.Length - (i + j));
                xmlString = xmlString.Substring(0, i) + ">" + "i:nil=\"true\"" + "</" + string1 + ">" + string2;
            }

            if (xmlString.Contains("&amp;#"))
            {
                xmlString = xmlString.Replace("&amp;#229;", "å");
                xmlString = xmlString.Replace("&amp;#228;", "ä");
                xmlString = xmlString.Replace("&amp;#246;", "ö");
                xmlString = xmlString.Replace("&amp;#197;", "Å");
                xmlString = xmlString.Replace("&amp;#196;", "Ä");
                xmlString = xmlString.Replace("&amp;#214;", "Ö");
            }
            return xmlString;
        }

    }
}