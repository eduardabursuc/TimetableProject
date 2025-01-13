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

import re
from datetime import time, timedelta

def parse_time_24h(hour_str: str, minute_str: str = None):
    """
    Given strings for hour and optional minute, return (hour, minute) in 24-hour format.
    If minute_str is None or empty, assume 00.
    Example: parse_time_24h('10') -> (10, 0)
             parse_time_24h('9','30') -> (9, 30)
    """
    hour = int(hour_str)
    minute = int(minute_str) if minute_str else 0
    # Basic clamp/validation if needed:
    # e.g., hour=23 max, minute=59 max, but that might be up to you
    return hour, minute

def time_to_string(hour: int, minute: int):
    """
    Convert hour and minute to 'HH:MM' string (24h format).
    Example: (9,5) -> '09:05'
    """
    return f"{hour:02d}:{minute:02d}"

def add_hours(hour: int, minute: int, hrs_to_add: int = 2):
    """
    Add hrs_to_add hours to the given hour/minute, 
    wrapping around if hour >= 24.
    """
    new_hour = hour + hrs_to_add
    new_minute = minute
    if new_hour >= 24:
        new_hour -= 24  # simple wrap for next day
    return new_hour, new_minute

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
#   - "course"
#   - "laboratory"
#   - "seminary"
# If multiple keywords appear, we pick the first one found in the text.

EVENT_KEYWORD_MAPPING = {
    # Lab synonyms
    "lab": "laboratory",
    "laboratory": "laboratory",
    # Lecture synonyms
    "class": "course",
    "lecture": "course",
    # Seminary synonyms
    "seminar": "seminary",
    "seminary": "seminary",
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

    # A regex capturing hours and optional minutes, e.g. "10", "10:00", "9:15"
    # We'll skip AM/PM logic for simplicity, but you could expand if needed.
    time_pattern = r'\b(\d{1,2})(?::(\d{1,2}))?\b'
    all_times = re.findall(time_pattern, input_text)  # list of (hour_str, minute_str)

    parsed_times = []
    for (hr_str, min_str) in all_times:
        try:
            hour, minute = parse_time_24h(hr_str, min_str)
            parsed_times.append((hour, minute))
        except ValueError:
            # If parsing fails, skip or handle it in some way
            continue

    if len(parsed_times) == 0:
        # No times found -> do nothing (or matched_data["Time"] = None)
        pass
    elif len(parsed_times) >= 2:
        # Use the first two, ignoring extras
        hour1, minute1 = parsed_times[0]
        hour2, minute2 = parsed_times[1]

        # Ensure hour1:minute1 <= hour2:minute2 if you want chronological ordering
        # For example:
        t1 = hour1*60 + minute1
        t2 = hour2*60 + minute2
        if t1 > t2:
            # swap to keep them in ascending order
            hour1, minute1, hour2, minute2 = hour2, minute2, hour1, minute1

        # Format as "HH:MM - HH:MM"
        time_str_1 = time_to_string(hour1, minute1)
        time_str_2 = time_to_string(hour2, minute2)
        matched_data["Time"] = f"{time_str_1} - {time_str_2}"
    else:
        # Exactly 1 time found
        hour1, minute1 = parsed_times[0]

        # Add 2 hours
        hour2, minute2 = add_hours(hour1, minute1, hrs_to_add=2)

        time_str_1 = time_to_string(hour1, minute1)
        time_str_2 = time_to_string(hour2, minute2)
        matched_data["Time"] = f"{time_str_1} - {time_str_2}"

    # =========================
    #   MATCHING EVENT
    # =========================
    event_type = find_event_type(input_text)
    if event_type:
        matched_data["Event"] = event_type

    # =========================
    #  FETCH PROFESSOR ID
    # =========================
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