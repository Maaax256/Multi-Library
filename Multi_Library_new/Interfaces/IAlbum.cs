using Multi_Library.Models;
using System.Collections.Generic;

namespace Multi_Library.Interfaces
{
    public interface IAlbum
    {
        Album GetById(int id);
        IEnumerable<Album> GetAll();
        void Add(Album album);
        void Update(Album album);
        void Delete(int id);
    }
}
