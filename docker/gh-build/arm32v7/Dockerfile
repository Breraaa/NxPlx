﻿FROM alpine AS gitCloner
RUN apk --update --no-cache --quiet add git openssh && rm -rf /var/lib/apt/lists/*
WORKDIR /project
RUN git clone https://github.com/rosenbjerg/NxPlx.git


FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS backendBuilder
WORKDIR /app
COPY --from=gitCloner /project/NxPlx/nxplx-backend/ ./
RUN dotnet publish NxPlx.Infrastructure.WebApi/NxPlx.Infrastructure.WebApi.csproj -c Release -r linux-arm -o build


FROM node:12.10.0-alpine AS frontendBuilder
RUN apk --update --no-cache --quiet add git openssh && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=gitCloner /project/NxPlx/nxplx-frontend/ ./
RUN npm install --no-progress --silent
RUN npm run build


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim-arm32v7 AS runtime
ARG tmdbApiKey
LABEL maintainer="Malte Rosenbjerg"
LABEL description="NxPlx on dotnet 3.0"
LABEL repository="github.com/rosenbjerg/NxPlx"
WORKDIR /app
COPY --from=backendBuilder /app/build ./
COPY --from=frontendBuilder /app/build ./public/
ENV TMDB_API_KEY=$tmdbApiKey
EXPOSE 5353
ENTRYPOINT ["dotnet", "NxPlx.Infrastructure.WebApi.dll"]