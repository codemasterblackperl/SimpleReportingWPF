using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch
{
    internal class Client
    {
        public Client(string baseUrl)
        {
            //#if DEBUG
            //            _baseUrl = "http://localhost:61369/";
            //#else
            //            _baseUrl="http://darel.blackperlsolutions.com/";
            //#endif

            MainWindow.Log.Info("Rest client initialized with url: " + baseUrl);

            _client = new RestClient(baseUrl);


        }

        RestClient _client;       
        

        public async Task<string> Get(string url)
        {
            //MainWindow.Log.Info("Sending request to " + url);

            var request = new RestRequest(url, Method.GET);

            var resp = await _client.ExecuteTaskAsync(request);

            //MainWindow.Log.Info("Responce received: " + resp.Content);

            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //try
                //{
                //    var message = JsonConvert.DeserializeObject<ErrorResponse>(resp.Content);
                //    throw new Exception(message.error_description);
                //}
                //catch
                //{
                //    throw new Exception(resp.StatusDescription);
                //}
                MainWindow.Log.Error("Erorr for request: " + url+"\n Response: "+resp.Content);
                throw new Exception(resp.Content);
            }

            return resp.Content;
        }

        public async Task<string> PostJson(string url, object jsonData)
        {
            //MainWindow.Log.Info("Sending request to " + url);

            var request = new RestRequest(url, Method.POST);

            request.AddParameter("application/json; charset=utf-8", jsonData, ParameterType.RequestBody);

            var resp = await _client.ExecuteTaskAsync(request);

            //MainWindow.Log.Info("Responce received: " + resp.Content);

            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //try
                //{
                //    var message = JsonConvert.DeserializeObject<ErrorResponse>(resp.Content);
                //    throw new Exception(message.error_description);
                //}
                //catch
                //{
                //    throw new Exception(resp.StatusDescription);
                //}
                MainWindow.Log.Error("Erorr for request: " + url + "\n Response: " + resp.Content);
                throw new Exception(resp.Content);
            }

            return resp.Content;
        }

        public async Task<string> PostJson(string url)
        {
            //MainWindow.Log.Info("Sending request to " + url);

            var request = new RestRequest(url, Method.POST);

            //request.AddParameter("application/json; charset=utf-8", jsonData, ParameterType.RequestBody);

            var resp = await _client.ExecuteTaskAsync(request);

            //MainWindow.Log.Info("Responce received: " + resp.Content);

            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //try
                //{
                //    var message = JsonConvert.DeserializeObject<ErrorResponse>(resp.Content);
                //    throw new Exception(message.error_description);
                //}
                //catch
                //{
                //    throw new Exception(resp.StatusDescription);
                //}
                MainWindow.Log.Error("Erorr for request: " + url + "\n Response: " + resp.Content);
                throw new Exception(resp.Content);
            }

            return resp.Content;
        }
    }
}
