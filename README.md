# 📧 Email Verification System

A full-stack email verification system built with **ASP.NET Core API**, **RabbitMQ**, and **modern HTML/JavaScript**. Features secure code generation, message queuing, and real-time verification with anti-spam protection.

## 🚀 Features

### Web Application (ASP.NET Core API + HTML/JavaScript Frontend)
- ✅ **Modern Web Interface** - Responsive HTML/JavaScript with smooth animations
- ✅ **Secure Code Generation** - 4-digit verification codes with expiration
- ✅ **Message Queue Integration** - RabbitMQ for reliable message processing  
- ✅ **Anti-Spam Protection** - Rate limiting and cooldown periods
- ✅ **Real-time Validation** - Client-side and server-side input validation
- ✅ **RESTful API** - Clean endpoint design with Swagger documentation
- ✅ **CORS Support** - Cross-origin requests for development

### Email Consumer (Console Application)
- 📨 **Asynchronous Processing** - Listens to RabbitMQ queue
- 📨 **Real-time Display** - Shows verification codes as they're generated
- 📨 **Queue Reliability** - Message acknowledgment and error handling
- 📨 **Formatted Output** - `2024.08.10 15:30 user@email.com код: 1234`

### Security Features  
- 🔒 **Time-based Expiration** - Codes expire after configurable time
- 🔒 **Attempt Limiting** - Maximum verification attempts per code
- 🔒 **Rate Limiting** - 1-minute cooldown between code requests
- 🔒 **Input Sanitization** - Prevents injection attacks
- 🔒 **HTTPS Support** - Secure communication

## 🏗️ Architecture

```
┌─────────────┐    AJAX     ┌─────────────┐    Queue    ┌─────────────┐
│ HTML/JS     │ ──────────► │  ASP.NET    │ ──────────► │  RabbitMQ   │
│  Frontend   │   (CORS)    │Core API     │             │   Queue     │
└─────────────┘             └─────────────┘             └─────────────┘
                                   │                            │
                                   ▼                            ▼
                            ┌─────────────┐             ┌─────────────┐
                            │ Verification │             │   Email     │
                            │   Service   │             │  Consumer   │
                            └─────────────┘             └─────────────┘
```

## 🛠️ Tech Stack

### Backend API
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 11
- **Message Broker**: RabbitMQ 3.12
- **Documentation**: Swagger/OpenAPI
- **CORS**: Enabled for cross-origin requests

### Frontend
- **Type**: Static HTML/CSS/JavaScript
- **Style**: Modern CSS with gradients and animations
- **AJAX**: Fetch API for REST calls
- **Responsive**: Mobile-friendly design
- **Validation**: Real-time input validation

### Email Consumer
- **Type**: .NET Console Application
- **Language**: C# 11
- **Message Processing**: RabbitMQ Client

## 📦 Installation & Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [RabbitMQ Server](https://www.rabbitmq.com/download.html)
- Modern web browser (Chrome, Firefox, Safari, Edge)

### 1. Clone Repository
```bash
git clone https://github.com/yourusername/email-verification-system.git
cd email-verification-system
```

### 2. Web API Setup
```bash
cd EmailVerificationApi
dotnet restore
dotnet build
```

### 3. Email Consumer Setup  
```bash
cd EmailConsumer
dotnet restore
dotnet build
```

### 4. RabbitMQ Setup
```bash
# Install RabbitMQ and start service
# Default credentials: guest/guest
# Management UI: http://localhost:15672
```

### 5. Configuration
Update `EmailVerificationApi/appsettings.json`:
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

## 🚀 Running the Applications

### Method 1: Serve HTML from API Server (Recommended)

#### Step 1: Setup Static Files
1. Create `wwwroot` folder in your API project
2. Save the HTML file as `wwwroot/index.html`
3. Update `Program.cs` to serve static files:
```csharp
app.UseDefaultFiles();
app.UseStaticFiles();
```

#### Step 2: Start Applications
```bash
# Terminal 1: Start Email Consumer
cd EmailConsumer
dotnet run

# Terminal 2: Start API Server
cd EmailVerificationApi
dotnet run
```

#### Step 3: Access Application
- **Web Interface**: `https://localhost:7259/` or `http://localhost:5172/`
- **API Documentation**: `https://localhost:7259/swagger`

### Method 2: Separate HTML Server (Development)

#### Step 1: Start API Server
```bash
cd EmailVerificationApi
dotnet run
```

#### Step 2: Serve HTML (using VS Code Live Server or similar)
1. Open `index.html` in VS Code
2. Right-click → "Open with Live Server"
3. Updates the API URL in JavaScript:
```javascript
const API_BASE_URL = 'https://localhost:7259'; // Your API port
```

#### Step 3: Access Application
- **Web Interface**: `http://127.0.0.1:5500/index.html`
- **API Server**: `https://localhost:7259/swagger`

## 📡 API Endpoints

### Send Verification Code
```http
POST /api/registration/send-code
Content-Type: application/json

{
  "email": "user@example.com"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Verification code sent successfully."
}
```

### Verify Code
```http
POST /api/registration/verify-code
Content-Type: application/json

{
  "email": "user@example.com",
  "code": "1234"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Email verified successfully!"
}
```

## 🧪 Testing

### Web Interface Testing
1. **Start both applications** (Consumer + API)
2. **Access web interface**:
   - Method 1: `https://localhost:7259/`
   - Method 2: `http://127.0.0.1:5500/index.html`
3. **Enter email** and click "Continue"
4. **Check EmailConsumer console** for generated code
5. **Enter code** on verification page
6. **See success message**

### API Testing
1. **Use Swagger UI** at `https://localhost:7259/swagger`
2. **Test endpoints** with sample data
3. **Monitor console output** in EmailConsumer

### Example Testing Flow
```bash
# 1. Start EmailConsumer
dotnet run  # Shows: "Listening for email tasks from RabbitMQ"

# 2. Start API Server  
dotnet run  # Shows: "Now listening on: https://localhost:7259"

# 3. Open web interface
# Browser: https://localhost:7259/

# 4. Request verification code
# Input: test@example.com → Click "Continue"

# 5. Check console output
# EmailConsumer shows: "2024.08.10 15:30 test@example.com код: 1234"

# 6. Verify code
# Input: 1234 → Click "Verify" → Success!
```

## 🏗️ Project Structure

```
email-verification-system/
├── EmailVerificationApi/          # Web API Project
│   ├── Controllers/
│   │   └── RegistrationController.cs  # API endpoints
│   ├── Services/
│   │   ├── RabbitMqProducer.cs       # Message queue integration
│   │   └── EmailVerificationService.cs # Core verification logic
│   ├── Models/
│   │   └── ApiModels.cs              # DTOs and data models
│   ├── wwwroot/                      # Static files (optional)
│   │   └── index.html                # Web interface
│   ├── Program.cs                    # API startup configuration
│   └── appsettings.json              # Configuration
├── EmailConsumer/                    # Console Application
│   ├── Program.cs                    # Email consumer logic
│   └── EmailConsumer.csproj          # Project file
└── index.html                        # Standalone HTML file (alternative)
```

## 🎨 Frontend Features

### Modern UI Design
- **Gradient backgrounds** with smooth animations
- **Responsive layout** that works on mobile and desktop
- **Loading states** with spinning indicators
- **Error handling** with user-friendly messages
- **Step-by-step wizard** interface

### User Experience
- **Auto-formatting** for 4-digit verification codes
- **Keyboard support** (Enter key to submit forms)
- **Input validation** in real-time
- **Back button** to return to previous step
- **Restart functionality** to verify another email

### Technical Features
- **Fetch API** for modern HTTP requests
- **Async/await** for clean asynchronous code
- **CORS handling** for cross-origin requests
- **Error handling** for network and API errors
- **Console logging** for debugging

## 🔒 Security Considerations

- **Code Expiration**: Configurable automatic expiration
- **Rate Limiting**: Cooldown periods between requests
- **Attempt Limiting**: Maximum verification attempts per code
- **Input Validation**: Both client-side and server-side validation
- **CORS Configuration**: Specific origin allowlisting
- **HTTPS Support**: Secure communication
- **Memory Storage**: Codes stored in-memory (auto-cleanup on restart)

## 🐛 Troubleshooting

### Common Issues

#### CORS Errors
```bash
# Error: "Access to fetch... has been blocked by CORS policy"
# Solution: Ensure CORS is properly configured in Program.cs
```

#### Network Errors  
```bash
# Error: "Network error. Please check your connection"
# Solution: Check API_BASE_URL in JavaScript matches your API server
```

#### API Not Found
```bash
# Error: 404 Not Found on API calls
# Solution: Verify API server is running and endpoints exist
```

## 🚀 Future Enhancements

- [ ] **Database Storage** - Persistent verification data with Entity Framework
- [ ] **Email Templates** - HTML email formatting and actual SMTP sending
- [ ] **Internationalization** - Multi-language support (English/Russian)
- [ ] **Authentication** - User accounts and session management
- [ ] **Admin Dashboard** - Verification statistics and management
- [ ] **SMS Support** - Alternative verification method
- [ ] **Docker Support** - Containerized deployment
- [ ] **Unit Tests** - Comprehensive test coverage
- [ ] **CI/CD Pipeline** - Automated testing and deployment
- [ ] **Production Deployment** - Cloud hosting configurations

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request


