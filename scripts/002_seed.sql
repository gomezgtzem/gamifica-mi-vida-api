USE gamificamivida;

SET @user_id = UNHEX(REPLACE(UUID(), "-", ""));

INSERT INTO users (id, email, display_name)
VALUES (@user_id, "emmanuel@example.com", "Emmanuel");

INSERT INTO items (id, user_id, type, title, notes, due_at_utc, xp_value)
VALUES
  (UNHEX(REPLACE(UUID(), "-", "")), @user_id, 1, "Plan del día (Top 3)", "Define 3 prioridades", UTC_TIMESTAMP(6) + INTERVAL 2 HOUR, 10),
  (UNHEX(REPLACE(UUID(), "-", "")), @user_id, 1, "Workout (Speediance)", "Upper/Lower según plan", UTC_TIMESTAMP(6) + INTERVAL 6 HOUR, 25),
  (UNHEX(REPLACE(UUID(), "-", "")), @user_id, 2, "Cita: Revisión semanal", "Revisar avance y ajustar", UTC_TIMESTAMP(6) + INTERVAL 1 DAY, 15),
  (UNHEX(REPLACE(UUID(), "-", "")), @user_id, 3, "Recordatorio: Agua", "2L al día", NULL, 5),
  (UNHEX(REPLACE(UUID(), "-", "")), @user_id, 1, "Aprender SQL: JOINs", "Practicar INNER/LEFT con items", UTC_TIMESTAMP(6) + INTERVAL 1 DAY + INTERVAL 3 HOUR, 20),
  (UNHEX(REPLACE(UUID(), "-", "")), @user_id, 1, "Push to GitHub", "Commit + push de avances", NULL, 10);

SELECT HEX(@user_id) AS seeded_user_id_hex;

SELECT
  u.email,
  i.type,
  i.title,
  i.is_completed,
  i.due_at_utc,
  i.xp_value
FROM users u
JOIN items i ON i.user_id = u.id
WHERE u.id = @user_id
ORDER BY i.due_at_utc IS NULL, i.due_at_utc
LIMIT 50;
