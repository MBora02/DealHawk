using System.Collections.Generic;

namespace DealHawk.Domain.Entities
{

    public class Genre
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<GameGenre> GameGenres { get; set; }
    }
}
