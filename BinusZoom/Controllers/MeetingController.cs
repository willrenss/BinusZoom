using BinusZoom.Data;
using BinusZoom.Models;
using BinusZoom.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom.Controllers;

public class MeetingController : Controller
{
    private readonly BinusZoomContext _context;

    public MeetingController(BinusZoomContext context)
    {
        _context = context;
    }

    // GET: Meeting
    public async Task<IActionResult> Index()
    {
        return View(await _context.Meeting.ToListAsync());
    }

    // GET: Meeting/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null) return NotFound();

        var meeting = await _context.Meeting
            .FirstOrDefaultAsync(m => m.Id == id);
        if (meeting == null) return NotFound();

        return View(meeting);
    }

    // GET: Meeting/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Meeting/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("MeetingDate,PosterPath, LinkUrl")] 
        Meeting meeting, 
        IFormFile templateFile)
    {
        if (ModelState.IsValid)
        {
            if (templateFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(templateFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    templateFile.CopyTo(fileStream);
                }

                meeting.PosterPath = fileName;
            }
            _context.Add(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(meeting);
    }

    // GET: Meeting/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null) return NotFound();

        var meeting = await _context.Meeting.FindAsync(id);
        if (meeting == null) return NotFound();
        return View(meeting);
    }

    // POST: Meeting/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id,
        [Bind("Id,MeetingDate,MeetingEndDate,PosterPath,CertificateTemplatePath")] Meeting meeting)
    {
        if (id != meeting.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(meeting);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingExists(meeting.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(meeting);
    }

    // GET: Meeting/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();

        var meeting = await _context.Meeting
            .FirstOrDefaultAsync(m => m.Id == id);
        if (meeting == null) return NotFound();

        return View(meeting);
    }

    // POST: Meeting/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        
        var meeting = await _context.Meeting.FindAsync(id);
        if (meeting != null) _context.Meeting.Remove(meeting);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    // GET: Meeting/5/Participants
    [HttpGet("Meeting/{id}/Participants")]
    public async Task<IActionResult> Participants(string id)
    {
        // get current time in long
        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        Task task = new Task(() =>
        {
            Console.WriteLine("HELLO WORLD! FROM TASK => " + DateTime.Now.ToString("HH:mm:ss.fff"));
        });
        TimeSpan timeSpan = TimeSpan.FromSeconds(10);
        EmailScheduler es = new EmailScheduler(task, timeSpan);
        await es.StartAsync(CancellationToken.None);
        if (id == null) return NotFound();

        var meeting = await _context.Meeting
            .Include(meeting1 => meeting1.Registrations)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (meeting == null) return NotFound();

        return View(meeting);
    }

    private bool MeetingExists(string id)
    {
        return _context.Meeting.Any(e => e.Id == id);
    }
}