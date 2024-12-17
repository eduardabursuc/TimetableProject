export interface Constraint {
    id: string,
    type: ConstraintType,
    professorId: string,
    courseName: string,
    roomName: string,
    wantedRoomName: string,
    groupName: string,
    day: string,
    wantedDay: string,
    time: string,
    wantedTime: string,
    event: string
  }

  export enum ConstraintType {
    HARD_NO_OVERLAP,
        HARD_YEAR_PRIORITY,
        HARD_ROOM_CAPACITY,
        
        // Soft Constraints (Room)
        SOFT_ROOM_CHANGE,
        SOFT_ROOM_PREFERENCE,
        
        // Soft Constraints (Time)
        SOFT_TIME_CHANGE,
        SOFT_DAY_CHANGE,
        SOFT_ADD_WINDOW,
        SOFT_REMOVE_WINDOW,
        SOFT_DAY_OFF,
        
        // Soft Constraints (Structure)
        SOFT_WEEK_EVENNESS,
        SOFT_CONSECUTIVE_HOURS,
        SOFT_INTERVAL_AVAILABILITY,
        SOFT_INTERVAL_UNAVAILABILITY,
        SOFT_LECTURE_BEFORE_LABS
  }
  