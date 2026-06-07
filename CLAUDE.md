# CLAUDE.md — Sistema de Reservas de Restaurante API
## Instrucciones de desarrollo para Claude Code
> Este archivo es leído automáticamente por Claude Code al abrir el proyecto.
> Implementa el backend fase por fase, en el orden indicado.
> **Nunca saltes una fase. Nunca pases a la siguiente sin que el proyecto compile.**

---

## 🎯 CONTEXTO DEL PROYECTO

**Nombre:** Sistema de Reservas de Restaurante  
**Stack:** C# .NET 8 | Entity Framework Core | AutoMapper | Swagger | SQL Server  
**Arquitectura:** 3 capas — Domain / DataAccess / API  
**Referencia de arquitectura:** SportsLeague API (ITM 2026)  
**Entidades:** Customer, RestaurantTable, MenuItem, Reservation, ReservationMenuItem

### Principios que se deben respetar en CADA archivo generado
- Todas las entidades heredan de `AuditBase`
- Los servicios lanzan solo `KeyNotFoundException` (→ 404) o `InvalidOperationException` (→ 409)
- Los controladores capturan esas excepciones con try/catch y devuelven el código HTTP correcto
- Nunca se exponen entidades directamente — siempre DTOs
- Namespace base del proyecto: **ajustarlo al nombre real de la solución existente**

---

## 📋 TABLA DE FASES

| # | Fase | Commit al finalizar |
|---|------|---------------------|
| 0 | Diagnóstico + .gitignore + estructura | `chore: initial project audit and gitignore` |
| 1 | Entidades + AuditBase + Enums | `feat: add domain entities and enums` |
| 2 | DbContext + Fluent API + Migraciones | `feat: add DbContext and EF Core migrations` |
| 3 | Repositorios genérico + específicos | `feat: add generic and specific repositories` |
| 4 | Servicios con validaciones | `feat: add business logic services with validations` |
| 5 | DTOs + AutoMapper | `feat: add DTOs and AutoMapper mapping profile` |
| 6 | Controladores + Swagger | `feat: add REST controllers and Swagger config` |
| 7 | DataSeeder | `feat: add DataSeeder with initial Colombian data` |
| 8 | Program.cs + README | `chore: finalize DI registration and add README` |
| ✅ | Checklist final | `chore: final validation and cleanup` |

---

## 🔍 FASE 0 — DIAGNÓSTICO + .GITIGNORE + ESTRUCTURA

### Paso 0.1 — Verificar .gitignore

Verifica si existe un `.gitignore` en la raíz de la solución. Si no existe o está incompleto, créalo o complétalo con exactamente este contenido:

```gitignore
# Build output
bin/
obj/

# Visual Studio files
.vs/
*.user
*.suo
*.userosscache
*.sln.docstates

# Rider / JetBrains
.idea/
*.DotSettings.user

# NuGet packages (se restauran con dotnet restore)
packages/
*.nupkg
project.lock.json
project.fragment.lock.json
artifacts/

# Secrets y configuración local — NUNCA al repositorio
appsettings.Development.json
appsettings.Local.json
*.pfx
*.p12
secrets.json
.env
.env.*

# Entity Framework migrations snapshot temp
*.mdf
*.ldf

# OS files
.DS_Store
Thumbs.db
desktop.ini

# Logs
*.log
logs/
```

### Paso 0.2 — Verificar appsettings.json

Asegúrate de que `appsettings.json` (que SÍ va al repo) tenga solo un placeholder para la conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RestauranteDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Verifica que `appsettings.Development.json` esté en `.gitignore`. Si el proyecto usa una cadena de conexión con usuario y contraseña, debe estar en `appsettings.Development.json`, no en `appsettings.json`.

### Paso 0.3 — Diagnóstico del código existente

Analiza todos los archivos del proyecto y genera un reporte con este formato exacto. Para cada punto indica CUMPLE ✅, NO CUMPLE ❌, o PARCIAL ⚠️ y explica qué falta:

```
ESTRUCTURA:
[ ] Existen 3 proyectos: Domain, DataAccess, API
[ ] References correctas: API→Domain, DataAccess→Domain, API→DataAccess
[ ] El proyecto usa .NET 8

ENTIDADES:
[ ] Existe clase AuditBase con Id, CreatedAt, UpdatedAt
[ ] Customer hereda AuditBase y tiene: FirstName, LastName, Email, Phone
[ ] RestaurantTable hereda AuditBase y tiene: Number, Capacity, Status, Location
[ ] MenuItem hereda AuditBase y tiene: Name, Description, Price, Category, IsAvailable
[ ] Reservation hereda AuditBase y tiene: CustomerId, TableId, ReservationDate, PartySize, Status, Notes
[ ] ReservationMenuItem hereda AuditBase y tiene: ReservationId, MenuItemId, Quantity

ENUMS:
[ ] TableStatus: Available=0, Reserved=1, Occupied=2
[ ] ReservationStatus: Pending=0, Confirmed=1, Cancelled=2, Completed=3
[ ] MenuItemCategory: Appetizer=0, MainCourse=1, Dessert=2, Beverage=3

DBCONTEXT:
[ ] RestauranteDbContext tiene los 5 DbSets
[ ] Fluent API: índice único compuesto en ReservationMenuItem(ReservationId, MenuItemId)
[ ] Fluent API: precisión decimal en MenuItem.Price (HasPrecision(10, 2))
[ ] Fluent API: DeleteBehavior.Restrict en Reservation→Table

REPOSITORIOS:
[ ] IGenericRepository<T> con los 5 métodos CRUD
[ ] GenericRepository<T> implementa la interfaz
[ ] 5 interfaces de repositorios específicos
[ ] 5 implementaciones de repositorios específicos

SERVICIOS:
[ ] 5 interfaces de servicios en Domain
[ ] 5 implementaciones de servicios
[ ] Validaciones usan KeyNotFoundException e InvalidOperationException

DTOS + AUTOMAPPER:
[ ] DTOs de entrada (Create/Update) para cada entidad
[ ] DTOs de salida con campos calculados (CustomerName, TableNumber, Subtotal)
[ ] MappingProfile registrado en Program.cs

CONTROLADORES:
[ ] 5 controladores con rutas correctas
[ ] try/catch en cada action: KeyNotFoundException→404, InvalidOperationException→409
[ ] Swagger configurado y funcional

SEEDER:
[ ] DataSeeder existe con datos para las 5 tablas
[ ] Seeder solo siembra si las tablas están vacías
[ ] Seeder se invoca en Program.cs

PROGRAM.CS:
[ ] Todos los repositorios registrados con AddScoped
[ ] Todos los servicios registrados con AddScoped
[ ] CORS configurado para el frontend
```

Al terminar el diagnóstico, lista explícitamente los archivos que hay que crear y los que hay que modificar antes de continuar.

### Commit de fase 0
```bash
dotnet build
git add .
git commit -m "chore: initial project audit and gitignore"
```

---

## 🏗️ FASE 1 — ENTIDADES + AUDITBASE + ENUMS

> Solo implementar lo que el diagnóstico indicó como faltante. No duplicar lo que ya existe.

### Archivos a crear o corregir

**`Domain/Entities/AuditBase.cs`**
```csharp
namespace [Namespace].Domain.Entities;

public abstract class AuditBase
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```

**`Domain/Enums/TableStatus.cs`**
```csharp
namespace [Namespace].Domain.Enums;

public enum TableStatus
{
    Available = 0,
    Reserved = 1,
    Occupied = 2
}
```

**`Domain/Enums/ReservationStatus.cs`**
```csharp
namespace [Namespace].Domain.Enums;

public enum ReservationStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3
}
```

**`Domain/Enums/MenuItemCategory.cs`**
```csharp
namespace [Namespace].Domain.Enums;

public enum MenuItemCategory
{
    Appetizer = 0,
    MainCourse = 1,
    Dessert = 2,
    Beverage = 3
}
```

**`Domain/Entities/Customer.cs`**
```csharp
namespace [Namespace].Domain.Entities;

public class Customer : AuditBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Navigation
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
```

**`Domain/Entities/RestaurantTable.cs`**
> IMPORTANTE: No usar "Table" como nombre de clase porque puede generar conflictos con EF Core
```csharp
namespace [Namespace].Domain.Entities;

public class RestaurantTable : AuditBase
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public TableStatus Status { get; set; } = TableStatus.Available;
    public string? Location { get; set; }

    // Navigation
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
```

**`Domain/Entities/MenuItem.cs`**
```csharp
namespace [Namespace].Domain.Entities;

public class MenuItem : AuditBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public MenuItemCategory Category { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation
    public ICollection<ReservationMenuItem> ReservationMenuItems { get; set; } = new List<ReservationMenuItem>();
}
```

**`Domain/Entities/Reservation.cs`**
```csharp
namespace [Namespace].Domain.Entities;

public class Reservation : AuditBase
{
    public int CustomerId { get; set; }
    public int TableId { get; set; }
    public DateTime ReservationDate { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public RestaurantTable Table { get; set; } = null!;
    public ICollection<ReservationMenuItem> ReservationMenuItems { get; set; } = new List<ReservationMenuItem>();
}
```

**`Domain/Entities/ReservationMenuItem.cs`**
```csharp
namespace [Namespace].Domain.Entities;

public class ReservationMenuItem : AuditBase
{
    public int ReservationId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; } = 1;

    // Navigation
    public Reservation Reservation { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
}
```

### Validación y commit
```bash
dotnet build
# Debe compilar sin errores antes de continuar
git add .
git commit -m "feat: add domain entities and enums"
```

---

## 🗄️ FASE 2 — DBCONTEXT + FLUENT API + MIGRACIONES

**`DataAccess/Context/RestauranteDbContext.cs`**
```csharp
using Microsoft.EntityFrameworkCore;
using [Namespace].Domain.Entities;

namespace [Namespace].DataAccess.Context;

public class RestauranteDbContext : DbContext
{
    public RestauranteDbContext(DbContextOptions<RestauranteDbContext> options)
        : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<RestaurantTable> Tables { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationMenuItem> ReservationMenuItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Índice único compuesto N:M
        modelBuilder.Entity<ReservationMenuItem>()
            .HasIndex(rm => new { rm.ReservationId, rm.MenuItemId })
            .IsUnique();

        // Precisión decimal para precio
        modelBuilder.Entity<MenuItem>()
            .Property(m => m.Price)
            .HasPrecision(10, 2);

        // Restrict delete: una mesa no se puede borrar si tiene reservas
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Table)
            .WithMany(t => t.Reservations)
            .HasForeignKey(r => r.TableId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### Migraciones EF Core

Ejecuta los siguientes comandos desde la raíz de la solución. Ajusta los nombres de proyecto según la estructura real:

```bash
# Verificar que EF Tools esté instalado
dotnet ef --version

# Si no está instalado:
dotnet tool install --global dotnet-ef

# Eliminar migraciones anteriores si existen y están desactualizadas
# (solo si el diagnóstico indicó que las entidades cambiaron)

# Crear migración inicial
dotnet ef migrations add InitialCreate \
  --project [NombreProyecto].DataAccess \
  --startup-project [NombreProyecto].API

# Aplicar migración a la base de datos
dotnet ef database update \
  --project [NombreProyecto].DataAccess \
  --startup-project [NombreProyecto].API
```

Si hay error de conexión a SQL Server, verifica que:
1. SQL Server esté corriendo
2. La cadena de conexión en `appsettings.json` sea correcta
3. El DbContext esté registrado en `Program.cs` (añadir temporalmente si no existe aún)

### Validación y commit
```bash
dotnet build
git add .
git commit -m "feat: add DbContext and EF Core migrations"
```

---

## 📦 FASE 3 — REPOSITORIOS

### Interfaces en Domain

**`Domain/Interfaces/Repositories/IGenericRepository.cs`**
```csharp
namespace [Namespace].Domain.Interfaces.Repositories;

public interface IGenericRepository<T> where T : AuditBase
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

**`Domain/Interfaces/Repositories/ICustomerRepository.cs`**
```csharp
public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
}
```

**`Domain/Interfaces/Repositories/ITableRepository.cs`**
```csharp
public interface ITableRepository : IGenericRepository<RestaurantTable>
{
    Task<IEnumerable<RestaurantTable>> GetAvailableTablesAsync();
    Task<IEnumerable<RestaurantTable>> GetByStatusAsync(TableStatus status);
}
```

**`Domain/Interfaces/Repositories/IMenuItemRepository.cs`**
```csharp
public interface IMenuItemRepository : IGenericRepository<MenuItem>
{
    Task<IEnumerable<MenuItem>> GetByCategoryAsync(MenuItemCategory category);
    Task<IEnumerable<MenuItem>> GetAvailableAsync();
}
```

**`Domain/Interfaces/Repositories/IReservationRepository.cs`**
```csharp
public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Reservation>> GetByTableIdAsync(int tableId);
    Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date);
    Task<Reservation?> GetWithDetailsAsync(int id); // Include: Customer, Table, ReservationMenuItems→MenuItem
}
```

**`Domain/Interfaces/Repositories/IReservationMenuItemRepository.cs`**
```csharp
public interface IReservationMenuItemRepository : IGenericRepository<ReservationMenuItem>
{
    Task<IEnumerable<ReservationMenuItem>> GetByReservationAsync(int reservationId);
    Task<bool> ExistsByReservationAndMenuItemAsync(int reservationId, int menuItemId);
}
```

### Implementaciones en DataAccess

**`DataAccess/Repositories/GenericRepository.cs`**
```csharp
public class GenericRepository<T> : IGenericRepository<T> where T : AuditBase
{
    protected readonly RestauranteDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(RestauranteDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(int id) =>
        await _dbSet.FindAsync(id);

    public async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Record with ID {id} not found.");
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
```

Implementa los 5 repositorios específicos heredando de `GenericRepository<T>`.
Usa `Include()` y `ThenInclude()` de EF Core donde sea necesario para cargar navegaciones.
El método `GetWithDetailsAsync` en `ReservationRepository` debe cargar:
- `Customer`
- `Table`
- `ReservationMenuItems` → `MenuItem`

### Validación y commit
```bash
dotnet build
git add .
git commit -m "feat: add generic and specific repositories"
```

---

## ⚙️ FASE 4 — SERVICIOS CON VALIDACIONES

### Regla de errores en TODOS los servicios
```csharp
// Entidad no encontrada → Controller la convierte en 404
throw new KeyNotFoundException($"Customer with ID {id} not found.");

// Regla de negocio violada → Controller la convierte en 409
throw new InvalidOperationException("A customer with this email already exists.");
```

### Interfaces en Domain/Interfaces/Services

Crea las 5 interfaces correspondientes antes de las implementaciones:
- `ICustomerService`, `ITableService`, `IMenuItemService`, `IReservationService`, `IReservationMenuItemService`

### CustomerService — Validaciones
- `CreateAsync`: si ya existe un cliente con el mismo email → `InvalidOperationException`
- `UpdateAsync`: si el cliente no existe → `KeyNotFoundException`
- `DeleteAsync`: si el cliente no existe → `KeyNotFoundException`

### TableService — Validaciones
- `CreateAsync`: si ya existe una mesa con el mismo número → `InvalidOperationException`
- `DeleteAsync`: si la mesa tiene reservas con status `Pending` o `Confirmed` → `InvalidOperationException("Cannot delete a table with active reservations.")`

### MenuItemService — Validaciones
- `CreateAsync`: si ya existe un ítem con el mismo nombre → `InvalidOperationException`

### ReservationService — Validaciones completas

**Al crear una reserva (`CreateAsync`):**
```
V1: Customer debe existir         → KeyNotFoundException
V2: Table debe existir            → KeyNotFoundException
V3: PartySize <= Table.Capacity   → InvalidOperationException("Party size exceeds table capacity.")
V4: La mesa no debe tener otra reserva Confirmed o Pending en la misma fecha
    → InvalidOperationException("Table is not available on the requested date.")
```

**Máquina de estados (`ChangeStatusAsync`):**
```
Transiciones válidas:
Pending   → Confirmed  ✅  + Table.Status = Reserved
Pending   → Cancelled  ✅  + Table.Status = Available
Confirmed → Completed  ✅  + Table.Status = Available
Confirmed → Cancelled  ✅  + Table.Status = Available

Transiciones inválidas (lanzar InvalidOperationException):
Completed → cualquier estado ❌  "Reservation is already completed."
Cancelled → cualquier estado ❌  "Reservation is already cancelled."
Cualquier otro cambio no listado ❌  "Invalid status transition."
```

### ReservationMenuItemService — Validaciones
```
V1: Reserva debe existir                              → KeyNotFoundException
V2: Reserva debe estar Pending o Confirmed            → InvalidOperationException("Cannot modify items on a non-active reservation.")
V3: MenuItem debe existir                             → KeyNotFoundException
V4: MenuItem debe tener IsAvailable = true            → InvalidOperationException("Menu item is not available.")
V5: El mismo ítem no puede agregarse dos veces        → InvalidOperationException("This menu item is already in the reservation.")
```

### Validación y commit
```bash
dotnet build
git add .
git commit -m "feat: add business logic services with validations"
```

---

## 🔄 FASE 5 — DTOS + AUTOMAPPER

### DTOs de entrada (API/DTOs/)

**Customer:**
```csharp
// CreateCustomerDto.cs
public class CreateCustomerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
```

**RestaurantTable:**
```csharp
// CreateTableDto.cs
public class CreateTableDto
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Location { get; set; }
}

// UpdateTableDto.cs
public class UpdateTableDto : CreateTableDto
{
    public TableStatus Status { get; set; }
}
```

**MenuItem:**
```csharp
// CreateMenuItemDto.cs
public class CreateMenuItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public MenuItemCategory Category { get; set; }
    public bool IsAvailable { get; set; } = true;
}
```

**Reservation:**
```csharp
// CreateReservationDto.cs
public class CreateReservationDto
{
    public int CustomerId { get; set; }
    public int TableId { get; set; }
    public DateTime ReservationDate { get; set; }
    public int PartySize { get; set; }
    public string? Notes { get; set; }
}

// UpdateReservationDto.cs
public class UpdateReservationDto
{
    public DateTime ReservationDate { get; set; }
    public int PartySize { get; set; }
    public string? Notes { get; set; }
}
```

**ReservationMenuItem:**
```csharp
// AddMenuItemDto.cs
public class AddMenuItemDto
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; } = 1;
}
```

### DTOs de salida

```csharp
// CustomerDto.cs
public class CustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}

// TableDto.cs
public class TableDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; }   // enum → string
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
}

// MenuItemDto.cs
public class MenuItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }   // enum → string
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ReservationDto.cs
public class ReservationDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }    // FirstName + " " + LastName
    public int TableId { get; set; }
    public int TableNumber { get; set; }        // Table.Number
    public int TableCapacity { get; set; }      // Table.Capacity
    public DateTime ReservationDate { get; set; }
    public int PartySize { get; set; }
    public string Status { get; set; }          // enum → string
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ReservationDetailDto.cs — incluye ítems del pedido
public class ReservationDetailDto : ReservationDto
{
    public List<ReservationMenuItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }    // suma de subtotales
}

// ReservationMenuItemDto.cs
public class ReservationMenuItemDto
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }       // Price * Quantity
}
```

### MappingProfile.cs

```csharp
// API/Mappings/MappingProfile.cs
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Customer
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<Customer, CustomerDto>();

        // RestaurantTable
        CreateMap<CreateTableDto, RestaurantTable>();
        CreateMap<UpdateTableDto, RestaurantTable>();
        CreateMap<RestaurantTable, TableDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // MenuItem
        CreateMap<CreateMenuItemDto, MenuItem>();
        CreateMap<MenuItem, MenuItemDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()));

        // Reservation
        CreateMap<CreateReservationDto, Reservation>();
        CreateMap<UpdateReservationDto, Reservation>();
        CreateMap<Reservation, ReservationDto>()
            .ForMember(d => d.CustomerName,
                o => o.MapFrom(s => s.Customer.FirstName + " " + s.Customer.LastName))
            .ForMember(d => d.TableNumber,
                o => o.MapFrom(s => s.Table.Number))
            .ForMember(d => d.TableCapacity,
                o => o.MapFrom(s => s.Table.Capacity))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => s.Status.ToString()));
        CreateMap<Reservation, ReservationDetailDto>()
            .IncludeBase<Reservation, ReservationDto>()
            .ForMember(d => d.Items,
                o => o.MapFrom(s => s.ReservationMenuItems))
            .ForMember(d => d.TotalAmount,
                o => o.MapFrom(s => s.ReservationMenuItems
                    .Sum(ri => ri.MenuItem.Price * ri.Quantity)));

        // ReservationMenuItem
        CreateMap<AddMenuItemDto, ReservationMenuItem>();
        CreateMap<ReservationMenuItem, ReservationMenuItemDto>()
            .ForMember(d => d.MenuItemName,
                o => o.MapFrom(s => s.MenuItem.Name))
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.MenuItem.Price))
            .ForMember(d => d.Subtotal,
                o => o.MapFrom(s => s.MenuItem.Price * s.Quantity));
    }
}
```

### Validación y commit
```bash
dotnet build
git add .
git commit -m "feat: add DTOs and AutoMapper mapping profile"
```

---

## 🎮 FASE 6 — CONTROLADORES + SWAGGER

### Patrón obligatorio en CADA action de CADA controller

```csharp
try
{
    // lógica
}
catch (KeyNotFoundException ex)
{
    return NotFound(new { message = ex.Message });
}
catch (InvalidOperationException ex)
{
    return Conflict(new { message = ex.Message });
}
```

### CustomerController.cs — `[Route("api/Customer")]`

```
GET    /api/Customer          → GetAll()                → 200
GET    /api/Customer/{id}     → GetById(int id)          → 200 | 404
POST   /api/Customer          → Create(CreateCustomerDto)→ 201
PUT    /api/Customer/{id}     → Update(int id, dto)      → 200 | 404 | 409
DELETE /api/Customer/{id}     → Delete(int id)           → 204 | 404
```

POST devuelve `CreatedAtAction(nameof(GetById), new { id = result.Id }, result)`.

### TableController.cs — `[Route("api/Table")]`

```
GET    /api/Table             → GetAll()                    → 200
GET    /api/Table/{id}        → GetById(int id)              → 200 | 404
GET    /api/Table/available   → GetAvailable()               → 200
POST   /api/Table             → Create(CreateTableDto)       → 201 | 409
PUT    /api/Table/{id}        → Update(int id, UpdateTableDto)→ 200 | 404 | 409
DELETE /api/Table/{id}        → Delete(int id)               → 204 | 404 | 409
```

> ⚠️ La ruta `/api/Table/available` debe declararse ANTES de `/api/Table/{id}` para que ASP.NET Core no lo interprete como un id.

### MenuItemController.cs — `[Route("api/MenuItem")]`

```
GET    /api/MenuItem                   → GetAll()           → 200
GET    /api/MenuItem/{id}              → GetById(int id)    → 200 | 404
GET    /api/MenuItem/category/{cat}    → GetByCategory(MenuItemCategory cat) → 200
POST   /api/MenuItem                   → Create(dto)        → 201 | 409
PUT    /api/MenuItem/{id}              → Update(int id, dto)→ 200 | 404
DELETE /api/MenuItem/{id}              → Delete(int id)     → 204 | 404
```

### ReservationController.cs — `[Route("api/Reservation")]`

```
GET    /api/Reservation                        → GetAll()              → 200
GET    /api/Reservation/{id}                   → GetWithDetails(id)    → 200 | 404
GET    /api/Reservation/customer/{customerId}  → GetByCustomer(id)     → 200
GET    /api/Reservation/date/{date}            → GetByDate(DateTime)   → 200
POST   /api/Reservation                        → Create(dto)           → 201 | 404 | 409
PUT    /api/Reservation/{id}                   → Update(int id, dto)   → 200 | 404
DELETE /api/Reservation/{id}                   → Delete(int id)        → 204 | 404
PATCH  /api/Reservation/{id}/status            → ChangeStatus(int id, [FromBody] ReservationStatus newStatus) → 200 | 404 | 409
```

### ReservationMenuItemController.cs — `[Route("api/reservation/{reservationId}/items")]`

```
GET    /api/reservation/{reservationId}/items              → GetItems(int reservationId) → 200
POST   /api/reservation/{reservationId}/items              → AddItem(int reservationId, AddMenuItemDto dto) → 201 | 404 | 409
DELETE /api/reservation/{reservationId}/items/{menuItemId} → RemoveItem(int reservationId, int menuItemId) → 204 | 404 | 409
```

### Configuración Swagger en Program.cs

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Restaurante API",
        Version = "v1",
        Description = "Sistema de Reservas de Restaurante — ITM 2026"
    });
});

// En el pipeline (desarrollo):
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurante API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}
```

Después de implementar todos los controladores, corre el proyecto y verifica que Swagger cargue sin errores y muestre todos los endpoints.

### Validación y commit
```bash
dotnet build
dotnet run --project [NombreProyecto].API &
# Verifica que Swagger abra en https://localhost:{port}
# Luego detén el servidor
git add .
git commit -m "feat: add REST controllers and Swagger config"
```

---

## 🌱 FASE 7 — DATASEEDER

**`DataAccess/Seeders/DataSeeder.cs`**

El seeder SOLO inserta datos si las tablas están vacías. Usa `_context.Customers.Any()` antes de sembrar.

### Datos mínimos requeridos

**Customers (mínimo 10)** — nombres colombianos:
```
Carlos Andrés Gómez Restrepo | carlos.gomez@email.com | 3001234567
María Paula Hernández Vélez  | maria.hernandez@email.com | 3112345678
Juan David Martínez López    | juan.martinez@email.com | 3223456789
Ana Sofía Rodríguez Castaño  | ana.rodriguez@email.com | 3334567890
Luis Fernando Pérez Moreno   | luis.perez@email.com | 3445678901
Valentina Torres Quintero    | valentina.torres@email.com | 3556789012
Sebastián Ramírez Osorio     | sebastian.ramirez@email.com | 3667890123
Daniela Vargas Giraldo       | daniela.vargas@email.com | 3778901234
Andrés Felipe Castro Salazar | andres.castro@email.com | 3889012345
Camila Andrea Díaz Muñoz     | camila.diaz@email.com | 3990123456
```

**RestaurantTables (mínimo 8)**:
```
Mesa 1: Capacity=2, Location="Íntimo",          Status=Available
Mesa 2: Capacity=2, Location="Íntimo",          Status=Available
Mesa 3: Capacity=4, Location="Salón Principal", Status=Available
Mesa 4: Capacity=4, Location="Salón Principal", Status=Reserved  ← tiene reserva Confirmed
Mesa 5: Capacity=4, Location="Salón Principal", Status=Available
Mesa 6: Capacity=6, Location="Terraza",         Status=Available
Mesa 7: Capacity=6, Location="Terraza",         Status=Reserved  ← tiene reserva Confirmed
Mesa 8: Capacity=10, Location="Salón VIP",      Status=Available
```

**MenuItems (mínimo 12)**:
```
APPETIZERS:
- Patacones con hogao           | $12.000 | Available
- Empanadas de pipián (3 und.)  | $9.000  | Available
- Arepa de chócolo con quesito  | $8.000  | Available

MAIN COURSE:
- Bandeja paisa completa        | $38.000 | Available
- Sancocho de gallina           | $28.000 | Available
- Ajiaco santafereño            | $25.000 | Available
- Cazuela de mariscos           | $35.000 | Available

DESSERTS:
- Arroz con leche               | $8.000  | Available
- Tres leches artesanal         | $12.000 | Available
- Postre de natas               | $9.000  | Available

BEVERAGES:
- Limonada de coco              | $8.000  | Available
- Jugo natural de lulo          | $7.000  | Available
- Agua panela con limón         | $5.000  | Available
```

**Reservations (mínimo 15)** — variedad de estados:
```
- 3 reservas con Status=Confirmed  → sus mesas deben tener Status=Reserved
- 3 reservas con Status=Completed
- 2 reservas con Status=Cancelled
- El resto en Status=Pending
```

**ReservationMenuItems** — asociar ítems a las reservas Confirmed y Completed.

### Invocación del seeder en Program.cs
```csharp
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}
```

### Validación y commit
```bash
dotnet build
dotnet run --project [NombreProyecto].API
# Verifica en SQL Server que las tablas tienen datos
# Verifica que al correr por segunda vez NO duplica datos
git add .
git commit -m "feat: add DataSeeder with initial Colombian data"
```

---

## ⚙️ FASE 8 — PROGRAM.CS COMPLETO + README

### Program.cs — orden de registro

```csharp
// 1. DbContext
builder.Services.AddDbContext<RestauranteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 3. Repositorios
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationMenuItemRepository, ReservationMenuItemRepository>();

// 4. Servicios
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationMenuItemService, ReservationMenuItemService>();

// 5. DataSeeder
builder.Services.AddScoped<DataSeeder>();

// 6. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Restaurante API",
        Version = "v1",
        Description = "Sistema de Reservas de Restaurante — ITM 2026"
    });
});

// 7. CORS — necesario para el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// 8. Migraciones automáticas al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RestauranteDbContext>();
    db.Database.Migrate();
}

// 9. DataSeeder al iniciar
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurante API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### README.md

Crea un `README.md` en la raíz de la solución con esta estructura:

```markdown
# Sistema de Reservas de Restaurante API

## Integrantes
- [Nombre 1]
- [Nombre 2]
- [Nombre 3]

## Descripción
API REST para gestión de reservas de restaurante. Permite administrar clientes, mesas, 
menú e ítems de pedido asociados a reservas.

## Tecnologías
- .NET 8
- Entity Framework Core (Code First)
- AutoMapper
- Swagger / OpenAPI
- SQL Server

## Instrucciones de ejecución

### Prerrequisitos
- .NET 8 SDK
- SQL Server (local o Express)
- dotnet-ef tools (`dotnet tool install --global dotnet-ef`)

### Pasos
1. Clona el repositorio
2. Abre `appsettings.json` y verifica que la cadena de conexión apunte a tu SQL Server
3. Ejecuta: `dotnet run --project [NombreProyecto].API`
4. La base de datos se crea y puebla automáticamente
5. Abre `https://localhost:{port}` para acceder a Swagger

## Endpoints principales
- `GET  /api/Reservation` — listar reservas
- `POST /api/Reservation` — crear reserva
- `PATCH /api/Reservation/{id}/status` — cambiar estado
- `POST /api/reservation/{id}/items` — agregar ítem al pedido
- Ver Swagger para el listado completo
```

### Validación y commit
```bash
dotnet build
dotnet run --project [NombreProyecto].API
git add .
git commit -m "chore: finalize DI registration and add README"
```

---

## ✅ CHECKLIST FINAL DE VALIDACIÓN

Ejecuta este checklist completo. Para cada punto: CUMPLE ✅ o FALLA ❌ con descripción del error.

### Criterio 1 — Entidades y relaciones (1.0 punto)
```
[ ] 5 entidades heredan de AuditBase
[ ] Relación 1:N: Customer → Reservation (con FK CustomerId)
[ ] Relación 1:N: RestaurantTable → Reservation (con FK TableId y Restrict)
[ ] Relación N:M: Reservation ↔ MenuItem vía ReservationMenuItem
[ ] TableStatus con 3 valores, ReservationStatus con 4, MenuItemCategory con 4
[ ] DbContext tiene los 5 DbSets correctamente nombrados
[ ] Fluent API: índice único compuesto en ReservationMenuItem
[ ] Fluent API: HasPrecision(10, 2) en MenuItem.Price
[ ] Fluent API: DeleteBehavior.Restrict en Reservation→Table
[ ] Migración generada y base de datos creada sin errores
```

### Criterio 2 — Repository + Service (0.8 puntos)
```
[ ] IGenericRepository<T> con 5 métodos en Domain
[ ] GenericRepository<T> implementa la interfaz en DataAccess
[ ] 5 interfaces de repositorios específicos heredan IGenericRepository<T>
[ ] 5 implementaciones usan Include() donde es necesario
[ ] CustomerService: valida email duplicado
[ ] TableService: valida número de mesa duplicado y reservas activas al eliminar
[ ] ReservationService: 4 validaciones al crear (V1-V4)
[ ] ReservationService: máquina de estados con 4 transiciones válidas
[ ] ReservationMenuItemService: 5 validaciones al agregar
[ ] Todos los servicios usan KeyNotFoundException e InvalidOperationException exclusivamente
```

### Criterio 3 — DTOs + Controllers (0.8 puntos)
```
[ ] CreateDto para cada entidad (sin Id ni fechas de auditoría)
[ ] Dto de salida con campos calculados (CustomerName, TableNumber, Subtotal, TotalAmount)
[ ] MappingProfile mapea enums a string con .ToString()
[ ] MappingProfile mapea CustomerName desde Customer.FirstName + " " + Customer.LastName
[ ] 5 controladores con sus rutas correctas
[ ] TODOS los actions tienen try/catch con KeyNotFoundException→404 e InvalidOperationException→409
[ ] GET devuelve 200, POST devuelve 201 con CreatedAtAction, DELETE devuelve 204
[ ] Swagger abre sin errores y muestra todos los endpoints
[ ] Al menos un endpoint de cada controller probado desde Swagger
```

### Criterio 5 — DataSeeder + Migraciones (0.4 puntos)
```
[ ] DataSeeder verifica Any() antes de sembrar (no duplica datos)
[ ] Mínimo: 10 clientes, 8 mesas, 13 ítems de menú, 15 reservas
[ ] ReservationMenuItems asociados a reservas Confirmed y Completed
[ ] Mesas con Status=Reserved corresponden a reservas Confirmed
[ ] db.Database.Migrate() en Program.cs (crea BD automáticamente al clonar)
[ ] Seeder se invoca automáticamente en Program.cs
[ ] README.md con instrucciones claras para clonar y ejecutar
[ ] .gitignore cubre bin/, obj/, appsettings.Development.json
```

### Prueba de humo final
```bash
# 1. Eliminar la base de datos
# 2. Correr el proyecto desde cero
dotnet run --project [NombreProyecto].API
# 3. Verificar en Swagger:
#    - POST /api/Reservation con CustomerId y TableId válidos → 201
#    - PATCH /api/Reservation/{id}/status con "Confirmed" → 200
#    - POST /api/reservation/{id}/items con un MenuItemId → 201
#    - GET /api/Reservation/{id} → debe mostrar CustomerName, TableNumber, Items y TotalAmount
#    - PATCH /api/Reservation/{id}/status con "Completed" → 200
#    - PATCH /api/Reservation/{id}/status con "Confirmed" (desde Completed) → 409
```

### Commit final
```bash
git add .
git commit -m "chore: final validation and cleanup"
git push origin main
```

---

## 📋 TABLA DE ENDPOINTS COMPLETA (referencia)

```
── CUSTOMERS ──────────────────────────────────────────────
GET    /api/Customer
GET    /api/Customer/{id}
POST   /api/Customer                          → 201
PUT    /api/Customer/{id}                     → 200
DELETE /api/Customer/{id}                     → 204

── TABLES ─────────────────────────────────────────────────
GET    /api/Table
GET    /api/Table/{id}
GET    /api/Table/available
POST   /api/Table                             → 201
PUT    /api/Table/{id}                        → 200
DELETE /api/Table/{id}                        → 204

── MENU ITEMS ─────────────────────────────────────────────
GET    /api/MenuItem
GET    /api/MenuItem/{id}
GET    /api/MenuItem/category/{category}
POST   /api/MenuItem                          → 201
PUT    /api/MenuItem/{id}                     → 200
DELETE /api/MenuItem/{id}                     → 204

── RESERVATIONS ───────────────────────────────────────────
GET    /api/Reservation
GET    /api/Reservation/{id}
GET    /api/Reservation/customer/{customerId}
GET    /api/Reservation/date/{date}
POST   /api/Reservation                       → 201
PUT    /api/Reservation/{id}                  → 200
DELETE /api/Reservation/{id}                  → 204
PATCH  /api/Reservation/{id}/status           → 200

── RESERVATION ITEMS ──────────────────────────────────────
GET    /api/reservation/{reservationId}/items
POST   /api/reservation/{reservationId}/items              → 201
DELETE /api/reservation/{reservationId}/items/{menuItemId} → 204
```

## 📊 CÓDIGOS HTTP DEL PROYECTO

| Código | Cuándo se usa |
|--------|---------------|
| 200 | GET o PATCH exitoso |
| 201 | POST exitoso (con CreatedAtAction) |
| 204 | DELETE exitoso |
| 404 | KeyNotFoundException — entidad no existe |
| 409 | InvalidOperationException — regla de negocio violada |

---

*CLAUDE.md — Sistema de Reservas de Restaurante — ITM 2026*  
*Referencia: SportsLeague API Architecture*
