using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Multi_Library.Interfaces;
using Multi_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Multi_Library.Controllers
{
    public class SongController : Controller
    {
        private readonly Mul_Lib_Context Context;
        private readonly ISong _isong;
        private readonly IAlbum _ialbum;
        private readonly ICover _icover;
        private readonly IAuthorSong _iauthorSong;
        private readonly IUserTable _iuserTable;
        private readonly IVideoClip _ivideoClip;
        //private readonly ISongSongPlaylist _isongSongPlayList;
        //private readonly IVideoClipClipPlaylist _ivideoClipClipPlaylist;

        public SongController(ISong isong, IAlbum ialbum, ICover icover, IAuthorSong iauthorSong,
            IUserTable iuser, IVideoClip ivideoClip, Mul_Lib_Context context)
        {
            _isong = isong;
            _ialbum = ialbum;
            _icover = icover;
            _iauthorSong = iauthorSong;
            _iuserTable = iuser;
            _ivideoClip = ivideoClip;
            Context = context;
            //_isongSongPlayList = songSongPlaylist;
            //_ivideoClipClipPlaylist = ivideoClipClipPlaylist;
        }

        public ViewResult SongView()
        {
            IEnumerable<Song> songs = _isong.GetAll();
            var data = Tuple.Create(songs, _ialbum, _icover);
            return View(data);
        }

        public ViewResult SongViewPage(int id)
        {
            var song = _isong.GetById(id);
            var data = Tuple.Create((IEnumerable<Song>)new List<Song> { song }, _ialbum, _icover);
            return View("SongView", data);
        }

        public IActionResult CreateSongPage()
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value);
            var user = _iuserTable.GetById(userId);

            if (user.UserType == 1)
            {
                var authorSongs = _iauthorSong.GetAll();
                var albums = _ialbum.GetAll();
                var albumCover = new List<AlbumCover>();
                var authors = _iuserTable.GetAll()
                    .Where(x => x.UserType.ToString() == "Author"
                    && x.Id.ToString() != HttpContext.User.FindFirst("PersonId").Value);

                foreach (var album in albums)
                {
                    if (album != null)
                    {
                        var albumSongs = _isong.GetAll().Where(song => song.AlbumId == album.Id).ToList();

                        var authorAlbumSongs = albumSongs.Where(song => authorSongs.Any(authorSong => authorSong.SongId == song.Id)).ToList();

                        if (authorAlbumSongs.Any())
                        {
                            int cid = Convert.ToInt32(authorAlbumSongs[0].CoverId);
                            Cover cover = _icover.GetById(cid);
                            if (cover == null)
                            {
                                cover = new Cover { Link = "/Covers/Нет_Альбома.jpg" };
                            }

                            AlbumCover albumCoverCur = new AlbumCover
                            {
                                album = album,
                                cover = cover
                            };

                            albumCover.Add(albumCoverCur);
                        }
                    }
                }
                var data = Tuple.Create(albumCover, authors.ToList());

                return View("SongCreatePage", data);
            }
            else
            {
                TempData["Message"] = "Создавать песни могут только авторы";
                return RedirectToAction("Index", "Home");
            }
        }

            [HttpPost]
        public IActionResult CreateSong(Song model, int[] SelectedAuthors, IFormFile SongFile, IFormFile SongCoverfile)
        {

            if (SongFile == null)
            {
                TempData["Message"] = "Вы не добавили файл песни";
                return RedirectToAction("Index", "Home");
            }

            if (model.Name == null)
            {
                TempData["Message"] = "Вы не добавили название песни";
                return RedirectToAction("Index", "Home");
            }

            string sfilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Songs", SongFile.FileName);
            using (var sfileStream = new FileStream(sfilePath, FileMode.Create))
            {
                SongFile.CopyTo(sfileStream);
            }
            string sLinkdemo = "/Songs/" + SongFile.FileName;
            string sLink = Path.Combine(Path.GetDirectoryName(sLinkdemo), Path.GetFileName(sLinkdemo));

            Cover cover;
            if (SongCoverfile != null)
            {
                string cfilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Covers", SongCoverfile.FileName);
                using (var cfileStream = new FileStream(cfilePath, FileMode.Create))
                {
                    SongCoverfile.CopyTo(cfileStream);
                }
                string cLinkdemo = "/Covers/" + SongCoverfile.FileName;
                string cLink = Path.Combine(Path.GetDirectoryName(cLinkdemo), Path.GetFileName(cLinkdemo));

                cover = new Cover
                {
                    Link = cLink,
                    DateCreate = DateTime.Now
                };
                _icover.Add(cover);
            }
            else
            {
                if (!_icover.GetAll().Where(x =>
                x.Link == "/default-cover-art.png").Any())
                {
                    cover = new Cover
                    {
                        Link = "/default-cover-art.png",
                        DateCreate = DateTime.Now
                    };
                    _icover.Add(cover);
                }
            }
            //int count = _icover.GetAll().Where(x => x.Link == cover.Link).Count();
            //if (count == 0) _icover.Add(cover);
            int coverid = _icover.GetAll().LastOrDefault().Id;
            Song song;
            if (SongCoverfile != null)
            {
                song = new Song
                {
                    Name = model.Name,
                    Lyrics = model.Lyrics,
                    DateCreate = DateTime.Now,
                    CoverId = coverid,
                    Link = sLink
                };
            }
            else
            {
                song = new Song
                {
                    Name = model.Name,
                    Lyrics = model.Lyrics,
                    DateCreate = DateTime.Now,
                    CoverId = _icover.GetAll().ToList().Find(x => x.Link == "/default-cover-art.png").Id,
                    //Cover = cover,
                    Link = sLink
                };
            }
                _isong.Add(song);

            int songId = _isong.GetAll().Where(x => x.Name == song.Name).First().Id;
            var creatorSong = new AuthorSong
            {
                AuthorId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value),
                SongId = songId,
            };
            _iauthorSong.Add(creatorSong);
            foreach (var authorId in SelectedAuthors)
            {
                //if (!_iauthorSong.GetAll().Any(x => x.AuthorId == authorId && songId == x.SongId))
                //{
                var authorSong = new AuthorSong()
                {
                    AuthorId = authorId,
                    SongId = songId
                };
                _iauthorSong.Add(authorSong);
                //}
            }
            TempData["Message"] = "Песня создана";
            return RedirectToAction("Index", "Home");
        }
        public ViewResult SongEditPage(int id)
        {
            var song = _isong.GetById(id);
            var authorSongs = _iauthorSong.GetAll().Where(x => x.AuthorId.ToString() == HttpContext.User.FindFirst("PersonId").Value);     
             
            var songs = new List<Song>();
            foreach(var songId in authorSongs)
            {
                songs.Add(songId.Song);
            }
            var albums = _ialbum.GetAll();
            var albumList = new List<Album>();
            foreach (var album in albums)
            {
                if (album != null)
                {
                    var albumSongs = _isong.GetAll().Where(song => song.AlbumId == album.Id).ToList();

                    var authorAlbumSongs = albumSongs.Where(song => authorSongs.Any(authorSong => authorSong.SongId == song.Id)).ToList();

                    if (authorAlbumSongs.Any())
                    {
                        albumList.Add(album);
                    }
                }
            }
                var authors = _iauthorSong.GetAll().Where(x => x.SongId == song.Id);
            var authorIds = authors.Select(x => x.AuthorId);
            var authorNames = _iuserTable.GetAll().Where(user => authorIds.Contains(user.Id)).Select(user => user.Name).ToList();
            var videoClip = _ivideoClip.GetById(id);
            if (song.AlbumId != null)
            {
                song.Album = _ialbum.GetById(song.AlbumId.Value);
            }
            if (videoClip != null)
            {
                song.VideoClip = videoClip;
            }
            var data = Tuple.Create(song, albumList, _icover.GetAll().ToList(), authorNames);
            return View("EditSongPage", data);
        }

        public IActionResult UpdateSong(int songID, string SongName, string SongLyrics, int AlbumId, IFormFile SongFile)
        {
            var songForChange = _isong.GetById(songID);
            songForChange.Name = SongName;
            songForChange.Lyrics = SongLyrics;
            if (songForChange.AlbumId != null)
            {
                songForChange.AlbumId = AlbumId;
                songForChange.Album = _ialbum.GetById(AlbumId);
            }
            if(SongFile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(SongFile.FileName);

                string uniqueFileName = fileName + ".mp3";

                string uploadsFolder = Path.Combine("/", "Songs");

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    SongFile.CopyTo(fileStream);
                }

                string songFileUrl = Path.Combine("/Songs", uniqueFileName);
                songForChange.Link = songFileUrl;
            }
            
            _isong.Update(songForChange);
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult RemoveSong(int SongId)
        {
            // var removeClips = _ivideoClip.GetAll().Where(x => x.SongId == SongId);
            // var removeSongAuthors = _iauthorSong.GetAll().Where(x => x.SongId == SongId);
            // 
            // foreach(var clip in removeClips)
            // {
            //     _ivideoClip.Delete(clip.Id);
            // }
            //
            // foreach(var songAuthor in removeSongAuthors)
            // {
            //     _iauthorSong.Delete(SongId, songAuthor.AuthorId);
            // }
            //
            // _isong.Delete(SongId);
            Context.Remove_song(SongId);

            return RedirectToAction("Index", "Home");
        }

        public ViewResult SongInformation(int id)
        {
            var song = _isong.GetById(id);
            var authors = _iauthorSong.GetAll().Where(x => x.SongId == song.Id);
            var authorIds = authors.Select(x => x.AuthorId);
            var authorNames = _iuserTable.GetAll().Where(user => authorIds.Contains(user.Id)).Select(user => user.Name).ToList();
            var videoClip = _ivideoClip.GetById(id);
            if(song.AlbumId != null)
            {
                song.Album = _ialbum.GetById(song.AlbumId.Value);
            }
            if(videoClip != null)
            {
                song.VideoClip = videoClip;
            }

            var data = Tuple.Create(song, _ialbum, _icover, authorNames);
            return View("SongPageView", data);
        }

    }
}
