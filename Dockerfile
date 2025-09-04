FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY QNBScoring.sln ./
COPY QNBScoring.Web/*.csproj QNBScoring.Web/
COPY QNBScoring.Core/*.csproj QNBScoring.Core/
COPY QNBScoring.Infrastructure/*.csproj QNBScoring.Infrastructure/
COPY QNBScoring.UnitTests/*.csproj QNBScoring.UnitTests/
RUN dotnet restore QNBScoring.sln
COPY . .
RUN dotnet publish QNBScoring.Web/QNBScoring.Web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "QNBScoring.Web.dll"]