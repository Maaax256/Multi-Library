using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Multi_Library.Interfaces;
using Multi_Library.Models;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Multi_Library_new.Models.DBModels;
using Microsoft.AspNetCore.Http;

namespace Multi_Library.Controllers
{
    public class AccountController : Controller
    {
        private readonly Mul_Lib_Context Context;
        private readonly IUserTable _iuserTable;
        private readonly ICover _icover;
        private readonly IAlbum _ialbum;
        private readonly ISong _isong;
        private readonly IVideoClip _iVideoClip;
        private readonly IAuthorSong _iAuthorSong;

        public AccountController(Mul_Lib_Context context, IUserTable iuserTable,
            ICover icover, IAlbum ialbum, IAuthorSong iAuthorSong,
            ISong isong, IVideoClip ivideoClip)
        {
            Context = context;
            _iuserTable = iuserTable;
            _icover = icover;
            _ialbum = ialbum;
            _iAuthorSong = iAuthorSong;
            _isong = isong;
            _iVideoClip = ivideoClip;
        }

        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;

        //public class RegisterViewModel
        //{
        //    public string Name { get; set; }
        //    public string Login { get; set; }
        //    public string Password { get; set; }
        //    public bool IsAuthor { get; set; }
        //}
        
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
                return hash;
            }
        }
        
        //public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //}   
        
        public ViewResult RegistrationPage()
        {
            return View("RegisterViewcshtml");
        }
        
        [HttpPost]
        public IActionResult Register(string login, string password, bool IsAuthor, string description)
        {
            if (_iuserTable.GetAll().Any(x => x.Name == login))
            {
                TempData["Message"] = "Ошибка: пользователь с таким логином уже существует.";
                return RedirectToAction("Index", "Home");
            }

            int utype;
            if (IsAuthor)
                utype = 1;
            else
                utype = 0;
        
            var passwordHash = HashPassword(password);

            var user = new UserTable 
            {
                Name = login,
                Description = description,
                UserType = utype,
                PasswordHash = passwordHash,
                DateCreate = DateTime.Now
            };
            
            _iuserTable.Add(user);
        
            TempData["Message"] = "Регистрация успешна.";
            return RedirectToAction("Index", "Home");
        }
        //
        //<code>
        //[HttpPost]
        //public IActionResult Login(LoginViewModel model)
        //{
        //    var user = _iuserTabel.GetAll().FirstOrDefault(x => x.login == model.login);
        //
        //    if (user == null)
        //    {
        //        TempData["Message"] = "Ошибка: пользователь с таким логином не существует";
        //        return RedirectToAction("Index", "Home");
        //    }
        //
        //    if(user.password_hash == HashPassword(model.password))
        //    {
        //        UserTable.ActiveUser = user;
        //        TempData["Message"] = $"Добро пожаловать, {UserTable.ActiveUser.Name}";
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        TempData["Message"] = "Ошибка: неверный пароль";
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
        // 
        public ViewResult ProfilePage()
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value);
            var user = _iuserTable.GetById(userId);
            var songsAuthor = _iAuthorSong.GetAll().Where(x => x.AuthorId == userId).ToList();
            var songs = songsAuthor.Select(songAuthor => _isong.GetById(songAuthor.SongId));
            foreach (var song in songs)
            {
                var cid = Convert.ToInt32(song.CoverId);
                song.Cover = _icover.GetById(cid);
            }
            var videoClipsAuthor = _iVideoClip.GetAll().Where(clip => songs.Any(song => song.Id == clip.SongId)).ToList();
        
            var authorSongs = _iAuthorSong.GetAll().Where(x => x.AuthorId == userId);
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
                        int cid = Convert.ToInt32(authorAlbumSongs[0].CoverId);
                        Cover cover = _icover.GetById(cid);
                        if (cover == null)
                        {
                            cover = new Cover { Link = "/default-cover-art.png" };
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
        
            var data = Tuple.Create(songs.AsEnumerable(), videoClipsAuthor.AsEnumerable(), albumCover.AsEnumerable(), user);
            return View("ProfileView", data);
        }
        public async Task<IActionResult> ChangeProfileInfo(string name, string description, bool author, string password)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value);
            var user = _iuserTable.GetById(userId);

            if (user.PasswordHash == HashPassword(password))
            {
                //user.Name = name;
                //if (user.UserType != 2)
                //    if (author)
                //        user.UserType = 1;
                //    else
                //        user.UserType = 0;
                //user.Description = description;
                //_iuserTable.Update(user);
                Context.Change_user_info(userId, name, author, description);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await Authenticate(user.Name, user.UserType, user.Id, password);

                TempData["Message"] = "Вы успешно внесли изменения.";
                return RedirectToAction("Index", "Home");
            }
            TempData["Message"] = "Не верный пароль.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteUsersPage()
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value);
            var user = _iuserTable.GetById(userId);

            //if (user.UserType == 2)
            //{
                var authors = _iuserTable.GetAll().Where(x => x.UserType == 1);
                var AuthorSong = _iAuthorSong.GetAll();
                var users = _iuserTable.GetAll().Where(x => x.UserType == 0);
                foreach (var authorSong in AuthorSong)
                {
                    if (_iuserTable.GetById(authorSong.AuthorId).UserType == 1)
                    {
                        authorSong.Author = _iuserTable.GetById(authorSong.AuthorId);
                        authorSong.Song = _isong.GetById(authorSong.SongId);
                    }
                }
                var data = Tuple.Create(AuthorSong, authors, users);
                return View("DeleteUsers", data);
            //}
            //else
            //{
            //    TempData["Message"] = "Удалять пользователей может только администратор";
            //}
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteUsers(int authorId)
        {
            //var user = _iuserTable.GetById(authorId);
            
            var a_s = _iAuthorSong.GetAll().Where(x => x.AuthorId == authorId);
            foreach (var a in a_s)
                _iAuthorSong.Delete(a.SongId, a.AuthorId);

            _iuserTable.Delete(authorId);
            TempData["Message"] = "Пользователь удалён";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult MostPopularSongs()
        {
            var usersong_favorites = _iAuthorSong.GetAll().Where(x => _iuserTable.GetById(x.AuthorId).UserType == 0);
            var songs = _isong.GetAll();
            foreach (var song in songs)
            {
                if (song.AlbumId != null)
                    song.Album = _ialbum.GetById((int)song.AlbumId);
                if (song.CoverId != null)
                    song.Cover = _icover.GetById((int)song.CoverId);
                //if (usersong_favorites.Any(x => x.SongId == song.Id))
                    song.FavoriteCount = usersong_favorites.Where(x => x.SongId == song.Id).Count();
            }
            var sorted_songs = songs.Where(x => x.FavoriteCount > 0).OrderByDescending(x => x.FavoriteCount).Take(3);
            return View("MostPopularSongs", sorted_songs);
        }

        public IActionResult AddFavoriteSong(int songid)
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value);
            var user = _iuserTable.GetById(userId);

            if (user.UserType == 0)
            {
                if (_iAuthorSong.GetById(songid, userId) != null)
                {
                    TempData["Message"] = "Эта песня уже добавлена к вам в избранное";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var authorSong = new AuthorSong()
                    {
                        AuthorId = userId,
                        SongId = songid
                    };
                    _iAuthorSong.Add(authorSong);
                    TempData["Message"] = "Песня добавлена";
                }
            }
            else
            {
                TempData["Message"] = "Добавлять в избранное могут только обычные пользователи";
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult FavoriteSongs()
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirst("PersonId").Value);
            var user = _iuserTable.GetById(userId);

            if (user.UserType == 0)
            {
                var usersongs = _iAuthorSong.GetAll().Where(x => x.AuthorId == userId).ToList();
                var songs = usersongs.Select(songAuthor => _isong.GetById(songAuthor.SongId));
                var albums = _ialbum.GetAll();

                foreach (var song in songs)
                {
                    if (song.CoverId != null)
                        song.Cover = _icover.GetById((int)song.CoverId);
                    if (song.AlbumId.HasValue)
                        song.Album = albums.FirstOrDefault(x => x.Id == song.AlbumId);
                }
                return View("FavoriteSongsPage", songs);
            }
            else
            {
                TempData["Message"] = "Просматривать избранное могут только обычные пользователи";
            }
            return RedirectToAction("Index", "Home");
        }

        public ViewResult LoginPage()
        {
            return View("LoginView");
        }

        //[HttpGet]
        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _iuserTable.GetAll().FirstOrDefault(x => x.Name == login);
                    
                    if (user == null)
                    {
                        TempData["Message"] = "Ошибка: пользователь с таким логином не существует";
                        return View("LoginView");
                    }
                    else
                    {
                        if (user.PasswordHash == HashPassword(password))
                        {
                            LoginModel lm = Context.Check_what_role(login).ToList().First(); 

                            await Authenticate(login, lm.RoleInt, lm.UserId, password);

                            //string username = HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;
                            //TempData["Message"] = $"Добро пожаловать, {username}";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["Message"] = "Ошибка: неверный пароль";
                            return View("LoginView");
                        }
                    }
                }
                catch (Exception e)
                {
                    TempData["Message"] = e.Message;
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        private async Task Authenticate(string userName, int RoleInt, int PersonId, string Password)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, RoleInt.ToString()),
                new Claim(nameof(PersonId), PersonId.ToString()),
                new Claim(nameof(Password), Password),
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        
        //public IActionResult Logout()
        //{
        //    return View();
        //}

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View("LoginView");
        }
    }
}
