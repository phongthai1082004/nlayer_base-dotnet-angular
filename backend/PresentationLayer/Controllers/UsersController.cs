using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces.IRepositories.Common;
using DataAccessLayer.Shared;
using DataAccessLayer.Constants.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace PresentationLayer.Controllers
{
    [Route("users")]
    [ApiController]
    [AllowAnonymous]
    public class UsersController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public record UserDto(Guid Id, string Email, Role Role, bool IsActive, DateTime CreatedAt);

        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var repo = _unitOfWork.GenerateRepository<User, Guid>();
            var result = await repo.GetListAsync(new PageRequest { CurrentPage = pageNumber, PageSize = pageSize }, ct);

            var dtoResult = new PaginatedResult<UserDto>
            {
                Items = result.Items.Select(u => new UserDto(u.Id, u.Email, u.Role, u.IsActive, u.CreatedAt)).ToList(),
                PageSize = result.PageSize,
                CurrentPage = result.CurrentPage,
                PageCount = result.PageCount,
                TotalCount = result.TotalCount
            };

            return Ok(dtoResult);
        }
    }
}
