using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;
using System.Text;

namespace ProjectManagementFrontend.Controllers
{
    public class IssueStatusController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IssueStatusController> _logger;

        public IssueStatusController(IHttpClientFactory httpClientFactory, ILogger<IssueStatusController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7236/api/");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("IssueStatus");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<MST_StatusModel>>(json);
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in IssueStatus/Index");
                ViewBag.Error = "Failed to load issue statuses.";
                return View(new List<MST_StatusModel>());
            }
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                    return View(new MST_StatusModel());

                var response = await _httpClient.GetAsync($"IssueStatus/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<MST_StatusModel>(content);
                return View(model ?? new MST_StatusModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching status ID {id}");
                ViewBag.Error = "Unable to load status details.";
                return View(new MST_StatusModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(MST_StatusModel model)
        {
            try
            {
                HttpResponseMessage response;

                if (model.StatusId == 0)
                    response = await _httpClient.PostAsJsonAsync("IssueStatus", model);
                else
                    response = await _httpClient.PutAsJsonAsync($"IssueStatus/{model.StatusId}", model);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                ViewBag.Error = "Failed to save data.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving status.");
                ViewBag.Error = "An error occurred while saving the status.";
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"IssueStatus/{id}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting status ID {id}");
                TempData["Error"] = "Failed to delete the status.";
                return RedirectToAction("Index");
            }
        }
    }
}
