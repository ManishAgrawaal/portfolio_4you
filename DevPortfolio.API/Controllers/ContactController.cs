//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using DevPortfolio.API.Data;
//using DevPortfolio.API.Models;
//using DevPortfolio.API.Services;
//using Microsoft.AspNetCore.Authorization;

//[ApiController]
//[Route("api/[controller]")]
//public class ContactController : ControllerBase
//{
//    private readonly ApplicationDbContext _context;
//    private readonly IEmailService _emailService;

//    public ContactController(
//        ApplicationDbContext context,
//        IEmailService emailService)
//    {
//        _context = context;
//        _emailService = emailService;
//    }


//    // =====================================================
//    // POST: api/Contact
//    // PUBLIC
//    // Save contact message + Send Email
//    // =====================================================

//    // [HttpPost]
//    // public async Task<IActionResult> Create(ContactRequest request)
//    // {
//    //     try
//    //     {
//    //         var contact = new ContactMessage
//    //         {
//    //             Name = request.Name,
//    //             Email = request.Email,
//    //             Subject = request.Subject,
//    //             Message = request.Message,
//    //             CreatedAt = DateTime.Now
//    //         };

//    //         // Save to Database
//    //         _context.ContactMessages.Add(contact);

//    //         await _context.SaveChangesAsync();


//    //         // Send Email
//    //         await _emailService.SendEmailAsync(
//    //             request.Email,
//    //             request.Subject,
//    //             request.Message
//    //         );


//    //         return Ok(new
//    //         {
//    //             message = "Message sent successfully!"
//    //         });
//    //     }
//    //     catch (Exception ex)
//    //     {
//    //         return StatusCode(500, new
//    //         {
//    //             message = "Something went wrong.",
//    //             error = ex.Message
//    //         });
//    //     }
//    // }

//    [HttpPost]
//public async Task<IActionResult> Create(ContactRequest request)
//{
//    if (request == null)
//    {
//        return BadRequest(new
//        {
//            message = "Invalid request."
//        });
//    }

//    if (string.IsNullOrWhiteSpace(request.Name) ||
//        string.IsNullOrWhiteSpace(request.Email) ||
//        string.IsNullOrWhiteSpace(request.Subject) ||
//        string.IsNullOrWhiteSpace(request.Message))
//    {
//        return BadRequest(new
//        {
//            message = "All fields are required."
//        });
//    }

//    try
//    {
//        // ==========================================
//        // 1. SAVE TO SQL SERVER
//        // ==========================================

//        var contact = new ContactMessage
//        {
//            Name = request.Name,
//            Email = request.Email,
//            Subject = request.Subject,
//            Message = request.Message,
//            CreatedAt = DateTime.Now
//        };

//        _context.ContactMessages.Add(contact);

//        await _context.SaveChangesAsync();


//        // ==========================================
//        // 2. SEND EMAIL
//        // ==========================================

//        try
//        {
//            await _emailService.SendEmailAsync(
//                request.Email,
//                request.Subject,
//                request.Message
//            );
//        }
//        catch (Exception emailEx)
//        {
//            Console.WriteLine(
//                "EMAIL ERROR: " + emailEx.Message
//            );

//            // Database save was successful.
//            // Email failed, but don't lose the message.

//            return Ok(new
//            {
//                message = "Message saved successfully, but email notification failed.",
//                saved = true,
//                emailSent = false
//            });
//        }


//        // ==========================================
//        // 3. EVERYTHING SUCCESSFUL
//        // ==========================================

//        return Ok(new
//        {
//            message = "Message sent successfully!",
//            saved = true,
//            emailSent = true
//        });
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(
//            "CONTACT ERROR: " + ex.Message
//        );

//        return StatusCode(500, new
//        {
//            message = "Something went wrong.",
//            saved = false,
//            emailSent = false
//        });
//    }
//}


//    // =====================================================
//    // GET: api/Contact
//    // ADMIN ONLY
//    // Get all contact messages
//    // =====================================================

//    [HttpGet]
//    [Authorize(Roles = "Admin")]
//    public async Task<IActionResult> GetAll()
//    {
//        var messages = await _context.ContactMessages
//            .OrderByDescending(x => x.CreatedAt)
//            .ToListAsync();

//        return Ok(messages);
//    }


//    // =====================================================
//    // GET: api/Contact/{id}
//    // ADMIN ONLY
//    // Get contact message by ID
//    // =====================================================

//    [HttpGet("{id}")]
//    [Authorize(Roles = "Admin")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        var message = await _context.ContactMessages
//            .FirstOrDefaultAsync(x => x.Id == id);

//        if (message == null)
//        {
//            return NotFound(new
//            {
//                message = "Contact message not found."
//            });
//        }

//        return Ok(message);
//    }


//    // =====================================================
//    // DELETE: api/Contact/{id}
//    // ADMIN ONLY
//    // Delete contact message
//    // =====================================================

//    [HttpDelete("{id}")]
//    [Authorize(Roles = "Admin")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        var message = await _context.ContactMessages
//            .FirstOrDefaultAsync(x => x.Id == id);

//        if (message == null)
//        {
//            return NotFound(new
//            {
//                message = "Contact message not found."
//            });
//        }

//        _context.ContactMessages.Remove(message);

//        await _context.SaveChangesAsync();

//        return Ok(new
//        {
//            message = "Contact message deleted successfully."
//        });
//    }
//}
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
}