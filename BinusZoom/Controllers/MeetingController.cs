using System.Globalization;
using BinusZoom.Data;
using BinusZoom.Models;
using BinusZoom.Service.CertificateService;
using BinusZoom.Service.EmailService;
using BinusZoom.Service.ZoomService.DTO;
using BinusZoom.Template.MailTemplate;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom.Controllers;

[Authorize]
public class MeetingController : Controller
{
    private readonly BinusZoomContext _context;
    private readonly CSMailRenderer _csMailRenderer;
    private readonly MailSender _mailSender;

    public MeetingController(BinusZoomContext context, CSMailRenderer csMailRenderer, MailSender mailSender)
    {
        _context = context;
        _csMailRenderer = csMailRenderer;
        _mailSender = mailSender;
    }

    [AllowAnonymous]
    // GET: Meeting
    public async Task<IActionResult> Index(string titleFilter, string dateFilter, string endDateFilter)
    {
        var meetings = from m in _context.Meeting
            select m;

        if (!String.IsNullOrEmpty(titleFilter))
        {
            meetings = meetings.Where(s => s.Title.Contains(titleFilter));
        }

        if (!String.IsNullOrEmpty(dateFilter))
        {
            meetings = meetings.Where(s => s.MeetingDate.ToString().Contains(dateFilter));
        }

        if (!User.Identity.IsAuthenticated)
        {
            meetings = meetings.Where(meeting => meeting.MeetingDate > DateTime.Now);
        }

        return View(await meetings.ToListAsync());
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
        [Bind("Title, MeetingDate, PosterPath, Description, LinkUrl")]
        Meeting meeting,
        IFormFile posterTemplate,
        IFormFile certificateTemplate)
    {
        if (ModelState.IsValid)
        {
            if (posterTemplate != null)
            {
                var posterFilename = Guid.NewGuid() + Path.GetExtension(posterTemplate.FileName);
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
                var certificateExtension = Path.GetExtension(certificateTemplate.FileName);
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
        [Bind("Id, MeetingDate, PosterPath, LinkUrl")]
        Meeting meeting)
    {
        Meeting? meetingTarget = await _context.Meeting.FindAsync(id);
        if (meetingTarget != null)
        {
            try
            {
                meetingTarget.MeetingDate = meeting.MeetingDate;
                meetingTarget.LinkUrl = meeting.LinkUrl;
                
                _context.Update(meetingTarget);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingExists(meeting.Id))
                    return NotFound();
                throw;
            }
            
            // send notification to all attendees
            var emailBody = await _csMailRenderer.RenderCSHtmlToString(ControllerContext,
                "Template/MailTemplate/MeetingUpdateMail", meetingTarget);
            
            var participants = await _context.Registration
                .Where(reg => reg.Meeting.Id == id)
                .ToListAsync();

            foreach (var participant in participants)
            {
                var mailData = new MailData
                {
                    EmailToId = participant.Email,
                    EmailToName = participant.NamaLengkap,
                    EmailSubject = "Webinar Update Information",
                    EmailBody = emailBody
                };
                await _mailSender.SendMail(mailData);
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
        var startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var participants = await _context.Registration
            .Where(reg => reg.Meeting.Id == meeting_id)
            .Where(reg => reg.EligibleForCertificate == true)
            .ToListAsync();

        if (participants.Count > 0)
        {
            var firstParticipant = participants.First();

            // send certificate to participants
            var emailBody = await _csMailRenderer.RenderCSHtmlToString(ControllerContext,
                "Template/MailTemplate/ConfirmationMail", firstParticipant);

            foreach (var participant in participants)
            {
                var mailData = new MailData
                {
                    EmailToId = participant.Email,
                    EmailToName = participant.NamaLengkap,
                    EmailSubject = "Registration Confirmation",
                    EmailBody = emailBody
                };

                var certifRender = new CSCertificateRenderer();
                var pdfBytes = await certifRender.RenderCSHtmlToPdf(ControllerContext,
                    "Template/CertificateTemplate/Certificate", participant);

                await _mailSender.SendMailWithAttachment(mailData, pdfBytes);
            }

            Meeting? currentMeeting = _context.Meeting.Find(meeting_id);
            if (currentMeeting != null)
            {
                currentMeeting.hasSendCertificateToAll = true;
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            TempData["Alert"] = "Tidak ada peserta yang eligible";
        }

        return await Participants(meeting_id);
    }

    [HttpPost("Meeting/{id}/CsvAttendance")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AttendanceCsv(String id, IFormFile? attendanceCsv)
    {
        if (attendanceCsv != null)
        {
            var sr = new StreamReader(attendanceCsv.OpenReadStream());
            await sr.ReadLineAsync();
            await sr.ReadLineAsync();
            await sr.ReadLineAsync();

            var csv = new CsvReader(sr, CultureInfo.InvariantCulture);
            await csv.ReadAsync();
            csv.ReadHeader();

            var listOfAttendees = new List<Participants>();
            while (await csv.ReadAsync())
            {
                var obj = new Participants
                {
                    UserEmail = csv.GetField<string>("User Email"),
                    Duration = csv.GetField<int>("Duration (Minutes)")
                };
                listOfAttendees.Add(obj);
            }
        

            var result = listOfAttendees.AsEnumerable();

            // set all to not eligible
            await _context.Registration
                .Where(registration => registration.Meeting.Id == id)
                .ExecuteUpdateAsync(calls =>
                    calls.SetProperty( registration => registration.EligibleForCertificate, false)
                );
        
            var attendeesListOver35Minutes = from attendee in result
                where attendee.Duration >= 35
                group attendee by attendee.UserEmail
                into g
                select new
                {
                    UserEmail = g.Key,
                    TotalDuration = g.Max(attendee => attendee.Duration)
                };

            // find all attendeesListOver35Minutes where email is in the list of Registration table
            var studentResult = _context.Registration
                .Where(registration => attendeesListOver35Minutes
                    .Select(attendee => attendee.UserEmail)
                    .Contains(registration.Email));
  
            foreach (var x1 in studentResult)
            {
                x1.EligibleForCertificate = true;
            }

            await _context.SaveChangesAsync();
        }
        
        return RedirectToAction(nameof(Participants), new { id = id});
    }


    private bool MeetingExists(string id)
    {
        return _context.Meeting.Any(e => e.Id == id);
    }
}