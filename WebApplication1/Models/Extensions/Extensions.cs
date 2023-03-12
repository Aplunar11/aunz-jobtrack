using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models.Extensions
{
    public static class Extensions
    {
        public static string ToText(this bool isCheck)
        {
            return isCheck ? "1" : "0";
        }
    }
}