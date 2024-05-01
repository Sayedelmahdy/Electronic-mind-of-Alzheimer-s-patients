
# Electronic Mind of Alzheimer Patient

## Project Overview
The Electronic Mind of Alzheimer Patient is an advanced healthcare management system tailored to assist caregivers in managing Alzheimer's patients. This application integrates real-time patient monitoring, appointment scheduling, and medication management across mobile platforms using Flutter, with a robust backend supported by FastAPI and AI capabilities for facial recognition.

## Key Features
- **User Authentication**: Secure role-based access control with ASP.NET Core Identity, JWT for authentication, and face recognition for patient login.
- **Real-Time Communication**: Utilizes SignalR for live updates on patient location, appointment scheduling, and medication alerts.
- **AI-Enhanced Features**:
  - **Face Recognition for Patient Login and Family Recognition**: Implements facial recognition for patient login and identifying family members or caregivers in images to assist patients with memory loss. Built using FastAPI, OpenCV, and the face_recognition library to process and recognize facial features securely and efficiently.
- **Email Integration**: Automated email notifications for account verification and significant health reminders using SMTP.
- **Database Management**: Handles data operations efficiently with Entity Framework Core for high performance and security.

## Technologies Used

This project incorporates a variety of technologies, each chosen for their specific strengths in handling aspects of backend development, security, frontend integration, and more.

### Backend Technologies

- **FastAPI**: Chosen for its high performance and ease of use, FastAPI is used to handle asynchronous server requests, enhancing the application's responsiveness. It also provides automatic interactive API documentation.
- **ASP.NET Core**: Utilized for its robustness in developing secure and scalable applications. It simplifies user management and secure authentication.
- **Entity Framework Core**: This ORM simplifies data interaction, allowing for LINQ queries and strong type-checking, which minimizes runtime errors.
- **SignalR**: Employed for its real-time web functionality, enabling instant communication for updates on patient tracking, appointments, and medication reminders.

### AI & Image Processing

- **OpenCV**: Used for image processing tasks, enhancing image quality before they are processed for facial recognition, thereby improving accuracy.
- **face_recognition library**: Implements facial detection and recognition efficiently, essential for the application's feature allowing patients to identify family and caregivers easily.

### Security

- **JWT (JSON Web Tokens)**: Secures information transmission between parties, essential for creating access tokens post-authentication.
- **ASP.NET Core Identity**: Manages user authentication and authorization, supporting features like password hashing and role management.

### Frontend

- **Flutter**: Allows compilation into native applications from a single codebase, providing a seamless user experience across all platforms.

### Database

- **Microsoft SQL Server**: Chosen for its robustness and scalability, it handles all application data securely and efficiently.

### Other Tools

- **SMTP for email services**: Handles all email-related functionalities within the application, including alerts and notifications.
- **GitHub for version control**: Manages source code, tracking changes, and collaborative development effectively.

## Development Approach
- Applied Agile methodologies for rapid development cycles and continuous iteration based on user feedback.
- Collaborated with a multidisciplinary team to align backend solutions with frontend needs and user experience.

## Repository Structure
```
/ElectronicMind
  ├── /Controllers      # API controllers including authentication and image processing
  ├── /Models           # Entity models and business logic
  ├── /Services         # Business services for data management and AI integration
  ├── /Hubs             # SignalR hubs for real-time web communication
  ├── /DTOs             # Data transfer objects
  ├── /Migrations       # Database migrations for Entity Framework
  └── README.md
```

## Setup and Installation
1. **Clone the Repository**:
   ```bash 
   git clone https://github.com/Sayedelmahdy/Electronic-mind-of-Alzheimer-s-patients/.git
   ```
2. **Navigate to the project directory**:
   ```bash
   cd ElectronicMind
   dotnet restore
   ```
3. **Configure the Database Connection** in `appsettings.json`.
4. **Apply Migrations**:
   ```bash
   dotnet ef database update
   ```
5. **Run the Application**:
   ```bash
   dotnet run
   ```

## Contributing
We welcome contributions to improve application functionalities or fix issues. Please discuss potential changes via issues before submitting a pull request.

## License
[MIT](https://choosealicense.com/licenses/mit/)
