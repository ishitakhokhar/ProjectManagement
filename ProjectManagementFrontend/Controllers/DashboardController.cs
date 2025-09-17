using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;

namespace ProjectManagementFrontend.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("UserRole") ?? "User";

            if (!int.TryParse(userIdString, out int userId) || userId <= 0)
            {
                return RedirectToAction("Login", "User");
            }

            DashboardCountsDTO counts = new DashboardCountsDTO();

            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7236/");
                client.DefaultRequestHeaders.Clear();

                // send headers required by API
                client.DefaultRequestHeaders.Add("userId", userId.ToString());
                client.DefaultRequestHeaders.Add("role", role);

                var response = await client.GetAsync("api/Dashboard/counts");
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    counts = JsonConvert.DeserializeObject<DashboardCountsDTO>(json) ?? new DashboardCountsDTO();
                }
                else
                {
                    ViewBag.Error = "Failed to load dashboard: " + json;
                }

                // Debug log
                Console.WriteLine($"[DASHBOARD DEBUG] Role={role}, UserId={userId}, " +
                                  $"Projects={counts.TotalProjects}, Sprints={counts.TotalSprints}, " +
                                  $"Issues={counts.TotalIssues}, Comments={counts.TotalComments}");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error while loading dashboard: " + ex.Message;
            }

            // return role-based view
            return role.ToLower() switch
            {
                "admin" => View("AdminDashboard", counts),
                "projectmanager" => View("ProjectManagerDashboard", counts),
                "pm" => View("ProjectManagerDashboard", counts),
                _ => View("UserDashboard", counts)
            };
        }
    }
}
