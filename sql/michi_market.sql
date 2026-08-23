use master;
go

drop database if exists michi_market;
go

create database michi_market;
go

use michi_market;
go

drop table if exists tb_pedido_detalle;
drop table if exists tb_carrito_detalle;
drop table if exists tb_pedido;
drop table if exists tb_carrito;
drop table if exists tb_producto;
drop table if exists tb_usuario;
go

-------------------------------------------------Tablas--------------------------------------------------

--Tabla de usuarios
create table tb_usuario(
id_usuario int primary key identity(1,1),
nombres varchar(30),
apellidos varchar(30),
correo varchar(50) unique,
password varchar(255),
estado bit default 1,
foto_perfil varchar(200),
rol varchar(20) check (rol in ('admin', 'cliente'))
);
go

--Tabla de productos
create table tb_producto(
id_producto int primary key identity(1,1),
nombre varchar(50),
descripcion varchar(200),
precio decimal(10,2),
estado bit default 1,
stock int,
foto_producto varchar(200),
tipo varchar(20) check (tipo in ('alimento', 'higiene', 'accesorio','juguetes','salud')),
animal varchar(20) check (animal in ('gato', 'perro', 'cuy'))
);
go

--Tabla del carrito
create table tb_carrito(
id_carrito int primary key identity(1,1),
id_usuario int,
fecha_creacion date default getdate(),
foreign key (id_usuario) references tb_usuario(id_usuario)
);
go

--Tabla detalle del carrito
create table tb_carrito_detalle(
id_carrito_detalle int primary key identity(1,1),
id_carrito int,
id_producto int,
cantidad int,
precio_unit decimal(10,2),
subtotal decimal(10,2),
foreign key (id_carrito) references tb_carrito(id_carrito),
foreign key (id_producto) references tb_producto(id_producto)
);
go

--Tabla de pedidos
create table tb_pedido(
id_pedido int primary key identity(1,1),
id_usuario int,
total decimal(10,2),
estado varchar(20) default 'pendiente' check (estado in ('pendiente','en_camino','entregado')),
fecha_creacion date default getdate(),
foreign key (id_usuario) references tb_usuario(id_usuario)
);
go

--Tabla detalle del pedido
create table tb_pedido_detalle(
id_pedido_detalle int primary key identity(1,1),
id_pedido int,
id_producto int,
cantidad int,
precio_unit decimal(10,2),
subtotal decimal(10,2),
foreign key (id_pedido) references tb_pedido(id_pedido),
foreign key (id_producto) references tb_producto(id_producto)
);
go

----------------------------Procedimientos almacenados de usuarios para Admin--------------------------------------------------

--Listar usuarios (Admin)
drop procedure if exists sp_admin_listar_usuarios;
go
create procedure sp_admin_listar_usuarios
@rol varchar(20)
as
begin
    if (@rol = 'admin')
    select * from tb_usuario;
    else
    select 'Acceso denegado' as mensaje;
end
go

--Eliminar usuario (Admin)
drop procedure if exists sp_admin_eliminar_usuario;
go
create procedure sp_admin_eliminar_usuario
@id_usuario int,
@rol varchar(20)
as
begin
    if (@rol = 'admin')
    delete from tb_usuario where id_usuario = @id_usuario;
    else
    select 'Acceso denegado' as mensaje;
end
go

--Buscar usuario (Admin)
drop procedure if exists sp_admin_buscar_usuario;
go
create procedure sp_admin_buscar_usuario
@id_usuario int,
@rol varchar(20)
as
begin
    if (@rol = 'admin')
    select * from tb_usuario where id_usuario = @id_usuario;
    else
    select 'Acceso denegado' as mensaje;
end
go

------------------------------------Procediminetos almacenados para usuarios--------------------------------------------------

--Iniciar sesion
drop procedure if exists sp_iniciar_sesion;
go
create procedure sp_iniciar_sesion
@correo varchar(50),
@password varchar(255)
as
begin
    select id_usuario, nombres, apellidos, correo, foto_perfil, rol
    from tb_usuario
    where correo = @correo and password = @password and estado = 1;
end
go

--Obtener usuario por correo
drop procedure if exists sp_obtener_usuario_por_correo;
go
create procedure sp_obtener_usuario_por_correo
@correo varchar(50)
as
begin
    select id_usuario, nombres, apellidos, correo, password, foto_perfil, rol
    from tb_usuario
    where correo = @correo and estado = 1;
end
go

--Guardar usuario - MERGE
drop procedure if exists sp_guardar_usuario;
go
create procedure sp_guardar_usuario
@id_usuario int,
@nombres varchar(30),
@apellidos varchar(30),
@correo varchar(50),
@password varchar(255),
@foto_perfil varchar(200),
@rol varchar(20) = 'cliente'
as
begin
    merge tb_usuario as target
    using (select @id_usuario as id_usuario) as source
    on target.id_usuario = source.id_usuario
    when matched then
        update set nombres = @nombres, apellidos = @apellidos,
        correo = @correo, password = @password, foto_perfil = @foto_perfil
    when not matched then
        insert (nombres, apellidos, correo, password, foto_perfil, rol)
        values (@nombres, @apellidos, @correo, @password, @foto_perfil, @rol);
end
go

-----------------------------Procedimientos almacenados de productos para Admin--------------------------------------------------

--Guardar producto (Admin) - MERGE
drop procedure if exists sp_admin_guardar_producto;
go
create procedure sp_admin_guardar_producto
@id_producto int,
@nombre varchar(50),
@descripcion varchar(200),
@precio decimal(10,2),
@estado bit,
@stock int,
@foto_producto varchar(200),
@tipo varchar(20),
@animal varchar(20),
@rol varchar(20)
as
begin
    if (@rol = 'admin')
        merge tb_producto as target
        using (select @id_producto as id_producto) as source
        on target.id_producto = source.id_producto
        when matched then
            update set nombre = @nombre, descripcion = @descripcion,
            precio = @precio, estado = @estado, stock = @stock,
            foto_producto = @foto_producto, tipo = @tipo, animal = @animal
        when not matched then
            insert (nombre, descripcion, precio, estado, stock, foto_producto, tipo, animal)
            values (@nombre, @descripcion, @precio, @estado, @stock, @foto_producto, @tipo, @animal);
    else
        select 'Acceso denegado' as mensaje;
end
go

--Eliminar producto (Admin)
drop procedure if exists sp_admin_eliminar_producto;
go
create procedure sp_admin_eliminar_producto
@id_producto int,
@rol varchar(20)
as
begin
    if (@rol = 'admin')
       delete from tb_producto where id_producto = @id_producto;
    else
    select 'Acceso denegado' as mensaje;
end
go

--Eliminar varios productos (Admin)
drop procedure if exists sp_admin_eliminar_productos;
go
create procedure sp_admin_eliminar_productos
@id_productos varchar(max),
@rol varchar(20)
as
begin
    if (@rol = 'admin')
       delete from tb_producto where id_producto in (select value from string_split(@id_productos,','));
    else
    select 'Acceso denegado' as mensaje;
end
go

-------------------------------Procedimientos almacenados de productos para Usuarios--------------------------------------------------

--Listar productos
drop procedure if exists sp_listar_productos;
go
create procedure sp_listar_productos
as
begin
    select * from tb_producto where estado = 1;
end
go

--Buscar productos
drop procedure if exists sp_buscar_productos;
go
create procedure sp_buscar_productos
@nombre varchar(50) = null,
@tipo varchar(20) = null,
@animal varchar(20) = null,
@precio_min decimal(10,2) = null,
@precio_max decimal(10,2) = null
as
begin
    select * from tb_producto where estado = 1 and
               (@nombre is null or nombre like '%' + @nombre + '%') and
               (@tipo is null or tipo = @tipo) and
               (@animal is null or animal = @animal) and
               (@precio_min is null or precio >= @precio_min) and
               (@precio_max is null or precio <= @precio_max)
end
go

--Obtener producto por ID
drop procedure if exists sp_obtener_producto_por_id;
go
create procedure sp_obtener_producto_por_id
@id_producto int
as
begin
    select * from tb_producto where id_producto = @id_producto;
end
go

-------------------------Procedimientos almacenados de carritos y pedidos para Usuarios--------------------------------------------------

--Agregar al carrito
drop procedure if exists sp_agregar_carrito;
go
create procedure sp_agregar_carrito
@id_usuario int,
@id_producto int,
@cantidad int
as
begin
    declare @id_carrito int;
    select @id_carrito = id_carrito from tb_carrito where id_usuario = @id_usuario;

    if (@id_carrito is null)
    begin
        insert into tb_carrito(id_usuario) values (@id_usuario);
        set @id_carrito = scope_identity();
    end

    if exists (select 1 from tb_carrito_detalle where id_carrito = @id_carrito and id_producto = @id_producto)
        update tb_carrito_detalle set cantidad = cantidad + @cantidad,
        subtotal = (cantidad + @cantidad) * precio_unit
        where id_carrito = @id_carrito and id_producto = @id_producto;
    else
        insert into tb_carrito_detalle(id_carrito, id_producto, cantidad, precio_unit, subtotal)
        values (@id_carrito, @id_producto, @cantidad,
                (select precio from tb_producto where id_producto = @id_producto),
                (select precio * @cantidad from tb_producto where id_producto = @id_producto));
end
go

--Listar carrito
drop procedure if exists sp_listar_carrito;
go
create procedure sp_listar_carrito
@id_usuario int
as
begin
    select cd.id_carrito_detalle, cd.id_producto, p.nombre, p.foto_producto, p.tipo, p.animal,
           cd.cantidad, cd.precio_unit, cd.subtotal
    from tb_carrito c
    inner join tb_carrito_detalle cd on c.id_carrito = cd.id_carrito
    inner join tb_producto p on cd.id_producto = p.id_producto
    where c.id_usuario = @id_usuario;
end
go

--Eliminar del carrito
drop procedure if exists sp_eliminar_carrito;
go
create procedure sp_eliminar_carrito
@id_usuario int,
@id_carrito_detalle int
as
begin
    delete from tb_carrito_detalle
    where id_carrito_detalle = @id_carrito_detalle
    and id_carrito = (select id_carrito from tb_carrito where id_usuario = @id_usuario);
end
go

--Actualizar cantidad carrito
drop procedure if exists sp_actualizar_cantidad_carrito;
go
create procedure sp_actualizar_cantidad_carrito
@id_usuario int,
@id_carrito_detalle int,
@cantidad int
as
begin
    update cd set cd.cantidad = @cantidad, cd.subtotal = @cantidad * cd.precio_unit
    from tb_carrito_detalle cd
    inner join tb_carrito c on cd.id_carrito = c.id_carrito
    where cd.id_carrito_detalle = @id_carrito_detalle
    and c.id_usuario = @id_usuario;
end
go

--Realizar pedido
drop procedure if exists sp_realizar_pedido;
go
create procedure sp_realizar_pedido
@id_usuario int
as
begin
    declare @id_carrito int;
    declare @total decimal(10,2);

    select @id_carrito = id_carrito from tb_carrito where id_usuario = @id_usuario;

    if (@id_carrito is null)
    begin
        select 'No tiene productos en el carrito' as mensaje;
        return;
    end

    select @total = sum(subtotal) from tb_carrito_detalle where id_carrito = @id_carrito;

    if (@total is null or @total = 0)
    begin
        select 'No tiene productos en el carrito' as mensaje;
        return;
    end

    if exists (
        select 1 from tb_carrito_detalle cd
        inner join tb_producto p on cd.id_producto = p.id_producto
        where cd.id_carrito = @id_carrito and p.stock < cd.cantidad
    )
    begin
        select 'Stock insuficiente para uno o mas productos' as mensaje;
        return;
    end

    begin transaction;

    update p set p.stock = p.stock - cd.cantidad
    from tb_producto p
    inner join tb_carrito_detalle cd on p.id_producto = cd.id_producto
    where cd.id_carrito = @id_carrito;

    insert into tb_pedido(id_usuario, total) values (@id_usuario, @total);
    declare @id_pedido int = scope_identity();

    insert into tb_pedido_detalle(id_pedido, id_producto, cantidad, precio_unit, subtotal)
    select @id_pedido, id_producto, cantidad, precio_unit, subtotal
    from tb_carrito_detalle where id_carrito = @id_carrito;

    delete from tb_carrito_detalle where id_carrito = @id_carrito;

    commit transaction;

    select 'Pedido realizado con exito' as mensaje;
end
go

--Listar mis pedidos
drop procedure if exists sp_listar_mis_pedidos;
go
create procedure sp_listar_mis_pedidos
@id_usuario int
as
begin
    select * from tb_pedido where id_usuario = @id_usuario;
end
go

--Detalle pedido
drop procedure if exists sp_detalle_pedido;
go
create procedure sp_detalle_pedido
@id_usuario int,
@id_pedido int
as
begin
    select pd.id_pedido_detalle, pd.id_producto, p.nombre, p.foto_producto, pd.cantidad, pd.precio_unit, pd.subtotal
    from tb_pedido_detalle pd
    inner join tb_producto p on pd.id_producto = p.id_producto
    where pd.id_pedido = @id_pedido
    and exists (select 1 from tb_pedido where id_pedido = @id_pedido and id_usuario = @id_usuario);
end
go

------------------------------Procedimientos almacenados de pedidos para Admins--------------------------------------------------

--Listar pedidos (Admin)
drop procedure if exists sp_admin_listar_pedidos;
go
create procedure sp_admin_listar_pedidos
@rol varchar(20)
as
begin
    if (@rol = 'admin')
        select p.id_pedido, p.id_usuario, u.nombres + ' ' + u.apellidos as cliente,
               p.total, p.estado, p.fecha_creacion
        from tb_pedido p
inner join tb_usuario u on p.id_usuario = u.id_usuario;
    else
        select 'Acceso denegado' as mensaje;
end
go

--Actualizar estado pedido (Admin)
drop procedure if exists sp_admin_actualizar_estado_pedido;
go
create procedure sp_admin_actualizar_estado_pedido
@id_pedido int,
@estado varchar(20),
@rol varchar(20)
as
begin
    if (@rol = 'admin')
        if (@estado in ('pendiente','en_camino','entregado'))
            update tb_pedido set estado = @estado where id_pedido = @id_pedido;
        else
            select 'Estado no valido' as mensaje;
    else
        select 'Acceso denegado' as mensaje;
end
go


-------------------------------------------------Inserts--------------------------------------------------
--Admins
insert into tb_usuario (nombres, apellidos, correo, password, foto_perfil, rol) values
('Angelo', 'Pool', 'angelo@gmail.com', '1234', '/img/default_avatar.png', 'admin'),
('Pepito','Craft','pepitocraft@gmail.com','4321','/img/default_avatar.png','admin');
go
--30 Productos
insert into tb_producto (nombre, descripcion, precio, estado, stock, foto_producto, tipo, animal) values
('Whiskas Deli amortigueros', 'Bolsa 1kg de croquetas sabor atun', 32.90, 1, 120, 'img/productos/whiskas.jpg', 'alimento', 'gato'),
('Cat Chow Gourmet', 'Comida premium con trocitos de pollo', 45.50, 1, 85, 'img/productos/cat_chow_gourmet.jpg', 'alimento', 'gato'),
('ProPlan Excellent Gatitos', 'Alimento para gatitos 2 a 12 meses', 68.00, 1, 60, 'img/productos/proplan_excellent_gatitos.jpg', 'alimento', 'gato'),
('Felix Pouch Carne', 'Pouch 85g carne en gelatina x6', 22.50, 1, 200, 'img/productos/felix_pouch _carne.png', 'alimento', 'gato'),
('Sheba Filetes Variados', 'Latas 40g salmon y atun x12', 38.00, 1, 90, 'img/productos/whiskas.jpg', 'alimento', 'gato'),
('Dog Chow Vital Adulto', 'Bolsa 3kg perros adultos', 55.00, 1, 100, 'img/productos/dog_chow.jpg', 'alimento', 'perro'),
('Pedigree Original Carne', 'Alimento sabor carne 4.5kg', 42.90, 1, 75, 'img/productos/dog_chow.jpg', 'alimento', 'perro'),
('Royal Canin Medium', 'Razas medianas 11 a 25kg', 78.50, 1, 50, 'img/productos/dog_chow.jpg', 'alimento', 'perro'),
('Cachorros Madra Bife', 'Alimento cachorros 1.5kg', 35.00, 1, 110, 'img/productos/dog_chow.jpg', 'alimento', 'perro'),
('Gravy Train Pollo', 'Croquetas sabor pollo 2.5kg', 29.90, 1, 130, 'img/productos/dog_chow.jpg', 'alimento', 'perro'),
('Cuy Nutritious Plus', 'Avena con vitamina C 500g', 18.50, 1, 45, 'img/productos/cuy_nutritious_plus.png', 'alimento', 'cuy'),
('Cuy Master Vitaminas', 'Alimento con frutas 1kg', 25.00, 1, 35, 'img/productos/cuy_master _vitaminas.png', 'alimento', 'cuy'),
('Shampoo Hipoalergenico Gatitos', 'Aloe vera 400ml', 28.90, 1, 70, 'img/productos/shampoo_hipoalergenico_gatitos.png', 'higiene', 'gato'),
('Shampoo Antipulgas Perros', 'Extracto de neem 500ml', 35.00, 1, 65, 'img/productos/shampoo_hipoalergenico_gatitos.png', 'higiene', 'perro'),
('Arena Antibacteriana Premium', 'Control de olores 10kg', 42.00, 1, 150, 'img/productos/shampoo_hipoalergenico_gatitos.png', 'higiene', 'gato'),
('Tojillas Humedas Mascotas', 'Sin perfume x80', 15.90, 1, 200, 'img/productos/shampoo_hipoalergenico_gatitos.png', 'higiene', 'gato'),
('Cepillo Deshedding Perro', 'Pelo corto y largo', 32.50, 1, 55, 'img/productos/shampoo_hipoalergenico_gatitos.png', 'higiene', 'perro'),
('Vitaminas Nutrigel Gatitos', 'Pasta con taurina 100g', 24.90, 1, 80, 'img/productos/nutrigel-large.jpg', 'salud', 'gato'),
('Antipulgas Frontline Plus', 'Spot-on 10 a 20kg x3', 65.00, 1, 40, 'img/productos/antipulgas_frontline_plus.jpg', 'salud', 'perro'),
('Desparasitante Caniverm', 'Comprimidos internos x6', 28.50, 1, 90, 'img/productos/default.jpg', 'salud', 'perro'),
('Vitamina C para Cuyes', 'Polvo vitamico 150g', 19.90, 1, 30, 'img/productos/default.jpg', 'salud', 'cuy'),
('Collar Azul Ajustable', 'Nylon hebilla seguridad M', 18.90, 1, 100, 'img/productos/transportadora_airline.png', 'accesorio', 'perro'),
('Cama Donut Luxe', 'Redonda antiansiedad L', 89.90, 1, 25, 'img/productos/transportadora_airline.png', 'accesorio', 'gato'),
('Ropa Superheroe Gatito', 'Disfraz fiestas S', 35.00, 1, 40, 'img/productos/transportadora_airline.png', 'accesorio', 'gato'),
('Correa Retractil 5m', 'Automatica freno 20kg', 42.00, 1, 60, 'img/productos/transportadora_airline.png', 'accesorio', 'perro'),
('Transportadora Airline', 'Plastica vuelos M', 120.00, 1, 20, 'img/productos/transportadora_airline.png', 'accesorio', 'gato'),
('Raton Laser Interactivo', 'Laser y pluma giratoria', 55.00, 1, 35, 'img/productos/default.jpg', 'juguetes', 'gato'),
('Hueso de Goma Indestructible', 'Sabor carne M', 22.90, 1, 80, 'img/productos/default.jpg', 'juguetes', 'perro'),
('Rueda para Cuyes', 'Metal silenciosa 28cm', 45.00, 1, 25, 'img/productos/default.jpg', 'juguetes', 'cuy'),
('Pluma con Cascabel', 'Varita plumas y cascabel', 12.90, 1, 150, 'img/productos/default.jpg', 'juguetes', 'gato');
go
