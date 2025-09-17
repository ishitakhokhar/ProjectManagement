using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;
using ProjectManagementFrontend.Models.DTO;
using System.Text;

namespace ProjectManagementFrontend.Controllers
{
    public class ProjectIssueCommentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProjectIssueCommentController> _logger;

        public ProjectIssueCommentController(IHttpClientFactory httpClientFactory, ILogger<ProjectIssueCommentController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7236/api/");
            _logger = logger;
        }

      
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("IssueComments");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to fetch comments.";
                    return View(new List<IssueCommentDTO>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<IssueCommentDTO>>(json);

                return View(data ?? new List<IssueCommentDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comments");
                TempData["Error"] = "Unexpected error occurred while fetching comments.";
                return View(new List<IssueCommentDTO>());
            }
        }


     
        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            var model = new IssueCommentDTO();

            try
            {
                if (id.HasValue && id > 0)
                {
                    var response = await _httpClient.GetAsync($"IssueComments/{id.Value}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var list = JsonConvert.DeserializeObject<List<IssueCommentDTO>>(json);

                        if (list != null && list.Any())
                            model = list.First();
                        else
                        {
                            TempData["Error"] = "Comment not found.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Failed to fetch comment.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                await LoadDropdowns(model);

            
                if (!id.HasValue || id == 0)
                {
                    var userIdString = HttpContext.Session.GetString("UserId");
                    if (!string.IsNullOrEmpty(userIdString))
                        model.CreatedBy = int.Parse(userIdString);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Add/Edit form");
                TempData["Error"] = "Unexpected error occurred while loading form.";
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(IssueCommentDTO model)
        {
            try
            {
                var role = HttpContext.Session.GetString("UserRole")?.ToLower();
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    TempData["Error"] = "User session expired. Please log in again.";
                    return RedirectToAction(nameof(Index));
                }
            
                if (role == "user")
                    model.CreatedBy = int.Parse(userIdString);

                if (!ModelState.IsValid)
                {
                    await LoadDropdowns(model);
                    TempData["Error"] = "Please fix validation errors.";
                    return View(model);
                }

             
                var apiModel = new
                {
                    model.CommentId,
                    model.IssueId,
                    model.CommentText,
                    model.IsShow,
                    model.CreatedBy,
                    model.CreatedAt
                };

                var content = new StringContent(JsonConvert.SerializeObject(apiModel), Encoding.UTF8, "application/json");

                HttpResponseMessage response = model.CommentId == 0
                    ? await _httpClient.PostAsync("IssueComments", content)
                    : await _httpClient.PutAsync($"IssueComments/{model.CommentId}", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Comment saved successfully!";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogError("API save failed: {Response}", responseContent);
                TempData["Error"] = $"Failed to save comment: {responseContent}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AddEdit POST");
                TempData["Error"] = "Unexpected error occurred while saving comment.";
                return RedirectToAction(nameof(Index));
            }
        }



        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"IssueComments/{id}");
                TempData["Success"] = response.IsSuccessStatusCode
                    ? "Comment deleted successfully."
                    : "Failed to delete comment.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting comment {id}");
                TempData["Error"] = "Unexpected error occurred while deleting comment.";
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Load dropdowns
        // =========================
        private async Task LoadDropdowns(IssueCommentDTO model)
        {
            // Status dropdown
            try
            {
                var response = await _httpClient.GetAsync("IssueStatus/StatusDropdown");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var statuses = JsonConvert.DeserializeObject<List<MST_StatusModel>>(json);

                    model.StatusList = statuses?.Select(s => new SelectListItem
                    {
                        Value = s.StatusId.ToString(),
                        Text = s.StatusName
                    }).ToList() ?? new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching StatusDropdown");
            }

            // User dropdown (only for Admin/PM)
            var role = HttpContext.Session.GetString("UserRole");
            if (role?.ToLower() != "user")
            {
                var userResp = await _httpClient.GetAsync("User/DropDown");
                if (userResp.IsSuccessStatusCode)
                {
                    var json = await userResp.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<SEC_Users>>(json);

                    model.UserList = users?.Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.FullName
                    }).ToList() ?? new List<SelectListItem>();
                }
            }

            var issuesResp = await _httpClient.GetAsync("ProjectIssue/Dropdown");
            if (issuesResp.IsSuccessStatusCode)
            {
                var json = await issuesResp.Content.ReadAsStringAsync();
                var issues = JsonConvert.DeserializeObject<List<ProjectIssueDTO>>(json);

                model.IssueList = issues?.Select(i => new SelectListItem
                {
                    Value = i.IssueId.ToString(),
                    Text = i.Title
                }).ToList() ?? new List<SelectListItem>();
            }

        }

        // =========================
        // Assign comment to logged-in user
        // =========================
        public async Task<IActionResult> AssignToMe(int id)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    TempData["Error"] = "You must be logged in to assign comments.";
                    return RedirectToAction(nameof(Index));
                }

                int userId = int.Parse(userIdString);

                var apiModel = new { AssignedTo = userId };
                var content = new StringContent(JsonConvert.SerializeObject(apiModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"IssueComments/{id}/AssignTo", content);

                TempData["Success"] = response.IsSuccessStatusCode
                    ? "Comment successfully assigned to you."
                    : "Failed to assign comment.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assigning comment {Id}", id);
                TempData["Error"] = "Unexpected error occurred while assigning comment.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
