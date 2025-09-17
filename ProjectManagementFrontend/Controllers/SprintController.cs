using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;
using System.Text;

namespace ProjectManagementFrontend.Controllers
{
    public class SprintController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SprintController> _logger;

        public SprintController(IHttpClientFactory httpClientFactory, ILogger<SprintController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7236/api/");
            _logger = logger;
        }

        // GET: Sprint List
        public async Task<IActionResult> Index(string search = "", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync("Sprint");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to fetch sprints.";
                    return View(new List<SPR_Sprint>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<SPR_Sprint>>(json);
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sprint list.");
                TempData["Error"] = "An unexpected error occurred while fetching sprints.";
                return View(new List<SPR_Sprint>());
            }
        }

        // GET: Add / Edit Sprint
        public async Task<IActionResult> AddEdit(int? id)
        {
            SPR_Sprint model = new SPR_Sprint
            {
                ProjectList = await LoadProjectDropdownAsync()
            };

            if (id != null)
            {
                var response = await _httpClient.GetAsync($"Sprint/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var sprint = JsonConvert.DeserializeObject<SPR_Sprint>(json);

                    if (sprint != null)
                    {
                        model.SprintId = sprint.SprintId;
                        model.SprintName = sprint.SprintName;
                        model.ProjectId = sprint.ProjectId;
                        model.StartDate = sprint.StartDate;
                        model.EndDate = sprint.EndDate;
                        model.IsCompleted = sprint.IsCompleted;
                        model.TotalTasks = sprint.TotalTasks;
                        //model.CompletedTasks = sprint.CompletedTasks;
                    }
                }
                else
                {
                    TempData["Error"] = "Sprint not found.";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        // POST: Add / Edit Sprint
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(SPR_Sprint model)
        {
            model.ProjectList = await LoadProjectDropdownAsync();

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                                         .SelectMany(v => v.Errors)
                                         .Select(e => e.ErrorMessage));
                TempData["Error"] = $"Please correct the errors: {errors}";
                return View(model);
            }

          
            var apiModel = new
            {
                SprintId = model.SprintId,
                ProjectId = model.ProjectId,
                SprintName = model.SprintName,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsCompleted = model.IsCompleted,
                TotalTasks = model.TotalTasks
            };

            var jsonData = JsonConvert.SerializeObject(apiModel);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (model.SprintId == 0)
                response = await _httpClient.PostAsync("Sprint", content);
            else
                response = await _httpClient.PutAsync($"Sprint/{model.SprintId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Sprint saved successfully!";
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to save sprint: {Error}", error);
            TempData["Error"] = $"Failed to save sprint: {error}";
            return RedirectToAction("Index");
        }



        // DELETE Sprint
        public async Task<IActionResult> DeleteSprint(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Sprint/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Sprint deleted successfully.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to delete sprint: {error}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting sprint {id}");
                TempData["Error"] = "Unexpected error occurred while deleting sprint.";
            }

            return RedirectToAction("Index");
        }

        // Load project dropdown
        private async Task<List<SelectListItem>> LoadProjectDropdownAsync()
        {
            var list = new List<SelectListItem>();
            try
            {
                var response = await _httpClient.GetAsync("Project/Dropdown");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var projects = JsonConvert.DeserializeObject<List<PRJ_ProjectModel>>(json);

                    if (projects != null)
                    {
                        list = projects.Select(p => new SelectListItem
                        {
                            Value = p.ProjectId.ToString(),
                            Text = p.ProjectName
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading project dropdown");
            }

            return list;
        }

        public async Task<IActionResult> AssignToMe()
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    TempData["Error"] = "User is not logged in.";
                    return RedirectToAction("Index");
                }

                int userId = Convert.ToInt32(userIdString);

                var response = await _httpClient.GetAsync($"Sprint/My/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to fetch sprints assigned to you.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
            
                var list = JsonConvert.DeserializeObject<List<SPR_Sprint>>(json) ?? new List<SPR_Sprint>();

             
                var projects = await LoadProjectDropdownAsync();
                foreach (var sprint in list)
                {
                    sprint.ProjectList = projects;
                }

              
                return View("Index", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my sprints.");
                TempData["Error"] = "Unexpected error occurred.";
                return RedirectToAction("Index");
            }
        }




    }
}
