using Microsoft.AspNetCore.Mvc;
using Multi_Library.Interfaces;
using Multi_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Multi_Library.Controllers
{
    public class CoverController : Controller
    {
        private readonly ICover _icover;
        private readonly IAlbum _ialbum;
        private readonly ISong _isong;
        
        public CoverController(ICover icover, IAlbum ialbum, ISong isong)
        {
            _icover = icover;
            _ialbum = ialbum;
            _isong = isong;
        }

        public ViewResult AllCovers()
        {
            //var coverAlbum = new List<CoverAlbum>();
            //foreach(var cover in _icover.GetAll())
            //{
            //    cover.Songs = _isong.GetAll().Where(x => x.CoverId == cover.Id).ToList();
            //}
            //foreach(var cover in _icover.GetAll())
            //{
            //    foreach (var song in cover.Songs)
            //    {
            //        if (song.AlbumId != null)
            //        {
            //            if (_ialbum.GetById(song.AlbumId.Value) != null)
            //            {
            //                var coverAlbumElem = new CoverAlbum
            //                {
            //                    Cover = cover,
            //                    Album = _ialbum.GetById(song.AlbumId.Value)
            //                };
            //                coverAlbum.Add(coverAlbumElem);
            //            }
            //        }
            //    }
            //}

            //var allcovers = _icover.GetAll();
            var allsongs = _isong.GetAll().ToList();
            foreach (var song in allsongs)
            {
                if (song.CoverId != null)
                {
                    int s = Convert.ToInt32(song.CoverId);
                    song.Cover = _icover.GetById(s);
                }

                if (song.AlbumId != null)
                {
                    int a = Convert.ToInt32(song.AlbumId);
                    song.Album = _ialbum.GetById(a);
                }
            }

            var allalbums = _ialbum.GetAll();
            foreach (var album in allalbums)
            {
                
            }
            var covers_albums = new List<CoverAlbum>();
            foreach (var album in allalbums) 
            {
                //int c;
                album.Songs = allsongs.Where(x => x.AlbumId == album.Id).ToList();
                if (album.Songs.Any())
                {
                    int cid = Convert.ToInt32(album.Songs.First().CoverId);
                    var cover_album = new CoverAlbum { Album = album, Cover = _icover.GetById(cid) };
                    covers_albums.Add(cover_album);
                }
            }

            var data = Tuple.Create(allsongs, covers_albums);
            return View("CoversPageView", data);
        }
    }
}
