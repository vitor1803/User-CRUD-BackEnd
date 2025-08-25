# 1. Imagem base com SDK .NET 9 (para desenvolvimento)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# 2. Diretório de trabalho dentro do container
WORKDIR /app

# 3. Copiar arquivos de projeto para restaurar pacotes
COPY UserCRUD.csproj ./
COPY UserCRUD.sln ./

# 4. Restaurar dependências
RUN dotnet restore

# 5. Copiar todo o código fonte
COPY . ./

# 6. Comando padrão para rodar o projeto
CMD ["dotnet", "run", "--urls", "http://0.0.0.0:4200"]