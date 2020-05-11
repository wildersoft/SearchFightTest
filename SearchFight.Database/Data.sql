/*
Plantilla de script posterior a la implementación
--------------------------------------------------------------------------------------
 Este archivo contiene instrucciones de SQL que se anexarán al script de compilación.
 Use la sintaxis de SQLCMD para incluir un archivo en el script posterior a la implementación.
 Ejemplo:      :r .\miArchivo.sql
 Use la sintaxis de SQLCMD para hacer referencia a una variable en el script posterior a la implementación.
 Ejemplo:      :setvar TableName miTabla
               SELECT * FROM [$(TableName)]
--------------------------------------------------------------------------------------
*/
INSERT INTO SearchEngines(Name)
    VALUES
        ('Google'),
        ('MSN Search'),
        ('Yahoo Search');
GO
INSERT INTO ProgrammingLanguages(Name)
    VALUES
        ('.Net'),
        ('Java'),
        ('C#'),
        ('VB.Net'),
        ('VB6'),
        ('PowerBuilder'),
        ('Java Script'),
        ('Python'),
        ('PHP');
GO
INSERT INTO ProgrammingLanguagesPopularity(SearchEngineID, ProgrammingLanguageID, Rating)
    VALUES
        ('1', '1', '23'),
        ('2', '1', '53'),
        ('3', '1', '11'),
        ('1', '2', '52'),
        ('2', '2', '156'),
        ('3', '2', '85'),
        ('1', '3', '94'),
        ('3', '3', '33'),
        ('1', '4', '356'),
        ('2', '4', '225'),
        ('1', '5', '24'),
        ('2', '6', '38'),
        ('1', '7', '25'),
        ('3', '7', '456'),
        ('2', '7', '256'),
        ('2', '8', '126'),
        ('1', '8', '974');
GO