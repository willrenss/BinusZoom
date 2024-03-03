using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BinusZoom.Data;
using BinusZoom.Models;
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

        // GET: Registration
        public async Task<IActionResult> Index()
        {
            return View(await _context.Registration.ToListAsync());
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
            Meeting meeting = _context.Meeting.Find(event_id);
            if (meeting == null)
            {
                return NotFound();
            }
            ViewData["Meeting"] = meeting;
            return View();
        }

        // POST: Registration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Registration/{eventId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NIM,Email,NamaLengkap")] Registration registration, String eventId)
        {
            if (ModelState.IsValid)
            {
                registration.Meeting = await _context.Meeting.FindAsync(eventId);
                _context.Add(registration);
                await _context.SaveChangesAsync();
                
                String emailBody = await _csMailRenderer.RenderCSHtmlToString(this.ControllerContext, "Template/MailTemplate/ConfirmationMail", registration);
                MailData mailData = new MailData
                {
                    EmailToId = registration.Email,
                    EmailToName = registration.NamaLengkap,
                    EmailSubject = "Registration Confirmation",
                    EmailBody = emailBody
                };
                await _mailSender.SendMail(mailData);
                    
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
                return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }

        private bool RegistrationExists(string id)
        {
            return _context.Registration.Any(e => e.Id == id);
        }
    }
}
