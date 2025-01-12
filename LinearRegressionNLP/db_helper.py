import uuid
import psycopg2
from psycopg2.extras import RealDictCursor
from constraint_types import TYPE_MAPPING

class DatabaseHelper:
    def __init__(self, connection_string):
        self.connection_string = connection_string

    def fetch_user_data(self, user_email):
        try:
            with psycopg2.connect(self.connection_string) as conn:
                with conn.cursor(cursor_factory=RealDictCursor) as cursor:
                    # Fetch rooms
                    cursor.execute("SELECT * FROM rooms WHERE \"UserEmail\" = %s", (user_email,))
                    rooms = cursor.fetchall()

                    # Fetch groups
                    cursor.execute("SELECT * FROM groups WHERE \"UserEmail\" = %s", (user_email,))
                    groups = cursor.fetchall()
                    
                    # Fetch courses
                    cursor.execute("SELECT * FROM courses WHERE \"UserEmail\" = %s", (user_email,))
                    courses = cursor.fetchall()

                    # Fetch professors
                    cursor.execute("SELECT * FROM professors WHERE \"UserEmail\" = %s", (user_email,))
                    professors = cursor.fetchall()

            return {
                "rooms": rooms,
                "groups": groups,
                "courses": courses,
                "professors": professors
            }

        except Exception as e:
            print(f"Database error: {e}")
            return None

    def create_new_constraint(self, timetable_id, constraint_type, matched_data, user_data):
        """
        Insert a new constraint into the database, converting matched_data
        from 'name' fields into the actual IDs. Then store only the columns
        that exist in the constraints table.

        Returns a dict with structure:
        {
           "IsSuccess": bool,
           "Data": {...} or None,
           "ErrorMessage": str or None
        }
        """

        # 1) Generate the constraint's ID
        new_id = str(uuid.uuid4())

        # 2) Convert the textual constraint_type -> integer using TYPE_MAPPING
        #    (If missing, default to e.g., 999 or raise an error)
        constraint_type_value = TYPE_MAPPING.get(constraint_type, 999)

        # 3) Build a dictionary that maps exactly to the constraints table columns
        data_for_insert = {
            "Id": new_id,
            "TimetableId": timetable_id,
            "Type": constraint_type_value
        }

        # ============ PROFESSOR ID ==============
        # If we have matched_data["ProfessorId"], that is likely already a UUID
        # so we can store that in "ProfessorId":
        if "ProfessorId" in matched_data:
            data_for_insert["ProfessorId"] = matched_data["ProfessorId"]

        # ============ ROOM IDs ==============
        # The table has "RoomId" and "WantedRoomId".
        # If you want to store an "old" room, you'd do data_for_insert["RoomId"] = ...
        # For the "WantedRoomName", find the DB room ID in user_data.
        # Example: matched_data["WantedRoomName"] -> user_data["rooms"] => Id
        # but only if "WantedRoomName" actually exists:
        if "WantedRoomName" in matched_data:
            # find the DB entry
            name = matched_data["WantedRoomName"]
            found_room = next((r for r in user_data["rooms"] if r["Name"] == name), None)
            if found_room:
                data_for_insert["WantedRoomId"] = found_room["Id"]

        # If you also want to store the "old" room, do something like
        # if "OldRoomName" in matched_data:
        #     ...
        #     data_for_insert["RoomId"] = ...
        # (But your current code doesn't track the old room explicitly.)

        # ============ COURSE ID ==============
        if "CourseName" in matched_data:
            cname = matched_data["CourseName"]
            found_course = next((c for c in user_data["courses"] if c["CourseName"] == cname), None)
            if found_course:
                data_for_insert["CourseId"] = found_course["Id"]

        # ============ GROUP ID ==============
        if "GroupName" in matched_data:
            gname = matched_data["GroupName"]
            found_group = next((g for g in user_data["groups"] if g["Name"] == gname), None)
            if found_group:
                data_for_insert["GroupId"] = found_group["Id"]

        # ============ DAY / TIME / WANTEDDAY / WANTEDTIME / EVENT ==============
        # 'Day', 'Time', 'WantedDay', 'WantedTime', 'Event' exist in constraints table.
        if "Day" in matched_data:
            data_for_insert["Day"] = matched_data["Day"]
        if "Time" in matched_data:
            data_for_insert["Time"] = matched_data["Time"]

        # If you implement "WantedDay"/"WantedTime" in your matching logic,
        # place them here:
        # data_for_insert["WantedDay"] = ...
        # data_for_insert["WantedTime"] = ...

        if "Event" in matched_data:
            data_for_insert["Event"] = matched_data["Event"]

        # ============ Perform DB Insert ==============
        try:
            with psycopg2.connect(self.connection_string) as conn:
                with conn.cursor() as cursor:
                    # Dynamically build the SQL query using only the keys in data_for_insert
                    columns = ', '.join(f'"{key}"' for key in data_for_insert.keys())
                    placeholders = ', '.join(['%s'] * len(data_for_insert))
                    insert_sql = f'INSERT INTO "constraints" ({columns}) VALUES ({placeholders})'

                    cursor.execute(insert_sql, list(data_for_insert.values()))

            # Return success
            return {
                "IsSuccess": True,
                "Data": data_for_insert,  # the actual row inserted
                "ErrorMessage": None
            }

        except Exception as e:
            # Return failure
            return {
                "IsSuccess": False,
                "Data": None,
                "ErrorMessage": f"Database error inserting constraint: {e}"
            }