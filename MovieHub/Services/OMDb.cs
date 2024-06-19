using Microsoft.Extensions.Configuration;
using MovieHub.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MovieHub.Services
{
    public class OMDb
    {
        private readonly IConfiguration _configuration;

        public OMDb(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IList<Movie>> GetMoviesAsync(string searchQuery, int maxResultsPerPage, int pageNumber)
        {
            return await GetMediaAsync(searchQuery, maxResultsPerPage, "movie", pageNumber);
        }

        public async Task<IList<Movie>> GetSeriesAsync(string searchQuery, int maxResultsPerPage, int pageNumber)
        {
            return await GetMediaAsync(searchQuery, maxResultsPerPage, "series", pageNumber);
        }

        private async Task<string> GetJsonResponseAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<IList<Movie>> GetMediaAsync(string searchQuery, int maxResultsPerPage, string searchType, int pageNumber)
        {
            var mediaList = new List<Movie>();

            var apiKey = _configuration["OMDbKey"];
            var url = $"https://www.omdbapi.com/?apikey={apiKey}&s={searchQuery}&page={pageNumber}&type={searchType}";
            var jsonResponse = await GetJsonResponseAsync(url);

            var jsonObject = JObject.Parse(jsonResponse);

            if (jsonObject["Search"] != null)
            {
                foreach (var item in jsonObject["Search"])
                {
                    var title = item["Title"]?.ToString();
                    int year;
                    if (int.TryParse(item["Year"]?.ToString(), out year))
                    {
                        var type = item["Type"]?.ToString();
                        var poster = item["Poster"]?.ToString();
                        var imdbID = item["imdbID"]?.ToString();

                        var media = new Movie()
                        {
                            Title = title,
                            Year = year,
                            Type = type,
                            Poster = poster,
                            imdbID = imdbID,
                            isFavorite = false
                        };

                        mediaList.Add(media);
                        if (mediaList.Count >= maxResultsPerPage)
                        {
                            break;
                        }
                    }
                }
            }
            return mediaList;
        }

        public async Task<int> GetTotalResultsAsync(string searchQuery, string searchType)
        {
            var apiKey = _configuration["OMDbKey"];
            var url = $"https://www.omdbapi.com/?apikey={apiKey}&s={searchQuery}&type={searchType}";
            var jsonResponse = await GetJsonResponseAsync(url);
            var jsonObject = JObject.Parse(jsonResponse);
            return jsonObject["totalResults"] != null ? int.Parse(jsonObject["totalResults"].ToString()) : 0;
        }

        public async Task<string> GetMediaByIdAsync(string id)
        {
            var apiKey = _configuration["OMDbKey"];
            var url = $"https://www.omdbapi.com/?apikey={apiKey}&i={id}&plot=full";

            var jsonResponse = await GetJsonResponseAsync(url);

            var jsonObject = JObject.Parse(jsonResponse);

            return jsonObject.ToString();
        }
    }
}
