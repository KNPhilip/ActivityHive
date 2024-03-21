namespace API.Controllers
{
    public class ErrorController : ControllerTemplate
    {
        [HttpGet("bad-request")]
        public ActionResult GetBadRequest() =>
            BadRequest("This is a bad request");
        
        [HttpGet("server-error")]
        public ActionResult GetServerError() =>
            throw new Exception("This is a server error");
    }
}
