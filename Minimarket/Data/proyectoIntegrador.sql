USE master;

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'proyectoIntegrador')
BEGIN
    ALTER DATABASE [proyectoIntegrador] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [proyectoIntegrador];
END

CREATE DATABASE [proyectoIntegrador];

USE [proyectoIntegrador];

CREATE TABLE [dbo].[Roles](
    IdRol INT NOT NULL PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

CREATE TABLE [dbo].[Permissions](    
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

CREATE TABLE [dbo].[user](
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    direccion VARCHAR(255) NULL,
    email VARCHAR(255) NULL,
    nombre VARCHAR(255) NULL,
    password VARCHAR(255) NULL,
    telefono VARCHAR(255) NULL,
    tipo VARCHAR(255) NULL,
    username VARCHAR(255) NULL,
    createdAt DATETIME NULL,
    IdRol INT DEFAULT 2,
    CONSTRAINT FK_user_Roles FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);

CREATE TABLE [dbo].[productos](
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    stock INT NULL,
    categoria VARCHAR(250) NULL,
    descripcion VARCHAR(255) NULL,
    imagen VARCHAR(255) NULL,
    nombre VARCHAR(255) NULL,
    precio FLOAT NULL,
    fechaIngreso DATETIME NULL,
    usuario_id INT NULL,
    CONSTRAINT FK_productos_user FOREIGN KEY (usuario_id) REFERENCES [user](id)
);

CREATE TABLE [dbo].[ordenes](
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    fecha_creacion DATETIME2(6) NULL,
    fecha_recibida DATETIME2(6) NULL,
    numero VARCHAR(255) NULL,
    total FLOAT NULL,
    usuario_id INT NULL,
    CONSTRAINT FK_ordenes_user FOREIGN KEY (usuario_id) REFERENCES [user](id)
);

CREATE TABLE [dbo].[detalle](
    id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    cantidad INT NULL,
    nombre VARCHAR(255) NULL,
    precio FLOAT NULL,
    total FLOAT NULL,
    orden_id INT NULL,
    producto_id INT NULL,
    CONSTRAINT FK_detalle_ordenes FOREIGN KEY (orden_id) REFERENCES ordenes(id),
    CONSTRAINT FK_detalle_productos FOREIGN KEY (producto_id) REFERENCES productos(id)
);

CREATE TABLE [dbo].[RolePermissions](
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdRol INT NOT NULL,
    IdPermisos INT NOT NULL,
    CONSTRAINT FK_RP_Roles FOREIGN KEY (IdRol) REFERENCES Roles(IdRol),
    CONSTRAINT FK_RP_Permissions FOREIGN KEY (IdPermisos) REFERENCES Permissions(Id)
);

CREATE TABLE [dbo].[NavItems](
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Title VARCHAR(100) NULL,
    Controller VARCHAR(50) NULL,
    Action VARCHAR(50) NULL,
    NombrePermisos VARCHAR(50) NULL,
    Icono VARCHAR(255) NULL,
    Estilo VARCHAR(255) NULL   
);

INSERT INTO Roles (IdRol, Nombre) VALUES (0, 'Sin Rol'), (1,'Admin'), (2,'Cliente');
INSERT INTO Permissions (Nombre) VALUES ('ManageUsers'), ('ViewSales'),('ViewCompras'), ('VerProductos'), ('VerMantenimiento'), ('VerCarrito');
INSERT INTO RolePermissions (IdRol, IdPermisos) VALUES (1, 1), (1, 2), (2,3), (2, 2), (1, 4), (2, 4), (1, 5), (1, 6), (2, 6);

INSERT INTO NavItems (Title, Controller, Action, NombrePermisos, Icono, Estilo) VALUES
('Panel Admin', 'Admin', 'Index', 'ManageUsers', 'fas fa-tachometer-alt', 'nav-link text-dark'),
('Ventas', 'Home', 'ListaOrden', 'ViewSales', 'fas fa-chart-line', 'nav-link text-dark'),
('Compras', 'Home', 'ListaOrden', 'ViewCompras', 'fas fa-chart-line', 'nav-link text-dark'),
('Ropas', 'Productos', 'Index', 'VerProductos', 'fas fa-cogs', 'nav-link text-dark'),
('Mantenimiento', 'Mantenimiento', 'Index', 'VerMantenimiento', 'fas fa-wrench', 'nav-link text-dark'),
('Carrito', 'Home', 'Carrito', 'VerCarrito', 'fas fa-cart-plus', 'nav-link active');

INSERT INTO [dbo].[user] (direccion, email, nombre, password, telefono, tipo, username, createdAt, IdRol)
VALUES
('Av. Los Olivos 123', 'asd@gmail.com','Juan Perez','123','987654321', 'admin', 'asd',GETDATE(),1),
('Av. Los Olivos 123','julio@gmail.com', 'Juan Perez','123','987654321', 'client', 'julio',GETDATE(),2),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'sin rol', 'pepe',GETDATE(),0),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'admin', 'juanp', GETDATE(), 1),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'client', 'marial', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'client', 'carlosr', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'client', 'anatorres', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'admin', 'luisv', GETDATE(), 1),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'client', 'luciap', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'client', 'pedrog', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Perez','123','987654321', 'admin', 'andreas', GETDATE(), 1),
('Jr. Sur 45', 'a9@b.com', 'Diego Ríos', '123', '987654329', 'client', 'diegor', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'client', 'karlam', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'client', 'oscarl', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'admin', 'sandraf', GETDATE(), 1),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'client', 'jorget', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'client', 'carmend', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'admin', 'raulh', GETDATE(), 1),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'client', 'elenar', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'client', 'tomasn', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'client', 'brendaq', GETDATE(), 2),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'admin', 'manuelc', GETDATE(), 1),
('Av. Los Olivos 123','a@c.com', 'Juan Pérez','123','987654321', 'sin rol', 'estefz', GETDATE(), 0);

INSERT INTO [dbo].[productos] (stock, categoria, descripcion, imagen, nombre, precio, fechaIngreso, usuario_id)
VALUES
(100, 'Hombre', 'Camisa manga larga de algodón', NULL, 'Camisa Casual Blanca Hombre', 39.99, '2025-04-01', 1),
(80, 'Hombre', 'Polo básico 100% algodón', NULL, 'Polo Azul Marino', 19.99, '2025-04-01', 2),
(120, 'Hombre', 'Jean ajustado de mezclilla', NULL, 'Jean Slim Fit Hombre', 49.99, '2025-04-02', 1),
(90, 'Mujer', 'Vestido corto estampado floral', NULL, 'Vestido Primavera Mujer', 59.99, '2025-04-02', 2),
(110, 'Hombre', 'Casaca impermeable ligera', NULL, 'Casaca Rompevientos', 69.99, '2025-04-03', 1),
(100, 'Mujer', 'Falda plisada hasta la rodilla', NULL, 'Falda Clásica Mujer', 34.99, '2025-04-03', 2),
(95, 'Unisex', 'Chompa tejida cuello redondo', NULL, 'Chompa Beige Unisex', 44.99, '2025-04-04', 1),
(130, 'Hombre', 'Short deportivo con bolsillos', NULL, 'Short Running Hombre', 24.99, '2025-04-04', 2),
(85, 'Mujer', 'Blusa con cuello en V', NULL, 'Blusa Casual Mujer', 29.99, '2025-04-05', 1),
(75, 'Unisex', 'Jogger con ajuste elástico', NULL, 'Pantalón Jogger Unisex', 39.99, '2025-04-05', 2),
(90, 'Hombre', 'Polo gráfico manga corta', NULL, 'Polo Estampado Urbano', 22.99, '2025-04-06', 1),
(60, 'Mujer', 'Casaca jean oversize', NULL, 'Casaca Denim Mujer', 64.99, '2025-04-06', 2),
(70, 'Hombre', 'Pantalón formal de vestir', NULL, 'Pantalón de Terno Hombre', 54.99, '2025-04-07', 1),
(80, 'Mujer', 'Top crop deportivo', NULL, 'Top Fitness Mujer', 27.99, '2025-04-07', 2),
(100, 'Hombre', 'Camisa hawaiana estampada', NULL, 'Camisa Hawaiana Hombre', 34.99, '2025-04-08', 1),
(120, 'Unisex', 'Chompa con cierre completo', NULL, 'Chompa Full Zip Unisex', 49.99, '2025-04-08', 2),
(60, 'Mujer', 'Vestido largo de noche', NULL, 'Vestido Gala Mujer', 89.99, '2025-04-09', 1),
(110, 'Hombre', 'Chaleco acolchado', NULL, 'Chaleco Abrigador Hombre', 59.99, '2025-04-09', 2),
(95, 'Mujer', 'Falda tipo lápiz', NULL, 'Falda Formal Mujer', 39.99, '2025-04-10', 1),
(100, 'Hombre', 'Polo sin mangas', NULL, 'Polo Deportivo Hombre', 19.99, '2025-04-10', 2),
(80, 'Mujer', 'Blazer formal entallado', NULL, 'Blazer Elegante Mujer', 79.99, '2025-04-11', 1),
(75, 'Unisex', 'Sudadera con capucha', NULL, 'Hoodie Básico Unisex', 44.99, '2025-04-11', 2),
(60, 'Hombre', 'Pantalón de buzo', NULL, 'Buzo Deportivo Hombre', 34.99, '2025-04-12', 1),
(90, 'Mujer', 'Polera corta con estampa', NULL, 'Polera Urban Mujer', 29.99, '2025-04-12', 2),
(100, 'Hombre', 'Casaca con capucha', NULL, 'Casaca Invierno Hombre', 74.99, '2025-04-13', 1),
(85, 'Mujer', 'Vestido camisero', NULL, 'Vestido Casual Mujer', 49.99, '2025-04-13', 2),
(110, 'Hombre', 'Chompa cuello alto', NULL, 'Chompa Térmica Hombre', 54.99, '2025-04-14', 1),
(120, 'Mujer', 'Top lencero elegante', NULL, 'Top Satinado Mujer', 34.99, '2025-04-14', 2),
(70, 'Hombre', 'Polo tipo rugby', NULL, 'Polo Rayado Hombre', 29.99, '2025-04-15', 1),
(100, 'Mujer', 'Falda corta con vuelo', NULL, 'Falda Casual Mujer', 27.99, '2025-04-15', 2),
(90, 'Hombre', 'Camisa manga corta de lino', NULL, 'Camisa Lino Hombre', 42.99, '2025-04-16', 1),
(100, 'Mujer', 'Shorts de mezclilla', NULL, 'Short Denim Mujer', 29.99, '2025-04-16', 2),
(85, 'Unisex', 'Camiseta sin mangas oversize', NULL, 'Tank Top Unisex', 21.99, '2025-04-17', 1),
(70, 'Mujer', 'Vestido midi con volantes', NULL, 'Vestido Floral Mujer', 64.99, '2025-04-17', 2),
(110, 'Hombre', 'Chompa ligera de primavera', NULL, 'Chompa Spring Hombre', 39.99, '2025-04-18', 1),
(60, 'Mujer', 'Blusa de encaje elegante', NULL, 'Blusa Encaje Mujer', 44.99, '2025-04-18', 2),
(100, 'Hombre', 'Pantalón cargo con bolsillos', NULL, 'Cargo Pants Hombre', 49.99, '2025-04-19', 1),
(95, 'Mujer', 'Top de tiras cruzadas', NULL, 'Top Verano Mujer', 24.99, '2025-04-19', 2),
(120, 'Hombre', 'Casaca estilo bomber', NULL, 'Bomber Jacket Hombre', 74.99, '2025-04-20', 1),
(80, 'Mujer', 'Falda midi con abertura lateral', NULL, 'Falda Abertura Mujer', 39.99, '2025-04-20', 2),
(75, 'Hombre', 'Polo cuello mao', NULL, 'Polo Estilo Mao Hombre', 34.99, '2025-04-21', 1),
(70, 'Mujer', 'Body ceñido', NULL, 'Body Estilo Mujer', 27.99, '2025-04-21', 2),
(90, 'Hombre', 'Camisa con estampado tropical', NULL, 'Camisa Tropical Hombre', 37.99, '2025-04-22', 1),
(85, 'Mujer', 'Pantalón tipo palazzo', NULL, 'Palazzo Suelto Mujer', 44.99, '2025-04-22', 2),
(100, 'Hombre', 'Sudadera sin capucha', NULL, 'Sudadera Básica Hombre', 29.99, '2025-04-23', 1),
(60, 'Mujer', 'Vestido camisero con cinturón', NULL, 'Vestido Oficina Mujer', 49.99, '2025-04-23', 2),
(110, 'Hombre', 'Polera estampada manga larga', NULL, 'Polera Urbana Hombre', 32.99, '2025-04-24', 1),
(95, 'Mujer', 'Top off-shoulder', NULL, 'Top Hombros Descubiertos', 28.99, '2025-04-24', 2),
(120, 'Hombre', 'Chaleco de punto', NULL, 'Chaleco Retro Hombre', 34.99, '2025-04-25', 1),
(80, 'Mujer', 'Falda pantalón', NULL, 'Falda Pantalón Mujer', 33.99, '2025-04-25', 2),
(85, 'Hombre', 'Polo con cuello Henley', NULL, 'Polo Henley Hombre', 36.99, '2025-04-26', 1),
(75, 'Mujer', 'Mono corto con tirantes', NULL, 'Mono Verano Mujer', 43.99, '2025-04-26', 2),
(90, 'Hombre', 'Camisa formal slim fit', NULL, 'Camisa Oficina Hombre', 55.99, '2025-04-27', 1),
(100, 'Mujer', 'Blusa con lazo al cuello', NULL, 'Blusa Oficina Mujer', 38.99, '2025-04-27', 2),
(95, 'Hombre', 'Jogger casual de tela', NULL, 'Jogger Urbano Hombre', 47.99, '2025-04-28', 1),
(80, 'Mujer', 'Vestido estilo boho', NULL, 'Vestido Bohemio Mujer', 52.99, '2025-04-28', 2),
(120, 'Hombre', 'Casaca tipo piloto', NULL, 'Casaca Piloto Hombre', 79.99, '2025-04-29', 1),
(60, 'Mujer', 'Top halter cruzado', NULL, 'Top Halter Mujer', 31.99, '2025-04-29', 2),
(110, 'Hombre', 'Chompa de alpaca artesanal', NULL, 'Chompa Andina Hombre', 69.99, '2025-04-30', 1),
(90, 'Mujer', 'Blazer con hombreras', NULL, 'Blazer Vintage Mujer', 64.99, '2025-04-30', 2);
