CREATE DEFINER=`root`@`localhost` PROCEDURE `LoguearUsuario`(
in PEmail       varchar(45), 
in PContraseña varchar(45)
)
begin
select Nombre,Apellido,Email,Contraseña from Usuarios where Email=PEmail and Contraseña=PContraseña;

end