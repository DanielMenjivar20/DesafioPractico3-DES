# DesafioPractico3-DES

## Tecnologías Utilizadas
- **Backend:** ASP.NET Core 8, Entity Framework Core (Code First)
- **Base de Datos:** SQL Server (`MSI\SQLEXPRESS`)
- **Caché:** Redis[cite: 5]
- **Reportes:** SQL Server Reporting Services (SSRS) / Report Builder[cite: 5]
- **Seguridad:** ASP.NET Core Identity (Roles: `Admin`, `User`)[cite: 3, 5]
- **Pruebas:** xUnit, Moq[cite: 5]

---

## Instrucciones de Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone <url-de-tu-repositorio>
cd DesafioPractico3

2. Configurar la Cadena de Conexión
Abre el archivo appsettings.json en tu proyecto y verifica la cadena de conexión hacia tu SQL Server:

JSON
"ConnectionStrings": {
  "DefaultConnection": "Server=MSI\\SQLEXPRESS;Database=CompanyManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}

3. Configurar Redis
Asegúrate de que el servicio de Redis esté corriendo y verifica el puerto en tu appsettings.json:

JSON
"Redis": {
  "ConnectionString": "localhost:6379"
}

4. Base de Datos y Migraciones
Puedes importar el script SQL incluido en el repositorio (CompanyManagement_Script.sql) directamente en SQL Server Management Studio (SSMS)[cite: 5], o aplicar las migraciones ejecutando en la consola:


Update-Database

5. Ejecutar la Aplicación
Abre la solución en Visual Studio y presiona F5, o ejecuta desde la terminal:

dotnet run

Descripción de Endpoints y Ejemplos de Uso

Autenticación (Públicos)Registrar Usuario: POST /api/auth/register   
Cuerpo (JSON):JSON{
  "email": "usuario@test.com",
  "password": "Password123*"
}
Iniciar Sesión: POST /api/auth/login

Clientes (Requieren Autenticación)Listar Clientes: GET /api/clientes (Cacheado en Redis por 10 min) 
Obtener Cliente por ID: GET /api/clientes/{id} (Cacheado)  
Crear Cliente: POST /api/clientes (Invalida caché) 
Actualizar Cliente: PUT /api/clientes/{id} (Invalida caché) 
Eliminar Cliente: DELETE /api/clientes/{id} (Invalida caché)

Órdenes (Requieren Autenticación)
Listar Órdenes: GET /api/ordenes (Cacheado)
Crear Orden: POST /api/ordenes (Invalida caché)
Órdenes por Cliente: GET /api/ordenes/cliente/{clienteId}

Reportes SSRS (Requieren Rol Admin)
Reporte 1 (Clientes Activos): GET /api/reportes/clientes-activos
Reporte 2 (Ingresos por Cliente): GET /api/reportes/ingresos-clientes
Reporte 3 (Clientes Inactivos): GET /api/reportes/clientes-inactivos


Pruebas Unitarias
El proyecto incluye pruebas con xUnit aplicando el patrón Arrange-Act-Assert.Para ejecutarlas desde Visual Studio:
Ve a Prueba > Explorador de pruebas.
Haz clic en Ejecutar todas.

