DELETE FROM constraints;

INSERT INTO constraints VALUES
  (uuid_generate_v4(), 4, 'd43ca857-02ae-4449-8f86-2c7e2e1bca69', NULL, NULL, 'C305', NULL, NULL, NULL, NULL, NULL, NULL), --SOFT_ROOM_PREFERENCE
  (uuid_generate_v4(), 5, NULL, 'Data Science', NULL, NULL, NULL, 'Friday', '14:00 - 16:00', NULL, NULL, NULL), --SOFT_TIME_CHANGE
  (uuid_generate_v4(), 6, NULL, 'Artificial Intelligence', NULL, NULL, NULL, 'Thursday', '08:00 - 10:00', NULL, NULL, NULL), --SOFT_DAY_CHANGE
  (uuid_generate_v4(), 13, 'd43ca857-02ae-4449-8f86-2c7e2e1bca69', NULL, NULL, NULL, NULL, 'Tuesday', '08:00 - 20:00', NULL, NULL, NULL), --SOFT_INTERVAL_UNAVAILABILITY
  (uuid_generate_v4(), 10, NULL, 'Operating Systems', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL); --SOFT_WEEK_EVENNESS