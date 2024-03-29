## Posts API Asp.Net Core

API Back-End para sistema no qual usuários criam posts com texto e/ou imagem.

Posts de usuários comuns devem ser aceitos por usuários administradores.

### Features

- [x] Autenticação
- [x] Autorização (Utilização de Json Web Token)
- [x] Criar Post (Admin)
- [x] Aceitar Post (Admin)
- [x] Criar Post para aguardar aceitação (Common user)
- [x] Listagem de Posts
- [x] Alteração de Posts
- [ ] Exclusão de Posts
- [x] Paginação
- [x] Adicionar usuários admins
- [x] Envio de token por e-mail para um novo admin criar sua senha
- [x] Envio de token por e-mail para alteração de senha
- [x] Versionamento da API
- [x] Documentação com Swagger
- [x] Testes Unitários para PostService
- [] Testes Unitários para SessionService
- [] Testes Unitários para TokenService
- [] Testes Unitários para UserService

### Patterns

- [x] Generic Repository
- [ ] Unit Of Work

### Tecnologias

- Asp.Net Core
- Entity Framework Core
- Asp.Net Core Identity
