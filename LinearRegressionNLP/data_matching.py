from input_preprocessing import preprocess_for_matching
from constraint_types import validate_constraint_data, CONSTRAINT_REQUIREMENTS
from difflib import get_close_matches
import re

def standardize_room_number(room_text):
    """Convert room numbers to standard format (e.g., '411' → 'C411')"""
    room = re.sub(r'[^a-zA-Z0-9]', '', room_text)
    if room and room[0].isdigit():
        room = 'C' + room
    return room.upper()

def standardize_group_name(group_text):
    """Standardize group format (e.g., 'E12' → '1E2')"""
    group = re.sub(r'[^a-zA-Z0-9]', '', group_text.upper())
    if len(group) == 3:
        if group[0].isalpha():
            return group[1] + group[0] + group[2]
        return group
    return None

def get_course_abbreviation(course_name):
    """Get abbreviation from course name"""
    return ''.join(word[0].upper() for word in course_name.split())

def match_course(course_text, courses):
    """Match course name using fuzzy matching and abbreviations"""
    course_text = course_text.lower().strip()
    for course in courses:
        if course["CourseName"].lower() == course_text:
            return course
    for course in courses:
        abbr = get_course_abbreviation(course["CourseName"])
        if course_text.upper() == abbr:
            return course
    course_names = [c["CourseName"].lower() for c in courses]
    matches = get_close_matches(course_text, course_names, n=1, cutoff=0.6)
    if matches:
        return next(c for c in courses if c["CourseName"].lower() == matches[0])
    return None

def find_professor_id(professors, professor_email):
    """Find the ProfessorId based on the professor's email."""
    for professor in professors:
        if professor["Email"].lower() == professor_email.lower():
            return professor["Id"]
    return None

# ====================================================
# EVENT DETECTION
# ====================================================
# We’ll map various keywords to exactly one of:
#   - "Lecture"
#   - "Laboratory"
#   - "Seminary"
# If multiple keywords appear, we pick the first one found in the text.

EVENT_KEYWORD_MAPPING = {
    # Lab synonyms
    "lab": "Laboratory",
    "laboratory": "Laboratory",
    # Lecture synonyms
    "lecture": "Lecture",
    "class": "Lecture",
    "course": "Lecture",
    # Seminary synonyms
    "seminar": "Seminary",
    "seminary": "Seminary",
}

def find_event_type(text: str) -> str:
    text_lower = text.lower()
    # We'll search for these in the text in their lowercased form
    for keyword, normalized_event in EVENT_KEYWORD_MAPPING.items():
        if keyword in text_lower:
            return normalized_event
    return None

# ====================================================
# MAIN MATCH + VALIDATE
# ====================================================
def match_and_validate_data(input_text, constraint_type, user_data, professor_email):
    """
    Match and validate input text with database entities and validate against constraint requirements.

    Args:
        input_text (str): The original user input.
        constraint_type (str): Predicted constraint type.
        user_data (dict): User-specific data fetched from the database.
        professor_email (str): Email of the professor to find their ID.

    Returns:
        dict: Matched data if valid, or validation errors.
    """
    matched_data = {}
    tokens = preprocess_for_matching(input_text)

    # =========================
    #    MATCHING ROOMS
    # =========================
    room_pattern = r'\b[Cc]?\d{1,3}\b'
    potential_rooms = re.findall(room_pattern, input_text)

    # We'll collect all valid rooms that actually exist in the DB
    valid_rooms_found = []
    for room_text in potential_rooms:
        std_room = standardize_room_number(room_text)
        room_in_db = next((r for r in user_data["rooms"] if r["Name"] == std_room), None)
        if room_in_db:
            valid_rooms_found.append(std_room)

    # If we found at least one valid room, pick the *last* one as the wanted/new room
    if valid_rooms_found:
        matched_data["WantedRoomName"] = valid_rooms_found[-1]

    # =========================
    #    MATCHING GROUPS
    # =========================
    group_pattern = r'\b[1-9][A-Za-z][1-9]\b|\b[A-Za-z][1-9][1-9]\b'
    potential_groups = re.findall(group_pattern, input_text)
    for group_text in potential_groups:
        std_group = standardize_group_name(group_text)
        group_in_db = next((g for g in user_data["groups"] if g["Name"] == std_group), None)
        if group_in_db:
            matched_data["GroupName"] = std_group
            break

    # =========================
    #   MATCHING COURSES
    # =========================
    words = input_text.split()
    for i in range(len(words)):
        for j in range(i + 1, len(words) + 1):
            course_text = " ".join(words[i:j])
            course_match = match_course(course_text, user_data["courses"])
            if course_match:
                matched_data["CourseName"] = course_match["CourseName"]
                break
        if "CourseName" in matched_data:
            break

    # =========================
    #   MATCHING DAY / TIME
    # =========================
    day_pattern = r'\b(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)\b'
    matched_day = re.search(day_pattern, input_text, re.IGNORECASE)
    if matched_day:
        matched_data["Day"] = matched_day.group(0).capitalize()

    time_pattern = r'\b\d{1,2}:\d{2}(?:\s?[APap][Mm])?\b'
    matched_time = re.search(time_pattern, input_text)
    if matched_time:
        matched_data["Time"] = matched_time.group(0)

    # =========================
    #   MATCHING EVENT
    # =========================
    event_type = find_event_type(input_text)
    if event_type:
        matched_data["Event"] = event_type

    # =========================
    #  FETCH PROFESSOR ID (IF NEEDED)
    # =========================
    requires_professor_id = any("ProfessorId" in req for req in CONSTRAINT_REQUIREMENTS.get(constraint_type, []))
    if requires_professor_id:
        matched_data["ProfessorId"] = find_professor_id(user_data["professors"], professor_email)

    # =========================
    # VALIDATE THE MATCHED DATA
    # =========================
    is_valid, missing_fields = validate_constraint_data(constraint_type, matched_data)
    if not is_valid:
        return {
            "error": f"Validation failed for {constraint_type}. Missing fields: {missing_fields}"
        }

    return matched_data