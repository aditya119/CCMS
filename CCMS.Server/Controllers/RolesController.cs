using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<RoleModel>>> GetAllRoles()
        {
            IEnumerable<RoleModel> roles = await _rolesService.GetAllRoles();
            return Ok(roles);
        }

        [HttpGet]
        [Route("csv")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<int>> GetRoleId(string rolesCsv)
        {
            int roleId = await _rolesService.GetRoleId(rolesCsv);
            return Ok(roleId);
        }
    }
}
