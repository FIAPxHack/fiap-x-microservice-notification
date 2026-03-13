# FIAP X - Microsserviço de Notificação

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/aspnet/core/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

Microsserviço responsável pelo envio de notificações por e-mail aos usuários do sistema FIAP X, desenvolvido com **ASP.NET Core 9.0** e **C#**.

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Pré-requisitos](#-pré-requisitos)
- [Instalação e Execução](#-instalação-e-execução)
- [Endpoints da API](#-endpoints-da-api)
- [Tipos de Notificação](#-tipos-de-notificação)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Variáveis de Ambiente](#-variáveis-de-ambiente)
- [Integração com Outros Serviços](#-integração-com-outros-serviços)
- [Deployment](#-deployment)
- [Roadmap](#-roadmap)
- [Contribuição](#-contribuição)
- [Licença](#-licença)

---

## 🎯 Sobre o Projeto

O **fiap-x-microservice-notification** é um microsserviço RESTful que gerencia o envio de notificações por e-mail relacionadas ao processamento de vídeos e eventos do sistema.

### Funcionalidades Principais

- ✅ **Envio de E-mails** - Sistema pronto para integração SMTP real
- ✅ **Histórico de Notificações** - Rastreamento completo de notificações enviadas
- ✅ **Tipos Pré-definidos** - Notificações para início, conclusão e falha de processamento
- ✅ **Logging Estruturado** - Logs detalhados para auditoria
- ✅ **API REST Documentada** - Documentação interativa com Swagger
- ✅ **Health Check** - Endpoint de monitoramento de saúde
- ✅ **Injeção de Dependência** - Arquitetura desacoplada e testável

### Casos de Uso

- Notificar usuário quando processamento de vídeo começa
- Notificar usuário quando processamento é concluído
- Alertar sobre falhas no processamento
- Enviar notificações gerais do sistema

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture** e **SOLID**, com separação clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│                  (Controllers, API)                          │
├─────────────────────────────────────────────────────────────┤
│                    APPLICATION LAYER                         │
│              (Use Cases, DTOs, Interfaces)                   │
├─────────────────────────────────────────────────────────────┤
│                      DOMAIN LAYER                            │
│        (Entities, Domain Interfaces, Exceptions)             │
├─────────────────────────────────────────────────────────────┤
│                   INFRASTRUCTURE LAYER                       │
│          (Repositories, Email Service, External)             │
└─────────────────────────────────────────────────────────────┘
```

### Estrutura de Pastas

```
src/
├── Domain/                          # Camada de Domínio (sem dependências externas)
│   ├── Entities/                    # Entidades de domínio
│   │   └── NotificationHistory.cs
│   ├── Enums/                       # Enumerações
│   │   ├── NotificationType.cs
│   │   └── NotificationStatus.cs
│   ├── Interfaces/                  # Contratos do domínio
│   │   ├── Repositories/
│   │   │   └── INotificationRepository.cs
│   │   └── Services/
│   │       └── IEmailService.cs
│   └── Exceptions/                  # Exceções de domínio
│       └── NotificationException.cs
│
├── Application/                     # Camada de Aplicação
│   ├── DTOs/                        # Data Transfer Objects
│   │   ├── NotificationRequestDto.cs
│   │   ├── NotificationResponseDto.cs
│   │   └── NotificationHistoryDto.cs
│   ├── Interfaces/                  # Interfaces de casos de uso
│   │   ├── ISendNotificationUseCase.cs
│   │   └── IGetUserNotificationsUseCase.cs
│   └── UseCases/                    # Casos de uso (lógica de aplicação)
│       ├── SendNotificationUseCase.cs
│       └── GetUserNotificationsUseCase.cs
│
├── Infrastructure/                  # Camada de Infraestrutura
│   ├── Persistence/                 # Persistência de dados
│   │   └── InMemoryNotificationRepository.cs
│   └── Email/                       # Serviços de email
│       └── SmtpEmailService.cs
│
└── Presentation/                    # Camada de Apresentação
    └── Controllers/                 # Controllers REST
        └── NotificationsController.cs
```

### Camadas

**1. Domain (Domínio)**
- Núcleo da aplicação, sem dependências de frameworks
- Define entidades, interfaces e regras de negócio
- Independente de infraestrutura ou tecnologia

**2. Application (Aplicação)**
- Orquestra casos de uso
- Define DTOs para comunicação
- Contém a lógica de aplicação (não de negócio)

**3. Infrastructure (Infraestrutura)**
- Implementa interfaces definidas no domínio
- Integra com tecnologias externas (SMTP, banco de dados)
- Repositórios concretos

**4. Presentation (Apresentação)**
- Camada de API REST
- Controllers que expõem endpoints
- Validação de entrada

### Fluxo de Dados

```
Request (HTTP) 
    ↓
Controller (Presentation) 
    ↓
Use Case (Application)
    ↓
Domain Service + Repository (Domain Interfaces)
    ↓
Infrastructure Implementation
    ↓
External Services (SMTP, Database)
```

### Princípios Aplicados

- **Dependency Inversion**: Interfaces no domínio, implementações na infraestrutura
- **Single Responsibility**: Cada classe tem uma única responsabilidade
- **Open/Closed**: Extensível sem modificar código existente
- **Interface Segregation**: Interfaces específicas e coesas
- **Liskov Substitution**: Implementações substituíveis transparentemente
- **Domain-Driven Design**: Domínio livre de dependências de framework

---

## 🚀 Tecnologias

### Core
- **[.NET 9.0](https://dotnet.microsoft.com/)** - Runtime e Framework
- **[ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)** - Framework Web API
- **[C# 12.0](https://docs.microsoft.com/en-us/dotnet/csharp/)** - Linguagem

### Comunicação
- **REST API** - Endpoints HTTP
- **System.Net.Mail** - Envio de e-mails (pronto para SMTP)

### Documentação
- **[Swagger/OpenAPI](https://swagger.io/)** - Documentação interativa da API

### DevOps
- **[Docker](https://www.docker.com/)** (planejado) - Containerização
- **Environment Variables** - Configuração 12-factor

---

## 📦 Pré-requisitos

Antes de começar, você vai precisar ter instalado:

- **[.NET SDK 9.0+](https://dotnet.microsoft.com/download)** - Para compilar e executar
- **[Git](https://git-scm.com/)** - Para clonar o repositório
- **(Opcional) Servidor SMTP** - Para envio real de e-mails

---

## 🔧 Instalação e Execução

### 1️⃣ Clonar o Repositório

```bash
git clone https://github.com/FIAPxHack/fiap-x-microservice-notification.git
cd fiap-x-microservice-notification
```

### 2️⃣ Configurar Variáveis de Ambiente

Crie um arquivo `.env` ou configure no sistema (para produção):

```bash
# SMTP Configuration (para envio real)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=seu-email@gmail.com
SMTP_PASSWORD=sua-senha-app
SMTP_FROM_EMAIL=noreply@fiapx.com
SMTP_FROM_NAME=FIAP X System
SMTP_ENABLE_SSL=true
```

### 3️⃣ Executar com .NET CLI

```bash
# Restaurar dependências
dotnet restore

# Executar em modo desenvolvimento
cd src
dotnet run

# Ou executar a solution
dotnet run --project src/NotificationService.csproj
```

O serviço estará disponível em: **http://localhost:5004**

### 4️⃣ Acessar Documentação Swagger

Abra no navegador:
```
http://localhost:5004/swagger
```

### 5️⃣ Verificar Saúde do Serviço

```bash
# Health check
curl http://localhost:5004/healthz

# Health check de notificações
curl http://localhost:5004/api/notifications/health

# Resposta esperada: HTTP 200 OK
```

---

## 📡 Endpoints da API

### Base URL
```
http://localhost:5004/api/notifications
```

### Documentação Interativa (Swagger UI)
Acesse: **http://localhost:5004/swagger**

---

### 📬 Enviar Notificação

**Endpoint:** `POST /api/notifications/send`

**Descrição:** Envia uma notificação por e-mail para um usuário.

**Request Body:**
```json
{
  "userId": "user-123",
  "email": "usuario@example.com",
  "subject": "Processamento de vídeo concluído",
  "message": "Seu vídeo foi processado com sucesso! Baixe o arquivo ZIP no painel.",
  "type": 1
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Notificação enviada com sucesso",
  "notificationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "timestamp": "2026-03-10T14:30:00Z"
}
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Erro de validação: Email é obrigatório",
  "notificationId": null,
  "timestamp": "2026-03-10T14:30:00Z"
}
```

**Exemplo cURL:**
```bash
curl -X POST http://localhost:5004/api/notifications/send \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-123",
    "email": "usuario@example.com",
    "subject": "Processamento concluído",
    "message": "Seu vídeo está pronto!",
    "type": 1
  }'
```

---

### 📜 Buscar Histórico de Usuário

**Endpoint:** `GET /api/notifications/user/{userId}`

**Descrição:** Retorna todas as notificações enviadas para um usuário específico.

**Parâmetros:**
- `userId` (path) - ID do usuário

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "user-123",
    "email": "usuario@example.com",
    "subject": "Processamento iniciado",
    "message": "Seu vídeo está sendo processado...",
    "type": 0,
    "status": 1,
    "createdAt": "2026-03-10T14:00:00Z",
    "sentAt": "2026-03-10T14:00:01Z"
  },
  {
    "id": "7ba95f64-5717-4562-b3fc-2c963f66bcde",
    "userId": "user-123",
    "email": "usuario@example.com",
    "subject": "Processamento concluído",
    "message": "Seu vídeo está pronto!",
    "type": 1,
    "status": 1,
    "createdAt": "2026-03-10T14:15:00Z",
    "sentAt": "2026-03-10T14:15:02Z"
  }
]
```

**Exemplo cURL:**
```bash
curl http://localhost:5004/api/notifications/user/user-123
```

---

### ❤️ Health Check

**Endpoint:** `GET /api/notifications/health`

**Descrição:** Verifica se o serviço de notificações está saudável.

**Response (200 OK):**
```json
{
  "status": "Healthy",
  "service": "Notification Service",
  "timestamp": "2026-03-10T14:30:00Z"
}
```

**Exemplo cURL:**
```bash
curl http://localhost:5004/api/notifications/health
```

---

## 📨 Tipos de Notificação

O sistema suporta 4 tipos pré-definidos de notificações:

| Código | Tipo                          | Descrição                                  | Uso                                |
|--------|-------------------------------|--------------------------------------------|------------------------------------|
| `0`    | **VideoProcessingStarted**    | Processamento de vídeo iniciado            | Quando upload é recebido           |
| `1`    | **VideoProcessingCompleted**  | Processamento de vídeo concluído           | Quando ZIP está pronto para download |
| `2`    | **VideoProcessingFailed**     | Falha no processamento de vídeo            | Quando ocorre erro no FFmpeg       |
| `3`    | **General**                   | Notificação geral do sistema               | Avisos, manutenções, etc.          |

### Exemplo de Uso por Tipo

```csharp
// Tipo 0 - Processamento iniciado
{
  "type": 0,
  "subject": "Processamento iniciado",
  "message": "Seu vídeo está sendo processado. Você receberá uma notificação quando estiver pronto."
}

// Tipo 1 - Processamento concluído
{
  "type": 1,
  "subject": "Vídeo pronto para download",
  "message": "Seu vídeo foi processado com sucesso! Acesse o painel para baixar o arquivo ZIP."
}

// Tipo 2 - Falha no processamento
{
  "type": 2,
  "subject": "Erro no processamento",
  "message": "Ocorreu um erro ao processar seu vídeo. Por favor, tente novamente ou contate o suporte."
}

// Tipo 3 - Notificação geral
{
  "type": 3,
  "subject": "Manutenção programada",
  "message": "O sistema estará em manutenção no dia 15/03 das 2h às 4h."
}
```

---

## 🗂️ Estrutura do Projeto

```
fiap-x-microservice-notification/
├── src/
│   ├── Controllers/
│   │   └── NotificationsController.cs    # REST API endpoints
│   ├── Services/
│   │   ├── INotificationService.cs       # Interface do serviço
│   │   └── EmailNotificationService.cs   # Implementação
│   ├── Models/
│   │   ├── NotificationRequest.cs        # DTO de entrada
│   │   ├── NotificationResponse.cs       # DTO de saída
│   │   └── NotificationHistory.cs        # Entidade de histórico
│   ├── Properties/
│   │   └── launchSettings.json           # Configuração de execução
│   ├── appsettings.json                  # Config principal
│   ├── appsettings.Development.json      # Config dev
│   ├── Program.cs                        # Entry point
│   └── NotificationService.csproj        # Projeto
├── .gitignore
├── NotificationService.sln               # Solution
└── README.md                            # Este arquivo
```

---

## ⚙️ Variáveis de Ambiente

### Configuração SMTP (Produção)

Para habilitar o envio real de e-mails, configure:

```bash
# Servidor SMTP
SMTP_HOST=smtp.gmail.com              # Servidor SMTP
SMTP_PORT=587                         # Porta (587 para TLS, 465 para SSL)
SMTP_USERNAME=seu-email@gmail.com     # Usuário SMTP
SMTP_PASSWORD=sua-senha-app           # Senha ou App Password
SMTP_ENABLE_SSL=true                  # Habilitar SSL/TLS

# Remetente
SMTP_FROM_EMAIL=noreply@fiapx.com     # Email de origem
SMTP_FROM_NAME=FIAP X System          # Nome do remetente
```

### Exemplo appsettings.json (Produção)

```json
{
  "Email": {
    "SmtpHost": "${SMTP_HOST}",
    "SmtpPort": "${SMTP_PORT}",
    "Username": "${SMTP_USERNAME}",
    "Password": "${SMTP_PASSWORD}",
    "FromEmail": "${SMTP_FROM_EMAIL}",
    "FromName": "${SMTP_FROM_NAME}",
    "EnableSsl": "${SMTP_ENABLE_SSL}"
  }
}
```

---

## 🔗 Integração com Outros Serviços

### 1. Video Processing Service

O serviço de processamento de vídeo deve chamar o Notification Service nos seguintes momentos:

#### Quando o processamento inicia:
```bash
POST http://notification-service:5004/api/notifications/send
Content-Type: application/json

{
  "userId": "user-123",
  "email": "usuario@example.com",
  "subject": "Processamento iniciado",
  "message": "Seu vídeo foi recebido e está sendo processado.",
  "type": 0
}
```

#### Quando o processamento é concluído:
```bash
POST http://notification-service:5004/api/notifications/send
Content-Type: application/json

{
  "userId": "user-123",
  "email": "usuario@example.com",
  "subject": "Vídeo pronto!",
  "message": "Seu vídeo foi processado com sucesso. Faça o download no painel.",
  "type": 1
}
```

#### Quando ocorre uma falha:
```bash
POST http://notification-service:5004/api/notifications/send
Content-Type: application/json

{
  "userId": "user-123",
  "email": "usuario@example.com",
  "subject": "Erro no processamento",
  "message": "Ocorreu um erro ao processar seu vídeo. Tente novamente.",
  "type": 2
}
```

### 2. API Gateway

O Gateway deve rotear requisições para este serviço:

```csharp
// YARP Configuration
.ConfigureHttpClient(client => {
    client.BaseAddress = new Uri("http://notification-service:5004");
});
```

---

## 🐳 Deployment

### Docker (Planejado)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/NotificationService.csproj", "src/"]
RUN dotnet restore "src/NotificationService.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
```

### Docker Compose

```yaml
version: '3.8'

services:
  notification-service:
    build: ./fiap-x-microservice-notification
    ports:
      - "5004:5004"
    environment:
      SMTP_HOST: smtp.gmail.com
      SMTP_PORT: 587
      SMTP_USERNAME: ${SMTP_USERNAME}
      SMTP_PASSWORD: ${SMTP_PASSWORD}
      SMTP_FROM_EMAIL: noreply@fiapx.com
      SMTP_FROM_NAME: "FIAP X System"
      SMTP_ENABLE_SSL: "true"
    networks:
      - fiapx-network

networks:
  fiapx-network:
    driver: bridge
```

---

## 🗺️ Roadmap

### ✅ Implementado
- [x] API REST com 3 endpoints
- [x] Envio de e-mails (simulado)
- [x] Histórico em memória
- [x] Documentação Swagger
- [x] Health checks
- [x] Tipos de notificação pré-definidos

### 🚧 Em Progresso
- [ ] Integração SMTP real
- [ ] Persistência em banco de dados (PostgreSQL)
- [ ] Dockerfile e Docker Compose

### 📋 Planejado
- [ ] Suporte a templates de e-mail (HTML)
- [ ] Retry mechanism para falhas de envio
- [ ] Fila de notificações assíncrona (RabbitMQ/SQS)
- [ ] Notificações por SMS (Twilio)
- [ ] Notificações push (Firebase)
- [ ] Testes unitários e de integração
- [ ] Métricas e observabilidade (Prometheus)

---

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'feat: adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👥 Equipe

- **Elen** - Responsável pelo Notification Service e API Gateway
- **Marcelle** - Video Processing Service, Queue Service
- **Vitória** - User Service, Auth Service

---

## 📞 Suporte

Para questões e suporte, abra uma [issue](https://github.com/FIAPxHack/fiap-x-microservice-notification/issues) no repositório.

---

## 🙏 Agradecimentos

- Microsoft pela excelente documentação do .NET
- Comunidade ASP.NET Core
- FIAP pelo desafio e oportunidade de aprendizado
    "createdAt": "2026-03-10T...",
    "sentAt": "2026-03-10T..."
  }
]
```

### `GET /healthz`
Health check do serviço.

---

## ⚙️ Configuração

### Variáveis de Ambiente (Futuro - SMTP Real)
```bash
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SENDER_EMAIL=noreply@fiapx.com
SENDER_NAME=FIAP X Notifications
SMTP_USERNAME=your-email
SMTP_PASSWORD=your-password
```

---

## 🔧 Integração com Video Processing Service

O serviço de processamento de vídeo deve chamar este endpoint ao:

1. **Iniciar processamento:**
```http
POST /api/notifications/send
{
  "userId": "user-id",
  "email": "user@email.com",
  "type": 0,
  "message": "Seu vídeo está sendo processado..."
}
```

2. **Concluir processamento:**
```http
POST /api/notifications/send
{
  "userId": "user-id",
  "email": "user@email.com",
  "type": 1,
  "message": "Seu vídeo foi processado com sucesso! Download: [link]"
}
```

3. **Falha no processamento:**
```http
POST /api/notifications/send
{
  "userId": "user-id",
  "email": "user@email.com",
  "type": 2,
  "message": "Ocorreu um erro ao processar seu vídeo. Tente novamente."
}
```

---

## 📝 Notas de Implementação

### Implementação Atual (Simulada)
- E-mails são **simulados** via log
- Histórico armazenado **em memória** (perde ao reiniciar)
- Pronto para extensão com SMTP real

### Próximos Passos (Produção)
1. Implementar envio SMTP real
2. Persistir histórico em banco de dados
3. Adicionar fila de mensagens (RabbitMQ/SQS)
4. Implementar retry logic
5. Adicionar templates de e-mail

---

## 📦 Estrutura do Projeto

```
fiap-x-notification-service/
├── src/
│   ├── Controllers/
│   │   └── NotificationsController.cs
│   ├── Models/
│   │   ├── NotificationRequest.cs
│   │   ├── NotificationResponse.cs
│   │   └── NotificationHistory.cs
│   ├── Services/
│   │   ├── INotificationService.cs
│   │   └── EmailNotificationService.cs
│   ├── appsettings.json
│   ├── Program.cs
│   └── NotificationService.csproj
└── README.md
```

---

## 👥 Responsável

**Elen** - Microserviço de Notificações
