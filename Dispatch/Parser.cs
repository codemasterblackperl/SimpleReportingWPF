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
        public Parser()
        {
            _client = new Client();
        }

        private Client _client;

        private readonly string _unitUrl = "api/units/";
        private readonly string _callUrl = "api/calls/";

        public async Task<Unit> GetUnitAsync(int id)
        {
            var content = await _client.Get(_unitUrl + "GetUnit/" + id);
            var unit = JsonConvert.DeserializeObject<Unit>(content);
            return unit;
        }

        public async Task<Unit> GetUnitAsync(string name)
        {
            var content = await _client.Get(_unitUrl + "GetUnit/" + name);
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

        public async Task AcceptRejectRequest(UnitAcceptRejectRequestModel model)
        {
            var json = JsonConvert.SerializeObject(model);

            var content = await _client.PostJson(_unitUrl + "AcceptOrRejectRequest", json);
        }
    }
}
