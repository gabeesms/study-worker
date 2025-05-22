# study-worker

Sistema moderno de processamento de atividades de estudo construído com .NET 8, arquitetura em camadas e comunicação assíncrona via RabbitMQ.

## Tecnologias

- .NET 8
- Entity Framework Core
- RabbitMQ
- SQL Server
- Swagger/OpenAPI

## Arquitetura

O sistema é organizado em camadas:

- **API**: Interface RESTful
- **Domain**: Entidades, eventos e interfaces
- **Infrastructure**: Persistência e mensageria
- **Services**: Lógica de negócios

## Requisitos

- .NET 8 SDK
- SQL Server
- RabbitMQ
