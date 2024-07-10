using Microsoft.AspNetCore.Mvc;
using Multi_Library.Interfaces;
using Multi_Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Multi_Library.Controllers
{
    public class SearchController : Controller
    {
        public readonly ISong _isong;
        public readonly IAlbum _album;
        public readonly ICover _icover;
        public readonly IUserTable _iuserTable;
        public readonly IVideoClip _ivideoClip;
        public readonly IAuthorSong _authorSong;

        public SearchController(ISong isong, IAlbum album, ICover icover, IUserTable iuserTable, IVideoClip ivideoClip, IAuthorSong iAuthorSong)
        {
            _isong = isong;
            _album = album;
            _icover = icover;
            //_icompilation = icompilation;
            _iuserTable = iuserTable;
            _ivideoClip = ivideoClip;
            _authorSong = iAuthorSong;
        }

        [HttpGet]
        public ViewResult SerachByName(string searchWord)
        {
            if (String.IsNullOrWhiteSpace(searchWord))
            {
                var authorSongs = _authorSong.GetAll();
                var songs = _isong.GetAll().ToList();
                var albums = _album.GetAll().ToList();
                var videoClips = _ivideoClip.GetAll();
                var albumCover = new List<AlbumCover>();

                foreach (var song in songs)
                {
                    song.AuthorSongs = _authorSong.GetAll().Where(x => x.SongId == song.Id).ToList();
                }

                foreach (var album in albums)
                {
                    var albumSongs = _isong.GetAll().Where(albumSong => albumSong.AlbumId == album.Id).ToList();

                    foreach (var albumSong in albumSongs)
                    {
                        albumSong.AuthorSongs = authorSongs.Where(authorSong => authorSong.SongId == albumSong.Id).ToList();
                    }

                    Cover cover = albumSongs.Any() ? _icover.GetById(albumSongs[0].CoverId.Value) : null;

                    AlbumCover albumCoverCur = new AlbumCover
                    {
                        album = album,
                        cover = cover
                    };

                    albumCover.Add(albumCoverCur);
                }

                var AuthorSong = _authorSong.GetAll().ToList().FindAll(x => x.Author != null);
                foreach (var authorSong in AuthorSong)
                {
                    authorSong.Author = _iuserTable.GetById(authorSong.AuthorId);
                    authorSong.Song = _isong.GetById(authorSong.SongId);
                }
                var data = Tuple.Create(AuthorSong, songs, albumCover, videoClips);
                return View("SearchByNameResult", data);

            }
            else
            {
                var authorSongs = _authorSong.GetAll();
                var songs = _isong.GetAll().ToList().FindAll(word => word.Name.ToLower().Contains(searchWord.ToLower()));
                var albums = _album.GetAll().ToList().FindAll(word => word.Name.ToLower().Contains(searchWord.ToLower()));
                var videoClips = _ivideoClip.GetAll().ToList().FindAll(word => word.Song.Name.ToLower().Contains(searchWord.ToLower()));
                var albumCover = new List<AlbumCover>();

                foreach (var song in songs)
                {
                    song.AuthorSongs = _authorSong.GetAll().Where(x => x.SongId == song.Id).ToList();
                    if (song.AlbumId.HasValue)
                    {
                        int s = song.AlbumId ?? -1;
                        song.Album = _album.GetById(s);
                    }
                }

                /*foreach (var song in songs) //прописать через иф цикл отдельно для альбома и отдельно для песни
                {
                    //var albumSongs = _isong.GetAll().Where(albumSong => albumSong.AlbumId == song.Id).ToList();

                    int s = song.AlbumId ?? -1;

                    var album = _album.GetById(s);

                    song.AuthorSongs = authorSongs.Where(authorSong => authorSong.SongId == song.Id).ToList();

                    Cover cover = _icover.GetById(songs[0].CoverId.Value);

                    AlbumCover albumCoverCur = new()
                    {
                        album = album,
                        cover = cover
                    };

                    albumCover.Add(albumCoverCur);
                }*/

                foreach (var album in albums) //прописать через иф цикл отдельно для альбома и отдельно для песни
                {
                    var albumSongs = _isong.GetAll().Where(albumSong => albumSong.AlbumId == album.Id).ToList();

                    foreach (var albumSong in albumSongs)
                    {
                        albumSong.AuthorSongs = authorSongs.Where(authorSong => authorSong.SongId == albumSong.Id).ToList();
                    }

                    Cover cover = albumSongs.Any() ? _icover.GetById(albumSongs[0].CoverId.Value) : null;

                    AlbumCover albumCoverCur = new()
                    {
                        album = album,
                        cover = cover
                    };

                    albumCover.Add(albumCoverCur);
                }

                //var n = _iuserTable.GetById(x.AuthorId).Name;

                //_isong.GetById(x.SongId).Name.ToLower().Contains(searchWord.ToLower()) ||

                var authors = _iuserTable.GetAll().ToList().FindAll(x => x.Name.ToLower().Contains(searchWord.ToLower()));
                var AuthorSong = _authorSong.GetAll().ToList().FindAll(x => _iuserTable.GetById(x.AuthorId).Name.ToLower().Contains(searchWord.ToLower()));
                foreach (var authorSong in AuthorSong)
                {
                    authorSong.Author = _iuserTable.GetById(authorSong.AuthorId);
                    authorSong.Song = _isong.GetById(authorSong.SongId);
                }

                var data = Tuple.Create(AuthorSong, songs, albumCover, videoClips, authors);


                return View("SearchByNameResult", data);

            }
        }
    }
}
