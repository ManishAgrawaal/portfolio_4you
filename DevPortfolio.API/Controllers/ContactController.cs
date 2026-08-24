using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPortfolio.API.Data;
using DevPortfolio.API.Models;
using DevPortfolio.API.Services;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public ContactController(
        ApplicationDbContext context,
        IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }


    // =====================================================
    // POST: api/Contact
    // PUBLIC
    //
    // 1. Save message to Database
    // 2. Send Admin Notification
    // 3. Send Visitor Confirmation
    // =====================================================

    [HttpPost]
    public async Task<IActionResult> Create(
        ContactRequest request)
    {
        // =================================================
        // VALIDATE REQUEST
        // =================================================

        if (request == null)
        {
            return BadRequest(new
            {
                message = "Invalid request."
            });
        }


        // =================================================
        // VALIDATE REQUIRED FIELDS
        // =================================================

        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Subject) ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                message = "All fields are required."
            });
        }


        try
        {
            // =================================================
            // 1. SAVE CONTACT MESSAGE TO DATABASE
            // =================================================

            var contact = new ContactMessage
            {
                Name = request.Name,
                Email = request.Email,
                Subject = request.Subject,
                Message = request.Message,
                CreatedAt = DateTime.Now
            };

            _context.ContactMessages.Add(contact);

            await _context.SaveChangesAsync();


            // =================================================
            // EMAIL STATUS
            // =================================================

            bool adminEmailSent = false;

            bool confirmationEmailSent = false;


            // =================================================
            // 2. SEND ADMIN EMAIL
            // =================================================

            try
            {
                await _emailService.SendEmailAsync(
                    request.Email,
                    request.Subject,
                    request.Message
                );

                adminEmailSent = true;
            }
            catch (Exception emailEx)
            {
                Console.WriteLine(
                    "ADMIN EMAIL ERROR: " +
                    emailEx);
            }


            // =================================================
            // 3. SEND VISITOR CONFIRMATION EMAIL
            // =================================================

            try
            {
                await _emailService.SendConfirmationEmailAsync(
                    request.Email,
                    request.Name
                );

                confirmationEmailSent = true;
            }
            catch (Exception confirmationEx)
            {
                Console.WriteLine(
                    "CONFIRMATION EMAIL ERROR: " +
                    confirmationEx);
            }


            // =================================================
            // 4. RETURN SUCCESS
            // =================================================

            return Ok(new
            {
                message =
                    "Message received successfully!",

                saved = true,

                adminEmailSent =
                    adminEmailSent,

                confirmationEmailSent =
                    confirmationEmailSent
            });
        }
        catch (Exception ex)
        {
            // =================================================
            // GENERAL ERROR
            // =================================================

            Console.WriteLine(
                "CONTACT ERROR: " +
                ex);

            return StatusCode(
                500,
                new
                {
                    message =
                        "Something went wrong.",

                    saved = false,

                    adminEmailSent = false,

                    confirmationEmailSent = false
                });
        }
    }


    // =====================================================
    // GET: api/Contact
    // ADMIN ONLY
    // Get all contact messages
    // =====================================================

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var messages =
            await _context.ContactMessages
                .OrderByDescending(
                    x => x.CreatedAt)
                .ToListAsync();

        return Ok(messages);
    }


    // =====================================================
    // GET: api/Contact/{id}
    // ADMIN ONLY
    // Get contact message by ID
    // =====================================================

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var message =
            await _context.ContactMessages
                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (message == null)
        {
            return NotFound(new
            {
                message =
                    "Contact message not found."
            });
        }

        return Ok(message);
    }


    // =====================================================
    // DELETE: api/Contact/{id}
    // ADMIN ONLY
    // Delete contact message
    // =====================================================

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var message =
            await _context.ContactMessages
                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (message == null)
        {
            return NotFound(new
            {
                message =
                    "Contact message not found."
            });
        }

        _context.ContactMessages.Remove(
            message);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message =
                "Contact message deleted successfully."
        });
    }
    // =====================================================
    // GET: api/Contact/{id}/replies
    // ADMIN ONLY
    // Get complete email conversation
    // =====================================================

    [HttpGet("{id}/replies")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReplies(int id)
    {
        var contact = await _context.ContactMessages
            .FirstOrDefaultAsync(x => x.Id == id);

        if (contact == null)
        {
            return NotFound(new
            {
                message = "Contact message not found."
            });
        }

        var replies = await _context.EmailReplies
            .Where(x => x.ContactMessageId == id)
            .OrderBy(x => x.ReceivedAt)
            .ToListAsync();

        return Ok(replies);
    }

   
    [HttpPost("{id}/reply")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReplyToVisitor(
    int id,
    AdminReplyRequest request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                message = "Reply message is required."
            });
        }

        // Find original contact message
        var contact = await _context.ContactMessages
            .FirstOrDefaultAsync(x => x.Id == id);

        if (contact == null)
        {
            return NotFound(new
            {
                message = "Contact message not found."
            });
        }

        // Find latest visitor email reply, if any
        var latestVisitorReply =
            await _context.EmailReplies
                .Where(x =>
                    x.ContactMessageId == id &&
                    !string.IsNullOrWhiteSpace(x.MessageId) &&
                    x.FromEmail != "admin@manishtechnologysolution.com")
                .OrderByDescending(x => x.ReceivedAt)
                .FirstOrDefaultAsync();

        // Prepare subject
        var subject = contact.Subject ?? "Thank you for contacting us";

        if (!subject.StartsWith(
            "Re:",
            StringComparison.OrdinalIgnoreCase))
        {
            subject = "Re: " + subject;
        }

        // Get all message IDs for email threading
        var references =
            await _context.EmailReplies
                .Where(x =>
                    x.ContactMessageId == id &&
                    !string.IsNullOrWhiteSpace(x.MessageId))
                .OrderBy(x => x.ReceivedAt)
                .Select(x => x.MessageId)
                .ToListAsync();

        string result;

        // =====================================================
        // CASE 1:
        // Visitor has already replied by email
        // =====================================================

        if (latestVisitorReply != null)
        {
            result = await _emailService.SendReplyAsync(
                contact.Email,
                subject,
                request.Message,
                latestVisitorReply.MessageId,
                references
            );
        }

        // =====================================================
        // CASE 2:
        // Visitor has NOT replied yet
        // Send normal email to original visitor
        // =====================================================

        else
        {
            result = await _emailService.SendReplyAsync(
                contact.Email,
                subject,
                request.Message,
                string.Empty,
                references
            );
        }

        // =====================================================
        // Save admin reply
        // =====================================================

        var adminReply = new EmailReply
        {
            ContactMessageId = id,
            ResendEmailId = result,
            MessageId = string.Empty,
            FromEmail = "admin@manishtechnologysolution.com",
            ToEmail = contact.Email,
            Subject = subject,
            Message = request.Message,
            ReceivedAt = DateTime.UtcNow
        };

        _context.EmailReplies.Add(adminReply);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Reply sent successfully.",
            emailId = result
        });
    }


    // =====================================================
    // ADMIN REPLY REQUEST
    // =====================================================

    public class AdminReplyRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}