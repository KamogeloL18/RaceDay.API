using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RaceDay.Web.Helpers;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class ProfileController : Controller
{
    private readonly IApiClient _apiClient;

    public ProfileController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Account");

        var response = await _apiClient.GetAsync("api/profile");

        // THE FIX: If the API session expired, log the user out gracefully
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            HttpContext.Session.Clear(); // Destroy the local MVC session
            TempData["ErrorMessage"] = "Your session expired. Please log in again.";
            return RedirectToAction("Login", "Account");
        }

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Could not load profile. Please try again.";
            return RedirectToAction("Index", "Home");
        }

        var content = await response.Content.ReadAsStringAsync();
        var profile = JsonConvert.DeserializeObject<ProfileDto>(content);

        if (profile == null)
        {
            TempData["ErrorMessage"] = "Profile data was empty.";
            return RedirectToAction("Index", "Home");
        }

        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateProfileDto model)
    {
        var response = await _apiClient.PutAsync("api/profile", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Profile updated successfully!";
            // Update the session name in case they changed it
            HttpContext.Session.SetString("MvcUserFullName", model.FullName);
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update profile.";
        }

        return RedirectToAction("Index");
    }
}