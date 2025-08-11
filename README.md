# 📧 Email Verification System

A full-stack email verification system built with **ASP.NET Core**, **RabbitMQ**, and **Razor Pages**. Features secure code generation, message queuing, and real-time verification with anti-spam protection.



## 🚀 Features

### Web Application (ASP.NET Core + Razor Pages)
- ✅ **Secure Code Generation** - 4-digit verification codes with expiration
- ✅ **Message Queue Integration** - RabbitMQ for reliable message processing  
- ✅ **Anti-Spam Protection** - Rate limiting and cooldown periods
- ✅ **Input Validation** - Email format and code validation
- ✅ **Responsive UI** - Clean Razor Pages interface
- ✅ **RESTful API** - Clean endpoint design with Swagger documentation

### Email Consumer (Console Application)
- 📨 **Asynchronous Processing** - Listens to RabbitMQ queue
- 📨 **Real-time Display** - Shows verification codes as they're generated
- 📨 **Queue Reliability** - Message acknowledgment and error handling
- 📨 **Formatted Output** - `2024.08.10 15:30 user@email.com код: 1234`

### Security Features  
- 🔒 **Time-based Expiration** - Codes expire after 10 minutes
- 🔒 **Attempt Limiting** - Maximum 3 verification attempts
- 🔒 **Rate Limiting** - 1-minute cooldown between code requests
- 🔒 **Input Sanitization** - Prevents injection attacks

## 🏗️ Architecture

```
┌─────────────┐    HTTP     ┌─────────────┐    Queue    ┌─────────────┐
│ Razor Pages │ ──────────► │  ASP.NET    │ ──────────► │  RabbitMQ   │
│   Web App   │             │    API      │             │   Queue     │
└─────────────┘             └─────────────┘             └─────────────┘
                                   │                            │
                                   ▼                            ▼
                            ┌─────────────┐             ┌─────────────┐
                            │ Verification │             │   Email     │
                            │   Service   │             │  Consumer   │
                            └─────────────┘             └─────────────┘
```

## 🛠️ Tech Stack

### Web Application
- **Framework**: ASP.NET Core 8.0
- **Frontend**: Razor Pages + Bootstrap
- **Language**: C# 11
- **Message Broker**: RabbitMQ 3.12
- **Documentation**: Swagger/OpenAPI

### Email Consumer
- **Type**: .NET Console Application
- **Language**: C# 11
- **Message Processing**: RabbitMQ Client

## 📦 Installation & Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [RabbitMQ Server](https://www.rabbitmq.com/download.html)

### 1. Clone Repository
```bash
git clone https://github.com/yourusername/email-verification-system.git
cd email-verification-system
```

### 2. Web Application Setup
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

### Start Email Consumer (Terminal 1)
```bash
cd EmailSender
dotnet run
```

Console output will show:
```
Email Consumer Service Starting...
Listening for email tasks from RabbitMQ
Connected to RabbitMQ. Waiting for messages...
```

### Start Web Application (Terminal 2)
```bash
cd EmailVerificationApi
dotnet run
```

### Access Applications
- **Web Application**: `https://localhost:7xxx` (Razor Pages UI)
- **API Documentation**: `https://localhost:7xxx/swagger`
- **RabbitMQ Management**: `http://localhost:15672` (guest/guest)

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

### Check Status
```http
GET /api/registration/status/user@example.com
```

## 🧪 Testing

### Web Interface Testing
1. Start both applications (Consumer + Web App)
2. Navigate to `https://localhost:7xxx`
3. Enter email address and click "Send Code"
4. Check EmailConsumer console for generated code
5. Enter code on verification page

### API Testing
1. Use Swagger UI at `https://localhost:7xxx/swagger`
2. Test endpoints with sample data
3. Monitor console output in EmailConsumer

### Example Flow
```bash
# 1. Request code via web interface or API
# Web: https://localhost:7xxx
# API: POST /api/registration/send-code

# 2. Check EmailConsumer console output:
# "2024.08.10 15:30 test@example.com код: 1234"

# 3. Verify code via web interface or API  
# Web: Enter code in verification form
# API: POST /api/registration/verify-code
```

## 🏗️ Project Structure

```
email-verification-system/
├── EmailSender/              # Web Application Project
│   ├── Controllers/
│   │   └── RegistrationController.cs  # API endpoints
│   ├── Services/
│   │   ├── RabbitMqProducer.cs       # Message queue integration
│   │   └── EmailVerificationService.cs # Core verification logic
│   ├── Models/
│   │   └── ApiModels.cs              # DTOs and data models
│   ├── Pages/                        # Razor Pages (if implemented)
│   ├── Program.cs                    # Web app startup
│   └── appsettings.json              # Configuration
└── EmailConsumer/                    # Console Application Project
    ├── Program.cs                    # Email consumer logic
    └── EmailConsumer.csproj          # Project file
```

## 🔒 Security Considerations

- **Code Expiration**: 10-minute automatic expiration
- **Rate Limiting**: 1-minute cooldown between requests
- **Attempt Limiting**: Maximum 3 verification attempts
- **Input Validation**: Email format and code format validation
- **Memory Storage**: Codes stored in-memory (auto-cleanup on restart)
- **HTTPS**: Secure communication between client and server

## 🚀 Future Enhancements

- [ ] **Enhanced Razor Pages UI** - Complete frontend implementation
- [ ] **Internationalization** - English/Russian language support  
- [ ] **CAPTCHA Integration** - Additional spam protection
- [ ] **Database Storage** - Persistent verification data
- [ ] **Email Templates** - HTML email formatting
- [ ] **SMS Support** - Alternative verification method
- [ ] **Admin Dashboard** - Verification statistics
- [ ] **Docker Support** - Containerized deployment
- [ ] **Unit Tests** - Comprehensive test coverage

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request




## 🙏 Acknowledgments

- Built as part of technical assessment demonstration
- RabbitMQ for reliable message queuing
- ASP.NET Core team for excellent framework
- Community for inspiration and best practices

---

⭐ **Star this repository if it helped you!**
