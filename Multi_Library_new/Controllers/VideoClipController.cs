using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Multi_Library.Interfaces;
using Multi_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Multi_Library.Controllers
{
    public class VideoClipController : Controller
    {
        private readonly IVideoClip _ivideoClip;
        private readonly ISong _isong;
        private readonly IAuthorSong _iauthorSong;
        private readonly IUserTable _iuserTable;
        //private readonly IVideoClipClipPlaylist _playlistVideoClip;

        public VideoClipController(ISong isong, IVideoClip ivideoClip, IAuthorSong authorSong, IUserTable iuserTable)
        {
            _isong = isong;
            _ivideoClip = ivideoClip;
            _iuserTable = iuserTable;
            _iauthorSong = authorSong;
        }

        public ViewResult AllVideClips()
        {
            var videoclip = _ivideoClip.GetAll();
            foreach (var clip in videoclip) 
            {
                clip.Song = _isong.GetById(clip.SongId);
            }
            var data = Tuple.Create(videoclip);
            return View("VideClips", data);
        }

        public ViewResult InfoClip(int clipId)
        {
            var videoclip = _ivideoClip.GetById(clipId);
            videoclip.Song = _isong.GetById(videoclip.SongId);
            var data = Tuple.Create(videoclip);
            return View("VideoClipPage", data);
        }

        public IActionResult CreateVideoClipPage()
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value);
            var user = _iuserTable.GetById(userId);

            if (user.UserType == 1)
            {
                var songsCreatedByCurrentUser = _isong.GetAll()
                .Where(song => _iauthorSong.GetAll().Any(authorSong =>
                    authorSong.SongId == song.Id && authorSong.AuthorId.ToString() ==
                    HttpContext.User.FindFirst("PersonId").Value)).ToList();

                var songsWithoutVideoClips = songsCreatedByCurrentUser
                    .Where(song => !_ivideoClip.GetAll().Any(clip => clip.SongId == song.Id))
                    .ToList();

                var data = Tuple.Create(songsWithoutVideoClips);
                return View("CreateVideoClip", data);
            }
            else
            {
                TempData["Message"] = "Создавать видеоклипы могут только авторы";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public IActionResult CreateVideoClip(int songId, IFormFile VideoClip)
        {
            if (VideoClip == null)
            {
                TempData["Message"] = "Вы не добавили файл видеоклипа";
                return RedirectToAction("Index", "Home");
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Video_Clip");
            string uniqueFileName = VideoClip.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                VideoClip.CopyTo(fileStream);
            }

            string coverFileUrl = Path.Combine("/Video_Clip", uniqueFileName);

            var Clip = new VideoClip
            {
                SongId = songId,
                Link = coverFileUrl,
                DateCreate = DateTime.Now
            };
            _ivideoClip.Add(Clip);
            return RedirectToAction("Index", "Home");
        }

        public ViewResult EditVideoClipPage(int videoClipID)
        {
            //var videoClip = _ivideoClip.GetById(videoClipID);
            //videoClip.Song = _isong.GetById(videoClip.SongId);
            //
            //var songsCreatedByCurrentUser = _isong.GetAll()
            //    .Where(song => _iauthorSong.GetAll().Any(authorSong =>
            //        authorSong.SongId == song.Id && authorSong.AuthorId == UserTable.ActiveUser.Id))
            //    .ToList();
            //                                                                                                   fix this
            //var songsWithoutVideoClips = songsCreatedByCurrentUser
            //    .Where(song => !_ivideoClip.GetAll().Any(clip => clip.SongId == song.Id))
            //    .Where(song => song.Id != videoClipID)
            //    .ToList();
            //
            //var data = Tuple.Create(videoClip, songsWithoutVideoClips);
            return View();//return View("EditVideoClip", data);
        }
        public IActionResult UpdateVideoClip(int videoClipId, int SongId, IFormFile VideoClip)
        {
            var videoClip = _ivideoClip.GetById(videoClipId);
            videoClip.SongId = SongId;
            videoClip.Song = _isong.GetById(SongId);
            if(VideoClip != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Video_Clip");
                string uniqueFileName = VideoClip.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    VideoClip.CopyTo(fileStream);
                }

                string coverFileUrl = Path.Combine("/Video_Clip", uniqueFileName);
                videoClip.Link = coverFileUrl;
            }
            _ivideoClip.Update(videoClip);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteVideoClip(int videoClipId)
        {
            //var removePlayList = _playlistVideoClip.GetAll().Where(x => x.VideoClipId == videoClipId);  
            //foreach(var removePlay in removePlayList)
            //{
            //    _playlistVideoClip.Delete(removePlay.VideoClipId, removePlay.ClipPlaylistId);                 fix this
            //}
            _ivideoClip.Delete(videoClipId);
            return RedirectToAction("Index", "Home");
        }
    }
}
