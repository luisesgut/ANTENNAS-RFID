using Backend_Final.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.LLRP.LTK.LLRPV1;
using System.Reflection.PortableExecutable;

namespace Backend_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RfidController : ControllerBase
    {
        private readonly RfidService _rfidService;
        private readonly RosPecService _rosPecService;
        static LLRPClient reader;

        public RfidController(RfidService rfidService,RosPecService rosPecService)
        {
            _rfidService = rfidService;
            _rosPecService = rosPecService;
        }

        //[HttpPost("connect")]
        //public IActionResult ConnectLLRP()
        //{
        //    try
        //    {
        //        _rfidService.ConnectLLRP();

        //        return Ok("Connected and configured successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}

        [HttpPost("CONNECT-LLRP")]
        public IActionResult ConnectLLRP()
        {
            try
            {
                _rfidService.ConnectLLRP(); // Sin argumentos
                return Ok("LLRP client connected successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        //[HttpPost("start")]
        //public IActionResult StartReading()
        //{
        //    try
        //    {
        //        _rfidService.ConfigureReader();
        //        _rfidService.StartReading();
        //        return Ok("Reader started.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}



        [HttpPost("DELETE-ROSPEC")]
        public IActionResult Delete_RoSpec()
        {
            try
            {
                _rosPecService.Delete_RoSpec();
                return Ok("ROSpec deleted successfully.");

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("ADD-ROSPEC")]
        public IActionResult Add_RoSpec()
        {
            try
            {
                _rosPecService.Add_RoSpec();
                return Ok("ROSpec added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("ENABLE-ROSPEC")]
        public IActionResult Enable_RoSpec()
        {
            try
            {
                _rosPecService.Enable_RoSpec();
                return Ok("ROSpec enabled successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("GET-ROSPECS")]
        public IActionResult Get_RoSpecs()
        {
            try
            {
                _rosPecService.Get_RoSpecs();
                return Ok("ROSpecs retrieved successfully. Check logs for details.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }






        [HttpPost("stop-LLRP")]
        public IActionResult DisconnectLLRP()
        {
            try
            {
                _rfidService.DisconnectLLRP();
                return Ok("Connection LLRP Closed");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        //[HttpPost("disconnect")]
        //public IActionResult Disconnect()
        //{
        //    try
        //    {
        //        _rfidService.Disconnect();
        //        return Ok("Disconnected successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}


    }
}
