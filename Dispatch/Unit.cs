using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch
{
    public class Unit
    {
        public int Id { get; set; }

        public string Name { get; set; }

        //public string UserName { get; set; }

        public string Area { get; set; }

        public string Officers { get; set; }

        public string AdditionalInfo { get; set; }

        public int IsAvailable { get; set; }

        public bool IsRequestOn { get; set; }

        public string Message { get; set; }
    }


    public class UnitAcceptRejectRequestModel
    {
        public int Id { get; set; }

        public bool AcceptRequest { get; set; }
    }
}
