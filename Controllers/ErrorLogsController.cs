using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cafeteria.Data;
using System;

namespace Cafeteria.Controllers
{
  [ApiController]
  [Route("api/errors")]
  public class ErrorLogsController : ControllerBase
  {
    private readonly Context _db;

    public ErrorLogsController(Context db)
    {
      _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var logs = await _db.ErrorLogs
          .OrderByDescending(x => x.CreatedAt)
          .Take(200)
          .ToListAsync();

      return Ok(logs);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
      var stats = await _db.ErrorLogs
          .GroupBy(x => x.Path)
          .Select(g => new
          {
            Path = g.Key,
            Total = g.Count()
          })
          .OrderByDescending(x => x.Total)
          .ToListAsync();

      return Ok(stats);
    }
  }

}
