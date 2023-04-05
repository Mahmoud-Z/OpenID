using Newtonsoft.Json;
using OpenID.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OpenID.Controllers
{
    public class UserController : Controller
    {
        private List<User> _users;
        public UserController()
        {
            // Deserialize the users from the JSON file
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "userCred.json");
            string json = System.IO.File.ReadAllText(path);
            _users = JsonConvert.DeserializeObject<List<User>>(json);

        }
        public ActionResult Login()
        {
            return View();
        }
        //public ActionResult Login(string username, string password)
        //{
        //    // Check if the user credentials are valid
        //    User user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError("", "Invalid username or password");
        //        return View();
        //    }

        //    // Create a ticket with the user's name and roles
        //    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
        //        version: 1,
        //        name: user.Username,
        //        issueDate: DateTime.Now,
        //        expiration: DateTime.Now.AddMinutes(30),
        //        isPersistent: false,
        //        userData: user.email
        //    );

        //    // Encrypt the ticket and create a cookie with the encrypted value
        //    string encryptedTicket = FormsAuthentication.Encrypt(ticket);
        //    HttpCookie cookie = new HttpCookie("Cookie1", encryptedTicket);
        //    Response.Cookies.Add(cookie);

        //    return RedirectToAction("Index", "Home");
        //}
        public ActionResult Admin()
        {
            return View();
        }
        public void Loginn(string returnUrl = "/")
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge();
                return;
            }
            else
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    var emailClaim = identity.FindFirst(ClaimTypes.Email);
                    User user = _users.FirstOrDefault(u => u.email == emailClaim?.Value);
                    if (user != null)
                    {
                        // Create a ticket with the user's name and roles
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                            version: 1,
                            name: user.Username,
                            issueDate: DateTime.Now,
                            expiration: DateTime.Now.AddMinutes(30),
                            isPersistent: false,
                            userData: user.email
                        );

                        // Encrypt the ticket and create a cookie with the encrypted value
                        string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                        HttpCookie cookie = new HttpCookie("Cookie1", encryptedTicket);
                        Response.Cookies.Add(cookie);

                        Response.Redirect("/");
                    }
                    else
                    {
                        var authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                        HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());

                        ModelState.AddModelError("", "This Email is not existed in our database");
                        Response.Redirect("/User/Login");
                    }
                }


            }

        }
        public void LogOff()
        {
            if (Request.IsAuthenticated)
            {
                var authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
            }
            Response.Redirect("/");
        }
        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }

    }
}