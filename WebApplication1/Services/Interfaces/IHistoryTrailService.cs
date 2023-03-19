﻿using JobTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IHistoryTrailService
    {
        Task<List<HistoryTrailModel>> GetAllHistoryTrailAsync();
    }
}
