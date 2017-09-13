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
        public Client()
        {
#if DEBUG
            _baseUrl = "http://localhost:61369/";
#else
            _baseUrl="http://darel.blackperlsolutions.com/";
#endif
            _client = new RestClient(_baseUrl);
        }

        RestClient _client;

        private readonly string _baseUrl = "";
        

        public async Task<string> Get(string url)
        {
            var request = new RestRequest(url, Method.GET);

            var resp = await _client.ExecuteTaskAsync(request);

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
                throw new Exception(resp.Content);
            }

            return resp.Content;
        }

        public async Task<string> PostJson(string url, object jsonData)
        {
            var request = new RestRequest(url, Method.POST);

            request.AddParameter("application/json; charset=utf-8", jsonData, ParameterType.RequestBody);

            var resp = await _client.ExecuteTaskAsync(request);

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
                throw new Exception(resp.Content);
            }

            return resp.Content;
        }

    }
}
