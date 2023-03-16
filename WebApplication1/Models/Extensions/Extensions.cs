using System;
using System.Collections.Generic;
using System.Data;
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

        public static int ToInt(this bool isCheck)
        {
            return Convert.ToInt32(isCheck);
        }
    }
}