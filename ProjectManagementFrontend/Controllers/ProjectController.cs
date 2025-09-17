using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;
using System.Text;

namespace ProjectManagementFrontend.Controllers
{
    public class ProjectController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IHttpClientFactory httpClientFactory, ILogger<ProjectController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7236/api/");
            _logger = logger;
        }

        // ==========================
        // Normal Page Load (Full Page)
        // ==========================
        public async Task<IActionResult> Index(string search = "", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var url = $"Project?search={search}&pageNumber={pageNumber}&pageSize={pageSize}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Failed to fetch projects. StatusCode: {response.StatusCode}";
                    return View(new List<PRJ_ProjectModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);

                var projects = JsonConvert.DeserializeObject<List<PRJ_ProjectModel>>(Convert.ToString(result.data));

                ViewBag.TotalPages = (int)result.totalPages;
                ViewBag.CurrentPage = (int)result.pageNumber;
                ViewBag.PageSize = (int)result.pageSize;
                ViewBag.Search = search;

                return View(projects);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An unexpected error occurred while fetching projects.";
                _logger.LogError(ex, "Error fetching project list.");
                return View(new List<PRJ_ProjectModel>());
            }
        }

       
        public async Task<IActionResult> LoadProjects(string search = "", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var url = $"Project?search={search}&pageNumber={pageNumber}&pageSize={pageSize}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return PartialView("_ProjectsTable", new List<PRJ_ProjectModel>());

                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);

                var projects = JsonConvert.DeserializeObject<List<PRJ_ProjectModel>>(Convert.ToString(result.data));

                ViewBag.TotalPages = (int)result.totalPages;
                ViewBag.CurrentPage = (int)result.pageNumber;
                ViewBag.PageSize = (int)result.pageSize;
                ViewBag.Search = search;

                return PartialView("_ProjectsTable", projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading projects via AJAX.");
                return PartialView("_ProjectsTable", new List<PRJ_ProjectModel>());
            }
        }

      
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Project/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Project deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to delete project.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
        }


        // ==========================
        // Normal Add/Edit Page Load
        // ==========================
        public async Task<IActionResult> AddEdit(int? id)
        {
            PRJ_ProjectModel model = new PRJ_ProjectModel();
            try
            {
                // Load Project if editing
                if (id != null)
                {
                    var response = await _httpClient.GetAsync($"Project/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Project not found.";
                        return RedirectToAction("Index");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<PRJ_ProjectModel>(json);
                }

                // Load Status Dropdown
                var statuses = await GetStatusDropdownAsync();
                ViewBag.StatusList = new SelectList(statuses, "StatusId", "StatusName");

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading project form for ID {id}");
                TempData["Error"] = "Unable to load project form.";
                return RedirectToAction("Index");
            }
        }


        // ==========================
        // Normal Add/Edit Submit
        // ==========================
        [HttpPost]
        public async Task<IActionResult> AddEdit(PRJ_ProjectModel project)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model validation failed. Errors: {Errors}",
                        string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return View(project);
                }

                var userIdString = HttpContext.Session.GetString("UserId");
                if (!int.TryParse(userIdString, out int userId))
                {
                    TempData["Error"] = "Invalid user session. Please login again.";
                    return RedirectToAction("Login", "User");
                }

                project.CreatedBy = userId;

                var apiModel = new
                {
                    ProjectId = project.ProjectId,
                    ProjectName = project.ProjectName,
                    ProjectDescription = project.ProjectDescription,
                    ClientName = project.ClientName,
                    ProjectManagerName = project.ProjectManagerName,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    ProjectStatus = project.ProjectStatus,
                    Visibility = project.Visibility,
                    CreatedBy = project.CreatedBy
                };

                var jsonContent = JsonConvert.SerializeObject(apiModel);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (project.ProjectId == 0)
                {
                    response = await _httpClient.PostAsync("Project", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"Project/{project.ProjectId}", content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API Error on project save. Status: {StatusCode}. Message: {Message}",
                        response.StatusCode, errorMsg);

                    ModelState.AddModelError("", $"Failed to save the project. API returned: {errorMsg}");
                    return View(project);
                }

                TempData["Success"] = project.ProjectId == 0
                    ? "Project added successfully!"
                    : "Project updated successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving project.");
                ModelState.AddModelError("", "An unexpected error occurred.");
                return View(project);
            }
        }

        public async Task<List<MST_StatusModel>> GetStatusDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("IssueStatus/StatusDropdown");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to load StatusDropdown. StatusCode: {StatusCode}", response.StatusCode);
                    return new List<MST_StatusModel>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var statuses = JsonConvert.DeserializeObject<List<MST_StatusModel>>(json);
                return statuses ?? new List<MST_StatusModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching StatusDropdown");
                return new List<MST_StatusModel>();
            }
        }

    }
}
