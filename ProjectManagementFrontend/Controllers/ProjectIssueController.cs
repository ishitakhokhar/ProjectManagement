using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;
using ProjectManagementFrontend.Models.DTO;
using System.Text;

namespace ProjectManagementFrontend.Controllers
{
    public class ProjectIssueController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProjectIssueController> _logger;

        public ProjectIssueController(IHttpClientFactory httpClientFactory, ILogger<ProjectIssueController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7236/api/");
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("ProjectIssue");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error fetching issues: {response.StatusCode}");
                    return View(new List<ProjectIssueDTO>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<ProjectIssueDTO>>(json);

                return View(data ?? new List<ProjectIssueDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching issues.");
                return View(new List<ProjectIssueDTO>());
            }
        }


        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            var model = new PRJ_IssuesModel();
            try
            {
                await LoadDropdowns(model);

                if (id.HasValue && id > 0)
                {
                    var response = await _httpClient.GetAsync($"ProjectIssue/{id.Value}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Issue not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var issueDto = JsonConvert.DeserializeObject<ProjectIssueDTO>(json);
                    if (issueDto != null)
                    {
                        model.IssueId = issueDto.IssueId;
                        model.Title = issueDto.Title;
                        model.Description = ""; // Description not in DTO
                        model.Priority = issueDto.Priority;
                        model.StatusName = issueDto.StatusName;
                        model.ProjectName = issueDto.ProjectName;
                        model.CreatedByName = issueDto.CreatedByName;
                        model.AssignedToName = issueDto.AssignedToName;
                        model.RaisedOn = issueDto.RaisedOn;
                        model.DueDate = issueDto.DueDate;
                        model.Attachment1 = issueDto.Attachment1;
                        model.Attachment2 = issueDto.Attachment2;
                    }

                    // reload dropdowns for form
                    await LoadDropdowns(model);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading issue form ID {id}");
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction(nameof(Index));
            }
        }

        // =========================
        // POST: Add/Edit
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(PRJ_IssuesModel model)
        {
            try
            {
                await LoadDropdowns(model);

                if (!ModelState.IsValid)
                    return View(model);

                HttpResponseMessage response;

                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(model.IssueId.ToString()), "IssueId");
                formData.Add(new StringContent(model.ProjectId.ToString()), "ProjectId");
                formData.Add(new StringContent(model.Title ?? ""), "Title");
                formData.Add(new StringContent(model.Description ?? ""), "Description");
                formData.Add(new StringContent(model.StatusId.ToString()), "StatusId");
                formData.Add(new StringContent(model.Priority ?? ""), "Priority");
                formData.Add(new StringContent(model.CreatedBy.ToString()), "CreatedBy");
                formData.Add(new StringContent(model.AssignedTo?.ToString() ?? ""), "AssignedTo");
                formData.Add(new StringContent(model.RaisedOn.ToString("yyyy-MM-dd")), "RaisedOn");
                if (model.DueDate.HasValue)
                    formData.Add(new StringContent(model.DueDate.Value.ToString("yyyy-MM-dd")), "DueDate");

                // File uploads
                if (model.File1 != null)
                    formData.Add(new StreamContent(model.File1.OpenReadStream()), "Attachment1", model.File1.FileName);

                if (model.File2 != null)
                    formData.Add(new StreamContent(model.File2.OpenReadStream()), "Attachment2", model.File2.FileName);

                if (model.IssueId == 0)
                    response = await _httpClient.PostAsync("ProjectIssue", formData);
                else
                    response = await _httpClient.PutAsync($"ProjectIssue/{model.IssueId}", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Issue saved successfully!";
                    return RedirectToAction(nameof(Index));
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to save issue: {errorContent}");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddEdit POST");
                ModelState.AddModelError("", "Unexpected error occurred.");
                return View(model);
            }
        }



        // =========================
        // Delete Issue
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"ProjectIssue/{id}");
                TempData["Success"] = response.IsSuccessStatusCode ? "Issue deleted." : "Failed to delete issue.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting issue {id}");
                TempData["Error"] = "Unexpected error while deleting.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ProjectIssue/MyIssues
        public async Task<IActionResult> MyIssues()
        {
            try
            {
                int userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                var response = await _httpClient.GetAsync($"ProjectIssue/My/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to load issues.";
                    return View("Index", new List<ProjectIssueDTO>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<ProjectIssueDTO>>(json);

                return View("Index", data ?? new List<ProjectIssueDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching My Issues.");
                return View("Index", new List<ProjectIssueDTO>());
            }
        }


        // =========================
        // Load Dropdowns
        // =========================
        private async Task LoadDropdowns(PRJ_IssuesModel model)
        {
            // Projects
            var projResp = await _httpClient.GetAsync("Project/DropDown");
            if (projResp.IsSuccessStatusCode)
            {
                var projects = await projResp.Content.ReadFromJsonAsync<List<PRJ_ProjectModel>>() ?? new();
                model.ProjectList = projects.Select(p => new SelectListItem
                {
                    Text = p.ProjectName,
                    Value = p.ProjectId.ToString()
                }).ToList();
            }

            // Statuses
            var statusResp = await _httpClient.GetAsync("IssueStatus/StatusDropdown");
            if (statusResp.IsSuccessStatusCode)
            {
                var statuses = await statusResp.Content.ReadFromJsonAsync<List<MST_StatusModel>>() ?? new();
                model.StatusList = statuses.Select(s => new SelectListItem
                {
                    Text = s.StatusName,
                    Value = s.StatusId.ToString()
                }).ToList();
            }

            // Users
            var userResp = await _httpClient.GetAsync("User/DropDown");
            if (userResp.IsSuccessStatusCode)
            {
                var users = await userResp.Content.ReadFromJsonAsync<List<SEC_Users>>() ?? new();
                model.UserList = users.Select(u => new SelectListItem
                {
                    Text = u.FullName,
                    Value = u.UserId.ToString()
                }).ToList();
            }
        }
    }
}