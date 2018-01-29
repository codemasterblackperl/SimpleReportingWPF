using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch
{
    public class Call
    {
        public long Id { get; set; }

        public string ControlNumber { get; set; }

        public string EnteredBy { get; set; }

        public DateTime CallReceivedTime { get; set; }

        public string Description { get; set; }

        public string Barangay { get; set; }
        public string Purok { get; set; }
        public string Address { get; set; }

        public string CallersName { get; set; }
        public string CallersPhoneNumber { get; set; }

        public string EmergencyType { get; set; }
        public string IncidentType { get; set; }
        public string Incident { get; set; }

        public string NoOfPeopleHurt { get; set; }
        public string VehicleInvolved { get; set; }

        public string Age { get; set; }
        public string Gender { get; set; }
        public string PersonBreathing { get; set; }
        public string PersonConsciours { get; set; }

        public string IncidentNotes { get; set; }

        public List<Team> UnitsAssigned { get; set; }

        public bool Completed { get; set; }

        public Call()
        {
            UnitsAssigned = new List<Team>();
        }
    }


    public class UpdateDispacthTime
    {
        
        public long CallId { get; set; }
        
        public long TeamId { get; set; }
        
        public string SubUnitAssigned { get; set; }
    }

    public enum CallStatus
    {
        Received = 0,
        En_Route = 1,
        On_Scene = 2,
        Finished = 3
    }
}
