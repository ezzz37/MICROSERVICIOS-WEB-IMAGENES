# MICROSERVICIOS-WEB-IMAGENES
## Descripción

Este proyecto implementa un conjunto de microservicios para la gestión de imágenes en aplicaciones web. Permite cargar, procesar y almacenar imágenes de manera eficiente, utilizando tecnologías modernas y escalables.

## Características

- **Carga de imágenes**: Permite subir imágenes desde el cliente.
- **Procesamiento**: Redimensionamiento, compresión y conversión de formatos.
- **Almacenamiento**: Integración con servicios de almacenamiento en la nube.
- **API REST**: Interfaz para interactuar con los microservicios.
- **Escalabilidad**: Diseñado para manejar grandes volúmenes de datos.

## Requisitos

- **Node.js**: Versión 16 o superior.
- **Docker**: Para la ejecución de los contenedores.
- **Base de datos**: MongoDB o PostgreSQL.
- **Servicio de almacenamiento**: AWS S3, Google Cloud Storage o similar.

## Instalación

1. Clona el repositorio:
    ```bash
    git clone https://github.com/tu-usuario/microservicios-web-imagenes.git
    cd microservicios-web-imagenes
    ```

2. Instala las dependencias:
    ```bash
    npm install
    ```

3. Configura las variables de entorno:
    Crea un archivo `.env` con las siguientes variables:
    ```
    DB_HOST=localhost
    DB_PORT=5432
    STORAGE_BUCKET=my-bucket
    ```

4. Inicia los servicios:
    ```bash
    docker-compose up
    ```

## Uso

1. **Subir una imagen**:
    Realiza una solicitud POST al endpoint `/upload` con el archivo en el cuerpo de la solicitud.

2. **Procesar una imagen**:
    Utiliza el endpoint `/process` para aplicar transformaciones como redimensionamiento.

3. **Obtener una imagen**:
    Realiza una solicitud GET al endpoint `/images/:id` para recuperar una imagen almacenada.

## Contribución

1. Haz un fork del repositorio.
2. Crea una rama para tu funcionalidad:
    ```bash
    git checkout -b nueva-funcionalidad
    ```
3. Realiza tus cambios y crea un pull request.
