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
- [Instalação e Execução](#-instalação-e-execução)
- [Endpoints da API](#-endpoints-da-api)
- [Deployment](#-deployment)
- [Licença](#-licença)

---

## 🎯 Sobre o Projeto

O **fiap-x-microservice-notification** é um microsserviço RESTful que gerencia o envio de notificações por e-mail relacionadas ao processamento de vídeos e eventos do sistema.

### Funcionalidades

- **Envio de E-mails via SMTP** - Sistema configurável (logs em desenvolvimento, SMTP em produção)
- **Histórico de Notificações** - Rastreamento completo em memória
- **4 Tipos de Notificação** - Início, conclusão e falha de processamento, e notificações gerais
- **API REST** - Documentação interativa com Swagger/OpenAPI
- **Clean Architecture** - Separação em camadas Domain/Application/Infrastructure/Presentation
- **Testes Unitários** - Cobertura completa do código

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

### Camadas da Arquitetura

```
src/
├── Domain/              # Entidades, interfaces e regras de negócio
├── Application/         # Casos de uso e DTOs
├── Infrastructure/      # Implementações (repositórios, email)
└── Presentation/        # Controllers REST
```

### Princípios Aplicados

- **Clean Architecture** - Separação clara de responsabilidades em camadas
- **SOLID** - Dependency Inversion, Single Responsibility, Open/Closed
- **Domain-Driven Design** - Domínio rico e independente de frameworks

---

## 🚀 Tecnologias

- **.NET 9.0** e **ASP.NET Core** - Framework Web API
- **C# 12.0** - Linguagem de programação
- **System.Net.Mail** - Cliente SMTP
- **Swagger/OpenAPI** - Documentação interativa
- **xUnit, Moq, FluentAssertions** - Testes unitários
- **Docker** - Containerização
- **InMemory Repository** - Armazenamento de histórico

---

##  Instalação e Execução

### Executar Localmente

```bash
dotnet restore
cd src
dotnet run
```

Acesse: **http://localhost:8080/swagger**

### Executar com Docker

```bash
docker build -t fiapx-notification-service .
docker run -d -p 5001:8080 --name notification-service fiapx-notification-service

# Ou via docker-compose
docker-compose up notification-service
```

Acesse: **http://localhost:5001/swagger**

---

## 📡 Endpoints da API

### Documentação Completa
Acesse **http://localhost:8080/swagger** para documentação interativa completa.

### Resumo dos Endpoints

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

---

## 📨 Tipos de Notificação

O sistema suporta 4 tipos pré-definidos de notificações:

| Código | Tipo                          | Descrição                                  | Uso                                |
|--------|-------------------------------|--------------------------------------------|------------------------------------|
| `0`    | **VideoProcessingStarted**    | Processamento de vídeo iniciado            | Quando upload é recebido           |
| `1`    | **VideoProcessingCompleted**  | Processamento de vídeo concluído           | Quando ZIP está pronto para download |
| `2`    | **VideoProcessingFailed**     | Falha no processamento de vídeo            | Quando ocorre erro no FFmpeg       |
| `3`    | **General**                   | Notificação geral do sistema               | Avisos, manutenções, etc.          |



---

## 🐳 Deployment

### Docker Compose

O serviço está integrado no `docker-compose.yml` principal:

```yaml
notification-service:
  build:
    context: ./fiap-x-microservice-notification
    dockerfile: Dockerfile
  container_name: fiapx_notification_service
  restart: unless-stopped
  ports:
    - "5001:8080"  # Porta externa:interna
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - ASPNETCORE_URLS=http://+:8080
    - SMTP__Host=${SMTP_HOST:-smtp.mailtrap.io}
    - SMTP__Port=${SMTP_PORT:-2525}
    - SMTP__Username=${SMTP_USERNAME:-}
    - SMTP__Password=${SMTP_PASSWORD:-}
    - SMTP__FromEmail=${SMTP_FROM:-noreply@fiapx.com}
    - SMTP__FromName=${SMTP_FROM_NAME:-FIAP X Platform}
  networks:
    - fiapx-network
  healthcheck:
    test: ["CMD", "curl", "-f", "http://localhost:8080/api/notifications/health"]
    interval: 30s
    timeout: 10s
    retries: 3
    start_period: 20s

networks:
  fiapx-network:
    driver: bridge
```

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👥 Equipe FIAP X

**Projeto desenvolvido para Pós-Graduação em Arquitetura de Software - FIAP**

- Elen - Notification Service e API Gateway
- Marcelle - Video Processing Service
- Vitória - User Service e Auth Service
