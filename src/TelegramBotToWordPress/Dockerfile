FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/TelegramBotToWordPress/TelegramBotToWordPress.csproj src/TelegramBotToWordPress/
RUN dotnet restore src/TelegramBotToWordPress/TelegramBotToWordPress.csproj
COPY . .
WORKDIR /src/src/TelegramBotToWordPress
RUN dotnet build TelegramBotToWordPress.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TelegramBotToWordPress.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TelegramBotToWordPress.dll"]
