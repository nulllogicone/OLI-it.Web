USE [null]
GO
/****** Object:  StoredProcedure [oli].[fischen]    Script Date: 2026-06-18 22:25:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER       PROCEDURE [oli].[fischen] (@CodeGuid uniqueidentifier = '00000000-0000-0000-0000-000000000000' , @AnglerGuid uniqueidentifier = '00000000-0000-0000-0000-000000000000' )
AS

SET NOCOUNT ON

BEGIN TRAN

declare @rc int

-- Es können entweder CodeGuid und / oder AnglerGuid übergeben werden.
-- Wenn beide Werte = 0 sind läuft ein 'alle mit allen' Vergleichen.

if @CodeGuid = '00000000-0000-0000-0000-000000000000'
	Begin
		DECLARE Code_cursor CURSOR 
		LOCAL FAST_FORWARD
		FOR 
		SELECT CodeGuid
		FROM Code
	End
Else	
	Begin	
		DECLARE Code_cursor CURSOR 
		LOCAL FAST_FORWARD
		FOR 
		SELECT CodeGuid
		FROM Code
		Where CodeGuid = @CodeGuid
	End

if @AnglerGuid = '00000000-0000-0000-0000-000000000000'
	Begin
		DECLARE Angler_cursor CURSOR 
		LOCAL FAST_FORWARD
		FOR 
		SELECT AnglerGuid
		FROM Angler
	end
else
	Begin
		-- zuerst alle Anglereinträge aus News Tabelle löschen
		-- DELETE FROM News
		-- WHERE AnglerGuid = @AnglerGuid		

		DECLARE Angler_cursor CURSOR 
		LOCAL FAST_FORWARD
		FOR 
		SELECT AnglerGuid
		FROM Angler
		where AnglerGuid = @AnglerGuid
	end



-- Schleifenkopf Code
OPEN Code_cursor

FETCH NEXT FROM Code_cursor 
into @CodeGuid

WHILE @@FETCH_STATUS = 0
BEGIN
	--print 'Code ' + convert(nvarchar,@CodeGuid)
-- Ende Schleifenkopf Code



-- Schleifenkopf Angler
OPEN Angler_cursor

FETCH NEXT FROM Angler_cursor 
into @AnglerGuid

WHILE @@FETCH_STATUS = 0
BEGIN
	--print 'Angler ' + convert(nvarchar,@AnglerGuid)
-- Ende Schleifenkopf Angler


-------------------------------------
-- ausführen der Prozedur beissen ---
-------------------------------------
EXEC @rc = [oli].beissen @CodeGuid, @AnglerGuid

-- wenn @rc = 0 dann beisst es
--print @rc

if @rc=0
	Begin
		--print  ' beisst ' 
		if  (SELECT COUNT(*) 
			FROM Spiegel 
			where CodeGuid = @CodeGuid and AnglerGuid = @AnglerGuid	
			) = 0
			Begin
				insert
				into Spiegel
				(CodeGuid,AnglerGuid)
				VALUES (@CodeGuid,@AnglerGuid)
				--print ' eingefügt'
			end
	end
else
	Begin
		--print   ' beisst nicht' 
		if (SELECT COUNT(*) 
			FROM Spiegel 
			where CodeGuid = @CodeGuid and AnglerGuid = @AnglerGuid	
		) > 0
			Begin
				delete spiegel 
				where CodeGuid=@CodeGuid 
				and AnglerGuid=@AnglerGuid
				--print 'gelöscht'
			end
	end
-- Ende des Kernbereiches ----------------


-- Schleifenfuss Angler
   FETCH NEXT FROM Angler_cursor 
   INTO @AnglerGuid
end
CLOSE Angler_cursor
-- Ende Schleifenfuss Angler


-- Schleifenfuss Code
   FETCH NEXT FROM Code_cursor
   INTO @CodeGuid
end
CLOSE Code_cursor
-- ende Schlefenfuss Code


COMMIT TRAN


DEALLOCATE Angler_cursor
DEALLOCATE Code_cursor



SET NOCOUNT OFF