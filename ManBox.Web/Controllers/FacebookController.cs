using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManBox.Business;
using ManBox.Common;
using ManBox.Common.Logging;
using ManBox.Model.ViewModels;
using Microsoft.Web.WebPages.OAuth;
using System.Text;
using System.Linq.Expressions;

namespace ManBox.Web.Controllers
{
    public class FacebookController : ManBoxControllerBase
    {
        private readonly IUserRepository userRepository;

        public FacebookController(IUserRepository userRepoInject)
        {
            userRepository = userRepoInject;
        }

        [HttpPost]
        public ActionResult Login(string provider)
        {
            OAuthWebSecurity.RequestAuthentication(provider, Url.Action("LoginCallback", "Facebook"));

            return new EmptyResult();
        }

        public ActionResult LoginCallback()
        {
            try
            {
                var result = OAuthWebSecurity.VerifyAuthentication();
                if (result.IsSuccessful)
                {
                    var fullName = result.ExtraData["name"];
                    string firstName = string.Empty;
                    string lastName = string.Empty;

                    if (fullName.Contains(' '))
                    {
                        firstName = fullName.Substring(0, fullName.IndexOf(" "));
                        lastName = fullName.Substring(fullName.IndexOf(" ") + 1);
                    }
                    else
                    {
                        firstName = fullName;
                    }

                    var fbUser = userRepository.Register(
                        new UserViewModel()
                        {
                            Email = result.ExtraData["email"],
                            FirstName = firstName,
                            LastName = lastName
                        },
                        CodeValues.SignInType.Facebook,
                        this.CurrentSubscriptionToken
                    );

                    this.Authenticate(fbUser);

                    return RedirectToAction("Compose", "Box");
                }
            }
            catch (Exception e) 
            {
                this.Log(LogType.Error, e.Message);
            }

            // in case it fails silently
            this.Log(LogType.Warn, "Facebook login failed.");


            return View("Error");
        }
    }
}
