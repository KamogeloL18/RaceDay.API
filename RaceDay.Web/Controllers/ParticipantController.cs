using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RaceDay.Web.Helpers;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class ParticipantController : Controller
{
    private readonly IApiClient _apiClient;

    public ParticipantController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> MyEvents()
    {
        if (HttpContext.Session.GetRole() != "Participant")
            return RedirectToAction("Login", "Account");

        // Call the new API endpoint
        var response = await _apiClient.GetAsync("api/participants/my-enrolments");
        var enrolments = new List<ParticipantEnrolmentDto>();

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            enrolments = JsonConvert.DeserializeObject<List<ParticipantEnrolmentDto>>(content) ?? new List<ParticipantEnrolmentDto>();
        }

        return View(enrolments);
    }

    public async Task<IActionResult> MyResults()
    {
        if (HttpContext.Session.GetRole() != "Participant")
            return RedirectToAction("Login", "Account");

        // Call the new API endpoint
        var response = await _apiClient.GetAsync("api/participants/my-results");
        var results = new List<ParticipantResultDto>();

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            results = JsonConvert.DeserializeObject<List<ParticipantResultDto>>(content) ?? new List<ParticipantResultDto>();
        }

        return View(results);
    }

    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        if (HttpContext.Session.GetRole() != "Participant")
            return RedirectToAction("Login", "Account");

        var payload = new { CategoryID = categoryId };
        var response = await _apiClient.PostAsync($"api/events/{eventId}/enrol", payload);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Enrolment successful!";
            return RedirectToAction("MyEvents");
        }

        var error = await response.Content.ReadAsStringAsync();
        TempData["ErrorMessage"] = $"Failed to enrol: {error}";
        return RedirectToAction("Details", "Event", new { id = eventId });
    }
}