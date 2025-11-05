CREATE DATABASE PASTELERIA
GO

USE PASTELERIA
GO

-- Tabla Cliente
CREATE TABLE Cliente(
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    NombreCliente VARCHAR(100) NOT NULL,
    Cedula VARCHAR(20) NOT NULL UNIQUE,
    Correo VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20),
    Direccion VARCHAR(200),
    Contrasenna VARCHAR(255) NOT NULL,
    Estado BIT NOT NULL
)

-- Tabla Usuario
CREATE TABLE Usuario(
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Contrasenna VARCHAR(255) NOT NULL,
    IdRol INT NOT NULL,
    Estado BIT NOT NULL
)

-- Tabla Rol
CREATE TABLE Rol(
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol VARCHAR(50) NOT NULL,
    Estado BIT NOT NULL
)

-- Tabla Categoria
CREATE TABLE Categoria(
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    NombreCategoria VARCHAR(100) NOT NULL,
    Imagen VARBINARY(MAX) NOT NULL,
    Estado BIT NOT NULL
)

-- Tabla Producto
CREATE TABLE Producto(
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    IdCategoria INT NOT NULL,
    NombreProducto VARCHAR(200) NOT NULL,
    DescripcionProducto TEXT NOT NULL,
    Cantidad INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    PorcentajeImpuesto DECIMAL(5,2) NOT NULL,
    Imagen VARBINARY(MAX) NOT NULL,
    Estado BIT NOT NULL
)

-- Tabla Pedido
CREATE TABLE Pedido(
    IdPedido INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    IdUsuario INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Subtotal DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    Estado VARCHAR(50) NOT NULL
)

-- Tabla DetallePedido
CREATE TABLE DetallePedido(
    IdDetalle INT IDENTITY(1,1) PRIMARY KEY,
    IdPedido INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Descuento DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL
)

-- Agregar claves foráneas
ALTER TABLE Usuario
ADD CONSTRAINT FK_Usuario_Rol FOREIGN KEY (IdRol) REFERENCES Rol(IdRol)

ALTER TABLE Producto
ADD CONSTRAINT FK_Producto_Categoria FOREIGN KEY (IdCategoria) REFERENCES Categoria(IdCategoria)

ALTER TABLE Pedido
ADD CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (IdCliente) REFERENCES Cliente(IdCliente)

ALTER TABLE Pedido
ADD CONSTRAINT FK_Pedido_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)

ALTER TABLE DetallePedido
ADD CONSTRAINT FK_DetallePedido_Pedido FOREIGN KEY (IdPedido) REFERENCES Pedido(IdPedido)

ALTER TABLE DetallePedido
ADD CONSTRAINT FK_DetallePedido_Producto FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)

INSERT INTO Producto (IdCategoria, NombreProducto, DescripcionProducto, Cantidad, Precio, PorcentajeImpuesto, Imagen, Estado)
   VALUES (1, 'Pastel de Chocolate', 'Delicioso pastel de chocolate', 10, 15000.00, 13.00, 0x, 1);

INSERT INTO Producto (IdCategoria, NombreProducto, DescripcionProducto, Cantidad, Precio, PorcentajeImpuesto, Imagen, Estado)
VALUES (1, 'Pastel de Vainilla', 'Suave pastel de vainilla con relleno de fresas frescas', 8, 22000.00, 13.00, 0x, 1);

INSERT INTO Producto (IdCategoria, NombreProducto, DescripcionProducto, Cantidad, Precio, PorcentajeImpuesto, Imagen, Estado)
VALUES (2, 'Pan Artesanal', 'Pan de masa madre con corteza crujiente y miga suave', 20, 3500.00, 13.00, 0x, 1);

INSERT INTO Producto (IdCategoria, NombreProducto, DescripcionProducto, Cantidad, Precio, PorcentajeImpuesto, Imagen, Estado)
VALUES (3, 'Galletas Artesanales', 'Variedad de galletas con chips de chocolate y nueces', 30, 8000.00, 13.00, 0x, 1);

INSERT INTO Producto (IdCategoria, NombreProducto, DescripcionProducto, Cantidad, Precio, PorcentajeImpuesto, Imagen, Estado)
VALUES (4, 'Cheesecake', 'Cremoso cheesecake con base de galleta y frutos rojos', 6, 28000.00, 13.00, 0x, 1);

INSERT INTO Producto (IdCategoria, NombreProducto, DescripcionProducto, Cantidad, Precio, PorcentajeImpuesto, Imagen, Estado)
VALUES (5, 'Cupcakes', 'Set de 6 cupcakes decorados con buttercream', 15, 12000.00, 13.00, 0x, 1);

INSERT INTO Categoria(NombreCategoria, Imagen, Estado)
   VALUES ('Pasteles', 0x, 1);
   INSERT INTO Categoria (NombreCategoria, Imagen, Estado)
VALUES ('Panadería', 0x, 1);

INSERT INTO Categoria (NombreCategoria, Imagen, Estado)
VALUES ('Galletas', 0x, 1);

INSERT INTO Categoria (NombreCategoria, Imagen, Estado)
VALUES ('Postres Especiales', 0x, 1);

INSERT INTO Categoria (NombreCategoria, Imagen, Estado)
VALUES ('Cupcakes y Muffins', 0x, 1);

insert into Cliente values ('Juan Perez Solano', '302220222', 'juan@gmail.com','98765432', 'San Jose', 'l1234567', 1);

USE PASTELERIA
GO

-- Crear la nueva tabla EstadoPedido
CREATE TABLE EstadoPedido(
    IdEstadoPedido INT IDENTITY(1,1) PRIMARY KEY,
    NombreEstado VARCHAR(50) NOT NULL UNIQUE,
    Descripcion VARCHAR(200),
    Estado BIT NOT NULL DEFAULT 1
)
GO

-- Insertar los estados predefinidos
INSERT INTO EstadoPedido (NombreEstado, Descripcion, Estado)
VALUES 
    ('Pendiente', 'Pedido registrado, pendiente de procesamiento', 1),
    ('En Proceso', 'Pedido en preparación', 1),
    ('Completado', 'Pedido completado y listo para entrega', 1),
    ('Cancelado', 'Pedido cancelado', 1),
    ('Entregado', 'Pedido entregado al cliente', 1)
GO

-- Agregar nueva columna IdEstadoPedido a la tabla Pedido
ALTER TABLE Pedido
ADD IdEstadoPedido INT NOT NULL
GO

-- Agregar la clave foránea
ALTER TABLE Pedido
ADD CONSTRAINT FK_Pedido_EstadoPedido FOREIGN KEY (IdEstadoPedido) 
    REFERENCES EstadoPedido(IdEstadoPedido)
GO

-- Agregar columnas de fecha a la tabla Producto
ALTER TABLE Producto
ADD FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NOT NULL DEFAULT GETDATE();

-- Actualizar registros existentes con fecha actual
UPDATE Producto
SET FechaCreacion = GETDATE(),
    FechaActualizacion = GETDATE()
WHERE FechaCreacion IS NULL OR FechaActualizacion IS NULL;

-- Crear tabla de Auditoría
CREATE TABLE Auditoria (
    IdAuditoria INT PRIMARY KEY IDENTITY(1,1),
    Tabla VARCHAR(100) NOT NULL,
    IdRegistro INT NOT NULL,
    Accion VARCHAR(50) NOT NULL, -- 'CREAR', 'ACTUALIZAR', 'ELIMINAR'
    UsuarioId INT NULL, -- Si tienes usuarios
    UsuarioNombre VARCHAR(200) NULL,
    ValoresAnteriores NVARCHAR(MAX) NULL,
    ValoresNuevos NVARCHAR(MAX) NULL,
    Descripcion NVARCHAR(500) NULL,
    FechaAccion DATETIME NOT NULL DEFAULT GETDATE()
);