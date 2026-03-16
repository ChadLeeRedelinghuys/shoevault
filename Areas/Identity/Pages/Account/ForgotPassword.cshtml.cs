using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _config;


    public ForgotPasswordModel(
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender, IConfiguration config)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _config = config;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email!);

        if (user == null)
        {
            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

        var baseUrl = _config["AppSettings:BaseUrl"];

        var callbackUrl = $"{baseUrl}/Identity/Account/ResetPassword?code={Uri.EscapeDataString(code)}";


        await _emailSender.SendEmailAsync(
    Input.Email!,
    "ShoeVault Password Reset",
    $@"
    <div style='font-family: Arial, sans-serif; line-height:1.6; color:#333'>
        <h2 style='color:#111;'>ShoeVault</h2>

        <p>Hello,</p>

        <p>We received a request to reset your <strong>ShoeVault</strong> password.</p>

        <p>Click the button below to choose a new password:</p>

        <p style='margin:30px 0;'>
            <a href='{callbackUrl}' 
               style='background-color:#111;
                      color:#fff;
                      padding:12px 20px;
                      text-decoration:none;
                      border-radius:6px;
                      font-weight:bold;'>
                Reset Your Password
            </a>
        </p>

        <p>If you did not request this password reset, you can safely ignore this email.</p>

        <p style='margin-top:30px; font-size:14px; color:#777'>
            — ShoeVault Security
        </p>
    </div>
    ");


        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
