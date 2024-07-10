using System;
using System.Collections.Generic;

#nullable disable

namespace Multi_Library.Models
{
    public partial class Album
    {
        public Album()
        {
            Songs = new HashSet<Song>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime? DateCreate { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
    }
}
