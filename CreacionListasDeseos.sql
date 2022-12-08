CREATE TABLE [dbo].[Listas]
(
	[IdLista] VARCHAR(200) NOT NULL PRIMARY KEY, 
	[IdCliente] VARCHAR(200) NOT NULL,
    [Titulo] VARCHAR(MAX) NULL, 
    [Descripcion] VARCHAR(MAX) NULL, 
    [ImagenLista] VARCHAR(MAX) NULL
)