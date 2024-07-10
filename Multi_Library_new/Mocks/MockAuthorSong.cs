using Multi_Library.Interfaces;
using Multi_Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace Multi_Library.Mocks
{
    public class MockAuthorSong : IAuthorSong
    {
        private readonly Mul_Lib_Context _context;

        public MockAuthorSong(Mul_Lib_Context context)
        {
            _context = context;
        }

        public AuthorSong GetById(int songId, int authorId)
        {
            return _context.AuthorSongs
                .FirstOrDefault(a_s => a_s.SongId == songId && a_s.AuthorId == authorId);
        }

        public IEnumerable<AuthorSong> GetAll()
        {
            return _context.AuthorSongs.ToList();
        }

        public void Add(AuthorSong authorSong)
        {
            _context.AuthorSongs.Add(authorSong);
            _context.SaveChanges();
        }

        public void Update(AuthorSong authorSong)
        {
            _context.AuthorSongs.Update(authorSong);
            _context.SaveChanges();
        }

        public void Delete(int songId, int authorId)
        {
            var authorSong = _context.AuthorSongs
                .FirstOrDefault(a_s => a_s.SongId == songId && a_s.AuthorId == authorId);
            if (authorSong != null)
            {
                _context.AuthorSongs.Remove(authorSong);
                _context.SaveChanges();
            }
        }
    }
}
