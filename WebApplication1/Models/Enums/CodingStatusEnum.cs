using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.Enums
{
    public enum CodingStatusEnum
    {
        New = 1,
        Completed = 2,
        Cancelled = 3,
        Late = 4,
        Due = 5,
        Revised = 6
    }
}