drop function check_what_role; 

CREATE OR REPLACE FUNCTION check_what_role(login text)
RETURNS table(id integer, u_type integer) AS
$BODY$
  declare
  c_1 integer;
  
begin

  select count(*) into c_1 from user_table where name = login limit 1;
  
  if c_1 > 0 then
	return query select user_table.id, user_table.u_type FROM user_table WHERE name = login;
  end if;
  
end;
$BODY$
LANGUAGE plpgsql;

drop function get_author_singles;

CREATE OR REPLACE FUNCTION get_author_singles(user_id INTEGER)
RETURNS SETOF song AS $$
BEGIN
  RETURN QUERY
  SELECT *
  FROM song
  WHERE id IN (
    SELECT song_id
    FROM author_song
    WHERE author_id = user_id
  )
  AND album_id IS NULL;
END;
$$ LANGUAGE plpgsql;
