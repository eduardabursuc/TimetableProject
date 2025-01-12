# constraint_types.py

CONSTRAINT_REQUIREMENTS = {
    "SOFT_ROOM_CHANGE": [
        ["CourseName", "Event", "GroupName"],
        ["CourseName", "Event", "Day", "Time"],
    ],
    "SOFT_ROOM_PREFERENCE": [
        ["WantedRoomName", "CourseName", "Event"],
        ["WantedRoomName", "ProfessorId"],
    ],
    "SOFT_TIME_CHANGE": [
        ["Day", "Time"],
        ["CourseName", "Event", "GroupName"],
    ],
    "SOFT_DAY_CHANGE": [
        ["Day", "Time"],
        ["CourseName", "Event", "GroupName"],
    ],
    "SOFT_ADD_WINDOW": [
        ["Day", "Time"],
        ["Day"],
    ],
    "SOFT_REMOVE_WINDOW": [
        ["Day", "Time"],
        ["Day"],
    ],
    "SOFT_DAY_OFF": [["Day"]],
    "SOFT_WEEK_EVENNESS": [["CourseName"]],
    "SOFT_CONSECUTIVE_HOURS": [["Day"]],
    "SOFT_INTERVAL_AVAILABILITY": [["Day", "Time"]],
    "SOFT_INTERVAL_UNAVAILABILITY": [["Day", "Time"]],
    "SOFT_LECTURE_BEFORE_LABS": [
        ["CourseName"],
        ["ProfessorId"],
    ]
}

TYPE_MAPPING = {
    "SOFT_ROOM_CHANGE": 3,
    "SOFT_ROOM_PREFERENCE": 4,
    "SOFT_TIME_CHANGE": 5,
    "SOFT_DAY_CHANGE": 6,
    "SOFT_ADD_WINDOW": 7,
    "SOFT_REMOVE_WINDOW": 8,
    "SOFT_DAY_OFF": 9,
    "SOFT_WEEK_EVENNESS": 10,
    "SOFT_CONSECUTIVE_HOURS": 11,
    "SOFT_INTERVAL_AVAILABILITY": 12,
    "SOFT_INTERVAL_UNAVAILABILITY": 13,
    "SOFT_LECTURE_BEFORE_LABS": 14
}

def validate_constraint_data(constraint_type, matched_data):
    """
    Validate matched data against the requirements for the given constraint type.

    Args:
        constraint_type (str): The type of constraint being validated.
        matched_data (dict): The data matched from the input.

    Returns:
        tuple: (is_valid (bool), missing_fields (list)).
    """
    if constraint_type not in CONSTRAINT_REQUIREMENTS:
        return False, [f"Unknown constraint type: {constraint_type}"]

    requirements = CONSTRAINT_REQUIREMENTS[constraint_type]

    for option in requirements:
        missing_fields = [field for field in option if field not in matched_data]
        if not missing_fields:  # If all fields in this option are present
            return True, []  # Valid for this option

    # If no option is satisfied, return missing fields from the last checked option
    return False, missing_fields

