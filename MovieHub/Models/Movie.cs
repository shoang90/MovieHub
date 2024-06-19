﻿using System.ComponentModel.DataAnnotations;

namespace MovieHub.Models
{
    public class Movie
    {
        [Key]
        public string imdbID { get; set; }
        public string Title { get; set; }
        public string Plot { get; set; }
        public int Year { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string imdbRating { get; set; }
        public string DVD { get; set; }
        public string BoxOffice { get; set; }
        public bool isFavorite {  get; set; }
    }
}
