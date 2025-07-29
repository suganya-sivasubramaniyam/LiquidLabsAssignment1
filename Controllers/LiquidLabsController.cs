using LiquidLapsAPI.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LiquidLapsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiquidLabsController : ControllerBase
    {
        private readonly ILiquidLabsService _liquidLabsService;
        public LiquidLabsController(ILiquidLabsService liquidLabsService)
        {
            _liquidLabsService = liquidLabsService;
        }
        // GET: api/<LiquidLabsController1>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _liquidLabsService.GetAll();
            return Ok(result);
        }

        // GET api/<LiquidLabsController1>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _liquidLabsService.GetById(id);
            return Ok(result);
        }

        //// POST api/<LiquidLabsController1>
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<LiquidLabsController1>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<LiquidLabsController1>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
