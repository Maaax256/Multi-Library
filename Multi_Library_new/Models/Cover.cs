using System;
using System.Collections.Generic;

#nullable disable

namespace Multi_Library.Models
{
    public partial class Cover
    {
        public Cover()
        {
            Songs = new HashSet<Song>();
        }

        public int Id { get; set; }
        public string Link { get; set; }
        public DateTime? DateCreate { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
    }
}
