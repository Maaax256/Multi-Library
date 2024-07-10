CREATE ROLE admin login WITH PASSWORD '5927';

GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO Admin;
GRANT ALL PRIVILEGES ON ALL PROCEDURES IN SCHEMA public TO Admin;
GRANT ALL PRIVILEGES ON ALL FUNCTIONS IN SCHEMA public TO Admin;
grant select on author_with_song_details, song_with_cover_details to admin;

create role guest login with password '1234';

grant select on all tables in schema public to guest;
grant insert on user_table to guest;
grant execute on function check_what_role to guest; 

create role author login password '4758';

grant select, insert, update on all tables in schema public to author;
grant delete on album, video_clip, song, cover, author_song to author;
grant execute on procedure change_user_info to author;
grant execute on procedure remove_song to author;
grant execute on function get_author_singles to author;
grant execute on function check_what_role to author;

create role authorized_user login password '1425';

grant select on all tables in schema public to authorized_user;
grant insert on author_song, user_table to authorized_user;
grant update on user_table to authorized_user;
grant execute on procedure change_user_info to authorized_user;
grant execute on function check_what_role to authorized_user;

