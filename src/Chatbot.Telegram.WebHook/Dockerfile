FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/net5.0 App/
ARG ONEMENU_URL
ARG TELEGRAM_TOKEN

ENV OneMenuServiceUrl=$ONEMENU_URL
ENV chatbotToken=$TELEGRAM_TOKEN

EXPOSE 80
EXPOSE 443

WORKDIR /App

ENTRYPOINT ["dotnet", "Chatbot.Telegram.WebHook.dll"]