﻿using GamedayTracker.Models;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IBotTimer
    {
        Task<Result<bool, SystemError<BotTimerDataServiceProvider>>> WriteTimestampToTextAsync();
        Task<Result<DateTime, SystemError<BotTimerDataServiceProvider>>> GetTimestampFromTextAsync();
    }
}
