using JobTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationModel>> GetNoticationByUserAsync(string username);

        Task<NotificationModel> UpdateNotificationUserRecordAsync(NotificationModel model);
    }
}
