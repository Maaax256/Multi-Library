using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Multi_Library.Models
{
    public partial class Song
    {
        public Song()
        {
            AuthorSongs = new HashSet<AuthorSong>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Lyrics { get; set; }
        public string Link { get; set; }
        public DateTime? DateCreate { get; set; }
        public int? AlbumId { get; set; }
        public int? CoverId { get; set; }

        public virtual Album Album { get; set; }
        public virtual Cover Cover { get; set; }
        public virtual VideoClip VideoClip { get; set; }
        public virtual ICollection<AuthorSong> AuthorSongs { get; set; }
        [NotMapped]
        public int FavoriteCount { get; set; }
    }
}
