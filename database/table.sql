
CREATE EXTENSION IF NOT EXISTS btree_gist;  -- needed for the overlap constraint on schedule

-- ---------------------------------------------------------------------
-- People and access
-- ---------------------------------------------------------------------

CREATE TABLE contributors (
  id      INTEGER      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name    VARCHAR(100) NOT NULL,
  contact VARCHAR(200)  -- Optional: YouTube, Niconico, email, LINE, etc.
);

-- access_role = what the person can do on the website.
CREATE TABLE accounts (
  username      VARCHAR(10) PRIMARY KEY,
  access_role   VARCHAR(20) NOT NULL DEFAULT 'Viewer' CHECK (access_role IN ('Viewer', 'Admin', 'Artist', 'Staff')),
  password_hash VARCHAR(255) NOT NULL,
  last_login    TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
  created_at    TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

  -- Lets the link tables below prove the account's access_role matches.
  UNIQUE (username, access_role)
);

-- job_role = what the person does in the team
-- (Coordinator, Logistics, Consumption, Venue, Manager, ...).
-- Left as free text because the list is open-ended; swap for a
-- staff_roles lookup table later if you want it controlled.
CREATE TABLE staff (
  id        VARCHAR(10)  PRIMARY KEY,
  name      VARCHAR(100) NOT NULL,
  join_date DATE         NOT NULL,
  birthday  DATE         NOT NULL,
  job_role  VARCHAR(30)  NOT NULL DEFAULT 'Staff'
);

CREATE TABLE artists (
  id        INTEGER      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name      VARCHAR(150) NOT NULL, -- Stage name, not real name; e.g. Isekaijoucho / Joucho
  debut     DATE         NOT NULL, -- Used to calculate debut anniversaries
  birthday  DATE         NOT NULL, -- Used to calculate birthday :D
  is_active BOOLEAN      NOT NULL DEFAULT TRUE
);

-- An account linked to an artist must have access_role = 'Artist'.
-- The composite FK enforces that, and it also means one account can
-- never be linked to both an artist and a staff member.
-- To change the role of a linked account, delete the link row first.
CREATE TABLE account_artists (
  username    VARCHAR(10) PRIMARY KEY,
  access_role VARCHAR(20) NOT NULL DEFAULT 'Artist' CHECK (access_role = 'Artist'),
  artist_id   INTEGER     NOT NULL UNIQUE REFERENCES artists(id) ON DELETE CASCADE,

  FOREIGN KEY (username, access_role)
    REFERENCES accounts(username, access_role) ON DELETE CASCADE
);

CREATE TABLE account_staff (
  username    VARCHAR(10) PRIMARY KEY,
  access_role VARCHAR(20) NOT NULL DEFAULT 'Staff' CHECK (access_role = 'Staff'),
  staff_id    VARCHAR(10) NOT NULL UNIQUE REFERENCES staff(id) ON DELETE CASCADE,

  FOREIGN KEY (username, access_role)
    REFERENCES accounts(username, access_role) ON DELETE CASCADE
);

CREATE TABLE staff_artists (
  staff_id  VARCHAR(10) NOT NULL REFERENCES staff(id)   ON DELETE CASCADE,
  artist_id INTEGER     NOT NULL REFERENCES artists(id) ON DELETE CASCADE,

  PRIMARY KEY (staff_id, artist_id)
);

-- ---------------------------------------------------------------------
-- Music catalogue
-- ---------------------------------------------------------------------

CREATE TABLE albums (
  album_id     VARCHAR(10)  PRIMARY KEY,
  title        VARCHAR(150) NOT NULL,
  release_date DATE         NOT NULL,
  cover_url    VARCHAR(500) NOT NULL,
  album_type   VARCHAR(20)  NOT NULL
               CHECK (album_type IN ('Single', 'EP', 'Album', 'Compilation'))
);

CREATE TABLE songs (
  song_id       VARCHAR(5)   PRIMARY KEY, -- Example: x9aS8 (generate with collision retry)
  youtube_id    VARCHAR(15),              -- Nullable: not every song is on YouTube
  title         VARCHAR(100) NOT NULL,
  special_notes VARCHAR(1000),            -- Nullable: exclusive tracks, unreleased tracks, etc.
  release_date  TIMESTAMPTZ,              -- Nullable: release date/time may be unknown or internal
  thumbnail_url VARCHAR(500)
);

CREATE UNIQUE INDEX songs_youtube_id_key ON songs (youtube_id) WHERE youtube_id IS NOT NULL;

CREATE TABLE song_artists (
  song_id   VARCHAR(5) NOT NULL REFERENCES songs(song_id)  ON DELETE CASCADE,
  artist_id INTEGER    NOT NULL REFERENCES artists(id)     ON DELETE CASCADE,

  PRIMARY KEY (song_id, artist_id)
);

CREATE TABLE album_tracks (
  album_id     VARCHAR(10) NOT NULL REFERENCES albums(album_id) ON DELETE CASCADE,
  song_id      VARCHAR(5)  NOT NULL REFERENCES songs(song_id)   ON DELETE RESTRICT,
  track_number INTEGER     NOT NULL CHECK (track_number > 0),

  -- Deferred so tracks can be reordered/swapped inside one transaction.
  PRIMARY KEY (album_id, track_number) DEFERRABLE INITIALLY DEFERRED,
  UNIQUE (album_id, song_id)
);

CREATE TABLE song_credits (
  song_id        VARCHAR(5)   NOT NULL REFERENCES songs(song_id)        ON DELETE CASCADE,
  contributor_id INTEGER      NOT NULL REFERENCES contributors(id)      ON DELETE RESTRICT,
  role           VARCHAR(100) NOT NULL,

  -- A contributor can have several roles on a song, but not the same role twice.
  PRIMARY KEY (song_id, contributor_id, role)
);

-- ---------------------------------------------------------------------
-- Concerts
-- ---------------------------------------------------------------------

CREATE TABLE concerts (
  concert_id      VARCHAR(50)  PRIMARY KEY, -- Example: ISEKAIJOUCHO-ANIMAIV
  concert_name    VARCHAR(200) NOT NULL,    -- ANIMA, ANIMA II, ANIMA III, ANIMA IV
  estimated_start TIMESTAMPTZ  NOT NULL,
  estimated_end   TIMESTAMPTZ  NOT NULL,
  venue_location  VARCHAR(200) NOT NULL,    -- Venue name or address
  created_at      TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at      TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
  created_by      VARCHAR(10)  REFERENCES accounts(username) ON DELETE SET NULL,
  updated_by      VARCHAR(10)  REFERENCES accounts(username) ON DELETE SET NULL,

  CHECK (estimated_start < estimated_end)
);

CREATE TABLE concert_artists (
  concert_id VARCHAR(50) NOT NULL REFERENCES concerts(concert_id) ON DELETE CASCADE,
  artist_id  INTEGER     NOT NULL REFERENCES artists(id)          ON DELETE CASCADE,

  PRIMARY KEY (concert_id, artist_id)
);

-- Surrogate id so setlist_performers can reference an entry without
-- depending on its position (a deferrable key cannot be referenced).
-- Positions can therefore be reordered freely inside a transaction.
CREATE TABLE concert_setlist (
  id         INTEGER     GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  concert_id VARCHAR(50) NOT NULL REFERENCES concerts(concert_id) ON DELETE CASCADE,
  song_id    VARCHAR(5)  NOT NULL REFERENCES songs(song_id)       ON DELETE RESTRICT,
  position   INTEGER     NOT NULL CHECK (position > 0),

  -- Each position appears once per concert; deferred to allow reordering.
  UNIQUE (concert_id, position) DEFERRABLE INITIALLY DEFERRED,

  -- Lets setlist_performers prove its concert_id matches the entry's.
  UNIQUE (id, concert_id)
);

CREATE TABLE setlist_performers (
  setlist_id INTEGER     NOT NULL,
  concert_id VARCHAR(50) NOT NULL,
  artist_id  INTEGER     NOT NULL,

  PRIMARY KEY (setlist_id, artist_id),

  FOREIGN KEY (setlist_id, concert_id)
    REFERENCES concert_setlist(id, concert_id) ON DELETE CASCADE,

  -- Performer must actually be an artist of this concert.
  FOREIGN KEY (concert_id, artist_id)
    REFERENCES concert_artists(concert_id, artist_id) ON DELETE CASCADE
);

-- ---------------------------------------------------------------------
-- Schedule
-- ---------------------------------------------------------------------

-- One row = one artist's activity (a group activity means one row per artist).
-- ASSUMPTION: an artist cannot be double-booked. Cancelled items are
-- ignored. If overlaps are sometimes valid, drop the EXCLUDE constraint
-- and detect conflicts in the backend instead.
CREATE TABLE schedule (
  id                   VARCHAR(5)   PRIMARY KEY, -- generate with collision retry
  artist_id            INTEGER      NOT NULL REFERENCES artists(id) ON DELETE RESTRICT,
  activity_title       VARCHAR(100) NOT NULL,
  activity_description VARCHAR(2500),
  start_time           TIMESTAMPTZ  NOT NULL,
  estimated_end_time   TIMESTAMPTZ  NOT NULL,
  location             VARCHAR(200) NOT NULL,    -- Venue, office, etc.
  category             VARCHAR(20)  NOT NULL DEFAULT 'Other'
                            CHECK (category IN ('Concert', 'Recording', 'Rehearsal',
                                                'Interview', 'Event', 'Off', 'Other')),
  status               VARCHAR(20)  NOT NULL DEFAULT 'Planned' CHECK (status IN ('Planned', 'Confirmed', 'Cancelled')),
  is_public            BOOLEAN      NOT NULL DEFAULT FALSE, -- visible to Viewer accounts?
  concert_id           VARCHAR(50),                         -- NULL if unrelated to a concert
  created_at           TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at           TIMESTAMPTZ  NOT NULL DEFAULT CURRENT_TIMESTAMP,
  created_by           VARCHAR(10)  REFERENCES accounts(username) ON DELETE SET NULL,
  updated_by           VARCHAR(10)  REFERENCES accounts(username) ON DELETE SET NULL,

  CHECK (start_time < estimated_end_time),

  -- The artist must be part of the concert. If the concert (or the artist's
  -- place in it) is removed, the activity stays and is detached from it.
  -- Column list in SET NULL requires PostgreSQL 15+.
  FOREIGN KEY (concert_id, artist_id)
    REFERENCES concert_artists(concert_id, artist_id)
    ON DELETE SET NULL (concert_id),

  CONSTRAINT schedule_no_artist_overlap
    EXCLUDE USING gist (
      artist_id WITH =,
      tstzrange(start_time, estimated_end_time) WITH &&
    ) WHERE (status <> 'Cancelled')
);

-- Which staff members are responsible for / attending an activity.
CREATE TABLE schedule_staff (
  schedule_id VARCHAR(5)  NOT NULL REFERENCES schedule(id) ON DELETE CASCADE,
  staff_id    VARCHAR(10) NOT NULL REFERENCES staff(id)    ON DELETE RESTRICT,

  PRIMARY KEY (schedule_id, staff_id)
);

-- ---------------------------------------------------------------------
-- Indexes (FK columns not already covered by a primary/unique key)
-- ---------------------------------------------------------------------

CREATE INDEX ON song_artists       (artist_id);
CREATE INDEX ON song_credits       (contributor_id);
CREATE INDEX ON album_tracks       (song_id);
CREATE INDEX ON staff_artists      (artist_id);
CREATE INDEX ON concert_artists    (artist_id);
CREATE INDEX ON concert_setlist    (song_id);
CREATE INDEX ON setlist_performers (concert_id, artist_id);
CREATE INDEX ON setlist_performers (artist_id);
CREATE INDEX ON schedule           (artist_id, start_time);
CREATE INDEX ON schedule           (start_time);  -- all-artist calendar range queries
CREATE INDEX ON schedule           (concert_id);
CREATE INDEX ON schedule_staff     (staff_id);