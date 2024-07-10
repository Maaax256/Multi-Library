using Microsoft.AspNetCore.Mvc;
using Multi_Library.Interfaces;
using Multi_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Multi_Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISong _isong;
        private readonly IAlbum _ialbum;
        private readonly ICover _icover;
        private readonly IAuthorSong _iauthorSong;

        public HomeController(ISong isong, IAlbum ialbum, ICover icover, IAuthorSong iauthorSong)
        {
            _isong = isong;
            _ialbum = ialbum;
            _icover = icover;
            _iauthorSong = iauthorSong;
        }

        public ViewResult Index()
        {
            IEnumerable<Song> songs = _isong.GetAll();                //fix this   IEnumerable<Song> songs = _isong.GetAll().OrderByDescending(s => s.DatePut).Take(4);   
            var albumCover = new List<AlbumCover>();
            var albums = _ialbum.GetAll().OrderByDescending(a => a.Id).Take(4);

            foreach (var album in albums)
            {
                if(album != null)
                {
                    var albumSongs = _isong.GetAll().Where(song => song.AlbumId == album.Id).ToList();

                    foreach (var song in albumSongs)
                    {
                        song.AuthorSongs = _iauthorSong.GetAll().Where(authorSong => authorSong.SongId == song.Id).ToList();
                    }
                    Cover cover = new Cover();
                    
                    if (albumSongs.Any())
                    {
                        if (albumSongs[0].CoverId.HasValue && _icover.GetById(albumSongs[0].CoverId.Value) != null)
                        {
                            cover = _icover.GetById(albumSongs[0].CoverId.Value);
                        }
                        else
                        {
                            cover.Link = "/Covers/Нет_Альбома.jpg";
                        }
                    }
                    else 
                    {
                         cover.Link = "/Covers/Нет_Альбома.jpg";
                    }

                    AlbumCover albumCoverCur = new AlbumCover
                    {
                        album = album,
                        cover = cover
                    };

                    albumCover.Add(albumCoverCur);

                }
            }

            var data = Tuple.Create(songs, _ialbum, _icover, albumCover);
            return View("Index", data);
        }

        public IActionResult Index0()
        {
            return View();
        }
    }
}
