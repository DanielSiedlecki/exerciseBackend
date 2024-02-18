using exerciseBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace exerciseBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private readonly ExerciseDBContext _context;

        public TaskController(ExerciseDBContext context)
        {
            _context = context;
        }

        // GET api/task
        [HttpGet]
        public ActionResult<object> GetLastTask()
        {
            var lastTask = _context.Tasks
                .OrderByDescending(t => t.created_at)
                .Select(t => new
                {
                    t.taskName,
                    t.created_at,
                    t.nowDate,
                    t.countOccurrences,
                    intervalDay = t.intervalDay!,
                    t.startDate,
                    t.intervalCount,
                    t.firstExecutionDate,
                    t.lastExecutionDate,
                    t.nextExecutionDate
                })
                .FirstOrDefault();

            if (lastTask == null)
            {
                return NotFound();
            }
            return lastTask;
        }
        private DateTime? FindFirstOccurrence(DateTime startDate, DateTime nowDate, string dayOfWeek)
        {
            while (startDate <= nowDate)
            {
                if (startDate.DayOfWeek.ToString() == dayOfWeek)
                {
                    return startDate;
                }
                else
                {
                    startDate = startDate.AddDays(1);
                }
            }
            return null;
        }

        private DateTime? FindLastOccurrence(DateTime? firstOccurrence, DateTime nowDate, string dayOfWeek, int interval)
        {
            while (firstOccurrence.HasValue && firstOccurrence.Value <= nowDate)
            {
                if (firstOccurrence.Value.DayOfWeek.ToString() == dayOfWeek)
                {
                    var lastOccurrence = firstOccurrence.Value;

                    while (lastOccurrence <= nowDate)
                    {
                        lastOccurrence = lastOccurrence.AddDays(7 * interval);
                    }


                    if (lastOccurrence > nowDate)
                    {
                        lastOccurrence = lastOccurrence.AddDays(-7 * interval);
                    }

                    return lastOccurrence;
                }
                else
                {
                    firstOccurrence = firstOccurrence.Value.AddDays(1);
                }
            }
            return null;
        }
        // POST api/task
        [HttpPost]
        public IActionResult PostTask(exerciseBackend.Models.Task task)
        {
            DateTime nowDate = DateTime.Now;

            // timestamp format validation
            if (!(task.timestampStartDate is int timestamp))
            {
                return BadRequest("Invalid timestampStartDate");
            }

            // convert from timestamp to UTC
            DateTime startDateUTC = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

            int intervalWeek = task.intervalCount;

            if (Enum.TryParse(task.intervalDay, out DayOfWeek intervalDay))
            {
                int countOccurrences = 0;
                DateTime currentTaskDate = startDateUTC;

                DateTime? firstOccurrence = FindFirstOccurrence(startDateUTC, nowDate, intervalDay.ToString());

                if (firstOccurrence.HasValue)
                {
                    DateTime? lastOccurrence = FindLastOccurrence(firstOccurrence, nowDate, intervalDay.ToString(), intervalWeek);
                    DateTime? nextOccurrence = lastOccurrence.Value.AddDays(7 * intervalWeek);

                    // simple counter of the number of occurrences
                    countOccurrences = (int)((nowDate - firstOccurrence.Value).TotalDays / (7 * intervalWeek)) + 1;
                    var taskResult = new exerciseBackend.Models.Task
                    {
                        taskName = task.taskName,
                        countOccurrences = countOccurrences,
                        intervalDay = intervalDay.ToString(),
                        intervalCount = intervalWeek,
                        nowDate = nowDate,
                        startDate = startDateUTC,
                        firstExecutionDate = firstOccurrence.Value,
                        lastExecutionDate = lastOccurrence.Value,
                        nextExecutionDate = nextOccurrence.Value
                    };

                    _context.Tasks.Add(taskResult);
                    _context.SaveChanges();

                    var result = new
                    {
                        CountOccurrences = countOccurrences,
                        TodayDate = nowDate.ToString("yyyy-MM-dd"),
                        FirstExecutionDate = firstOccurrence.Value.ToString("yyyy-MM-dd"),
                        LastExecutionDate = lastOccurrence.Value.ToString("yyyy-MM-dd"),
                        NextExecutionDate = nextOccurrence.Value.ToString("yyyy-MM-dd")
                    };

                    return Ok(result);
                }
                else
                {
                    return NotFound("Not found date");
                }
            }
            else
            {
                return BadRequest("Invalid day of week name");
            }
        }
    }
}