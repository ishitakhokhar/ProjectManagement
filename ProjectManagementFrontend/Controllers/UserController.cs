using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;
using System.Text;


namespace ProjectManagementFrontend.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7236/");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // -------------------- REGISTER --------------------
        [HttpGet]
        public IActionResult Register() => View("~/Views/User/Register.cshtml");

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // JSON must exactly match backend property names
            
            
                var body = new
                {
                    fullName = model.FullName,
                    email = model.Email,
                    password = model.Password,
                    isActive = true
                };

      


            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/User/Register", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful. Please login.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Registration failed: " + responseContent;
            return View(model);
        }

        // -------------------- LOGIN --------------------
        [HttpGet]
        public IActionResult Login() => View("~/Views/User/Login.cshtml");

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var body = new
            {
                email = Email,
                password = Password
            };

            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/User/Login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

                // Safely store values in session
                if (data.TryGetValue("token", out var token))
                    _httpContextAccessor.HttpContext.Session.SetString("JWTToken", token.ToString());

                if (data.TryGetValue("fullName", out var fullName))
                    _httpContextAccessor.HttpContext.Session.SetString("FullName", fullName.ToString());

                if (data.TryGetValue("role", out var roleObj))
                {
                    var role = roleObj.ToString() ?? "User";
                    _httpContextAccessor.HttpContext.Session.SetString("UserRole", role);

                    // Check for userId (handle both camelCase and PascalCase); default to "0" if missing
                    if (data.TryGetValue("userId", out var userId) || data.TryGetValue("UserId", out userId))
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("UserId", userId?.ToString() ?? "0");
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("UserId", "0");
                    }

                    // Redirect to Dashboard (single view for all roles)
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.Error = "Login failed: Role information missing.";
                    return View("~/Views/User/Login.cshtml");
                }
            }

            ViewBag.Error = "Invalid credentials: " + responseContent;
            return View("~/Views/User/Login.cshtml");
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            AddJwtToken(); // attach JWT to header

            var response = await _client.GetAsync("api/User/profile");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load profile.";
                return RedirectToAction("Index", "Dashboard");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<ProfileModel>(responseContent);

            return View("~/Views/User/Profile.cshtml", user);
        }


        [HttpPost]
        public async Task<IActionResult> Profile(ProfileModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            AddJwtToken();

            var body = new
            {
                fullName = model.FullName,
                email = model.Email,
                newPassword = model.NewPassword // optional
            };

            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/User/profile", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            TempData["Error"] = "Update failed: " + responseContent;
            return View(model);
        }
        // -------------------- LOGOUT --------------------
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
