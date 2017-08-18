CREATE DEFINER=`root`@`localhost` PROCEDURE `InsertarEvento`(
in PNombreEvento varchar(45), PDescripcion varchar(500), PMeta int(11), PFechaTermina datetime, 
PEmbedUbicacion varchar(300), PUsuarioAdmin int(11)
)
begin
insert into evento(NombreEvento,Descripcion,Meta,FechaTermina,EmbedUbicacion,UsuarioAdmin) values (PNombreEvento,
PDescripcion, PMeta, PFechaTermina, PEmbedUbicacion, PUsuarioAdmin);
end