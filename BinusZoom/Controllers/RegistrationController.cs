using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BinusZoom.Data;
using BinusZoom.Models;
using BinusZoom.Service.CertificateService;
using BinusZoom.Service.EmailService;
using BinusZoom.Template.MailTemplate;

namespace BinusZoom.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly BinusZoomContext _context;
        private readonly CSMailRenderer _csMailRenderer;
        private readonly MailSender _mailSender;

        public RegistrationController(BinusZoomContext context, CSMailRenderer csMailRenderer, MailSender mailSender)
        {
            _context = context;
            _csMailRenderer = csMailRenderer;
            _mailSender = mailSender;
        }

        // GET: Registration/Details/5
        [HttpGet("Registration/Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registration
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // GET: Registration/Create
        [HttpGet("Registration/{event_id}")]
        public IActionResult Create(String event_id)
        {
            Meeting? meeting = _context.Meeting
                .Where(meeting1 => meeting1.Id == event_id)
                .Where(meeting1 => meeting1.MeetingDate > DateTime.Now)
                .FirstOrDefault();
            if (meeting == null)
            {
                // redirect to available meeting list
                return RedirectToAction("Index", "Meeting");
            }

            ViewData["Meeting"] = meeting;
            return View();
        }

        // POST: Registration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Registration/{eventId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NIM,Email,NamaLengkap")] Registration registration,
            String eventId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Registration? existedRegistration = _context.Registration
                        .Where(r => r.Email == registration.Email)
                        .FirstOrDefault(r => r.Meeting != null && r.Meeting.Id == eventId);

                    if (existedRegistration != null)
                        return RedirectToAction(nameof(Confirmation), new { registration_id = existedRegistration.Id });

                    Meeting? targetMeeting = await _context.Meeting.FindAsync(eventId);
                    if(targetMeeting == null)
                        return RedirectToAction("Index", "Meeting");
                    
                    registration.Meeting = targetMeeting;
                    await _context.AddAsync(registration);
                    await _context.SaveChangesAsync();

                    String emailBody = await _csMailRenderer.RenderCSHtmlToString(this.ControllerContext,
                        "Template/MailTemplate/ConfirmationMail", registration);
                    MailData mailData = new MailData
                    {
                        EmailToId = registration.Email,
                        EmailToName = registration.NamaLengkap,
                        EmailSubject = "Registration Confirmation",
                        EmailBody = emailBody
                    };

                    await _mailSender.SendMail(mailData);
                }
                catch (Exception e)
                {
                    return View(registration);
                }

                return RedirectToAction(nameof(Confirmation), new { registration_id = registration.Id });
            }

            return View(registration);
        }

        // GET: Registration/Confirmation
        [HttpGet("Registration/{registration_id}/Confirmation")]
        public async Task<IActionResult> Confirmation(String registration_id)
        {
            var registration = await _context.Registration
                .Include(r => r.Meeting)
                .FirstOrDefaultAsync(registration1 => registration1.Id == registration_id);

            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // GET: Registration/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registration.FindAsync(id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // POST: Registration/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,NIM,Email,NamaLengkap")] Registration registration)
        {
            if (id != registration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationExists(registration.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index), "Meeting");
            }

            return View(registration);
        }

        // GET: Registration/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.Registration
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // POST: Registration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var registration = await _context.Registration.FindAsync(id);
            if (registration != null)
            {
                _context.Registration.Remove(registration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Meeting");
        }

        private bool RegistrationExists(string id)
        {
            return _context.Registration.Any(e => e.Id == id);
        }

        // GET: Registration/5/Certificate
        [HttpGet("Registration/{id}/Certificate")]
        public async Task<IActionResult> SendCertificateTo(string? id)
        {
            if (id == null) return NotFound();

            var registration = await _context.Registration
                .Include(registration1 => registration1.Meeting)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registration == null) return NotFound();

            if (registration.EligibleForCertificate)
            {
                try
                {
                    String emailBody = await _csMailRenderer.RenderCSHtmlToString(this.ControllerContext,
                        "Template/MailTemplate/CertificateMail", registration);
                    MailData mailData = new MailData
                    {
                        EmailToId = registration.Email,
                        EmailToName = registration.NamaLengkap,
                        EmailSubject = "Registration Confirmation",
                        EmailBody = emailBody
                    };
                    var certifRender = new CSCertificateRenderer();
                    var pdfBytes = await certifRender.RenderCSHtmlToPdf(this.ControllerContext,
                        "Template/CertificateTemplate/Certificate", registration);

                    await _mailSender.SendMailWithAttachment(mailData, pdfBytes);
                }
                catch (Exception e)
                {
                    // todo: write exception
                }

                return RedirectToAction("Participants", "Meeting", new { id = registration.Meeting.Id });
            }

            return RedirectToAction("Participants", "Meeting", new { id = registration.Meeting.Id });
        }
    }
}