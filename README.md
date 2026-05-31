# Sistema de Reservas de Restaurante API

## Integrantes
- [Nombre 1]
- [Nombre 2]
- [Nombre 3]

## Descripción
API REST para gestión de reservas de restaurante desarrollada con .NET 8.
Permite administrar clientes, mesas, menú y reservas con pedidos asociados.

## Tecnologías
- .NET 8 Web API
- Entity Framework Core 8 (Code First)
- AutoMapper 16.x
- Swagger / OpenAPI 3.0
- SQL Server

## Arquitectura
Solución en 3 capas:
- `RestauranteAPI.Domain` — Entidades, enums, interfaces, servicios con lógica de negocio
- `RestauranteAPI.DataAccess` — DbContext, repositorios, migraciones, seeder
- `RestauranteAPI.API` — Controladores, DTOs, mappings, Program.cs

## Requisitos previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local, Express, o Developer Edition)
- `dotnet-ef` tools: `dotnet tool install --global dotnet-ef`

## Instrucciones de ejecución

### 1. Clonar el repositorio
```bash
git clone [URL_DEL_REPOSITORIO]
cd RestauranteAPI
```

### 2. Configurar la cadena de conexión
Abrir `RestauranteAPI.API/appsettings.json` y verificar:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RestauranteDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
Si tu SQL Server usa usuario y contraseña, usar:
`Server=localhost;Database=RestauranteDB;User Id=sa;Password=TuPassword;TrustServerCertificate=True;`

### 3. Ejecutar
```bash
dotnet run --project RestauranteAPI.API
```
La base de datos se crea y puebla automáticamente al iniciar.

### 4. Abrir Swagger
Navegar a `http://localhost:{puerto}` — Swagger UI carga en la raíz.

## Endpoints principales

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /api/Customer | Listar clientes |
| POST | /api/Reservation | Crear reserva |
| PATCH | /api/Reservation/{id}/status | Cambiar estado |
| POST | /api/reservation/{id}/items | Agregar ítem al pedido |
| GET | /api/Reservation/{id} | Ver detalle con items y total |
| GET | /api/Table/available | Mesas disponibles |
| GET | /api/MenuItem/category/{cat} | Menú por categoría |

Ver Swagger para el listado completo de los 28 endpoints.

## Estados de reserva

```
Pending → Confirmed   ✅  (mesa pasa a Reserved)
Pending → Cancelled   ✅  (mesa vuelve a Available)
Confirmed → Completed ✅  (mesa vuelve a Available)
Confirmed → Cancelled ✅  (mesa vuelve a Available)
Completed → cualquiera ❌  409 Conflict
Cancelled → cualquiera ❌  409 Conflict
```

## Códigos HTTP

| Código | Cuándo |
|--------|--------|
| 200 | GET o PATCH exitoso |
| 201 | POST exitoso (con Location header) |
| 204 | DELETE exitoso |
| 404 | Entidad no encontrada |
| 409 | Regla de negocio violada |

## Datos iniciales (DataSeeder)
Al iniciar por primera vez se insertan automáticamente:
- 10 clientes colombianos
- 8 mesas (2 íntimas, 3 salón principal, 2 terraza, 1 VIP)
- 13 ítems de menú (3 entradas, 4 platos fuertes, 3 postres, 3 bebidas)
- 15 reservas (3 Confirmed, 3 Completed, 2 Cancelled, 7 Pending)
- 18 ítems de pedido asociados a reservas activas y completadas

---

## Frontend (Angular)

### Tecnologías
- Angular 21 (Standalone Components)
- Angular Material 21
- TypeScript

### Instrucciones de ejecución del frontend

**Prerrequisitos:** Node.js 18+ y Angular CLI (`npm install -g @angular/cli`)

1. Asegúrate de que el backend esté corriendo en `http://localhost:5127`
2. En una terminal separada:
```bash
cd restaurante-frontend
npm install
ng serve
```
3. Abre `http://localhost:4200`

### Vistas disponibles

| Ruta | Vista | Descripción |
|------|-------|-------------|
| `/dashboard` | Dashboard | Resumen con métricas en tiempo real |
| `/reservations` | Listado | Reservaciones con filtro por estado |
| `/reservations/new` | Nueva reserva | Formulario con validación y dropdowns |
| `/reservations/:id` | Detalle | Estado, pedido, agregar ítems, totales |
| `/menu` | Menú | Catálogo con filtro por categoría |
