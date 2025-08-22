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
            var url = "/usuarios";
            app.MapGet(url + "/full-usuarios", async (UserContext userContext) => 
            {
                var usuarios = await userContext.Usuarios.ToListAsync();
                return Results.Ok(usuarios);
            }).RequireAuthorization();

            app.MapGet(url, async (UserContext userContext) =>
            {
                var usuarios = await userContext.Usuarios
                            .Where(u => u.Ativo)
                            .ToListAsync();
                return Results.Ok(usuarios);
            }).RequireAuthorization();


            app.MapPost(url, async (HttpRequest request, UserContext userContext) =>
            {
                request.EnableBuffering(); 
                using var reader = new StreamReader(request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                Console.WriteLine("JSON recebido:");
                Console.WriteLine(body);
                
                var usuarioDTO = JsonSerializer.Deserialize<UsuarioDTO>(body);

                Console.WriteLine(usuarioDTO);
                var usuario = new Usuario(usuarioDTO.name, usuarioDTO.email, usuarioDTO.password);
                await userContext.AddAsync(usuario);
                await userContext.SaveChangesAsync();
                return Results.Ok(usuario);
            });

            app.MapPut(url + "/{id:int}", async (int id, UsuarioDTO usuarioDTO, UserContext userContext) =>
            {
                var usuario = await userContext.Usuarios.FindAsync(id);

                if (usuario == null)
                    return Results.NotFound();

                usuario.ChangeName(usuarioDTO.name);
                await userContext.SaveChangesAsync();
                return Results.Ok(usuario);
            });

            app.MapDelete(url + "/{id:int}", async (int id, UserContext userContext) =>
            {
                var usuario = await userContext.Usuarios.FindAsync(id);
                if (usuario == null)
                    return Results.NotFound();


                // Soft Delete
                usuario.Desativar(); 

                await userContext.SaveChangesAsync();
                return Results.NoContent();
            });


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
            });
        }
    }
}
