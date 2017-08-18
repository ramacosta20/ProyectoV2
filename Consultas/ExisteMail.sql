CREATE DEFINER=`root`@`localhost` PROCEDURE `ExisteMail`(
in PEmail       varchar(45)
)
begin
select Nombre from Usuarios where Email=PEmail;

end