using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch
{
    internal class Parser
    {
        public Parser(string apiUrl)
        {
            _client = new Client(apiUrl);
        }

        private Client _client;

        private string _authString = "";

        public string AuthString { get { return _authString; } }

        private readonly string _unitUrl = "api/units/";
        private readonly string _callUrl = "api/calls/";

        public async Task LoginAsync(string username, string password)
        {
            var param = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username },
                { "password", password }
            };

            var content = await _client.PostParams("oauth/token", param);

            var restResp = JsonConvert.DeserializeObject<AuthResponse>(content);
            _authString = $"{restResp.token_type} {restResp.access_token}";

            _client.SetAuth(_authString);

            return;
        }

        public async Task<List<string>> GetAllUnitNames()
        {
            try
            {
                var content = await _client.Get(_unitUrl + "GetAllUnitNames");
                var list = JsonConvert.DeserializeObject<List<string>>(content);
                return list;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Unit> GetUnitAsync(int id)
        {
            var content = await _client.Get(_unitUrl + "GetUnit/" + id);
            var unit = JsonConvert.DeserializeObject<Unit>(content);
            return unit;
        }

        public async Task<Unit> GetUnitAsync(string userName)
        {
            var content = await _client.Get(_unitUrl + "GetUnitByUserName/" + userName);
            var unit = JsonConvert.DeserializeObject<Unit>(content);
            return unit;
        }

        public async Task<List<Call>> GetCallsAsync(string unitName)
        {
            try
            {
                var content = await _client.Get(_callUrl + "GetCallsByUnitName/"+unitName);

                var list = JsonConvert.DeserializeObject<List<Call>>(content);

                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Call> UpdateDispatchTime(UpdateDispacthTime model)
        {
            var json = JsonConvert.SerializeObject(model);

            var content = await _client.PostJson(_callUrl + "UpdateDispatchTime",json);

            var call = JsonConvert.DeserializeObject<Call>(content);

            return call;
        }


    }

    public class AuthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }
}
