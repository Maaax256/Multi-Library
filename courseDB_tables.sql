
CREATE TABLE user_table
(
  id integer PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  description text,
  u_type integer NOT NULL,
  password_hash text NOT NULL,
  date_create DATE
);

CREATE TABLE song
(
  id integer PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  lyrics text,
  link text NOT NULL,
  date_create DATE 
);

CREATE TABLE album
(
  id integer PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  description text, 
  date_create DATE
);

CREATE TABLE video_clip
(
  id integer PRIMARY KEY,
  link text NOT NULL,
  date_create DATE
);

CREATE TABLE cover
(
  id integer PRIMARY KEY,
  link text NOT NULL,
  date_create DATE
);

CREATE TABLE author_song
(
  author_id INTEGER NOT NULL REFERENCES user_table(id),
  song_id INTEGER NOT NULL REFERENCES song(id),
  PRIMARY KEY (author_id, song_id)
);


ALTER TABLE video_clip
	ADD COLUMN song_id int REFERENCES song(id) NOT NULL UNIQUE;
ALTER TABLE song
	ADD COLUMN album_id int REFERENCES album(id);
ALTER TABLE song
	ADD COLUMN cover_id int REFERENCES cover(id);


INSERT INTO user_table (name, description, u_type, password_hash)
VALUES ('Alex Ender', 'some description 1', 2,
 'fd0b2117927814c91a69f0caad051a6bcc8788c90c04edfd4ad9c7f2c43e3a0c');                 971385


       ('Max Lanford', 'some description 2', 1),		123456       
       ('Jake Sanders', 'some description 3', 0),               842679
       ('Amelia Jackson', 'some description 4', '1'),           134679
       ('Benjamin Miller', 'some description 5', '1'),          d6g5b1drh5fd3
       ('Charlotte Brown', 'some description 6', '1'),          f5vf5vd1lbkhvvod
       ('David Davis', 'some description 7', '0'),              gzzgdddvdbabejd
       ('Emily Walker', 'some description 8', '0'),             htgjftfsjarhah
       ('Felix Hernandez', 'some description 9', '0'),          thjrjdhfgsrj
       ('Grace Johnson', 'some description 10', '0'),           yfdhfhjsttjt
       ('Henry Moore', 'some description 11', '0'),             yvfjdzzjzj
       ('Isabella Garcia', 'some description 12', '0'),         gjxfjkfgkhgzd
       ('Jack Robinson', 'some description 13', '0');