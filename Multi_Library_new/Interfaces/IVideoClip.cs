using Multi_Library.Models;
using System.Collections.Generic;

namespace Multi_Library.Interfaces
{
    public interface IVideoClip
    {
        VideoClip GetById(int id);
        IEnumerable<VideoClip> GetAll();
        void Add(VideoClip videoclip);
        void Update(VideoClip videoclip);
        void Delete(int id);
    }
}
