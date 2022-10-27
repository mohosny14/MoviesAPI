
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesAPI.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte GenreId { get; set; }


        [MaxLength(length: 100)]
        public string Name { get; set; }

    }
}
