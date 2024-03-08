using BinusZoom.Data;
using BinusZoom.Models;
using BinusZoom.Service;
using BinusZoom.Service.CertificateService;
using BinusZoom.Service.EmailService;
using BinusZoom.Service.ZoomService;
using BinusZoom.Template.MailTemplate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom.Controllers;

public class MeetingController : Controller
{
    private readonly BinusZoomContext _context;
    private readonly CSMailRenderer _csMailRenderer;
    private readonly MailSender _mailSender;
    private readonly ZoomMeetingService _zoomMeetingService;

    public MeetingController(BinusZoomContext context, CSMailRenderer csMailRenderer, MailSender mailSender, ZoomMeetingService zoomMeetingService)
    {
        _context = context;
        _csMailRenderer = csMailRenderer;
        _mailSender = mailSender;
        _zoomMeetingService = zoomMeetingService;
    }
    
    public async Task<JsonResult> Trigger()
    {
        await _zoomMeetingService.GetParticipantList("0");
        return Json(_zoomMeetingService.ZoomAccountList.accounts);
    }
    
    // return create action that return json
    public async Task<JsonResult> Token()
    {
        
        return Json(_zoomMeetingService.ZoomAccountList.accounts);
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
        [Bind("Title,MeetingDate,PosterPath, LinkUrl")] 
        Meeting meeting, 
        IFormFile posterTemplate,
        IFormFile certificateTemplate)
    {
        if (ModelState.IsValid)
        {
            if (posterTemplate != null)
            {
                var posterFilename = Guid.NewGuid().ToString() + Path.GetExtension(posterTemplate.FileName);
                var posterFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/public_imgs");
                
                Directory.CreateDirectory(posterFolderPath);
                var filePath = Path.Combine(posterFolderPath, posterFilename);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    posterTemplate.CopyTo(fileStream);
                }

                meeting.PosterPath = posterFilename;
            }
                
            if (certificateTemplate != null)
            {
                String certificateExtension = Path.GetExtension(certificateTemplate.FileName);
                if (certificateExtension != ".pdf")
                {
                    ModelState.AddModelError("CertificatePath", "Certificate template must be in PDF format");
                    return View(meeting);
                }

                var certificateFilename = Guid.NewGuid() + certificateExtension;
                var certificateFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "certificate template");
                Directory.CreateDirectory(certificateFolderPath);
                var filePath = Path.Combine(certificateFolderPath, certificateFilename);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    certificateTemplate.CopyTo(fileStream);
                }

                meeting.CertificatePath = certificateFilename;
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
        if (id == null) return NotFound();

        var meeting = await _context.Meeting
            .Include(meeting1 => meeting1.Registrations)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (meeting == null) return NotFound();

        return View(meeting);
    }

    [HttpGet("Meeting/{meeting_id}/SendCertificate")]
    public async Task<IActionResult> SendCertificateToAll(string meeting_id)
    {
        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        // get 50 participants
        var participants = await _context.Registration
            .Where(reg => reg.Meeting.Id == meeting_id)
            .Take(50)
            .ToListAsync();
        
        var firstParticipant = participants.First();
        
        // send certificate to participants
        String emailBody = await _csMailRenderer.RenderCSHtmlToString(this.ControllerContext, "Template/MailTemplate/ConfirmationMail", firstParticipant);

        foreach (var participant in participants)
        {
            MailData mailData = new MailData
            {
                EmailToId = participant.Email,
                EmailToName = participant.NamaLengkap,
                EmailSubject = "Registration Confirmation",
                EmailBody = emailBody
            };
         
            var certifRender = new CSCertificateRenderer();
            var pdfBytes = await certifRender.RenderCSHtmlToPdf(this.ControllerContext, "Template/CertificateTemplate/Certificate", participant);
            
            await _mailSender.SendMailWithAttachment(mailData, pdfBytes);
        }
        
        long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Console.WriteLine("\nTime taken to send certificate to 50 participants: " + (endTime - startTime) + "ms");
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost("Meeting/{id}/CsvAttendance")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AttendanceCsv(IFormFile attendanceCsv) 
    {
       // todo: read CSV
        
    }


    private bool MeetingExists(string id)
    {
        return _context.Meeting.Any(e => e.Id == id);
    }
}