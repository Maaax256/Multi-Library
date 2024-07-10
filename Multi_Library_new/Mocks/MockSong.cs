using Multi_Library.Interfaces;
using Multi_Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace Multi_Library.Mocks
{
    public class MockSong : ISong
    {
        private readonly Mul_Lib_Context _context;

        public MockSong(Mul_Lib_Context context)
        {
            _context = context;
        }

        public Song GetById(int id)
        {
            return _context.Songs.Find(id);
        }

        public IEnumerable<Song> GetAll()
        {
            return _context.Songs.ToList();
        }

        public void Add(Song song)
        {
            _context.Songs.Add(song);
            _context.SaveChanges();
        }

        public void Update(Song song)
        {
            _context.Songs.Update(song);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var song = _context.Songs.Find(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
                _context.SaveChanges();
            }
        }
    }
}
