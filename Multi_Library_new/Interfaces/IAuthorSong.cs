using Multi_Library.Models;
using System.Collections.Generic;

namespace Multi_Library.Interfaces
{
    public interface IAuthorSong
    {
        AuthorSong GetById(int songId, int authorId);
        IEnumerable<AuthorSong> GetAll();
        void Add(AuthorSong authorsong);
        void Update(AuthorSong authorsong);
        void Delete(int songId, int authorId);
    }
}
