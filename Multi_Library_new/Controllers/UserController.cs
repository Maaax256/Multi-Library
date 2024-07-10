using Microsoft.AspNetCore.Mvc;
using Multi_Library.Interfaces;
using Multi_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Multi_Library.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserTable _iuserTable;
        private readonly IAlbum _iAlbum;
        private readonly ICover _iCover;
        private readonly IAuthorSong _iAuthorSong;
        private readonly ISong _iSong;
        
        public UserController(IUserTable iuserTable, IAuthorSong authorSong, IAlbum album, ICover cover, IAuthorSong iAuthorSong, ISong isong)
        {
            _iuserTable = iuserTable;
            _iAlbum = album;
            _iCover = cover;
            _iAuthorSong = iAuthorSong;
            _iSong = isong;
        }

        public ViewResult AuthorInfo(int authorId)
        {
            var author = _iuserTable.GetById(authorId);

            var SongsAuthor = _iAuthorSong.GetAll().Where(x => x.AuthorId == authorId);
            var songs = new List<Song>();
            foreach (var songAuthor in SongsAuthor)
            {
                songs.Add(_iSong.GetById(songAuthor.SongId));
            }

            var albums = _iAlbum.GetAll().Where(album => album.Songs.Any(song => song.AuthorSongs.Any(author => author.AuthorId == authorId)));
            var coverAlbum = new List<CoverAlbum>();

            foreach (var album in albums)
            {
                if (album != null)
                {
                    var albumSongs = _iSong.GetAll().Where(song => song.AlbumId == album.Id).ToList();

                    var authorAlbumSongs = albumSongs.Where(song => SongsAuthor.Any(authorSong => authorSong.SongId == song.Id)).ToList();

                    if (authorAlbumSongs.Any())
                    {
                        Cover cover = _iCover.GetById(authorAlbumSongs[0].CoverId.Value);
                        if (cover == null)
                        {
                            cover = new Cover { Link = "/Covers/Нет_Альбома.jpg" };
                        }

                        CoverAlbum albumCoverCur = new CoverAlbum
                        {
                            Album = album,
                            Cover = cover
                        };

                        coverAlbum.Add(albumCoverCur);
                    }
                }
            }

            var data = Tuple.Create(author, songs, coverAlbum);

            return View("AuthorPage", data);
        }

        public ViewResult AllAuthors()
        {
            //var t = _iuserTable.GetAll().First();
            var authors = _iuserTable.GetAll().Where(x => x.UserType == 1);     
            var AuthorSong = _iAuthorSong.GetAll();
            foreach(var authorSong in AuthorSong)
            {
                authorSong.Author = _iuserTable.GetById(authorSong.AuthorId);
                authorSong.Song = _iSong.GetById(authorSong.SongId);
            }
            var data = Tuple.Create(AuthorSong, authors);

            return View("AuthorsPage", data);
        }
    }
}
