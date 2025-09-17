using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProjectManagementFrontend.Models;
using System.Text;

public class ProjectMemberController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProjectMemberController> _logger;

    public ProjectMemberController(IHttpClientFactory httpClientFactory, ILogger<ProjectMemberController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7236/api/"); 
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("ProjectMember");
        var json = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<List<PRJ_ProjectMemberModel>>(json);
        var projects = await LoadProjectDropdownAsync();
        var users = await LoadUserDropdownAsync();

        foreach (var item in list)
        {
            item.ProjectName = projects.FirstOrDefault(p => p.Value == item.ProjectId.ToString())?.Text;
            item.FullName = users.FirstOrDefault(u => u.Value == item.UserId.ToString())?.Text;
        }
        return View(list);
    }

    public async Task<IActionResult> AddEdit(int? id)
    {
        var model = new PRJ_ProjectMemberModel
        {
            ProjectList = await LoadProjectDropdownAsync(),
            UserList = await LoadUserDropdownAsync()
        };

        if (id != null)
        {
            var response = await _httpClient.GetAsync($"ProjectMember/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<PRJ_ProjectMemberModel>(json);
                model.ProjectList = await LoadProjectDropdownAsync();
                model.UserList = await LoadUserDropdownAsync();
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddEdit(PRJ_ProjectMemberModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ProjectList = await LoadProjectDropdownAsync();
            model.UserList = await LoadUserDropdownAsync();
            return View(model);
        }

        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        HttpResponseMessage response;

        if (model.ProjectMemberId == 0)
            response = await _httpClient.PostAsync("ProjectMember", content);
        else
            response = await _httpClient.PutAsync($"ProjectMember/{model.ProjectMemberId}", content);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        TempData["Error"] = "Failed to save Project Member.";
        return View(model);
    }

    public async Task<IActionResult> DeleteProjectMember(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"ProjectMember/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "ProjectMember deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete the projectMember.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting projectMember with ID {id}");
            TempData["Error"] = "An unexpected error occurred while deleting the project.";
        }

        return RedirectToAction("Index");
    }


    private async Task<List<SelectListItem>> LoadUserDropdownAsync()
    {
        var response = await _httpClient.GetAsync("User/dropdown");
        var data = await response.Content.ReadFromJsonAsync<List<UserDropDownModel>>();
        return data.Select(static x => new SelectListItem { Text = x.FullName, Value = x.UserID.ToString() }).ToList();
    }

    private async Task<List<SelectListItem>> LoadProjectDropdownAsync()
    {
        var response = await _httpClient.GetAsync("Project/dropdown");
        var data = await response.Content.ReadFromJsonAsync<List<PRJ_ProjectModel>>();
        return data.Select(static x => new SelectListItem { Text = x.ProjectName, Value = x.ProjectId.ToString() }).ToList();
    }


}
