﻿using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface IProceedingDecisionsService
    {
        Task<ProceedingDecisionModel> RetrieveAsync(int proceedingDecisionId);
        Task<IEnumerable<ProceedingDecisionModel>> RetrieveAllAsync();
    }
}