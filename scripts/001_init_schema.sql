USE gamificamivida;

CREATE TABLE IF NOT EXISTS users (
  id BINARY(16) NOT NULL,
  email VARCHAR(254) NOT NULL,
  display_name VARCHAR(100) NOT NULL,
  created_at_utc DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
  PRIMARY KEY (id),
  UNIQUE KEY uq_users_email (email)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS items (
  id BINARY(16) NOT NULL,
  user_id BINARY(16) NOT NULL,
  type TINYINT NOT NULL,
  title VARCHAR(200) NOT NULL,
  notes TEXT NULL,
  due_at_utc DATETIME(6) NULL,
  is_completed TINYINT(1) NOT NULL DEFAULT 0,
  completed_at_utc DATETIME(6) NULL,
  xp_value INT NOT NULL DEFAULT 0,
  created_at_utc DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
  updated_at_utc DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
  PRIMARY KEY (id),
  KEY ix_items_user_due (user_id, due_at_utc),
  KEY ix_items_user_completed (user_id, is_completed),
  CONSTRAINT fk_items_users
    FOREIGN KEY (user_id) REFERENCES users(id)
    ON DELETE CASCADE
) ENGINE=InnoDB;
