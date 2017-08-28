using FSDBugTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSDBugTracker.ViewModel
{
    public class IndexVM
    {
        Dictionary<string, int> TicketStatuses = new Dictionary<string, int>();

        public IndexVM()
        {
            this.TicketStatuses.Add("Open/Unassigned", CounterHelper.CountTicketsByStatus("Open/Asssigned"));
        }
    }
}