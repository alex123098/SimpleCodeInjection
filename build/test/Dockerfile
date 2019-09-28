FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine3.9


LABEL "com.github.actions.name"="Test package"
LABEL "com.github.actions.description"="Test dotnet project"
LABEL "com.github.actions.icon"="log-in"
LABEL "com.github.actions.color"="blue"

ADD entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
