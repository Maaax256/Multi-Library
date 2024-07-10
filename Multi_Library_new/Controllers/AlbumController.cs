using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multi_Library.Interfaces;
using Multi_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Multi_Library.Controllers
{
    public class AlbumController : Controller
    {
        private readonly Mul_Lib_Context Context;
        readonly IAlbum _ialbum;
        readonly ISong _isong;
        readonly IAuthorSong _iauthorSong;
        readonly IUserTable _iuserTable;
        readonly ICover _icover;


        public AlbumController(IAlbum ialbum, ISong isong, IAuthorSong iauthorSong,
            IUserTable iuserTable, ICover icover, Mul_Lib_Context context)
        {
            _ialbum = ialbum;
            _isong = isong;
            _iauthorSong = iauthorSong;
            _iuserTable = iuserTable;
            _icover = icover;
            Context = context;
        }

        public ViewResult AllAlbum()
        {
            var authorSongs = _iauthorSong.GetAll();
            var albums = _ialbum.GetAll();
            var albumCover = new List<AlbumCover>();

            foreach (var album in albums)
            {
                if (album != null)
                {
                    var albumSongs = _isong.GetAll().Where(song => song.AlbumId == album.Id).ToList();

                    var authorAlbumSongs = albumSongs.Where(song => authorSongs.Any(authorSong => authorSong.SongId == song.Id)).ToList();

                    if (authorAlbumSongs.Any())
                    {
                        Cover cover;
                        if (!authorAlbumSongs[0].CoverId.HasValue || _icover.GetById(authorAlbumSongs[0].CoverId.Value) == null)
                        {
                            cover = new Cover { Link = "/Covers/Нет_Альбома.jpg" };
                        }
                        else cover = _icover.GetById(authorAlbumSongs[0].CoverId.Value);

                        AlbumCover albumCoverCur = new AlbumCover
                        {
                            album = album,
                            cover = cover
                        };

                        albumCover.Add(albumCoverCur);
                    }
                }
            }

            var data = Tuple.Create(albumCover, _iuserTable);

            return View("AlbumsPage", data);
        }

        public ViewResult InfoAlbum(int albumId, int covertId)
        {
            var firstsong = _isong.GetAll().Where(x => x.AlbumId == albumId).First();
            var authorsong_s = _iauthorSong.GetAll().Where(x => x.SongId == firstsong.Id);
            var authors = new List<UserTable>();

            foreach (var authorsong in authorsong_s)
            {
                authors.Add(_iuserTable.GetById(authorsong.AuthorId));
            }

            //var album = _ialbum.GetAll().FirstOrDefault(x => x.Id == albumId);
            var album = _ialbum.GetById(albumId);
            var covertLink = _icover.GetById(covertId).Link;

            //foreach (var song in songs)
            //{
            //    if (_iauthorSong.GetAll().Any(x => x.SongId == song.Id))
            //    {
            //        var authorId = _iauthorSong.GetAll().FirstOrDefault(x => x.SongId == song.Id).AuthorId;
            //        authors.Add(_iuserTable.GetById(authorId));
            //    }
            //}

            var data = Tuple.Create(authors, album, covertLink);
            return View("AlbumPage", data);
        }

        public IActionResult CreateAlbumPage()
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value);
            var user = _iuserTable.GetById(userId);

            if (user.UserType == 1)
            {
                var authors = _iuserTable.GetAll()
                .Where(x => x.Id != userId);
                //var songsByAuthor = _iauthorSong.GetAll()
                //    .Where(x => x.AuthorId == userId);
                //
                //var songs = new List<Song>();
                //foreach (var authorSong in songsByAuthor)
                //{
                //    var song = _isong.GetById(authorSong.SongId);
                //    if (song != null && song.AlbumId == null)
                //    {
                //        songs.Add(song);
                //    }
                //}
                var songs = Context.Get_author_singles(userId).AsEnumerable();

                var data = Tuple.Create(authors, songs);
                return View("CreateAlbumPage", data);
            }
            else
            {
                TempData["Message"] = "Создавать альбомы могут только авторы";
                return RedirectToAction("Index", "Home");
            }
        }

        public ViewResult EditAlbumPage(int albumID, int covertId)
        {
            var songs = _isong.GetAll().Where(x => x.AlbumId == albumID);
            var authors = new List<UserTable>();
            var album = _ialbum.GetAll().FirstOrDefault(x => x.Id == albumID);
            var cover = new Cover();
            if (covertId != 0)
            {
                cover = _icover.GetById(covertId);
            }
            else
            {
                cover.Link = "/Covers/Нет_Альбома.jpg";
            }
            foreach (var song in songs)
            {
                if (_iauthorSong.GetAll().Any(x => x.SongId == song.Id))
                {
                    var authorId = _iauthorSong.GetAll().FirstOrDefault(x => x.SongId == song.Id).AuthorId;
                    authors.Add(_iuserTable.GetById(authorId));
                }
            }

            var data = Tuple.Create(authors, album, cover);
            return View("EditAlbumPage", data);
        }
        public IActionResult CreateAlbum(string Name, string Description, int[] SelectedSongs, IFormFile AlbumCover)
        {
            if (SelectedSongs.Length < 2)
            {
                TempData["Message"] = "Вы не выбрали хотя бы две песни";
                return RedirectToAction("Index", "Home");
            }

            if (Name == null)
            {
                TempData["Message"] = "Вы не добавили название альбома";
                return RedirectToAction("Index", "Home");
            }

            var selectedSongIds = SelectedSongs.ToList();
            var selectedSongs = _isong.GetAll().Where(song => selectedSongIds.Contains(song.Id));

            var album = new Album
            {
                Name = Name,
                Description = Description,                
            };

            _ialbum.Add(album);

            int id = _ialbum.GetAll().Last().Id;



            foreach (var song in selectedSongs)
            {
                song.AlbumId = id;
                _isong.Update(song);
            }


            /*string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Covers");
            string uniqueFileName = AlbumCover.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                AlbumCover.CopyTo(fileStream);
            }

            string path = "/Covers/" + uniqueFileName;
            string coverFileUrl = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path));*/

            int cid = Convert.ToInt32(selectedSongs.First().CoverId);
            var cover = new Cover
            {
                Link = _icover.GetById(cid).Link,
                //DatePut = DateTime.Now
            };

            //int count = _icover.GetAll().Where(x => x.Link == cover.Link).Count();
            _icover.Add(cover);

            int coverId = _icover.GetAll().Last().Id;
            foreach (var song in selectedSongs)
            {
                song.CoverId = coverId;
                _isong.Update(song);
            }
            
            return RedirectToAction("Index", "Home");
        }


        public IActionResult UpdateAlbum(int AlbumId, int CoverId, string Name, string Description, IFormFile cover)
        {
            var album = _ialbum.GetById(AlbumId);
            album.Name = Name;
            album.Description = Description;
            if(cover != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Covers");
                string uniqueFileName = cover.FileName + ".jpg";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    cover.CopyTo(fileStream);
                }

                string coverFileUrl = Path.Combine("/Covers", uniqueFileName);

                var coverChange = _icover.GetById(CoverId);
                coverChange.Link = coverFileUrl;
                _icover.Update(coverChange);
            }
            _ialbum.Update(album);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteAlbum(int albumID)
        {
            var songs = _isong.GetAll().Where(x => x.AlbumId == albumID);
            foreach(var song in songs)
            {
                song.Album = null;
                song.AlbumId = null;
            }
            _ialbum.Delete(albumID);
            return RedirectToAction("Index", "Home");
        }
    }
}
