using BitStore.Bitstamp.Models;
using BitStore.Core.Services;
using BitStore.Server.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitStore.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        private readonly IUserContext _user;
        private readonly ICoreService _coreService;

        public DataController(ICoreService coreService, ILogger<DataController> logger, IUserContext user)
        {
            _coreService = coreService;
            _logger = logger;
            _user = user;
        }

        [HttpGet(Name = "GetData")]
        public async Task<ActionResult<OrderBook>> Get()
        {
            var orderBook = await _coreService.GetLatestOrderBookAsync(_user.UserId);

            if (orderBook == null)
            {
                return NotFound("Order book data not available");
            }

            return orderBook;
        }
    }
}
