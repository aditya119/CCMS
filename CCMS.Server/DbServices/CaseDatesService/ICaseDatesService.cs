﻿using CCMS.Shared.Models.CaseDatesModels;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface ICaseDatesService
    {
        Task<CaseDatesModel> RetrieveAsync(int caseId);
        Task UpdateAsync(UpdateCaseDatesModel caseDatesModel, int currUser);
    }
}