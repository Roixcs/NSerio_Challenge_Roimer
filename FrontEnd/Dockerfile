# Stage 1: Build Angular app
FROM node:20-alpine AS build
WORKDIR /app
COPY team-tasks-app/package*.json ./
RUN npm ci
COPY team-tasks-app/ .
RUN npm run build -- --configuration=production

# Stage 2: Serve with nginx
FROM nginx:alpine AS final
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/team-tasks-app/browser /usr/share/nginx/html
EXPOSE 80
