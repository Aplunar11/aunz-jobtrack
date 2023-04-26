using JobTrack.Models.Enums;
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

        public static string AsTimeAgo(this DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("{0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("{0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("{0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("{0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("{0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }

        public static object[] ToArray(this object obj)
            => obj as object[];

        public static FilterModel ToDateModel(this FilterModel filterModel)
            => new FilterModel
            {
                FilterValue = Convert.ToDateTime(filterModel.FilterValue).ToString("MM/dd/yyyy")
            };

        public static DateTime ToDateValue(this object obj)
            => (string)obj == "Select" ? DateTime.Now : Convert.ToDateTime(obj).Date;

        public static UserAccessEnum ToUserAccessEnum(this string userAccess)
        {
            var userAccessEnum = new UserAccessEnum();

            switch (userAccess)
            {
                case "Client(LE)":
                    userAccessEnum = UserAccessEnum.Client_LE;
                    break;

                case "Straive(PE)":
                    userAccessEnum = UserAccessEnum.Straive_PE;
                    break;
            }

            return userAccessEnum;
        }
    }
}