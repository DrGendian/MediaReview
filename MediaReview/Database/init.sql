CREATE TABLE media_type (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    name varchar(100),
    email varchar(384),
    hadmin bool, 
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE user_profile (
    user_id INTEGER PRIMARY KEY,
    total_ratings INTEGER DEFAULT 0,
    total_favorites INTEGER DEFAULT 0,
    avg_rating FLOAT DEFAULT 0,
    CONSTRAINT fk_user_profile_user
    FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE
);

CREATE TABLE media_entry (
    id SERIAL PRIMARY KEY,
    creator_id INTEGER NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    media_type INTEGER NOT NULL,
    release_year INTEGER,
    age_restriction INTEGER,
    avg_score FLOAT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_media_creator
        FOREIGN KEY (creator_id)
        REFERENCES users(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_media_type
        FOREIGN KEY (media_type)
        REFERENCES media_type(id)
);

CREATE TABLE genre (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE media_genre (
    media_id INTEGER NOT NULL,
    genre_id INTEGER NOT NULL,

    PRIMARY KEY (media_id, genre_id),

    CONSTRAINT fk_media_genre_media
        FOREIGN KEY (media_id)
        REFERENCES media_entry(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_media_genre_genre
        FOREIGN KEY (genre_id)
        REFERENCES genre(id)
        ON DELETE CASCADE
);

CREATE TABLE rating (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    media_id INTEGER NOT NULL,
    stars INTEGER NOT NULL CHECK (stars BETWEEN 1 AND 5),
    comment TEXT,
    comment_confirmed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_rating_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_rating_media
        FOREIGN KEY (media_id)
        REFERENCES media_entry(id)
        ON DELETE CASCADE,

    CONSTRAINT uq_user_media_rating
        UNIQUE (user_id, media_id)
);

CREATE TABLE rating_like (
    user_id INTEGER NOT NULL,
    rating_id INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (user_id, rating_id),

    CONSTRAINT fk_rating_like_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_rating_like_rating
        FOREIGN KEY (rating_id)
        REFERENCES rating(id)
        ON DELETE CASCADE
);
 
CREATE TABLE favorite (
    user_id INTEGER NOT NULL,
    media_id INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (user_id, media_id),

    CONSTRAINT fk_favorite_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_favorite_media
        FOREIGN KEY (media_id)
        REFERENCES media_entry(id)
        ON DELETE CASCADE
);

INSERT INTO media_type (name) VALUES ('movie'), ('series'), ('game');
