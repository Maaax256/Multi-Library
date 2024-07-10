using System;
using System.Collections.Generic;

#nullable disable

namespace Multi_Library.Models
{
    public partial class VideoClip
    {
        public VideoClip()
        {
           
        }

        public int Id { get; set; }
        public string Link { get; set; }
        public DateTime? DateCreate { get; set; }
        public int SongId { get; set; }

        public virtual Song Song { get; set; }
    }
}
