CREATE TABLE [dbo].[ProgrammingLanguagesPopularity]
(
	ProgrammingLanguagesPopularityID INT IDENTITY PRIMARY KEY,
	SearchEngineID INT NOT NULL REFERENCES SearchEngines(SearchEngineID),
	ProgrammingLanguageID INT NOT NULL REFERENCES ProgrammingLanguages(ProgrammingLanguageID),
	Rating INT
)
