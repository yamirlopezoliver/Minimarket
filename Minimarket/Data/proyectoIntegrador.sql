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
('Productos', 'Productos', 'Index', 'VerProductos', 'fas fa-cogs', 'nav-link text-dark'),
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
(150, 'Hardware', 'Procesador Intel Core i9 de 12ª generación', NULL, 'Intel Core i9-12900K', 599.99, '2025-04-18', 1),
(200, 'Hardware', 'Memoria RAM DDR4 16GB', NULL, 'Corsair Vengeance 16GB', 79.99, '2025-04-17', 2),
(120, 'Hardware', 'Placa base para PC', NULL, 'ASUS ROG Strix Z590-E', 249.99, '2025-04-16', 1),
(80, 'Hardware', 'Disco duro sólido (SSD) 1TB', NULL, 'Samsung 970 Evo Plus 1TB', 129.99, '2025-04-15', 2),
(60, 'Hardware', 'Tarjeta gráfica Nvidia RTX 3080', NULL, 'Nvidia GeForce RTX 3080', 899.99, '2025-04-14', 1),
(100, 'Hardware', 'Fuente de alimentación 750W', NULL, 'Corsair RM750x', 109.99, '2025-04-13', 2),
(150, 'Hardware', 'Teclado mecánico RGB', NULL, 'Logitech G Pro X', 129.99, '2025-04-12', 1),
(200, 'Hardware', 'Ratón inalámbrico para gaming', NULL, 'Razer DeathAdder V2', 69.99, '2025-04-11', 2),
(110, 'Hardware', 'Monitor 27" 4K', NULL, 'LG 27GN950-B', 499.99, '2025-04-10', 1),
(80, 'Hardware', 'Caja de PC ATX', NULL, 'NZXT H510', 79.99, '2025-04-09', 2),
(50, 'Hardware', 'Unidad flash USB 64GB', NULL, 'SanDisk Ultra Flair 64GB', 15.99, '2025-04-08', 1),
(130, 'Hardware', 'Auriculares gaming con micrófono', NULL, 'SteelSeries Arctis 7', 169.99, '2025-04-07', 2),
(90, 'Hardware', 'Webcam 1080p', NULL, 'Logitech C920', 59.99, '2025-04-06', 1),
(40, 'Hardware', 'Refrigeración líquida para CPU', NULL, 'Corsair iCUE H100i', 149.99, '2025-04-05', 2),
(110, 'Hardware', 'Disco duro mecánico 2TB', NULL, 'Seagate Barracuda 2TB', 59.99, '2025-04-04', 1),
(60, 'Hardware', 'Adaptador de red Wi-Fi 6',NULL, 'TP-Link Archer TX3000E', 99.99, '2025-04-03', 2),
(80, 'Hardware', 'Pantalla táctil 15" para PC', NULL, 'ViewSonic TD1655', 299.99, '2025-04-02', 1),
(150, 'Hardware', 'Soporte para monitores', NULL, 'Ergotron LX Desk Mount', 179.99, '2025-04-01', 2),
(70, 'Hardware', 'Disco duro externo 1TB', NULL, 'WD My Passport 1TB', 59.99, '2025-03-31', 1),
(120, 'Hardware', 'Tarjeta de sonido externa', NULL, 'Focusrite Scarlett 2i2', 169.99, '2025-03-30', 2);
