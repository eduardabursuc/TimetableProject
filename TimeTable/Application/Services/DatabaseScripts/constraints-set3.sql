DELETE FROM constraints;

INSERT INTO constraints VALUES
  (uuid_generate_v4(), 7, '5499a2ba-3979-420a-9696-71fa801ca431', NULL, NULL, NULL, NULL, 'Monday', '12:00 - 14:00', NULL, NULL, NULL), --SOFT_ADD_WINDOW
  (uuid_generate_v4(), 8, 'fbd68d92-4c4d-44ac-922c-5200efb49cee', NULL, NULL, NULL, NULL, 'Wednesday', '11:00 - 12:00', NULL, NULL, NULL), --SOFT_REMOVE_WINDOW
  (uuid_generate_v4(), 9, 'fe059d2e-4fb1-46b8-81bf-d8cb1932c18f', NULL, NULL, NULL, NULL, 'Friday', NULL, NULL, NULL, NULL), --SOFT_DAY_OFF
  (uuid_generate_v4(), 11, '1cf940ce-6dad-45a5-985f-35afd00e10f0', NULL, NULL, NULL, NULL, 'Wednesday', NULL, NULL, NULL, NULL), --SOFT_CONSECUTIVE_HOURS
  (uuid_generate_v4(), 3, NULL, 'Machine Learning', NULL, NULL, '2B3', NULL, NULL, NULL, NULL, 'laboratory'); --SOFT_ROOM_CHANGE