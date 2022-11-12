SELECT Libros.* INTO Libros FROM OPENROWSET(
BULK 'C:\Users\saave\Downloads\libros.json', SINGLE_CLOB
) AS [Json]
CROSS APPLY OPENJSON (BulkColumn, '$')
WITH (
IdCategoria varchar(100) '$.IdCategoria',
ImagenLibro varchar(100) '$.ImagenLibro',
ImagenLibroBASE64 nvarchar(max) '$.ImagenLibroBASE64',
Titulo varchar(200) '$.Titulo',
Editorial varchar(200) '$.Editorial',
Autores varchar(200) '$.Autores',
Edicion varchar(100) '$.Edicion',
NumeroPaginas int '$.NumeroPaginas',
Dimensiones varchar(50) '$.Dimensiones',
Idioma varchar(50) '$.Idioma',
ISBN10 varchar(15) '$.ISBN10',
ISBN13 varchar(20) '$.ISBN13',
Resumen varchar(max) '$.Resumen',
Precio decimal(5,2) '$.Precio'

) AS [Libros]