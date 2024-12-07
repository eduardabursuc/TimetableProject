export enum ConstraintType {
    // Hard Constraints
    HARD_NO_OVERLAP = "HARD_NO_OVERLAP",
    HARD_YEAR_PRIORITY = "HARD_YEAR_PRIORITY",
    HARD_ROOM_CAPACITY = "HARD_ROOM_CAPACITY",
    
    // Soft Constraints (Room)
    SOFT_ROOM_CHANGE = "SOFT_ROOM_CHANGE",
    SOFT_ROOM_PREFERENCE = "SOFT_ROOM_PREFERENCE",
    
    // Soft Constraints (Time)
    SOFT_TIME_CHANGE = "SOFT_TIME_CHANGE",
    SOFT_DAY_CHANGE = "SOFT_DAY_CHANGE",
    SOFT_ADD_WINDOW = "SOFT_ADD_WINDOW",
    SOFT_REMOVE_WINDOW = "SOFT_REMOVE_WINDOW",
    SOFT_DAY_OFF = "SOFT_DAY_OFF",
    
    // Soft Constraints (Structure)
    SOFT_WEEK_EVENNESS = "SOFT_WEEK_EVENNESS",
    SOFT_CONSECUTIVE_HOURS = "SOFT_CONSECUTIVE_HOURS",
    SOFT_INTERVAL_AVAILABILITY = "SOFT_INTERVAL_AVAILABILITY",
    SOFT_INTERVAL_UNAVAILABILITY = "SOFT_INTERVAL_UNAVAILABILITY",
    SOFT_LECTURE_BEFORE_LABS = "SOFT_LECTURE_BEFORE_LABS"
}

export interface Constraint {
    timetableId: string;
    id: string;
    type: ConstraintType;
    professorId?: string;
    courseId?: string;
    roomId?: string;
    wantedRoomId?: string;
    groupId?: string;
    day?: string;
    time?: string;
    wantedDay?: string;
    wantedTime?: string;
    event?: string;
}