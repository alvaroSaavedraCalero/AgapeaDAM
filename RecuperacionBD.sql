

-- consulta para crearnos un backup de la base de datos --
-- backup database ApageaBD to disk = 'C:\Users\Alvaro Saavedra\source\repos\alvaroSaavedraCalero\AgapeaDAM\AgapeaBD.bak';


-- Consultamos el contenido del fichero de backup de la base de datos --
--restore filelistonly from disk = 'C:\Users\Alvaro Saavedra\source\repos\alvaroSaavedraCalero\AgapeaDAM\AgapeaBD.bak';


-- Restauramos la base de datos desde un fichero de backup.bak
 restore database AgapeaBDv2
	from disk = 'C:\Users\Alvaro Saavedra\source\repos\alvaroSaavedraCalero\AgapeaDAM\AgapeaBD.bak'
	with recovery,
	move 'AgapeaBD' TO 'C:\Users\Alvaro Saavedra\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\AgapeaBDv2.mdf',
	move 'AgapeaBD_log' to 'C:\Users\Alvaro Saavedra\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\AgapeaBDv2.ldf';
	