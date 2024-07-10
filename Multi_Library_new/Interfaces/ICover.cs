using Multi_Library.Models;
using System.Collections.Generic;

namespace Multi_Library.Interfaces
{
    public interface ICover
    {
        Cover GetById(int id);
        IEnumerable<Cover> GetAll();
        void Add(Cover cover);
        void Update(Cover cover);
        void Delete(int id);
    }
}
