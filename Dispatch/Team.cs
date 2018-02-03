using System;

namespace Dispatch
{
    public class Team
    {
        public long TeamId { get; set; }
        public string UnitName { get; set; }
        public string SubUnitDispatched { get; set; }
        public DateTime? Dispatched { get; set; }
        public DateTime? Arived { get; set; }
        public DateTime? Finished { get; set; }

        //public long CallId { get; set; }
    }
}