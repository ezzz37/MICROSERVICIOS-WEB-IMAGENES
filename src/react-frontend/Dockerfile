# 1) Build
FROM node:18-alpine AS builder
WORKDIR /app

# 1.1 Instala dependencias
COPY package.json package-lock.json ./
RUN npm ci

# 1.2 Genera el build de producción
COPY . .
RUN npm run build

# 2) Serve con Nginx
FROM nginx:stable-alpine

# 2.1 Elimina la conf por defecto de Nginx
RUN rm /etc/nginx/conf.d/default.conf

# 2.2 Copia tu propio nginx.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf

# 2.3 Copia los estáticos
COPY --from=builder /app/build /usr/share/nginx/html

# 2.4 Expone el puerto 80
EXPOSE 80

# 2.5 Arranca Nginx en primer plano
CMD ["nginx", "-g", "daemon off;"]
