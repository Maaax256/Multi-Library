using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

#nullable disable

namespace Multi_Library.Models
{
    public partial class UserTable
    {
        public UserTable()
        {
            AuthorSongs = new HashSet<AuthorSong>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        //2 - admin
        //1 - author
        //0 - authorized_user
        public int UserType { get; set; }
        public DateTime? DateCreate { get; set; }

        public virtual ICollection<AuthorSong> AuthorSongs { get; set; }

        //public static UserTable ActiveUser { get; set; }
    }
    //[Flags]
    public enum user_type
    {
        Admin, Author, User
    }
}
