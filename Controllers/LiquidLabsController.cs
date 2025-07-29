using LiquidLapsAPI.Services;
using Microsoft.AspNetCore.Mvc;


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
        // GET: api/<LiquidLabsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _liquidLabsService.GetAll();
                if (result == null) { 
                    return NotFound("data not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
           
        }

        // GET api/<LiquidLabsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _liquidLabsService.GetById(id);
                if (result == null)
                {
                    return NotFound("data not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        // GET api/<LiquidLabsController>/5
        [HttpGet("UserId/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var result = await _liquidLabsService.GetByUserId(userId);
                if (result == null)
                {
                    return NotFound("data not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }


    }
}
