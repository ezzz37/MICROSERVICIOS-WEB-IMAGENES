# 1) Build
FROM node:18-alpine AS builder

WORKDIR /app

# instalar dependencias
COPY package.json package-lock.json ./
RUN npm ci

# compilar la aplicación
COPY . .
RUN npm run build

# 2) Servir con Nginx
FROM nginx:stable-alpine

# elimina la configuración default para usar la tuya
RUN rm /etc/nginx/conf.d/default.conf

# tu config de rutas SPA
COPY nginx.conf /etc/nginx/conf.d/

# copia los archivos estáticos
COPY --from=builder /app/build /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
