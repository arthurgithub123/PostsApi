using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PostsApi.Models.SwaggerCustomFilter
{
    public class CustomOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Add examples with different messages to success, error responses to each API endpoint and authentication
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            AddResponseAndExample(operation, 500, "application/json", "Se ocorrer erro interno no servidor não provocado pelo usuário", "Erro interno no servidor");
            
            if(context.ApiDescription.RelativePath == "api/posts")
            {
                operation.Summary = "Create or recommend post based on user role";

                operation.Responses.Remove("200");
                AddResponseAndExample(operation, 201, "application/json", "Se o Post for cadastrado com sucesso", "Post cadastrado com sucesso");
                AddResponseAndExample(operation, 400, "application/json", "Se o Post for nulo ou não conter nome ou imagem", "Preencha, pelo menos, a descrição ou a imagem");
            }

            if(context.ApiDescription.RelativePath == "api/posts/accept/{id}")
            {
                operation.Summary = "Post for Administrator to accept";

                operation.Responses["200"].Description = "Se o Post for aceito com sucesso";
                SetExample(operation, 200, "application/json", "Post aceito com sucesso");
            }

            if(context.ApiDescription.RelativePath == "api/posts/list/{filter}")
            {
                operation.Summary = "List posts based on filter";

                operation.Description =
                    "<p>Parâmetro filter (não coloque espaços em branco antes e depois do filtro)</p>" +
                    "<ul>" +
                        "<li>all (posts criados por admins ou posts aceitos de usuários)</li>" +
                        "<li>onlyAdministratorsPosts (posts somente de admins)</li>" +
                    "</ul>" +
                    "<p>Para Administradores</p>" +
                        "<ul style='list-style-type: none;'>" +
                            "<li>recomendedNotAcceptedYet (posts ainda não aceitos criados por usuários)</li>" +
                            "<li>recomendedAndAccepted (posts aceitos)</li>" +
                            "<li>recomendDate (posts aceitos por data de criãção)</li>" +
                            "<li>acceptedDate (posts aceitos por data de aceitação)</li>" +
                        "</ul>" +
                    "<p>Para usuários</p>" +
                        "<ul>" +
                            "<li>accepted (posts aceitos do usuário)</li>" +
                            "<li>notAccepted (posts rejeitaos do usuário)</li>" +
                            "<li>myCreatedPosts (posts criados do usuário e ainda não aceitos e não rejeitados)</li>" +
                            "<li>onlyCommonUsersPosts (posts aceitos de usuários não administradores)</li>" +
                        "</ul>";

                operation.Responses["200"].Description = "Se o filtro da paginação não estiver vazio e os dados forem retornados com sucesso";
                operation.Responses["200"].Content.Add(
                    "application/json",
                    new OpenApiMediaType
                    {
                        Example = new OpenApiObject
                        {
                            ["totalRecords"] = new OpenApiInteger(100),
                            ["nextPage"] = new OpenApiString("http://domain/api/posts/list/all?page=2&per_page=10"),
                            ["previousPage"] = new OpenApiString("http://domain/api/posts/list/all?page=1&per_page=10"),
                            ["data"] = new OpenApiArray
                            {
                                new OpenApiObject
                                {
                                    ["id"] = new OpenApiString("0f8fad5b-d9cb-469f-a165-70867728950e"),
                                    ["description"] = new OpenApiString("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."),
                                    ["image"] = new OpenApiBinary(new byte[] {  }),
                                    ["imageUrl"] = new OpenApiString(""),
                                    ["createdAt"] = new OpenApiDateTime(DateTime.Now),
                                    ["recomendDate"] = new OpenApiDateTime(DateTime.Now),
                                    ["canEdit"] = new OpenApiBoolean(true)
                                },
                                new OpenApiObject
                                {
                                    ["id"] = new OpenApiString("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                                    ["description"] = new OpenApiString("Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo."),
                                    ["image"] = new OpenApiBinary(new byte[] {  }),
                                    ["imageUrl"] = new OpenApiString(""),
                                    ["createdAt"] = new OpenApiDateTime(DateTime.Now),
                                    ["recomendDate"] = new OpenApiDateTime(DateTime.Now),
                                    ["canEdit"] = new OpenApiBoolean(true)
                                }
                            }
                        }
                    }
                );

                AddResponseAndExample(operation, 400, "application/json", "Se o filtro da paginação estiver vazio", "O filtro da pesquisa não pode estar vazio");
            }

            if(context.ApiDescription.RelativePath == "api/posts/list_by_id/{id}")
            {
                operation.Summary = "Show post by id";

                operation.Responses["200"].Description = "Se o Post for recuperado com sucesso";
                operation.Responses["200"].Content.Add(
                    "application/json",
                    new OpenApiMediaType
                    {
                        Example = new OpenApiObject
                        {
                            ["id"] = new OpenApiString("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                            ["description"] = new OpenApiString("Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo."),
                            ["image"] = new OpenApiBinary(new byte[] { }),
                            ["imageUrl"] = new OpenApiString(""),
                            ["createdAt"] = new OpenApiDateTime(DateTime.Now),
                            ["recomendDate"] = new OpenApiDateTime(DateTime.Now),
                            ["canEdit"] = new OpenApiBoolean(true)
                        }
                    }
                );

                AddResponseAndExample(operation, 400, "application/json", "Se o Post não foi recuperado porque o id enviado estava errado ou se o usuário não for o autor do post", "Post inexistente");
            }

            if(context.ApiDescription.RelativePath == "api/posts/edit/{id}")
            {
                operation.Summary = "Edit post by id";

                operation.Responses["200"].Description = "Se o Post for alterado com sucesso";
                AddResponseAndExample(operation, 400, "application/json", "Se o Post for nulo ou se o usuário não for o autor do post ou se o post não possuir descrição e imagem", "Não é possível alterar posts de outra pessoa");
            }

            if(context.ApiDescription.RelativePath == "api/session/administrator")
            {
                operation.Summary = "Create an administrator and send a link to his/her email for password creation";
            
                operation.Responses.Remove("200");
                
                AddResponseAndExample(operation, 201, "application/json", "Se o usuário for criado com sucesso", "Administrador cadastrado com sucesso");
                AddResponseAndExample(operation, 400, "application/json",
                    "Se e-mail e senha não forem informados ou se já existe uma conta com o e-mail informado ou se a senha não estiver de acordo com as regras",
                    "Já existe uma conta com o e-mail informado");
            }

            if(context.ApiDescription.RelativePath == "api/session/common")
            {
                operation.Summary = "Create a common user";

                operation.Responses["200"].Content.Remove("text/plain");
                operation.Responses["200"].Content.Remove("text/json");

                operation.Responses.Add("201", new OpenApiResponse { Description = "Se o usuário for cadastrado com sucesso" });
                context.SchemaRepository.Schemas["UserToken"].Example = new OpenApiObject
                {
                    ["token"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"),
                    ["user"] = new OpenApiObject
                    {
                        ["id"] = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                        ["role"] = new OpenApiString("Common"),
                        ["name"] = new OpenApiString("Takeshi Kovacs"),
                        ["email"] = new OpenApiString("tak@gmail.com")
                    }
                };

                AddResponseAndExample(operation, 400, "application/json",
                    "Se e-mail e senha não forem informados ou se já existe uma conta com o e-mail informado ou se a senha não estiver de acordo com as regras ou se não for possível realizar o login",
                    "Já existe uma conta com o e-mail informado");
            }

            if(context.ApiDescription.RelativePath == "api/session/password")
            {
                operation.Summary = "Create password using token sent to user's email";

                operation.RequestBody.Content.Remove("text/json");
                operation.RequestBody.Content.Remove("application/*+json");

                context.SchemaRepository.Schemas["PasswordCreate"].Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString("Reileen_Kawahara@gmail.com"),
                    ["password"] = new OpenApiString("123456A-z"),
                    ["token"] = new OpenApiString("Qien9e83f-jhqwjh34j9y3-asknsajbasZ-hdYgcOhsAwReyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVC")
                };

                operation.Responses["200"].Description = "Se a senha for criada com sucesso";
                operation.Responses["200"].Content.Add(
                    "application/json",
                    new OpenApiMediaType
                    {
                        Example = new OpenApiObject
                        {
                            ["statusCode"] = new OpenApiInteger(200),
                            ["message"] = new OpenApiString("Senha criada com sucesso")
                        }
                    }
                );

                AddResponseAndExample(operation, 400, "application/json", "Se e-mail, senha e token não forem enviados", "E-mail, senha e token são necessários");
            }

            if(context.ApiDescription.RelativePath == "api/session/password/forgot/{email}")
            {
                operation.Summary = "Send a link to user's email for password creation";

                operation.Responses["200"].Description = "Se o e-mail com o link para criação da nova senha for enviado com sucesso";
                SetExample(operation, 200, "application/json", "E-mail com link para nova senha enviado com sucesso");

                AddResponseAndExample(operation, 400, "application/json", "Se o e-mail informado for incorreto", "Não existe um usuário com esse e-mail");
            }

            if(context.ApiDescription.RelativePath == "api/session/password/change")
            {
                operation.Summary = "Change user's password";

                operation.Responses["200"].Description = "Se o e-mail com o link para criação da nova senha for enviado com sucesso";
                SetExample(operation, 200, "application/json", "Senha alterada com sucesso");

                operation.RequestBody.Content.Remove("application/*+json");
                operation.RequestBody.Content.Remove("text/json");
                operation.RequestBody.Content["application/json"].Example = new OpenApiObject
                {
                    ["currentPassword"] = new OpenApiString("123456A-z"),
                    ["newPassword"] = new OpenApiString("nova123456A-z")
                };

                AddResponseAndExample(operation, 400, "application/json", "Se o e-mail informado for incorreto", "Não existe um usuário com esse e-mail");
            }

            if(context.ApiDescription.RelativePath == "api/session/login")
            {
                operation.Summary = "Login user";

                operation.RequestBody.Content.Remove("text/json");
                operation.RequestBody.Content.Remove("application/*+json");
                
                operation.RequestBody.Content["application/json"].Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString("Kristin_Ortega@gmail.com"),
                    ["password"] = new OpenApiString("1111117A-z")
                };

                operation.Responses["200"].Description = "Se e-mail e senha estiverem corretos";
                operation.Responses["200"].Content["application/json"].Example = new OpenApiObject
                {
                    ["token"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"),
                    ["user"] = new OpenApiObject
                    {
                        ["id"] = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                        ["role"] = new OpenApiString("Common"),
                        ["name"] = new OpenApiString("Kristin Ortega"),
                        ["email"] = new OpenApiString("Kristin_Ortega@gmail.com")
                    }
                };

                operation.Responses["200"].Content.Remove("text/plain");
                operation.Responses["200"].Content.Remove("text/json");

                AddResponseAndExample(operation, 400, "application/json", "Se e-mail ou senha não forem informados ou se os dados estiverem incorretos", "E-mail e senha devem ser informados");
            }

            AddAuthentication(operation, context);
        }

        private void SetExample(OpenApiOperation operation, int statusCode, string contentType, string message)
        {
            operation.Responses[statusCode.ToString()].Content.Add(
                contentType,
                new OpenApiMediaType
                {
                    Example = new OpenApiObject
                    {
                        ["statusCode"] = new OpenApiInteger(statusCode),
                        ["message"] = new OpenApiString(message)
                    }
                }
            );
        }
        
        private void AddResponseAndExample(OpenApiOperation operation, int statusCode, string contentType, string description, string message)
        {
            operation.Responses.Add(statusCode.ToString(), new OpenApiResponse { Description = description });

            operation.Responses[statusCode.ToString()].Content.Add(
                contentType,
                new OpenApiMediaType
                {
                    Example = new OpenApiObject
                    {
                        ["statusCode"] = new OpenApiInteger(statusCode),
                        ["message"] = new OpenApiString(message)
                    }
                }
            );
        }

        private void AddAuthentication(OpenApiOperation operation, OperationFilterContext context)
        {
            // Policy names map to scopes
            var requiredScopes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Select(customAttribute => customAttribute.Policy)
                .Distinct();
            
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                if (!context.ApiDescription.CustomAttributes().Any(customAttribute => customAttribute is AllowAnonymousAttribute) &&
                    (
                        context.ApiDescription.CustomAttributes().Any(customAttribute => customAttribute is AuthorizeAttribute) ||
                        controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>() != null
                    )
                )
                {
                    operation.Responses.Add("401", new OpenApiResponse
                    {
                        Description = "Se o usuário não possuir autorização",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = new OpenApiObject
                                {
                                    ["statusCode"] = new OpenApiInteger(401),
                                    ["message"] = new OpenApiString("Você não possui autorização")
                                }
                            }
                        }
                    });

                    OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    };

                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
                        {
                            [openApiSecurityScheme] = requiredScopes.ToList()
                        }
                    };
                }
            }
        }
    }
}
