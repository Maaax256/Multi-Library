CREATE VIEW song_with_cover_details AS
SELECT s.id AS song_id,
       s.name AS song_name,
       s.lyrics AS song_lyrics,
       s.link AS song_link,
       s.date_create AS song_date,
	   s.cover_id AS cover_id,
       c.link AS cover_link
FROM song AS s
INNER JOIN cover AS c ON s.cover_id = c.id
WHERE s.cover_id IS NOT NULL AND s.date_create = CURRENT_DATE;

CREATE VIEW author_with_song_details AS
SELECT u.id AS author_id,
       u.name AS author_name,
	   u.date_create AS author_date_create,
       s.id AS song_id,
       s.name AS song_name,
       s.lyrics,
       s.link AS song_link,
       s.date_create AS song_date_create
FROM user_table AS u
INNER JOIN author_song AS a ON u.id = a.author_id
INNER JOIN song AS s ON a.song_id = s.id
WHERE u.u_type = 1;