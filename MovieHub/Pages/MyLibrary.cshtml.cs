using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MovieHub.Data;
using MovieHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MovieHub.Pages
{
    public class MyLibraryModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DataContext dataContext;

        public List<Movie> Movies { get; set; }

        public MyLibraryModel(ILogger<IndexModel> logger, DataContext dataContext)
        {
            _logger = logger;
            this.dataContext = dataContext;
        }

        public async Task OnGetAsync(string searchType = "all")
        {
            var favoriteMovies = await dataContext.Movies
                        .Where(m => m.isFavorite)
                        .ToListAsync();

            if (searchType != "all")
            {
                favoriteMovies = favoriteMovies.Where(m => m.Type == searchType).ToList();
            }
       
            Movies = favoriteMovies.OrderBy(m => m.Title).ToList();
        }

        public async Task<IActionResult> OnPostDeleteFromFavoritesAsync(string id)
        {
            var favMovie = await dataContext.Movies.FirstOrDefaultAsync(m => m.imdbID == id);

            if (favMovie != null)
            {
                favMovie.isFavorite = false;
                await dataContext.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            return RedirectToPage("/MovieDetails", new { id });
        }
    }
}
