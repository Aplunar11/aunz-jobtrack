using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models
{
    public class TopMenuModel
    {
        public int UnreadCount { get; set; }

        public List<NotificationModel> Notifications { get; set; }

        public TopMenuModel()
        {
            Notifications = new List<NotificationModel>();
        }
    }
}