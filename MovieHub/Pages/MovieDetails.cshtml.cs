using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieHub.Data;
using MovieHub.Models;
using MovieHub.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieHub.Pages
{
    public class MovieDetailsModel : PageModel
    {
        private readonly OMDb omdbService;
        DataContext dataContext;

        public MovieDetailsModel(OMDb omdb, DataContext dataContext)
        {
            omdbService = omdb;
            this.dataContext = dataContext;
        }

        public Movie Movie { get; set; }
        public List<Movie> Recommandations { get; set; } = new List<Movie>();

        public async Task<IActionResult> OnGetAsync(string id, int pageNumber = 1)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Affichage du média recherché
            string searchMedia = await omdbService.GetMediaByIdAsync(id);
            Movie = JsonConvert.DeserializeObject<Movie>(searchMedia);

            // Affichage des recommandations
            string searchQuery = Movie.Title;
            string[] queryWords = searchQuery.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            int maxResults = 10;
            string searchType = Movie.Type;

            if (queryWords.Length > 0)
            {
                string firstWord = queryWords[0];
                await GetRecommandations(searchType, firstWord, maxResults, pageNumber);

                if (Recommandations.Count == 0 && queryWords.Length > 1)
                {
                    string secondWord = queryWords[1];
                    await GetRecommandations(searchType, secondWord, maxResults, pageNumber);
                }
            }

            var existingMovie = dataContext.Movies.FirstOrDefault(m => m.imdbID == id) ;
            if (existingMovie != null)
            {
                Movie.isFavorite = existingMovie.isFavorite;
            }

            if (Movie == null)
            {
                return NotFound();
            }

            return Page();
        }

        private async Task GetRecommandations(string searchType, string searchWord, int maxResults, int pageNumber)
        {
            if (searchType == "movie")
            {
                Recommandations = (List<Movie>)await omdbService.GetMoviesAsync(searchWord, maxResults, pageNumber);
            }
            else if (searchType == "series")
            {
                Recommandations = (List<Movie>)await omdbService.GetSeriesAsync(searchWord, maxResults, pageNumber);
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            // Vérifie si l'ID est null
            if (id == null)
            {
                return NotFound();
            }

            // Redirige vers la page MovieDetails avec l'ID
            return RedirectToPage("/MovieDetails", new { id = id });
        }

        public async Task<IActionResult> OnPostAddToFavoritesAsync(string id, bool isFavorite)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Récupère les détails du film à partir de l'API
            string searchMedia = await omdbService.GetMediaByIdAsync(id);
            var movie = JsonConvert.DeserializeObject<Movie>(searchMedia);

            // Vérifie si le film a été récupéré avec succès
            if (movie != null)
            {
                movie.isFavorite = !movie.isFavorite;

                // Ajoute le film à la base de données ou le supprime
                if (!dataContext.Movies.Contains(movie))
                {
                    movie.isFavorite = true;
                    dataContext.Movies.Add(movie);
                    await dataContext.SaveChangesAsync();

                }
                else
                {
                    movie.isFavorite = false;
                    dataContext.Movies.Remove(movie);
                    await dataContext.SaveChangesAsync();
                }

            }
            // Réactualise la page
            return RedirectToPage("/MovieDetails", new { id = id}) ;
        }

    }
}
