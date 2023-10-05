using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Models
{
    public class LoginViewModel /*: PageModel*/
    {
        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        //private readonly ILogger<LoginViewModel> _logger;

        //public LoginViewModel(SignInManager<User> signInManager,
        //    ILogger<LoginViewModel> logger,
        //    UserManager<User> userManager)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //    _logger = logger;
        //}


        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }


        public bool RememberMe { get; set; }

        //public IList<AuthenticationScheme> ExternalLogins { get; set; }

        //public string ReturnUrl { get; set; }

        //[TempData]
        //public string ErrorMessage { get; set; }


        //public async Task OnGetAsync(string returnUrl = null)
        //{
        //    if (!string.IsNullOrEmpty(ErrorMessage))
        //    {
        //        ModelState.AddModelError(string.Empty, ErrorMessage);
        //    }

        //    returnUrl ??= Url.Content("~/");

        //    // Clear the existing external cookie to ensure a clean login process
        //    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //    ReturnUrl = returnUrl;
        //}

        //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //{
        //    returnUrl ??= Url.Content("~/");

        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //    if (ModelState.IsValid)
        //    {
        //        // This doesn't count login failures towards account lockout
        //        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        //        var result = await _signInManager.PasswordSignInAsync(Username, Password, RememberMe, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("User logged in.");
        //            return LocalRedirect(returnUrl);
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = RememberMe });
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            _logger.LogWarning("User account locked out.");
        //            return RedirectToPage("./Lockout");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //            return Page();
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return Page();
        //}
    }
}
