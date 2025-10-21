🛒 E-Commerce Microservices Project (.NET 8 | Docker | Kubernetes)

  This project is a simple E-Commerce system built using .NET 8 and designed following a Microservices Architecture with the CQRS (Command Query Responsibility Segregation) pattern.
  The primary goal of this project is to learn and demonstrate how containerized microservices communicate and function together using Docker and Kubernetes, providing a foundational understanding of distributed systems and cloud-ready application design.

🚀 Current Overview

  At this stage, the system is containerized and runs locally using Docker.
  The setup includes:
  - .NET 8 microservices (e.g., Catalog API)
  - Local Databases (e.g., Mongo for Catalog API)
  - Docker Compose for service orchestration
  - Swagger for testing and API exploration
  You can run and test the services locally using Docker containers, including both the API and database layers.

⚙️ Current Architecture Highlights

  Microservices Architecture using .NET 8
  CQRS Pattern for clear separation between command and query responsibilities
  MongoDB for product catalog data
  Docker & Docker Compose for containerized development and orchestration
  Kubernetes (K8s) used locally.
  Azure API Gateway setup for routing 

🔮 Future Roadmap
  This project is an ongoing learning process, and several enhancements are planned:

  Database Segregation with Event Sourcing
  - Current setup uses local databases.
  - The plan is to publish the databases and introduce an event-driven synchronization layer to fully realize the CQRS pattern.

  Cloud Deployment (Azure / AWS)
  - The app currently runs locally using Docker.
  - Next step is to deploy to Azure or AWS under the free tier, including:
  - Hosting microservices
  - Using managed databases
  - Configuring domain setup

🧠 Learning Focus
  This project is primarily for hands-on learning and understanding how real systems work behind the scenes:

  - Microservices communication
  - Containerization
  - CQRS pattern implementation
  - Event-driven architecture
  - Cloud deployment basics

📦 Technologies Used

  | Layer            | Technology              |
  | ---------------- | ----------------------- |
  | Framework        | .NET 8                  |
  | Database         | MongoDB                 |
  | Containerization | Docker, Docker Compose  |
  | Orchestration    | Kubernetes     |
  | API Gateway      | Azure API Gateway       |
  | Architecture     | Microservices + CQRS    |
  | Cloud Target     | Azure / AWS (Free Tier) |

💬 Notes
  🧩 In the current setup, this application can be fully demonstrated locally — including all APIs and databases — using Docker containers defined in the Dockerfile, docker-compose.yml, and docker-compose.override.yml.
  Databases can also be viewed and managed using tools like MongoDB Compass or Mongo Express when connected to the local container.
  This README file will get updated with the project progress.
