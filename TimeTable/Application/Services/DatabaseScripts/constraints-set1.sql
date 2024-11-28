DELETE FROM constraints;

INSERT INTO constraints VALUES
  (uuid_generate_v4(), 10, '1cf940ce-6dad-45a5-985f-35afd00e10f0', NULL, NULL, NULL, NULL, 'Monday', NULL, NULL, NULL, NULL), --SOFT_CONSECUTIVE_HOURS
  (uuid_generate_v4(), 10, NULL, NULL, NULL, NULL, NULL, 'Monday', NULL, NULL, NULL, NULL), --SOFT_CONSECUTIVE_HOURS
  (uuid_generate_v4(), 3, NULL, 'Machine Learning', NULL, NULL, '2B3', NULL, NULL, NULL, NULL, 'laboratory'), --SOFT_ROOM_CHANGE
  (uuid_generate_v4(), 11, '1cf940ce-6dad-45a5-985f-35afd00e10f0', NULL, NULL, NULL, NULL, 'Monday', '10:00 - 18:00', NULL, NULL, NULL), --SOFT_INTERVAL_AVAILABILITY
  (uuid_generate_v4(), 14, NULL, 'Computer Graphics', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL); --SOFT_LECTURE_BEFORE_LABS