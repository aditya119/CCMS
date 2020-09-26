﻿using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface IActorTypesService
    {
        Task<IEnumerable<ActorTypeModel>> RetrieveAllAsync();
    }
}