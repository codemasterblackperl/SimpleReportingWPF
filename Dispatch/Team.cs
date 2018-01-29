using System;

namespace Dispatch
{
    public class Team
    {
        public long Id { get; set; }
        public string UnitName { get; set; }
        public string SubUnitDispatched { get; set; }
        public DateTime? Dispatched { get; set; }
        public DateTime? Arived { get; set; }
        public DateTime? Finished { get; set; }
    }
}