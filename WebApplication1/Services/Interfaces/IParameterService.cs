﻿using JobTrack.Models.Manuscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTrack.Services.Interfaces
{
    public interface IParameterService
    {
        Task<List<GetAllTurnAroundTime>> GetAllTurnAroundTimeAsync();
    }
}
