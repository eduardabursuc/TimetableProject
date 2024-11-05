# AI Generated TimeTable Feature Documentation

## 1. Abstract
Our goal is to develop a subscription-based web application that streamlines the scheduling process for educational institutions. The application will provide annual access to AI-generated timetables that accommodate custom constraints entered by professors in natural language. Once the core features are completed, we plan to implement additional enhancements.

---

## 2. Project Levels

### Overview of Levels
The project will be developed in stages to ensure a scalable and efficient system. Each level introduces new features and improvements.

---

### Level 1: Core Features

#### 1.1 Features
- **User Roles and Authentication**:
  - **Admin**:
    - Granted upon purchasing an annual subscription.
    - Perform CRUD operations on groups, rooms, professors, courses, and constraints.
    - Adding a professor generates an account for that professor.
  - **Professor**:
    - Manage (CRUD) constraints that shape the timetable.
    - Constraints are entered in natural language and mapped to predefined categories, such as:
      - `INTERVAL_UNAVAILABILITY`, `CONSECUTIVE_HOURS`, `TIME_CHANGE`, `DAY_CHANGE`, etc.

- **Timetable Generation**:
  - AI generates timetables that respect global constraints and as many professor-specified constraints as possible.

#### 1.2 Technical Implementation
- **Architecture**: Basic client-server model with a database for persistent data storage.
- **Technologies**: ASP.NET Core for the web application, Entity Framework for database operations, and an AI engine for processing constraints.
---

### Level 2: Enhanced Features

- **User Roles and Enhancements**:
  - **Admin**:
    - Import the entire university domain to enable automatic user creation and login using university credentials.
  - **Professor**:
    - Add events related to specific courses (e.g., tests, online sessions with links, cancellations, and rescheduling).
  - **Student**:
    - View their assigned timetable.
    - Mark courses for retakes ("restanta") and receive suggestions for available groups that do not overlap with their existing schedule, with the ability to add the retake to their timetable.
    - Add personal notes to events, which are visible only to the student.


### C4 Model - Level 2
![C4 Diagram](C4DiagramContainers.png)
---

## 3. Potential for Level 3
- Expanding the system for non-educational use cases.