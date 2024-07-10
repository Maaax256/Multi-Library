using Multi_Library.Models;
using System.Collections.Generic;

namespace Multi_Library.Interfaces
{
    public interface ISong
    {
        Song GetById(int id);
        IEnumerable<Song> GetAll();
        void Add(Song song);
        void Update(Song song);
        void Delete(int id);
    }
}
