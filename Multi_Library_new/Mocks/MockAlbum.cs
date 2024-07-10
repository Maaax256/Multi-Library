using Multi_Library.Interfaces;
using Multi_Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace Multi_Library.Mocks
{
    public class MockAlbum : IAlbum
    {
        private readonly Mul_Lib_Context _context;

        public MockAlbum(Mul_Lib_Context context)
        {
            _context = context;
        }

        public Album GetById(int id)
        {
            return _context.Albums.Find(id);
        }

        public IEnumerable<Album> GetAll()
        {
            return _context.Albums.ToList();
        }

        public void Add(Album album)
        {
            _context.Albums.Add(album);
            _context.SaveChanges();
        }

        public void Update(Album album)
        {
            _context.Albums.Update(album);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var album = _context.Albums.Find(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                _context.SaveChanges();
            }
        }
    }
}
