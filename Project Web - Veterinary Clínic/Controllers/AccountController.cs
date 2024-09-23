using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using Project_Web___Veterinary_Clínic.Migrations;
using Project_Web___Veterinary_Clínic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IConfiguration _configuration;

        public AccountController(IUserHelper userHelper,
            IEmailHelper emailHelper,
            IConfiguration configuration)
        {
            _userHelper = userHelper;
            _emailHelper = emailHelper;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }
                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        //public async Task<IActionResult> Register()
        //{
        //    var model = new RegisterNewUserViewModel
        //    {
        //        Roles = await _userHelper.GetRolesSelectListAsync() 
        //    };
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userHelper.GetUserByEmailAsync(model.Username);

        //        if (user == null)
        //        {
        //            user = new User
        //            {
        //                FirstName = model.FirstName,
        //                LastName = model.LastName,
        //                Email = model.Username,
        //                UserName = model.Username,
        //                Address = model.Address,
        //                PhoneNumber = model.PhoneNumber
        //            };


        //            var result = await _userHelper.AddUserAsync(user, model.Password);

        //            if (result != IdentityResult.Success)
        //            {
        //                ModelState.AddModelError(string.Empty, "The user couldn't be created");
        //                return View(model);
        //            }


        //            if (!string.IsNullOrEmpty(model.SelectedRole))
        //            {
        //                await _userHelper.AddUserToRoleAsync(user, model.SelectedRole);
        //            }


        //            var loginViewModel = new LoginViewModel
        //            {
        //                Password = model.Password,
        //                RememberMe = false,
        //                Username = model.Username
        //            };

        //            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

        //            string tokenLink = Url.Action("ConfirmEmail", "Account", new
        //            {
        //                userId = user.Id,
        //                token = myToken,

        //            }, protocol: HttpContext.Request.Scheme);


        //            Response response = _emailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email confirmation</h1>" +
        //                   $"To allow the user," +
        //                   $"please click on this link:</br></br><a href= \"{tokenLink}\">Confirm Email</a>");

        //            if (response.IsSuccess)
        //            {
        //                //ViewBag.Message = "The instructions to grant access to your user have been sent to your email";
        //                TempData["SuccessMessage"] = "The instructions to grant access to your user have been sent to your email";
        //                //return View(model);
        //                return RedirectToAction("Login", "Account");
        //            }
        //            ModelState.AddModelError(string.Empty, "The user couldn't be logged");
        //        }
        //    }
        //    model.Roles = await _userHelper.GetRolesSelectListAsync();
        //    return View(model);
        //}

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            var model = new ChangeUserViewModel();

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    var response = await _userHelper.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found");
                }
            }
            return this.View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't match a registered username.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken, email = user.UserName }, protocol: HttpContext.Request.Scheme);

                Response response = _emailHelper.SendEmail(model.Email, "Shop Password Reset", $"<h1>Shop Password Reset</h1>" +
                    $"To reset the password click on the link:</br></br>" +
                    $"<a href= \"{link}\">Reset Password</a>");

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to recover your password have been sent to your email";
                }

                return View();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {         
                Username = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Username);

            if (user != null)
            {                
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);

                if (!result.Succeeded)
                {
                    ViewBag.Message = "Password reset failed.";
                    return View();
                }

                if (!user.EmailConfirmed)
                {
                    var confirmToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                    result = await _userHelper.ConfirmEmailAsync(user, confirmToken);

                    if (result.Succeeded)
                    {
                        ViewBag.Message = "Password successfully reset and account confirmed.";
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = "Password successfully reset but account confirmation failed!";
                        return View();
                    }
                }
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password successfully reset.";
                    return View();
                }

                //ViewBag.Message = "Error resetting the password";
                //return View(model);
            }
            ViewBag.Message = "Username not found.";
            return View(model);
        }


        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
