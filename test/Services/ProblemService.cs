using MongoDB.Driver;
using test.Configuration;
using test.Models;

namespace test.Services
{
    public class ProblemService : IProblemService
    {
        private readonly IMongoCollection<Problem> _problems;
        private readonly IMongoCollection<Category> _categories;

        public ProblemService(MongoDbService mongoDbService, MongoDbSettings settings)
        {
            _problems = mongoDbService.Database.GetCollection<Problem>(settings.ProblemsCollectionName);
            _categories = mongoDbService.Database.GetCollection<Category>(settings.CategoriesCollectionName);
        }

        public async Task<List<Problem>> GetAllProblemsAsync()
        {
            return await _problems.Find(p => p.IsActive).ToListAsync();
        }

        public async Task<List<Problem>> GetProblemsByCategoryAsync(string category)
        {
            if (category.ToLower() == "all")
            {
                return await GetAllProblemsAsync();
            }

            return await _problems.Find(p => p.Category == category && p.IsActive).ToListAsync();
        }

        public async Task<Problem?> GetProblemByIdAsync(string id)
        {
            return await _problems.Find(p => p.Id == id && p.IsActive).FirstOrDefaultAsync();
        }

        public async Task<Problem> CreateProblemAsync(Problem problem)
        {
            // Auto-assign the next problem number
            problem.ProblemNumber = await GetNextProblemNumberAsync();
            problem.CreatedAt = DateTime.UtcNow;
            problem.UpdatedAt = DateTime.UtcNow;
            problem.IsActive = true;

            await _problems.InsertOneAsync(problem);
            return problem;
        }

        public async Task<Problem> UpdateProblemAsync(string id, Problem problem)
        {
            problem.UpdatedAt = DateTime.UtcNow;
            await _problems.ReplaceOneAsync(p => p.Id == id, problem);
            return problem;
        }

        public async Task<bool> DeleteProblemAsync(string id)
        {
            var result = await _problems.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _categories.Find(c => c.IsActive).ToListAsync();
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            category.IsActive = true;

            await _categories.InsertOneAsync(category);
            return category;
        }

        public async Task SeedDataAsync()
        {
            // Check if data already exists
            var existingProblems = await _problems.CountDocumentsAsync(FilterDefinition<Problem>.Empty);
            var existingCategories = await _categories.CountDocumentsAsync(FilterDefinition<Category>.Empty);

            if (existingProblems > 0 && existingCategories > 0)
            {
                return; // Data already seeded
            }

            // Seed Categories
            var categories = new List<Category>
            {
                new Category
                {
                    Key = "frontend",
                    Name = "Frontend Development",
                    Icon = "🎨",
                    Color = "#4CAF50",
                    ProblemCount = 5,
                    EasyCount = 3,
                    MediumCount = 2,
                    HardCount = 0
                },
                new Category
                {
                    Key = "api-backend",
                    Name = "API & Backend",
                    Icon = "⚙️",
                    Color = "#2196F3",
                    ProblemCount = 3,
                    EasyCount = 0,
                    MediumCount = 3,
                    HardCount = 0
                },
                new Category
                {
                    Key = "web-development",
                    Name = "Web Development",
                    Icon = "🌐",
                    Color = "#FF9800",
                    ProblemCount = 2,
                    EasyCount = 0,
                    MediumCount = 1,
                    HardCount = 1
                },
                new Category
                {
                    Key = "security",
                    Name = "Security",
                    Icon = "🔒",
                    Color = "#F44336",
                    ProblemCount = 3,
                    EasyCount = 0,
                    MediumCount = 1,
                    HardCount = 2
                },
                new Category
                {
                    Key = "database",
                    Name = "Database",
                    Icon = "🗄️",
                    Color = "#9C27B0",
                    ProblemCount = 2,
                    EasyCount = 0,
                    MediumCount = 2,
                    HardCount = 0
                },
                new Category
                {
                    Key = "mobile-development",
                    Name = "Mobile Development",
                    Icon = "📱",
                    Color = "#00BCD4",
                    ProblemCount = 4,
                    EasyCount = 1,
                    MediumCount = 2,
                    HardCount = 1
                },
                new Category
                {
                    Key = "devops-cloud",
                    Name = "DevOps & Cloud",
                    Icon = "☁️",
                    Color = "#FF5722",
                    ProblemCount = 3,
                    EasyCount = 0,
                    MediumCount = 2,
                    HardCount = 1
                },
                new Category
                {
                    Key = "ai-ml",
                    Name = "AI & Machine Learning",
                    Icon = "🤖",
                    Color = "#673AB7",
                    ProblemCount = 5,
                    EasyCount = 1,
                    MediumCount = 3,
                    HardCount = 1
                }
            };

            await _categories.InsertManyAsync(categories);

            // Seed Problems
            var problems = new List<Problem>
            {
                new Problem
                {
                    ProblemNumber = 1,
                    Title = "Build a Responsive Calculator",
                    Description = "Create a fully functional calculator with responsive design using HTML, CSS, and JavaScript.",
                    Difficulty = "Easy",
                    Category = "Frontend Development",
                    Tags = new List<string> { "HTML", "CSS", "JavaScript", "Responsive Design" },
                    AcceptanceRate = 85,
                    Likes = 156
                },
                new Problem
                {
                    ProblemNumber = 2,
                    Title = "Create a Todo List App",
                    Description = "Build a todo list application with add, edit, delete, and mark complete functionality.",
                    Difficulty = "Easy",
                    Category = "Frontend Development",
                    Tags = new List<string> { "React", "State Management", "CRUD" },
                    AcceptanceRate = 78,
                    Likes = 203
                },
                new Problem
                {
                    ProblemNumber = 3,
                    Title = "Build a REST API with Authentication",
                    Description = "Create a RESTful API with JWT authentication, user registration, and protected endpoints.",
                    Difficulty = "Medium",
                    Category = "API & Backend",
                    Tags = new List<string> { "API", "Authentication", "Node.js", "Express" },
                    AcceptanceRate = 68,
                    Likes = 245
                },
                new Problem
                {
                    ProblemNumber = 4,
                    Title = "Implement User Authentication System",
                    Description = "Build a complete authentication system with login, registration, password reset, and session management.",
                    Difficulty = "Medium",
                    Category = "API & Backend",
                    Tags = new List<string> { "Authentication", "Security", "JWT", "Bcrypt" },
                    AcceptanceRate = 72,
                    Likes = 189
                },
                new Problem
                {
                    ProblemNumber = 5,
                    Title = "Create a Full-Stack E-commerce Site",
                    Description = "Build a complete e-commerce website with product catalog, shopping cart, and payment integration.",
                    Difficulty = "Hard",
                    Category = "Web Development",
                    Tags = new List<string> { "Full-Stack", "E-commerce", "Payment", "Database" },
                    AcceptanceRate = 45,
                    Likes = 312
                },
                new Problem
                {
                    ProblemNumber = 6,
                    Title = "Build a Real-time Chat Application",
                    Description = "Create a real-time chat application using WebSockets with user rooms and message history.",
                    Difficulty = "Medium",
                    Category = "Web Development",
                    Tags = new List<string> { "WebSockets", "Real-time", "Socket.io", "Node.js" },
                    AcceptanceRate = 58,
                    Likes = 267
                },
                new Problem
                {
                    ProblemNumber = 7,
                    Title = "Implement Password Security Best Practices",
                    Description = "Create a secure password management system with encryption, salting, and validation.",
                    Difficulty = "Hard",
                    Category = "Security",
                    Tags = new List<string> { "Security", "Encryption", "Password Hashing", "Validation" },
                    AcceptanceRate = 42,
                    Likes = 178
                },
                new Problem
                {
                    ProblemNumber = 8,
                    Title = "Build a Secure API Gateway",
                    Description = "Implement an API gateway with rate limiting, authentication, and request validation.",
                    Difficulty = "Medium",
                    Category = "Security",
                    Tags = new List<string> { "API Gateway", "Rate Limiting", "Security", "Microservices" },
                    AcceptanceRate = 55,
                    Likes = 201
                },
                new Problem
                {
                    ProblemNumber = 9,
                    Title = "Design a Database Schema for Social Media",
                    Description = "Create an optimized database schema for a social media platform with users, posts, and relationships.",
                    Difficulty = "Hard",
                    Category = "Security",
                    Tags = new List<string> { "Database Design", "SQL", "Relationships", "Optimization" },
                    AcceptanceRate = 38,
                    Likes = 234
                },
                new Problem
                {
                    ProblemNumber = 10,
                    Title = "Optimize Database Queries",
                    Description = "Analyze and optimize slow database queries using indexing and query optimization techniques.",
                    Difficulty = "Medium",
                    Category = "Database",
                    Tags = new List<string> { "Database", "Optimization", "Indexing", "Performance" },
                    AcceptanceRate = 62,
                    Likes = 145
                },
                new Problem
                {
                    ProblemNumber = 11,
                    Title = "Implement Database Migrations",
                    Description = "Create a database migration system for version control and schema changes.",
                    Difficulty = "Medium",
                    Category = "Database",
                    Tags = new List<string> { "Database", "Migrations", "Version Control", "Schema" },
                    AcceptanceRate = 58,
                    Likes = 167
                },
                new Problem
                {
                    ProblemNumber = 12,
                    Title = "Create a Dynamic Form Builder",
                    Description = "Build a form builder that allows users to create custom forms with validation rules.",
                    Difficulty = "Easy",
                    Category = "Frontend Development",
                    Tags = new List<string> { "Forms", "Dynamic", "Validation", "UI Components" },
                    AcceptanceRate = 82,
                    Likes = 198
                },
                new Problem
                {
                    ProblemNumber = 13,
                    Title = "Build a Data Visualization Dashboard",
                    Description = "Create an interactive dashboard with charts and graphs using modern visualization libraries.",
                    Difficulty = "Medium",
                    Category = "Frontend Development",
                    Tags = new List<string> { "Data Visualization", "Charts", "Dashboard", "D3.js" },
                    AcceptanceRate = 65,
                    Likes = 223
                },
                new Problem
                {
                    ProblemNumber = 14,
                    Title = "Build a Cross-Platform Mobile App",
                    Description = "Create a mobile application using React Native or Flutter with offline capabilities and push notifications.",
                    Difficulty = "Medium",
                    Category = "Mobile Development",
                    Tags = new List<string> { "React Native", "Flutter", "Mobile", "Cross-Platform" },
                    AcceptanceRate = 58,
                    Likes = 189
                },
                new Problem
                {
                    ProblemNumber = 15,
                    Title = "Implement Mobile Authentication",
                    Description = "Build biometric authentication and secure token storage for mobile applications.",
                    Difficulty = "Hard",
                    Category = "Mobile Development",
                    Tags = new List<string> { "Biometrics", "Authentication", "Mobile Security", "Token Storage" },
                    AcceptanceRate = 42,
                    Likes = 156
                },
                new Problem
                {
                    ProblemNumber = 16,
                    Title = "Create a Mobile Game",
                    Description = "Develop a simple mobile game with touch controls, scoring system, and local leaderboards.",
                    Difficulty = "Easy",
                    Category = "Mobile Development",
                    Tags = new List<string> { "Game Development", "Touch Controls", "Mobile", "Unity" },
                    AcceptanceRate = 75,
                    Likes = 267
                },
                new Problem
                {
                    ProblemNumber = 17,
                    Title = "Build a Real-time Chat Mobile App",
                    Description = "Create a mobile chat application with real-time messaging, file sharing, and group chat features.",
                    Difficulty = "Medium",
                    Category = "Mobile Development",
                    Tags = new List<string> { "Real-time", "Chat", "Mobile", "WebSockets" },
                    AcceptanceRate = 62,
                    Likes = 234
                },
                new Problem
                {
                    ProblemNumber = 18,
                    Title = "Set up CI/CD Pipeline",
                    Description = "Create a complete CI/CD pipeline using GitHub Actions, Docker, and cloud deployment.",
                    Difficulty = "Medium",
                    Category = "DevOps & Cloud",
                    Tags = new List<string> { "CI/CD", "Docker", "GitHub Actions", "Deployment" },
                    AcceptanceRate = 55,
                    Likes = 178
                },
                new Problem
                {
                    ProblemNumber = 19,
                    Title = "Deploy Microservices to Cloud",
                    Description = "Design and deploy a microservices architecture on AWS or Azure with load balancing and monitoring.",
                    Difficulty = "Hard",
                    Category = "DevOps & Cloud",
                    Tags = new List<string> { "Microservices", "AWS", "Azure", "Load Balancing" },
                    AcceptanceRate = 38,
                    Likes = 201
                },
                new Problem
                {
                    ProblemNumber = 20,
                    Title = "Implement Infrastructure as Code",
                    Description = "Use Terraform or CloudFormation to provision and manage cloud infrastructure automatically.",
                    Difficulty = "Medium",
                    Category = "DevOps & Cloud",
                    Tags = new List<string> { "Terraform", "Infrastructure", "CloudFormation", "Automation" },
                    AcceptanceRate = 48,
                    Likes = 145
                },
                new Problem
                {
                    ProblemNumber = 21,
                    Title = "Build a Recommendation System",
                    Description = "Create a machine learning recommendation system using collaborative filtering and content-based approaches.",
                    Difficulty = "Hard",
                    Category = "AI & Machine Learning",
                    Tags = new List<string> { "Machine Learning", "Recommendation", "Python", "Scikit-learn" },
                    AcceptanceRate = 35,
                    Likes = 289
                },
                new Problem
                {
                    ProblemNumber = 22,
                    Title = "Implement Image Classification",
                    Description = "Build an image classification model using deep learning to identify objects in photos.",
                    Difficulty = "Medium",
                    Category = "AI & Machine Learning",
                    Tags = new List<string> { "Deep Learning", "Computer Vision", "TensorFlow", "CNN" },
                    AcceptanceRate = 52,
                    Likes = 234
                },
                new Problem
                {
                    ProblemNumber = 23,
                    Title = "Create a Chatbot with NLP",
                    Description = "Develop an intelligent chatbot using natural language processing and sentiment analysis.",
                    Difficulty = "Medium",
                    Category = "AI & Machine Learning",
                    Tags = new List<string> { "NLP", "Chatbot", "Sentiment Analysis", "Python" },
                    AcceptanceRate = 58,
                    Likes = 198
                },
                new Problem
                {
                    ProblemNumber = 24,
                    Title = "Build a Predictive Analytics Dashboard",
                    Description = "Create a dashboard that predicts user behavior and displays insights using machine learning models.",
                    Difficulty = "Medium",
                    Category = "AI & Machine Learning",
                    Tags = new List<string> { "Predictive Analytics", "Dashboard", "Data Science", "Visualization" },
                    AcceptanceRate = 45,
                    Likes = 167
                },
                new Problem
                {
                    ProblemNumber = 25,
                    Title = "Implement Time Series Forecasting",
                    Description = "Build a time series forecasting model to predict stock prices or sales trends using ARIMA or LSTM.",
                    Difficulty = "Easy",
                    Category = "AI & Machine Learning",
                    Tags = new List<string> { "Time Series", "Forecasting", "ARIMA", "LSTM" },
                    AcceptanceRate = 68,
                    Likes = 223
                }
            };

            await _problems.InsertManyAsync(problems);
        }

        private async Task<int> GetNextProblemNumberAsync()
        {
            // Find the highest problem number and increment by 1
            var highestProblem = await _problems
                .Find(FilterDefinition<Problem>.Empty)
                .Sort(Builders<Problem>.Sort.Descending(p => p.ProblemNumber))
                .FirstOrDefaultAsync();

            return highestProblem?.ProblemNumber + 1 ?? 1;
        }
    }
}
