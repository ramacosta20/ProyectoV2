CREATE DEFINER=`root`@`localhost` PROCEDURE `InsertarUsuario`(
in PNombre       varchar(45), 
in PApellido varchar(45),
in PEmail   varchar(45),
in PContraseña   varchar(45)
)
begin
insert into Usuarios (Nombre,Apellido,Email,Contraseña)values (PNombre , PApellido,PEmail ,PContraseña);
end