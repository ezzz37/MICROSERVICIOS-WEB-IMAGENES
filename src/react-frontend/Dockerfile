# Build
FROM node:18-alpine AS builder
WORKDIR /app

# Instala dependencias
COPY package.json package-lock.json ./
RUN npm ci

# Genera el build de producción
COPY . .
# Limpiar cualquier cache residual
RUN rm -rf node_modules/.cache build .eslintcache
# Build sin cache
ENV GENERATE_SOURCEMAP=false
ENV CI=false
RUN npm run build

# Serve con Nginx
FROM nginx:stable-alpine

# Elimina la conf por defecto de Nginx
RUN rm /etc/nginx/conf.d/default.conf

# Copia tu propio nginx.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copia los estáticos
COPY --from=builder /app/build /usr/share/nginx/html

# Expone el puerto 80
EXPOSE 80

# Arranca Nginx en primer plano
CMD ["nginx", "-g", "daemon off;"]
