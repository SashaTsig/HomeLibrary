using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLibrary.Core.Migrations
{
    /// <inheritdoc />
    public partial class SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var insertSPsql = @"CREATE TYPE dbo.AuthorList AS TABLE (
    Id        INT           NULL,
    FirstName NVARCHAR(50) NULL,
    LastName  NVARCHAR(50) NULL
);
GO

CREATE OR ALTER PROCEDURE SP_Books_Insert
    @Title           NVARCHAR(255),
    @PublishYear     INT,
    @Content		 XML,
    @Authors         AuthorList  READONLY,
    @NewBookId           INT           OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION

		IF EXISTS (
            SELECT 1 FROM @Authors
            WHERE ([Id] IS NULL AND ([FirstName] IS NULL OR [LastName] IS NULL))

        )
			THROW 50001, 'Id или First,Last Name обязательны', 1

		
		INSERT INTO dbo.[Books] ([Title], [PublishYear], [Content])
			VALUES (@Title, @PublishYear, @Content);

        SET @NewBookId = SCOPE_IDENTITY();
		

		DECLARE @InsertedAuthors TABLE (
            NewId     INT,
            FirstName NVARCHAR(50),
            LastName  NVARCHAR(50)
        );

        INSERT INTO dbo.[Authors] ([FirstName], [LastName])
        OUTPUT inserted.Id, inserted.FirstName, inserted.LastName
            INTO @InsertedAuthors ([NewId], [FirstName], [LastName])
        SELECT DISTINCT a.[FirstName], a.[LastName]
        FROM @Authors a
        WHERE a.[Id] IS NULL;

		
        DECLARE @FinalAuthors TABLE (AuthorId INT PRIMARY KEY);

        INSERT INTO @FinalAuthors ([AuthorId])
			SELECT [Id] FROM @Authors WHERE Id IS NOT NULL;

        INSERT INTO @FinalAuthors ([AuthorId])
			SELECT [NewId] FROM @InsertedAuthors;

		INSERT INTO dbo.[BookAuthors] ([BooksId], [AuthorsId])
			SELECT @NewBookId, [AuthorId] FROM @FinalAuthors;

	COMMIT;
       
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END";


            var updateSPsql = @"CREATE OR ALTER PROCEDURE SP_Books_Update
    @Id				 INT,
	@Title           NVARCHAR(255),
    @PublishYear     INT,
    @Content		 XML,
    @Authors         AuthorList READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION

		IF EXISTS (
            SELECT 1 FROM @Authors
            WHERE ([Id] IS NULL AND ([FirstName] IS NULL OR [LastName] IS NULL))

        )
			THROW 50001, 'Id или First,Last Name обязательны', 1

		
		
       DELETE FROM dbo.[BookAuthors] WHERE [BooksId]=@Id;
		

		DECLARE @InsertedAuthors TABLE (
            NewId     INT,
            FirstName NVARCHAR(50),
            LastName  NVARCHAR(50)
        );

        INSERT INTO dbo.[Authors] ([FirstName], [LastName])
        OUTPUT inserted.Id, inserted.FirstName, inserted.LastName
            INTO @InsertedAuthors ([NewId], [FirstName], [LastName])
        SELECT DISTINCT a.[FirstName], a.[LastName]
        FROM @Authors a
        WHERE a.[Id] IS NULL;

		
        DECLARE @FinalAuthors TABLE (AuthorId INT PRIMARY KEY);

        INSERT INTO @FinalAuthors ([AuthorId])
			SELECT [Id] FROM @Authors WHERE Id IS NOT NULL;

        INSERT INTO @FinalAuthors ([AuthorId])
			SELECT [NewId] FROM @InsertedAuthors;

		INSERT INTO dbo.[BookAuthors] ([BooksId], [AuthorsId])
			SELECT @Id, [AuthorId] FROM @FinalAuthors;

		UPDATE dbo.[Books] 
		SET [Title] = @Title, [PublishYear] = @PublishYear, [Content] = @Content
		WHERE Id = @Id;


	COMMIT;
       
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END";


            var getSPsql = @"CREATE OR ALTER PROCEDURE SP_Books_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.[Id],
        b.[Title],
        b.[PublishYear],
        b.[Content],
        CAST((
            SELECT
                a.[Id]        AS '@Id',
                a.[FirstName] AS '@FirstName',
                a.[LastName]  AS '@LastName'
            FROM [BookAuthors] ba
            JOIN [Authors] a ON a.[Id] = ba.[AuthorsId]
            WHERE ba.[BooksId] = b.[Id]
            FOR XML PATH('Author'), ROOT('Authors'), TYPE
        ) AS NVARCHAR(MAX)) AS AuthorsXml
    FROM [Books] b
    WHERE b.[Id] = @Id;
END;
GO";

            var findSPsql = @"CREATE OR ALTER PROCEDURE dbo.SP_Books_Search
    @Title            NVARCHAR(255) = NULL, 
    @Author  NVARCHAR(50) = NULL,   -- часть имени автора
    @ConentTerm       NVARCHAR(100) = NULL   -- ключевое слово в оглавлении
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.[Id],
        b.[Title],
        b.[PublishYear],
        CAST(b.[Content] AS NVARCHAR(MAX)) AS Content,
        CAST((
            SELECT
                a.[Id]        AS '@Id',
                a.[FirstName] AS '@FirstName',
                a.[LastName]  AS '@LastName'
            FROM [BookAuthors] ba
            JOIN [Authors] a ON a.Id = ba.[AuthorsId]
            WHERE ba.[BooksId] = b.[Id]
            FOR XML PATH('Author'), ROOT('Authors'), TYPE
        ) AS NVARCHAR(MAX)) AS AuthorsXml
    FROM [Books] b
    WHERE
        (@Title IS NULL OR b.[Title] LIKE N'%' + @Title + N'%')

        AND (@Author IS NULL
             OR EXISTS (
                 SELECT 1
                 FROM [BookAuthors] ba2
                 JOIN [Authors] a2 ON a2.Id = ba2.[AuthorsId]
                 WHERE ba2.[BooksId] = b.Id
                   AND (a2.[FirstName] LIKE N'%' + @Author + N'%' OR a2.[LastName]  LIKE N'%' + @Author  + N'%')
             ))


        AND (@ConentTerm IS NULL 
             OR b.Content.exist(
                 '/data/row/Chapter[
				 contains(string(.), sql:variable(""@ConentTerm""))
				 ]'
             ) = 1)

    ORDER BY b.[Title];
END;";


            migrationBuilder.Sql(insertSPsql);

            migrationBuilder.Sql(updateSPsql);

            migrationBuilder.Sql(getSPsql);

            migrationBuilder.Sql(findSPsql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
