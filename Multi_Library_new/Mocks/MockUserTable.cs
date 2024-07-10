using Multi_Library.Interfaces;
using Multi_Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace Multi_Library.Mocks
{
    public class MockUserTable : IUserTable
    {
        private readonly Mul_Lib_Context _context;

        public MockUserTable(Mul_Lib_Context context)
        {
            _context = context;
        }

        public UserTable GetById(int id)
        {
            return _context.UserTables.Find(id);
        }

        public IEnumerable<UserTable> GetAll()
        {
            return _context.UserTables.ToList();
        }

        public void Add(UserTable usertable)
        {
            _context.UserTables.Add(usertable);
            _context.SaveChanges();
        }

        public void Update(UserTable usertable)
        {
            _context.UserTables.Update(usertable);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var usertable = _context.UserTables.Find(id);
            if (usertable != null)
            {
                _context.UserTables.Remove(usertable);
                _context.SaveChanges();
            }
        }
    }
}
