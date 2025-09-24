using BitStore.Bitstamp.Models;
using BitStore.Core.Services;
using BitStore.Server.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitStore.Server.Controllers
{
    /// <summary>
    /// Controller providing access to order book data.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DataController(ICoreService coreService, IUserContext user) : ControllerBase
    {
        [HttpGet(Name = "GetData")]
        public async Task<ActionResult<OrderBook>> Get()
        {
            var orderBook = await coreService.GetLatestOrderBookAsync(user.UserId);

            if (orderBook == null)
            {
                return NotFound("Order book data not available");
            }

            return orderBook;
        }
    }
}
