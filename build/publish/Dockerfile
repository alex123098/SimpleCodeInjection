FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9


LABEL "com.github.actions.name"="Build package"
LABEL "com.github.actions.description"="Builds dotnet artifact"
LABEL "com.github.actions.icon"="log-in"
LABEL "com.github.actions.color"="blue"

ADD entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
