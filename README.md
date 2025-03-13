
# ForumAPI V2

Segunda versão de uma API criada para simular um fórum de discussão, com fórums, threads, posts, likes, dislikes, entre outros.

## Stack utilizada

- C#
- .NET 
- Entity Framework Core
- Docker


## Demonstração

A API está disponibilizada para testes usando Swagger no link a seguir: [Link para a API](http://45.32.215.95:5203/swagger/index.html)


# Documentação da API

## Índice
- [Usuários](#usuários)
- [Fóruns](#fóruns)
- [Threads](#threads)
- [Posts](#posts)
- [Reações de Thread](#reações-de-thread)
- [Reações de Post](#reações-de-post)
- [Imagens de Thread](#imagens-de-thread)

## Usuários

### Cadastrar Usuário
```
POST /user/signup
```

**Descrição:** Cadastra um novo usuário no sistema.

**Corpo da Requisição:**
```json
{
  "Name": "string",
  "Description": "string",
  "Email": "string",
  "Password": "string"
}
```


### Login de Usuário
```
POST /user/login
```

**Descrição:** Verifica se o usuário existe e retorna token de sessão JWT.

**Corpo da Requisição:**
```json
{
  "Email": "string",
  "Password": "string"
}
```

**Resposta de Erro:**
```
401 Unauthorized - "Usuário ou senha incorretos"
```

### Obter Usuário
```
GET /user/{userId}
```

**Descrição:** Retorna os dados de um usuário específico.

**Parâmetros de URL:**
- `userId` (string, obrigatório): ID do usuário

**Resposta de Sucesso:**
```json
{
  "id": "string",
  "name": "string",
  "description": "string",
  "lastLogin": "datetime",
  "dateJoined": "datetime"
}
```

## Fóruns

### Criar Fórum
```
POST /forum
```

**Descrição:** Cria um novo fórum.

**Corpo da Requisição:**
```json
{
  "Name": "string",
  "Description": "string"
}
```

**Resposta de Sucesso:**
```
201 Created
```

### Listar Fóruns
```
GET /forum
```

**Descrição:** Retorna todos os fóruns.

**Parâmetros de Query:**
- `take` (int, opcional, padrão=50): Número máximo de fóruns a retornar
- `forumId` (int, opcional): Filtrar por ID do fórum

**Resposta de Sucesso:**
```json
[
  {
    "id": 0,
    "name": "string",
    "description": "string"
  }
]
```

### Obter Fórum por ID
```
GET /forum/{forumId}
```

**Descrição:** Retorna o fórum que possui forumId como ID.

**Parâmetros de URL:**
- `forumId` (int, obrigatório): ID do fórum

**Parâmetros de Query:**
- `take` (int, opcional, padrão=50): Número máximo de resultados

**Resposta de Sucesso:**
```json
{
  "id": 0,
  "name": "string",
  "description": "string"
}
```

### Atualizar Fórum
```
PUT /forum/{forumId}
```

**Descrição:** Atualiza o fórum que possui forumId como ID.

**Parâmetros de URL:**
- `forumId` (int, obrigatório): ID do fórum

**Corpo da Requisição:**
```json
{
  "Name": "string",
  "Description": "string"
}
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Atualizar Parcialmente Fórum
```
PATCH /forum/{forumId}
```

**Descrição:** Atualiza uma parte do fórum que possui forumId como ID.

**Parâmetros de URL:**
- `forumId` (int, obrigatório): ID do fórum

**Corpo da Requisição:**
```json
[
  {
    "op": "replace",
    "path": "/propertyName",
    "value": "newValue"
  }
]
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Deletar Fórum
```
DELETE /forum/{forumId}
```

**Descrição:** Deleta o fórum que possui forumId como ID.

**Parâmetros de URL:**
- `forumId` (int, obrigatório): ID do fórum

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

## Threads

### Criar Thread
```
POST /fthread
```

**Descrição:** Cria uma nova thread.

**Corpo da Requisição:**
```json
{
  "ForumID": 0,
  "Name": "string",
  "Text": "string",
  "Sticky": false,
  "Active": true,
  "UserId": "string",
  "Locked": false
}
```

**Resposta de Sucesso:**
```
201 Created
```

### Listar Threads
```
GET /fthread
```

**Descrição:** Retorna todas as threads.

**Parâmetros de Query:**
- `forumId` (int, opcional): Filtrar por ID do fórum
- `take` (int, opcional, padrão=50): Número máximo de threads a retornar

**Resposta de Sucesso:**
```json
[
  {
    "id": 0,
    "forumID": 0,
    "name": "string",
    "text": "string",
    "sticky": false,
    "active": true,
    "dateCreated": "datetime",
    "userId": "string",
    "locked": false
  }
]
```

### Listar Threads Mais Curtidas
```
GET /fthread/most-liked/{period}
```

**Descrição:** Retorna as threads mais curtidas dentro de um período específico.

**Parâmetros de URL:**
- `period` (int, obrigatório): Período em dias

**Resposta de Sucesso:**
```json
[
  {
    "id": 0,
    "forumID": 0,
    "name": "string",
    "text": "string",
    "sticky": false,
    "active": true,
    "dateCreated": "datetime",
    "userId": "string",
    "locked": false
  }
]
```

### Obter Thread por ID
```
GET /fthread/{fthreadId}
```

**Descrição:** Retorna a thread que possui fthreadId como ID.

**Parâmetros de URL:**
- `fthreadId` (int, obrigatório): ID da thread

**Resposta de Sucesso:**
```json
{
  "id": 0,
  "forumID": 0,
  "name": "string",
  "text": "string",
  "sticky": false,
  "active": true,
  "dateCreated": "datetime",
  "userId": "string",
  "locked": false
}
```

### Atualizar Thread
```
PUT /fthread/{fthreadId}
```

**Descrição:** Atualiza a thread que possui fthreadId como ID.

**Parâmetros de URL:**
- `fthreadId` (int, obrigatório): ID da thread

**Corpo da Requisição:**
```json
{
  "ForumID": 0,
  "Name": "string",
  "Text": "string",
  "Sticky": false,
  "Active": true,
  "UserId": "string",
  "Locked": false
}
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Atualizar Parcialmente Thread
```
PATCH /fthread/{fthreadId}
```

**Descrição:** Atualiza uma parte da thread que possui fthreadId como ID.

**Parâmetros de URL:**
- `fthreadId` (int, obrigatório): ID da thread

**Corpo da Requisição:**
```json
[
  {
    "op": "replace",
    "path": "/propertyName",
    "value": "newValue"
  }
]
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Deletar Thread
```
DELETE /fthread/{fthreadId}
```

**Descrição:** Deleta a thread que possui fthreadId como ID.

**Parâmetros de URL:**
- `fthreadId` (int, obrigatório): ID da thread

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

## Posts

### Criar Post
```
POST /post
```

**Descrição:** Cria um novo post.

**Corpo da Requisição:**
```json
{
  "ThreadId": 0,
  "Text": "string",
  "UserId": "string",
  "Locked": false
}
```

**Resposta de Sucesso:**
```
201 Created
```

### Listar Posts
```
GET /post
```

**Descrição:** Retorna todos os posts.

**Parâmetros de Query:**
- `fthreadId` (int, opcional): Filtrar por ID da thread
- `take` (int, opcional, padrão=50): Número máximo de posts a retornar

**Resposta de Sucesso:**
```json
[
  {
    "id": 0,
    "threadId": 0,
    "text": "string",
    "userId": "string",
    "dateCreated": "datetime",
    "locked": false
  }
]
```

### Obter Post por ID
```
GET /post/{postId}
```

**Descrição:** Retorna o post que possui postId como ID.

**Parâmetros de URL:**
- `postId` (int, obrigatório): ID do post

**Resposta de Sucesso:**
```json
{
  "id": 0,
  "threadId": 0,
  "text": "string",
  "userId": "string",
  "dateCreated": "datetime",
  "locked": false
}
```

### Atualizar Post
```
PUT /post/{postId}
```

**Descrição:** Atualiza o post que possui postId como ID.

**Parâmetros de URL:**
- `postId` (int, obrigatório): ID do post

**Corpo da Requisição:**
```json
{
  "ThreadId": 0,
  "Text": "string",
  "UserId": "string",
  "Locked": false
}
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Atualizar Parcialmente Post
```
PATCH /post/{postId}
```

**Descrição:** Atualiza uma parte do post que possui postId como ID.

**Parâmetros de URL:**
- `postId` (int, obrigatório): ID do post

**Corpo da Requisição:**
```json
[
  {
    "op": "replace",
    "path": "/propertyName",
    "value": "newValue"
  }
]
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Deletar Post
```
DELETE /post/{id}
```

**Descrição:** Deleta o post que possui postId como ID.

**Parâmetros de URL:**
- `id` (int, obrigatório): ID do post

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

## Reações de Thread

### Criar Reação de Thread
```
POST /fthreadreaction
```

**Descrição:** Cria uma nova reação para uma thread.

**Corpo da Requisição:**
```json
{
  "ThreadId": 0,
  "UserId": "string",
  "Reaction": true
}
```

**Resposta de Sucesso:**
```
201 Created
```

### Listar Reações de Thread
```
GET /fthreadreaction
```

**Descrição:** Retorna todas as reações de threads.

**Parâmetros de Query:**
- `take` (int, opcional, padrão=50): Número máximo de reações a retornar

**Resposta de Sucesso:**
```json
[
  {
    "threadId": 0,
    "userId": "string",
    "reaction": true
  }
]
```

### Listar Reações por Thread
```
GET /fthreadreaction/{fthreadId}
```

**Descrição:** Retorna as reações de uma thread específica.

**Parâmetros de URL:**
- `fthreadId` (int, obrigatório): ID da thread

**Parâmetros de Query:**
- `reaction` (string, opcional): Filtrar por tipo de reação ("like" ou "dislike")

**Resposta de Sucesso:**
```json
[
  {
    "threadId": 0,
    "userId": "string",
    "reaction": true
  }
]
```

### Obter Pontuação de Reações da Thread
```
GET /fthreadreaction/score/{fthreadId}
```

**Descrição:** Retorna a pontuação de reações (likes - dislikes) de uma thread.

**Parâmetros de URL:**
- `fthreadId` (int, obrigatório): ID da thread

**Resposta de Sucesso:**
```
200 OK - valor numérico
```

### Obter Reação de Usuário em Thread
```
GET /fthreadreaction/{fthreadId}/{userId}
```

**Descrição:** Retorna a reação de um usuário específico em uma thread.

**Parâmetros de URL:**
- `fthreadId` (int, obrigatório): ID da thread
- `userId` (string, obrigatório): ID do usuário

**Resposta de Sucesso:**
```json
{
  "threadId": 0,
  "userId": "string",
  "reaction": true
}
```

### Atualizar Reação de Thread
```
PUT /fthreadreaction/{fthreadId}/{userId}
```

**Descrição:** Atualiza a reação de um usuário em uma thread.

**Parâmetros de Query:**
- `fThreadId` (int, obrigatório): ID da thread
- `UserId` (string, obrigatório): ID do usuário

**Corpo da Requisição:**
```json
{
  "ForumID": 0,
  "Name": "string",
  "Text": "string",
  "Sticky": false,
  "Active": true,
  "UserId": "string",
  "Locked": false
}
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Deletar Reação de Thread
```
DELETE /fthreadreaction/{fthreadId}/{userId}
```

**Descrição:** Deleta a reação de um usuário em uma thread.

**Parâmetros de URL:**
- `fthreadId` (int, obrigatório): ID da thread
- `userId` (string, obrigatório): ID do usuário

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

## Reações de Post

### Criar Reação de Post
```
POST /postreaction
```

**Descrição:** Cria uma nova reação para um post.

**Corpo da Requisição:**
```json
{
  "PostId": 0,
  "UserId": "string",
  "Reaction": true
}
```

**Resposta de Sucesso:**
```
201 Created
```

### Listar Reações de Post
```
GET /postreaction
```

**Descrição:** Retorna todas as reações de posts.

**Parâmetros de Query:**
- `postId` (int, opcional): Filtrar por ID do post
- `take` (int, opcional, padrão=50): Número máximo de reações a retornar

**Resposta de Sucesso:**
```json
[
  {
    "postId": 0,
    "userId": "string",
    "reaction": true
  }
]
```

### Listar Reações por Post
```
GET /postreaction/{postId}
```

**Descrição:** Retorna as reações de um post específico.

**Parâmetros de URL:**
- `postId` (int, obrigatório): ID do post

**Parâmetros de Query:**
- `reaction` (string, opcional): Filtrar por tipo de reação ("like" ou "dislike")

**Resposta de Sucesso:**
```json
[
  {
    "postId": 0,
    "userId": "string",
    "reaction": true
  }
]
```

### Obter Pontuação de Reações do Post
```
GET /postreaction/score/{postId}
```

**Descrição:** Retorna a pontuação de reações (likes - dislikes) de um post.

**Parâmetros de URL:**
- `postId` (int, obrigatório): ID do post

**Resposta de Sucesso:**
```
200 OK - valor numérico
```

### Obter Reação de Usuário em Post
```
GET /postreaction/{postId}/{userId}
```

**Descrição:** Retorna a reação de um usuário específico em um post.

**Parâmetros de URL:**
- `postId` (int, obrigatório): ID do post
- `userId` (string, obrigatório): ID do usuário

**Resposta de Sucesso:**
```json
{
  "postId": 0,
  "userId": "string",
  "reaction": true
}
```

### Atualizar Reação de Post
```
PUT /postreaction/{fthreadId}/{userId}
```

**Descrição:** Atualiza a reação de um usuário em um post.

**Parâmetros de Query:**
- `fthreadId` (int, obrigatório): ID do post
- `UserId` (string, obrigatório): ID do usuário

**Corpo da Requisição:**
```json
{
  "ThreadId": 0,
  "Text": "string",
  "UserId": "string",
  "Locked": false
}
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Deletar Reação de Post
```
DELETE /postreaction/{postId}/{userId}
```

**Descrição:** Deleta a reação de um usuário em um post.

**Parâmetros de URL:**
- `postId` (int, obrigatório): ID do post
- `userId` (string, obrigatório): ID do usuário

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

## Imagens de Thread

### Criar Imagem de Thread
```
POST /fthreadimage
```

**Descrição:** Adiciona uma nova imagem a uma thread.

**Corpo da Requisição:**
```json
{
  "FThreadId": 0,
  "ImgId": "string"
}
```

**Resposta de Sucesso:**
```
201 Created
```

### Listar Imagens de Thread
```
GET /fthreadimage
```

**Descrição:** Retorna todas as imagens de threads.

**Parâmetros de Query:**
- `fthreadId` (int, opcional): Filtrar por ID da thread
- `take` (int, opcional, padrão=50): Número máximo de imagens a retornar

**Resposta de Sucesso:**
```json
[
  {
    "id": 0,
    "fThreadId": 0,
    "imgId": "string"
  }
]
```

### Atualizar Imagem de Thread
```
PUT /fthreadimage/{fthreadImageId}
```

**Descrição:** Atualiza uma imagem de thread.

**Parâmetros de URL:**
- `fthreadImageId` (int, obrigatório): ID da imagem de thread

**Corpo da Requisição:**
```json
{
  "FThreadId": 0,
  "ImgId": "string"
}
```

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```

### Deletar Imagem de Thread
```
DELETE /fthreadimage/{fthreadImageId}
```

**Descrição:** Deleta uma imagem de thread.

**Parâmetros de URL:**
- `fthreadImageId` (int, obrigatório): ID da imagem de thread

**Resposta de Sucesso:**
```
204 No Content
```

**Resposta de Erro:**
```
404 Not Found
```


