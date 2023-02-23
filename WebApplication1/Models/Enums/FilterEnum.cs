using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.Enums
{
    public enum FilterEnum
    {
        Open = 1,
        Cancelled = 2,
        Closed = 3,
        Replied_by_Client = 4,
        Replied_by_Straive = 5
    }
}