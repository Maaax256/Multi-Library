
CREATE OR REPLACE PROCEDURE remove_song(id_song INT)
AS $$
BEGIN

  DELETE FROM video_clip
  WHERE song_id = id_song;

  DELETE FROM author_song
  WHERE song_id = id_song;

  DELETE FROM song
  WHERE id = id_song;

END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE change_user_info(
  IN user_id INT,
  IN new_name VARCHAR(50),
  IN is_author BOOLEAN,
  IN new_description TEXT
)
AS $$
BEGIN
  UPDATE user_table
  SET name = new_name,
      description = new_description,
      u_type = CASE WHEN u_type != 2 THEN
                  CASE WHEN is_author THEN 1 ELSE 0 END
               ELSE u_type
              END
  WHERE id = user_id;
END;
$$ LANGUAGE plpgsql;