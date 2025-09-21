using BitStore.Bitstamp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BitStore.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetData")]
        public OrderBook Get()
        {
            return new OrderBook();
        }
    }
}
