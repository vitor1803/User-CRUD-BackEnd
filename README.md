# UserCRUD API

API em .NET 9 para gerenciamento de usuários com autenticação JWT, incluindo cadastro, listagem, atualização, desativação (soft delete) e login.

---

## Funcionalidades

- **Cadastro de usuário** (`POST /usuarios`)
- **Listagem de usuários ativos** (`GET /usuarios`)
- **Listagem de todos os usuários (ativos e inativos)** (`GET /usuarios/full-usuarios`)
- **Atualização de usuário** (`PUT /usuarios/{id}`)
- **Desativação de usuário (soft delete)** (`DELETE /usuarios/{id}`)
- **Login de usuário** (`POST /usuarios/login`) com geração de token JWT

---

## Tecnologias utilizadas

- .NET 9
- Entity Framework Core
- JWT (JSON Web Token) para autenticação
- Swagger para documentação da API (`https://localhost:4200/swagger/index.html`)
- Docker para containerização
- CORS configurado para Angular (`http://localhost:4200`)

---

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Docker (opcional, caso queira rodar a aplicação em container)
- Banco de dados configurado (conforme `UserContext`)

---

## Configuração

1. Clone o repositório:

```bash
    git clone https://github.com/vitor1803/User-CRUD-BackEnd
```



## Rotas da API

### Usuários

| Método | Endpoint | Descrição |
|--------|---------|-----------|
| GET | `/usuarios` | Lista todos os usuários **ativos** |
| GET | `/usuarios/full-usuarios` | Lista **todos os usuários**, ativos e inativos |
| POST | `/usuarios` | Cadastra um novo usuário |
| PUT | `/usuarios/{id}` | Atualiza o nome de um usuário pelo **ID** |
| DELETE | `/usuarios/{id}` | Desativa um usuário (soft delete) |

### Autenticação

| Método | Endpoint | Descrição |
|--------|---------|-----------|
| POST | `/usuarios/login` | Realiza login e retorna **token JWT** e dados básicos do usuário |

## Autenticação

Para acessar a maioria dos endpoints, é necessário enviar um token JWT no cabeçalho da requisição:



- O token é obtido no endpoint de login (`POST /usuarios/login`).
- Endpoints de cadastro e login não exigem autenticação.
- Tokens possuem validade configurável no `JwtConfig` (em minutos).

## Documentação Swagger

A API possui documentação interativa via Swagger:

- Acesse em: `https://localhost:4200/swagger/index.html` (ou HTTP se configurado)
- Permite testar endpoints diretamente do navegador.
- Mostra detalhes de parâmetros, respostas e requisitos de autenticação.


**Observações:**

- Todas as rotas, exceto `/usuarios` (cadastro) e `/usuarios/login`, requerem **token JWT** no header:
  
