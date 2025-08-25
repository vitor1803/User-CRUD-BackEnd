using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UserCRUD.Context;
using UserCRUD.Models;
using UserCRUD.Services;
using UserCRUD.Config;


namespace UserCRUD.Rotas
{
    public static class UsuarioRotas
    {
        public static void Rotas(WebApplication app) 
        {
            // Rota para listar todos os usuários (inclusive inativos)
            var url = "/usuarios";
            app.MapGet(url + "/full-usuarios", async (UserContext userContext) =>
            {
                var usuarios = await userContext.Usuarios.ToListAsync();
                return Results.Ok(usuarios);
            })
                .WithName("ListarTodosUsuarios")
                .WithTags("Usuarios")
                .WithOpenApi(op =>
                {
                    op.Summary = "Lista todos os usuários";
                    op.Description = "Retorna todos os usuários cadastrados no sistema, incluindo ativos e inativos.";
                    op.Responses["200"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Lista de usuários retornada com sucesso."
                    };
                    op.Responses["401"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Usuário não autorizado a acessar este endpoint (JWT inválido ou ausente)."
                    };
                    return op;
                })
                .RequireAuthorization();

            // Rota de listagem de usuários ativos
            app.MapGet(url, async (UserContext userContext) =>
            {
                var usuarios = await userContext.Usuarios
                            .Where(u => u.Ativo)
                            .ToListAsync();
                return Results.Ok(usuarios);
            })
                .WithName("ListarUsuarios")
                .WithTags("Usuarios")
                .WithOpenApi(op =>
                {
                    op.Summary = "Lista todos os usuários ativos";
                    op.Description = "Retorna todos os usuários que estão marcados como ativos no banco de dados.";
                    op.Responses["200"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Lista de usuários retornada com sucesso."
                    };
                    op.Responses["401"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Usuário não autorizado a acessar este endpoint  (JWT inválido ou ausente)."
                    };

                    return op;
                })
                .RequireAuthorization();

            // Rota de cadrastro
            app.MapPost(url, async (UsuarioCadastroDTO usuarioDTO, UserContext userContext) => 
            {
                var usuario = new Usuario(usuarioDTO.name, usuarioDTO.email, usuarioDTO.password);
                await userContext.AddAsync(usuario);
                await userContext.SaveChangesAsync();
                return Results.Ok(usuario);
            })
                .WithName("CadastrarUsuario")
                .WithTags("Usuarios")
                .WithOpenApi(op =>
                {
                    op.Summary = "Cadastra um novo usuário";
                    op.Description = "Recebe os dados de um usuário e cria um registro no banco de dados. Retorna o usuário criado.";
                    op.Responses["200"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Usuário criado com sucesso."
                    };
                    op.Responses["400"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Dados inválidos enviados pelo cliente."
                    };

                    return op;
                });

            app.MapPut(url + "/{id:int}", async (int id, UsuarioCadastroDTO usuarioDTO, UserContext userContext) =>
            {
                var usuario = await userContext.Usuarios.FindAsync(id);
                if (usuario == null)
                    return Results.NotFound();

                usuario.ChangeName(usuarioDTO.name);
                await userContext.SaveChangesAsync();
                return Results.Ok(usuario);
            })
                .WithName("AtualizarUsuario")
                .WithTags("Usuarios")
                .WithOpenApi(op =>
                {
                    op.Summary = "Atualiza o nome de um usuário por ID";
                    op.Description = "Recebe os dados de um usuário e atualiza seu nome no banco de dados. Retorna o usuário atualizado.";
                    op.Responses["200"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Usuário atualizado com sucesso."
                    };
                    op.Responses["401"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Usuário não autorizado a acessar este endpoint (JWT inválido ou ausente)."
                    };
                    op.Responses["404"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Usuário não encontrado para o ID informado."
                    };

                    return op;
                })
                .RequireAuthorization();

            // Rota para "deletar"(desativar) um usuário por ID
            app.MapDelete(url + "/{id:int}", async (int id, UserContext userContext) =>
            {
                var usuario = await userContext.Usuarios.FindAsync(id);
                if (usuario == null)
                    return Results.NotFound();

                // Soft delete: marca o usuário como inativo
                usuario.Desativar();
                await userContext.SaveChangesAsync();

                return Results.NoContent();
            })
                .WithName("DeletarUsuario")
                .WithTags("Usuarios")
                .WithOpenApi(op =>
                {
                    op.Summary = "Desativa um usuário por ID";
                    op.Description = "Realiza um soft delete de um usuário. O usuário não é removido do banco, apenas marcado como inativo.";
                    op.Responses["204"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Usuário desativado com sucesso. Nenhum conteúdo retornado."
                    };
                    op.Responses["401"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Usuário não autorizado a acessar este endpoint (JWT inválido ou ausente)."
                    };
                    return op;
                })
                .RequireAuthorization();


            // Rota de login
            app.MapPost(url + "/login", async (UsuarioLoginDTO login, UserContext context, JwtConfig config) =>
            {
                Console.WriteLine($"Tentativa de login: {login.email} em {DateTime.UtcNow}");

                var usuario = await context.Usuarios
                                    .Where(u => u.Email == login.email && u.Ativo)
                                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    Console.WriteLine($"Login falhou: usuário não encontrado ({login.email})");
                    return Results.Unauthorized();
                }

                if (!usuario.VerificarSenha(login.password))
                {
                    Console.WriteLine($"Login falhou: senha incorreta ({login.email})");
                    return Results.Unauthorized();
                }

                var token = TokenService.GenerateToken(usuario, config);

                Console.WriteLine($"Login bem-sucedido: {usuario.Email}");
                return Results.Ok(new
                {
                    token,
                    usuario.Id,
                    usuario.Name,
                    usuario.Email
                });
            })
                .WithName("LogarUsuario")
                .WithTags("Autenticação")
                .WithOpenApi(op =>
                {
                    op.Summary = "Login de usuário";
                    op.Description = "Recebe email e senha e retorna um token JWT junto com os dados básicos do usuário.";

                    // Possíveis respostas
                    op.Responses["200"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Login bem-sucedido, retorna JWT e dados do usuário."
                    };
                    op.Responses["401"] = new Microsoft.OpenApi.Models.OpenApiResponse
                    {
                        Description = "Falha na autenticação (usuário não encontrado ou senha incorreta)."
                    };
                    return op;
                }
                );
        }
    }
}
