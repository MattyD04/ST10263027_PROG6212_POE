using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ST10263027_PROG6212_POE.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Data;

namespace ST10263027_PROG6212_POE.Views.Home
{
    public class HRDashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public HRDashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public List<ClaimViewModel> ApprovedClaims { get; set; }

      
    }
}