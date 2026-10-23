CREATE TABLE contributor (
  id      INTEGER      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name    VARCHAR(100) NOT NULL,
  contact VARCHAR(200) -- Optional: YouTube, Niconico, email, LINE, etc.
);

CREATE TABLE artists (
  id        INTEGER      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name      VARCHAR(150) NOT NULL, -- Stage name, not real name; e.g. Isekaijoucho / Joucho
  debut     DATE         NOT NULL, -- Used to calculate debut anniversaries
  birthday  DATE         NOT NULL -- Used to calculate birthday :D
);

CREATE TABLE songs (
  songID       VARCHAR(5)   PRIMARY KEY, -- Example: x9as8
  youtubeID    VARCHAR(15),              -- Nullable: not every song is on YouTube
  title        VARCHAR(100) NOT NULL,
  specialNotes VARCHAR(1000),            -- Nullable: exclusive tracks, unreleased tracks, etc.
  releaseDate  TIMESTAMPTZ,              -- Nullable: release date/time may be unknown or internal
  artists      INTEGER,

  FOREIGN KEY (songID, artists)
    REFERENCES songArtists(songID, artistId)
);

CREATE TABLE songArtists (
  songID   VARCHAR(5) NOT NULL REFERENCES songs(songID),
  artistId INTEGER    NOT NULL REFERENCES artists(id),
  PRIMARY KEY (songID, artistId)
);

CREATE TABLE songCredits (
  songID      VARCHAR(5)   NOT NULL REFERENCES songs(songID),
  contributor INTEGER      NOT NULL REFERENCES contributor(id),
  role        VARCHAR(100) NOT NULL,

  -- Each contributor can have multiple roles on a song,
  -- but the same role cannot be duplicated for that contributor/song.
  PRIMARY KEY (songID, contributor, role)
);

CREATE TABLE concerts (
  concertId       VARCHAR(50)  PRIMARY KEY, -- Example: ISEKAIJOUCHO-ANIMAIV
  concertName     VARCHAR(200) NOT NULL,    -- ANIMA, ANIMA II, ANIMA III, ANIMA IV
  estimatedStart  TIMESTAMPTZ  NOT NULL,
  estimatedEnd    TIMESTAMPTZ  NOT NULL,
  venueLocation   VARCHAR(200) NOT NULL,    -- Venue name or address

  -- Prevents a concert from ending before it starts.
  CHECK (estimatedStart < estimatedEnd)
);

CREATE TABLE concertArtists (
  concertId VARCHAR(50) NOT NULL REFERENCES concerts(concertId),
  artistId  INTEGER     NOT NULL REFERENCES artists(id),

  -- One concert can feature multiple artists.
  -- One artist can participate in multiple concerts.
  PRIMARY KEY (concertId, artistId)
);

CREATE TABLE concert_setlist (
  concertId VARCHAR(50) NOT NULL,
  songID    VARCHAR(5)  NOT NULL REFERENCES songs(songID),
  position  INTEGER     NOT NULL CHECK (position > 0),
  singer    INTEGER     NOT NULL,

  -- Checks if an artist is participating in the concert or not
  FOREIGN KEY (concertId, singer)
    REFERENCES concertArtists(concertId, artistId),

  -- Each position can appear only once per concert.
  PRIMARY KEY (concertId, position)
);

CREATE TABLE schedule (
  ID                  VARCHAR(5)    PRIMARY KEY,
  artistId            INTEGER       NOT NULL REFERENCES artists(id),
  activityTitle       VARCHAR(100)  NOT NULL,
  activityDescription VARCHAR(2500),
  startTime           TIMESTAMPTZ   NOT NULL,
  estimatedEndTime    TIMESTAMPTZ   NOT NULL,
  location            VARCHAR(200)  NOT NULL, -- Venue, office, etc.
  concertId           VARCHAR(50)   REFERENCES concerts(concertId), -- NULL if unrelated to a concert

  FOREIGN KEY (concertId, artistId)
    REFERENCES concertArtists(concertId, artistId)

  -- Prevents an activity from ending before it starts.
  CHECK (startTime < estimatedEndTime)
);

CREATE INDEX ON songArtists     (artistId);
CREATE INDEX ON songCredits     (contributor);
CREATE INDEX ON concert_setlist (songID);
CREATE INDEX ON schedule        (artistId, startTime);
CREATE INDEX ON schedule        (concertId);