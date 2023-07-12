namespace API.Controllers
{
    public class ActivitiesController : ControllerTemplate
    {

        private readonly DataContext _context;

        public ActivitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await _context.Activities.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            Activity? result = await _context.Activities.FindAsync(id);
            if (result is null)
                return NotFound();
            return Ok(result);
        }
    }
}