using System;
using System.Collections.Generic;

#nullable disable

namespace Multi_Library.Models
{
    public partial class AuthorSong
    {
        public int SongId { get; set; }
        public int AuthorId { get; set; }
        //public bool IsFavorite { get; set; }

        public virtual UserTable Author { get; set; }
        public virtual Song Song { get; set; }
    }
}
