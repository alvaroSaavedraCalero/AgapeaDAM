create table dbo.Categorias (
	[IdCategoria] varchar(100) not null primary key,
	[NombreCategoria] varchar(max) not null
);

insert into dbo.Categorias values ('1','Ciencias Humanas');
insert into dbo.Categorias values ('2','Ciencias Tecnicas');
insert into dbo.Categorias values ('3','Economia');
insert into dbo.Categorias values ('4','Libros de Texto');
insert into dbo.Categorias values ('5','Libros Juveniles');
insert into dbo.Categorias values ('6','Libros para niños');
insert into dbo.Categorias values ('7','Literatura');
insert into dbo.Categorias values ('8','Temario de oposiciones');
insert into dbo.Categorias values ('9','Tiempo Libre');
insert into dbo.Categorias values ('10','Novedades');

insert into dbo.Categorias values ('2-1','Agricultura');
insert into dbo.Categorias values ('2-2','Biologia');
insert into dbo.Categorias values ('2-3','Ecologia y medio ambiente');
insert into dbo.Categorias values ('2-4','Historia de la ciencia');
insert into dbo.Categorias values ('2-5','Matematicas');
insert into dbo.Categorias values ('2-6','Zootecnia');
insert into dbo.Categorias values ('2-7','Arquitectura');
insert into dbo.Categorias values ('2-8','Botanica');
insert into dbo.Categorias values ('2-9','Fisica');
insert into dbo.Categorias values ('2-10','Informatica');

insert into dbo.Categorias values ('2-10-1','Bases de datos');
insert into dbo.Categorias values ('2-10-2','Díseño de paginas web');
insert into dbo.Categorias values ('2-10-3','Ofimatica');
insert into dbo.Categorias values ('2-10-4','Redes');
insert into dbo.Categorias values ('2-10-5','Programacion');



select * from dbo.Categorias;