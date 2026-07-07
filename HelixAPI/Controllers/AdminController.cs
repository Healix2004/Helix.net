using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Strictly lock this to system administrators
    public class AdminController : AppControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetSystemOverview()
        {
            var dashboardData = await _adminDashboardService.GetSystemOverviewAsync();

            return NewResult(new Response<AdminDashboardDto>(dashboardData)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "System overview metrics retrieved successfully."
            });
        }


        [HttpGet("patients")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPatientsManagement([FromQuery] string searchTerm)
        {
            try
            {
                var dashboardData = await _adminDashboardService.GetPatientsManagementAsync(searchTerm);

                return NewResult(new Response<PatientsManagementDashboardDto>(dashboardData)
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Patients management data retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return NewResult(new Response<PatientsManagementDashboardDto>("An error occurred while fetching patient data.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }

        [HttpGet("doctors")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDoctorsManagement([FromQuery] string searchTerm)
        {
            try
            {
                var dashboardData = await _adminDashboardService.GetDoctorsManagementAsync(searchTerm);

                return NewResult(new Response<DoctorsManagementDashboardDto>(dashboardData)
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Doctors management data retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return NewResult(new Response<DoctorsManagementDashboardDto>("An error occurred while fetching doctor data.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }
        [HttpGet("facilities")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFacilitiesManagement([FromQuery] string searchTerm)
        {
            try
            {
                var dashboardData = await _adminDashboardService.GetFacilitiesManagementAsync(searchTerm);

                return NewResult(new Response<FacilitiesManagementDashboardDto>(dashboardData)
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Facilities management data retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return NewResult(new Response<FacilitiesManagementDashboardDto>("An error occurred while fetching facility data.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }

        [HttpGet("drugs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDrugsManagement([FromQuery] string searchTerm)
        {
            try
            {
                var dashboardData = await _adminDashboardService.GetDrugsManagementAsync(searchTerm);

                return NewResult(new Response<DrugsManagementDashboardDto>(dashboardData)
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Drugs catalog management data retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return NewResult(new Response<DrugsManagementDashboardDto>("An error occurred while fetching the drugs catalog.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }
    }
}