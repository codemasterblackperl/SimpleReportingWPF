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

        public string Address { get; set; }
        public string Area { get; set; }
        public string BusinessName { get; set; }

        public string CallType { get; set; }
        public string AdditionalInfo { get; set; }
        public string Complainant { get; set; }
        public string ContactAt { get; set; }

        public string IncidentNotes { get; set; }

        public string UnitAssigned { get; set; }

        public string Dispatched { get; set; }
        public string Arived { get; set; }
        public string Finished { get; set; }

        public string Disposition { get; set; }

        public CallStatus Status { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Control Number: " + ControlNumber);
            sb.AppendLine("Entered by: " + EnteredBy);
            sb.AppendLine("Call Received Time: " + CallReceivedTime.ToString());
            sb.AppendLine("Address: " + Address);
            sb.AppendLine("Area: " + Area);
            sb.AppendLine("Business Name: " + BusinessName);
            sb.AppendLine("Call Type: " + CallType);
            sb.AppendLine("Additional Info: " + AdditionalInfo);
            sb.AppendLine("Complainant: " + Complainant);
            sb.AppendLine("Contact At: " + ContactAt);
            sb.AppendLine("Incident Notes: " + IncidentNotes);

            return sb.ToString();
        }
    }


    public enum CallStatus
    {
        Received = 0,
        En_Route = 1,
        On_Scene = 2,
        Finished = 3
    }
}
