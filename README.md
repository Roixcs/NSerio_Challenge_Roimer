# Team Tasks Dashboard

Aplicacion web para gestionar proyectos, tareas y desarrolladores, con visualizacion de carga de trabajo, estado de proyectos y prediccion de riesgo de retraso.

Prueba tecnica de Roimer Esteban Ortiz
---

## Tabla de Contenido

- [Arquitectura](#arquitectura)
- [Tecnologias y Versiones](#tecnologias-y-versiones)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Supuestos y Decisiones de Diseno](#supuestos-y-decisiones-de-diseno)
- [Ejecucion con Docker](#ejecucion-con-docker-recomendado)
- [Ejecucion Manual](#ejecucion-manual)
- [Endpoints de la API](#endpoints-de-la-api)
- [Pruebas Unitarias](#pruebas-unitarias)

---

## Arquitectura

### Backend - Clean Architecture (.NET 8)

```
TeamTaskAPI (API Layer)
    --> TeamTasks.Application (Business Logic(Services), DTOs, Validators)
        --> TeamTasks.Domain (Entities, Enums, Interfaces)
    --> TeamTasks.Infrastructure (EF Core, Dapper, Repositories)
        --> TeamTasks.Domain
        --> TeamTasks.Application
```



- **Domain**: Entidades, enums y contratos (interfaces de repositorios). Sin dependencias externas.
- **Application**: Servicios, DTOs, validacion con FluentValidation. Patron `Result<T>` para estandarizar las respuesta de la API y le FrontEnd las valide mejor.
- **Infrastructure**: Implementacion de repositorios con EF Core (CRUD) y Dapper (Dapper y EF Core para tener lo mejor de los 2 ORM: consultas complejas, vistas SQL, stored procedures).
- **API**: Controllers, Middleware de excepciones, configuracion de CORS y Swagger.

### Frontend - Angular 18 (Standalone Components)

```
src/app/
    core/services/          --> Servicios HTTP (ApiService, TaskService, etc.)
    features/
        dashboard/          --> Vista principal con 3 tablas de metricas
        projects/           --> Vista de tareas por proyecto y grafico sencillo (ngx-chars)
        tasks/              --> Formulario de New Task y dialog de detalle
    shared/       --> Los reutilizables
        components/         --> DataTableComponent 
        pipes/              --> StatusBadgePipe
        directives/         --> HighlightRiskDirective
    models/                 --> Interfaces TypeScript
```

### Base de Datos - SQL Server

- Esquema `TaskManagement` con tablas: `Developers`, `Projects`, `Tasks`.
- 4 Vistas SQL: `vw_DeveloperWorkload`, `vw_ProjectHealth`, `vw_TasksDueSoon`, `vw_DeveloperDelayRisk`.
- Stored Procedures: `sp_InsertTask`, `sp_UpdateTaskStatus`.
- Datos poblados: 5 desarrolladores, 3 proyectos, 20 tareas.

---

## Tecnologias y Versiones

### Backend

| Paquete | Version |
|---|---|
| .NET SDK | 8.0 |
| Entity Framework Core (SQL Server) | 9.0.0 |
| Dapper | 2.1.66 |
| FluentValidation | 12.1.1 |
| Microsoft.Data.SqlClient | 5.1.4 |
| Swashbuckle (Swagger) | 6.6.2 |
| xUnit | 2.9.2 |
| Moq | 4.20.72 |
| FluentAssertions | 8.8.0 |

### Frontend

| Paquete | Version |
|---|---|
| Angular | 18.2.x |
| Angular Material | 18.2.14 |
| Angular CDK | 18.2.14 |
| @swimlane/ngx-charts | 23.1.0 |
| TypeScript | 5.5.2 |
| RxJS | 7.8.x |
| Node.js | 20.x | (el recomendado)

### Infraestructura

| Herramienta | Version |
|---|---|
| SQL Server | 2022 |
| Docker Compose | 3.8 |
| nginx | alpine (la ultima) |

---

## Estructura del Proyecto

```
NSerio_Challenge_Roimer/
|-- DBSetup_TeamTasks.sql           # Script completo de base de datos
|-- Dockerfile                      # Dockerfile del backend (.NET)
|-- docker-compose.yml              # Orquestacion de todos los servicios
|-- README.md
|-- BackEnd/
|   |-- TeamTaskAPI/
|       |-- TeamTaskAPI.sln
|       |-- TeamTaskAPI/            # API Layer (Controllers, Middlewares)
|       |-- TeamTasks.Domain/       # Entities, Enums, Interfaces
|       |-- TeamTasks.Application/  # Services, DTOs, Validators
|       |-- TeamTasks.Infrastructure/ # DbContext, Repositories (EF + Dapper)
|       |-- TeamTasks.Tests/        # Unit Tests (xUnit + Moq)
|-- FrontEnd/
    |-- Dockerfile                  # Dockerfile del frontend (Node + nginx)
    |-- nginx.conf                  # Configuracion nginx (SPA + reverse proxy)
    |-- team-tasks-app/             # Proyecto Angular 18
```

---

## Supuestos y Decisiones de Diseno

### Base de Datos

1. **HighRiskFlag**: La prueba define `HighRiskFlag = 1` cuando `PredictedCompletionDate > LatestDueDate` o `AvgDelayDays > 3`. Se ajusto a `AvgDelayDays > 0` para que sea mas visible en los datos semilla, ya que con solo 20 tareas pocos desarrolladores superarian el umbral de 3 dias.

2. **PredictedCompletionDate**: Se calcula como `LatestDueDate + AvgDelayDays` del historial de tareas completadas del desarrollador. Si no tiene historial de retrasos, se usa la `LatestDueDate` directamente.

3. **Esquema TaskManagement**: Se uso un esquema dedicado en lugar del esquema `dbo` por defecto, para mejor organizacion y separacion de responsabilidades en la base de datos.

4. **Stored Procedures**: `sp_InsertTask` incluye validaciones de negocio (existencia de ProjectId, AssigneeId activo, campos requeridos) y retorna mensajes detallados en caso de error. `sp_UpdateTaskStatus` permite actualizar status, priority y complexity.

**Nota**: Opté por usar IDENTITY para claves primarias por su simplicidad y performance en este escenario de prueba. Se podría usar GUID/ULID.

### Backend

5. **Hibrido EF Core + Dapper**: EF Core se usa para operaciones CRUD con entidades y relaciones. Dapper se usa para consultas complejas (vistas SQL del Dashboard) y ejecucion de stored procedures, optimizando rendimiento en lecturas.

6. **Patron Result\<T\>**: Todas las respuestas de la API estan envueltas en `Result<T>` con propiedades `IsSuccess`, `Data` y `ErrorMessage`. Esto estandariza el manejo de errores entre frontend y backend.

7. **ExceptionHandlingMiddleware**: Captura excepciones de FluentValidation y excepciones generales, retornando siempre el formato `Result<T>`.

8. **Swagger siempre habilitado**: Dejo Swagger accesible en todos los ambientes para facilitar la evaluacion y pruebas de los endpoints.

### Frontend

9. **Componentes reutilizables**:
   - `DataTableComponent`: Tabla generica con soporte de paginacion, sort y click en filas.
   - `StatusBadgePipe`: Pipe que aplica estilos visuales segun el estado/prioridad de las tareas.
   - `HighlightRiskDirective`: Directiva que resalta filas de desarrolladores con alto riesgo de retraso.

10. **Standalone Components**: Se utilizo la arquitectura de componentes standalone de Angular 18, sin NgModules, con lazy loading por rutas.

11. **Reverse Proxy (nginx)**: En Docker, el frontend usa nginx como reverse proxy para redirigir las llamadas `/api/*` al backend, eliminando problemas de CORS. En desarrollo local, Angular se conecta directamente al backend via CORS configurado en `localhost:4200`.

---

## Ejecucion con Docker

### Prerrequisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y ejecutandose.

### Pasos

1. Clonar el repositorio:
```bash
git clone https://github.com/Roixcs/NSerio_Challenge_Roimer.git
cd NSerio_Challenge_Roimer
git fetch
git checkout main
```

2. Ejecutar con Docker Compose:
```bash
docker-compose up --build
```

3. Esperar a que todos los servicios inicien (la base de datos toma unos segundos en estar healthy y ejecutar el script de inicializacion).

4. Acceder a la aplicacion:

| Servicio | URL |
|---|---|
| **Frontend (Angular)** | http://localhost:4200 |
| **Backend API (Swagger)** | http://localhost:5000/swagger |
| **SQL Server** | localhost:1433 (usuario: `sa`, password: `YourStrong@Passw0rd`) |

5. Para detener los servicios:
```bash
docker-compose down
```

Para eliminar tambien los volumenes de datos:
```bash
docker-compose down -v
```

---

## Ejecucion Manual

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20.x](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/sql-server) (local o en Docker)
- [Angular CLI 18](https://angular.dev/) (opcional, se puede usar `npx`)
- [Otra] Visual Studio y ejecutar en debug localmente y npm run start para el FrontEnd.

### 1. Base de Datos

Ejecutar el script `DBSetup_TeamTasks.sql` en SQL Server Management Studio o via `sqlcmd`:

```bash
sqlcmd -S localhost -U sa -P "YourPassword" -i DBSetup_TeamTasks.sql
```

Esto crea la base de datos `TeamTasksSample` con todas las tablas, vistas, stored procedures y datos de prueba.

### 2. Backend (.NET API)

```bash
cd BackEnd/TeamTaskAPI
```

Configurar la cadena de conexion en `TeamTaskAPI/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TeamTasksSample;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
  }
}
```

Ejecutar la API:

```bash
dotnet run --project TeamTaskAPI
```

La API estara disponible en:
- Swagger: https://localhost:7102/swagger
- API Base: https://localhost:7102/api

### 3. Frontend (Angular)

```bash
cd FrontEnd/team-tasks-app
npm install
ng serve 
ó 
npm run start
```

El frontend estara disponible en http://localhost:4200.

> **Nota**: El frontend en desarrollo se conecta a `https://localhost:7102/api` (configurado en `src/environments/environment.ts`). Asegurar que la API este corriendo antes de abrir el frontend.

---

## Endpoints de la API

### Endpoints Requeridos

| Metodo | Endpoint | Descripcion |
|---|---|---|
| GET | `/api/projects` | Proyectos con estadisticas (total, abiertas, completadas) |
| GET | `/api/projects/{id}/tasks` | Tareas del proyecto con paginacion y filtros (`?status=ToDo&assigneeId=1&page=1&pageSize=10`) |
| GET | `/api/dashboard/developer-workload` | Resumen de carga por desarrollador activo |
| GET | `/api/dashboard/project-health` | Resumen de estado por proyecto |
| GET | `/api/dashboard/developer-delay-risk` | Prediccion de riesgo de retraso por desarrollador |
| POST | `/api/tasks` | Crear nueva tarea (con validacion de campos y reglas de negocio) |

### Endpoints Opcionales

| Metodo | Endpoint | Descripcion |
|---|---|---|
| GET | `/api/developers` | Desarrolladores activos |
| GET | `/api/tasks/{id}` | Detalle de una tarea |
| PUT | `/api/tasks/{id}/status` | Actualizar estado, prioridad y complejidad |

### Formato de Respuesta

Todas las respuestas siguen el patron `Result<T>`:

```json
{
  "isSuccess": true,
  "data": { ... },
  "errorMessage": null
}
```

En caso de error:

```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Descripcion del error"
}
```

---

## Pruebas Unitarias

El proyecto incluye pruebas unitarias en `TeamTasks.Tests` usando xUnit, Moq y FluentAssertions.

### Ejecutar las pruebas

```bash
cd BackEnd/TeamTaskAPI
dotnet test
```

### Cobertura de pruebas

- **TaskServiceTests**: Creacion de tareas (exito, status invalido, error SQL), actualizacion de estado (exito, tarea no encontrada).
- **ProjectServiceTests**: Listado de proyectos con estadisticas, obtencion por ID (exito, no encontrado).

---

## Funcionalidades del Frontend

### Dashboard (Home `/`)
- Tabla de carga por desarrollador con sorting por columnas.
- Tabla de estado por proyecto con resaltado visual cuando tareas abiertas > completadas.
- Tabla de prediccion de riesgo con resaltado de desarrolladores de alto riesgo (`HighRiskFlag`).
- Boton "New Task" para crear tareas rapidamente.

### Tareas por Proyecto (`/projects/:id`)
- Tabla con paginacion server-side.
- Filtros por estado y desarrollador (dropdowns).
- Click en fila abre dialogo modal con detalle de la tarea y opcion de cambiar estado.
- Grafico doughnut de tareas por estado (ngx-charts).
- Actualizacion automatica de la vista al cambiar el estado de una tarea.

### Nueva Tarea (`/tasks/new`)
- Formulario con validacion de campos requeridos.
- Seleccion de proyecto, desarrollador, estado, prioridad, complejidad y fecha de vencimiento.
- Mensajes de error en campos y respuesta del servidor.
- Pre-seleccion de proyecto cuando se accede desde la vista de tareas.
