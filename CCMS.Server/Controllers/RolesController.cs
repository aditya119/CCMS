using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AuthenticateSession]
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
