FROM microsoft/dotnet:2.1-sdk AS build
LABEL maintainer="teodor.elstad@gmail.com"
WORKDIR /app
COPY . .
RUN dotnet restore Player.csproj \
    && dotnet publish Player.csproj -o ./out -c Release

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Player.dll"]