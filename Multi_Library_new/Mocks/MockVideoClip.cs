using Multi_Library.Interfaces;
using Multi_Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace Multi_Library.Mocks
{
    public class MockVideoClip : IVideoClip
    {
        private readonly Mul_Lib_Context _context;

        public MockVideoClip(Mul_Lib_Context context)
        {
            _context = context;
        }

        public VideoClip GetById(int id)
        {
            return _context.VideoClips.Find(id);
        }

        public IEnumerable<VideoClip> GetAll()
        {
            return _context.VideoClips.ToList();
        }

        public void Add(VideoClip videoclip)
        {
            _context.VideoClips.Add(videoclip);
            _context.SaveChanges();
        }

        public void Update(VideoClip videoclip)
        {
            _context.VideoClips.Update(videoclip);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var videoclip = _context.VideoClips.Find(id);
            if (videoclip != null)
            {
                _context.VideoClips.Remove(videoclip);
                _context.SaveChanges();
            }
        }
    }
}
