using Multi_Library.Models;
using System.Collections.Generic;

namespace Multi_Library.Interfaces
{
    public interface IUserTable
    {
        UserTable GetById(int id);
        IEnumerable<UserTable> GetAll();
        void Add(UserTable usertable);
        void Update(UserTable usertable);
        void Delete(int id);
    }
}
