using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;
using System.Text;

namespace ProjectManagementFrontend.Controllers
{
    public class RoleController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IHttpClientFactory httpClientFactory, ILogger<RoleController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7236/api/");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Role");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch roles.";
                return View(new List<SEC_Role>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<SEC_Role>>(json);
            return View(list);
        }

        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Role/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Role deleted successfully.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = !string.IsNullOrWhiteSpace(errorContent)
                        ? errorContent
                        : "Failed to delete the role.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting Role with ID {id}");
                TempData["Error"] = "An unexpected error occurred while deleting the role.";
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> AddEdit(int? id)
        {
            SEC_Role model = new SEC_Role();
            try
            {
                if (id != null)
                {
                    var response = await _httpClient.GetAsync($"Role/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Role not found.";
                        return RedirectToAction("Index");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<SEC_Role>(json);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while loading form for Role ID {id}.");
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(SEC_Role role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(role);
                }

                var content = new StringContent(JsonConvert.SerializeObject(role), Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (role.RoleId == 0)
                {
                    response = await _httpClient.PostAsync("Role", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"Role/{role.RoleId}", content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to save the role.");
                    return View(role);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving role.");
                ModelState.AddModelError("", "An unexpected error occurred.");
                return View(role);
            }
        }
    }
}
