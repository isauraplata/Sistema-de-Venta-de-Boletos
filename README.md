# Sistema de Venta de Boletos para Eventos

Este proyecto es un sistema de venta de boletos para eventos que permite gestionar eventos, comprar boletos, y recibir tickets electrónicos con códigos QR. La API está desarrollada utilizando **ASP.NET Web API** y utiliza diversas herramientas y servicios externos para proporcionar una experiencia completa.

## 🛠️ Características Principales

### Gestión de Eventos
- Crear, actualizar, y eliminar eventos
- Configuración de detalles del evento, como nombre, descripción, imagen, fecha, y capacidad

### Gestión de Tickets
- Compra y asignación de boletos a usuarios
- Validación de códigos QR.
- Generación de tickets electrónicos con información personalizada para cada usuario
- Envío automático de tickets por correo electrónico con código QR integrado

## 🚀 Tecnologías Utilizadas

### Backend
- **ASP.NET Web API**: Framework utilizado para crear la API REST
- **MySQL**: Base de datos relacional para almacenar información de eventos, usuarios y boletos

### Servicios Externos
- **PayPal**: Pasarela de pago integrada para procesar transacciones de manera segura
- **Generación de códigos QR**: Genera códigos únicos para cada ticket, utilizados para validar entradas a los eventos
- **Firebase Storage**: Servicio utilizado para almacenar y gestionar imágenes de eventos de manera segura
- **SMTP**: Servicio para envío de correos electrónicos con los tickets

## Requisitos del Sistema

### Software
- .NET 9
- MySQL 8.x

### Claves API
Para utilizar los servicios externos, es necesario configurar las claves de API:
- **PayPal**: Clave y secreto del cliente
- **Firebase**: Archivo `credentials.json` con la configuración del proyecto


## Instalación

### Clonar el repositorio
```bash
git clone https://github.com/isauraplata/Sistema-de-Venta-de-Boletos-.git
```

### Configurar la base de datos
1. Crear una base de datos en MySQL llamada `event_tickets`
2. Ejecutar las migraciones desde el proyecto:
```bash
dotnet ef database update
```

### Configurar Firebase
1. Descargar el archivo `credentials.json` desde la consola de Firebase
2. Colocarlo en el directorio raíz del proyecto

### Instalar dependencias
```bash
dotnet restore
```

### Ejecutar la aplicación
```bash
dotnet run
```