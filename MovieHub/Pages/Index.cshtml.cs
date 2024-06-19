using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MovieHub.Models;
using MovieHub.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieHub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly OMDb omdbService;

        public IndexModel(ILogger<IndexModel> logger, OMDb omdbService)
        {
            _logger = logger;
            this.omdbService = omdbService;
        }

        public List<Movie> Movies { get; set; } = new List<Movie>();
        public string SearchType { get; set; } = "movie";
        public int MaxResultsPerPage { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public string SearchQuery { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string searchQuery, string searchType = "movie", int maxResultsPerPage = 10, int pageNumber = 1)
        {
            SearchQuery = searchQuery;
            SearchType = searchType;
            MaxResultsPerPage = maxResultsPerPage;
            CurrentPage = pageNumber;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (searchType == "movie")
                {
                    Movies = (List<Movie>)await omdbService.GetMoviesAsync(searchQuery, 10, pageNumber); // Demander 10 résultats par page
                }
                else if (searchType == "series")
                {
                    Movies = (List<Movie>)await omdbService.GetSeriesAsync(searchQuery, 10, pageNumber); // Demander 10 résultats par page
                }

                // Get total results count from the first page response
                TotalResults = await omdbService.GetTotalResultsAsync(searchQuery, searchType);
                TotalPages = (int)Math.Ceiling((double)TotalResults / 10); // Diviser par 10 car l'API renvoie 10 résultats par page
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            return RedirectToPage("/MovieDetails", new { id = id });
        }
    }
}
