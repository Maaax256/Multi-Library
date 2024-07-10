using Multi_Library.Interfaces;
using Multi_Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace Multi_Library.Mocks
{
    public class MockCover : ICover
    {
        private readonly Mul_Lib_Context _context;

        public MockCover(Mul_Lib_Context context)
        {
            _context = context;
        }

        public Cover GetById(int id)
        {
            return _context.Covers.Find(id);
        }

        public IEnumerable<Cover> GetAll()
        {
            return _context.Covers.ToList();
        }

        public void Add(Cover cover)
        {
            _context.Covers.Add(cover);
            _context.SaveChanges();
        }

        public void Update(Cover cover)
        {
            _context.Covers.Update(cover);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var cover = _context.Covers.Find(id);
            if (cover != null)
            {
                _context.Covers.Remove(cover);
                _context.SaveChanges();
            }
        }
    }
}
