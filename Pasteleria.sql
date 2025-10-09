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