USE [null]
GO
/****** Object:  StoredProcedure [oli].[beissen]    Script Date: 2026-06-18 22:25:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Objekt:  Gespeicherte Prozedur [oli].beissen    Skriptdatum: 02.09.2003 15:16:57 ******/

ALTER         PROCEDURE [oli].[beissen] (@CodeGuid uniqueidentifier , @AnglerGuid uniqueidentifier )
AS

declare @rc int --Returncode
set @rc = 0


SET NOCOUNT ON



-- Der Code muss Ringe haben
if (
	select COUNT(*)
	from Ringe
	Where CodeGuid = @CodeGuid
) = 0
Begin
	-- print 'Fehler bei keine Ringe im Code'
	set @rc = -1
	return -1
end

-- Der Angler muss Löcher haben
if (
	select COUNT(*)
	from Löcher
	Where AnglerGuid = @anglerGuid
) = 0
Begin
	-- print 'Fehler bei keine Löcher im Angler'
	set @rc = -1
	return -1
end






-- ALLE muss müssen passen
-- wenn es ein Code.Ring gibt, für den kein passendes Angler.Loch gefunden wird ...
if (
	select COUNT(*) 
	from Ringe
	Where CodeGuid = @codeGuid
	AND OLIs=3
	AND KnotenGuid not in
		(select KnotenGuid
		from Löcher
		where AnglerGuid = @AnglerGuid
		and Löcher.ILOS >= Ringe.get
		and (Ringe.ZweigGuid is null OR Ringe.ZweigGuid=Löcher.ZweigGuid))
) > 0
Begin
	--print 'Fehler bei muss'
	set @rc = -1
	return -1
end



-- ALLE bin müssen passen
-- wenn es ein Angler.Loch gibt, für das kein Code.Ring gefunden wird ...
if (
	select COUNT(*) 
	from Löcher
	Where AnglerGuid = @AnglerGuid
	AND ILOs=3
	AND KnotenGuid not in
		(select KnotenGuid
		from Ringe
		where CodeGuid = @CodeGuid
		and Ringe.OLIs >= Löcher.fit
		and (Löcher.ZweigGuid is null OR Löcher.ZweigGuid=Ringe.ZweigGuid))
) > 0
Begin
	--print 'Fehler bei bin'
	set @rc = -1
	return -1
end

-- KEIN sollte sollte unerfüllt sein (für Markierung auch mit Baum-Zweig)
-- Wenn für ein Ringe.Baum kein Zweig in den Bäumen der Löcher gefunden wird ...
if (
	select COUNT(*)
	from ringe 
	where CodeGuid = @CodeGuid
	and OLIs = 2
	and get >= 2
	and BaumGuid is not null
	and BaumGuid not in
		(
		select BaumGuid 
		from Löcher
		Where AnglerGuid = @AnglerGuid
		and KnotenGuid in
			(
			select KnotenGuid
			from Ringe
			where CodeGuid = @CodeGuid
			and OLIs = 2
			and Löcher.ZweigGuid = Ringe.ZweigGuid
			and Löcher.ILOs >= Ringe.get
			)
		)
) > 0
Begin
	--print 'Fehler bei sollte baum>0'
	set @rc = -1
	return -1
end		



-- KEIN waere darf unerfüllt bleiben
-- Wenn für ein Löcher.Baum kein Zweig in den Bäumen der Ringe gefunden wird ...
if 
(
	select COUNT(*)
	from Löcher 
	where AnglerGuid = @AnglerGuid
	and ILOs = 2
	and fit >= 2
	and BaumGuid IS NOT NULL
	and BaumGuid not in
		(
		select BaumGuid 
		from Ringe
		Where CodeGuid = @CodeGuid
		and KnotenGuid in
			(
			select KnotenGuid
			from Löcher
			where AnglerGuid = @AnglerGuid
			and ILOs = 2
			and Ringe.ZweigGuid = Löcher.ZweigGuid
			and Ringe.OLIs >= Löcher.fit
			)
		)
) > 0
Begin
	--print 'Fehler bei waere baum > 0'
	set @rc = -1
	return -1
end	

-- KEIN sollte sollte unerfüllt bleiben (Für Markierung nur in Netz-Knoten)
-- Wenn für ein Ringe.Baum kein Zweig in dem Netz der Löcher gefunden wird ...
if 
(
	select COUNT(*)
	from ringe 
	where CodeGuid = @CodeGuid
	and OLIs = 2
	and get >= 2
	and BaumGuid is null
	and NetzGuid not in
		(
		select NetzGuid 
		from Löcher
		Where AnglerGuid = @AnglerGuid
		and KnotenGuid in
			(
			select KnotenGuid
			from Ringe
			where CodeGuid = @CodeGuid
			and OLIs = 2			and Löcher.KnotenGuid = Ringe.KnotenGuid
			and Löcher.ILOs >= Ringe.get
			)
		)
) > 0
Begin
	--print 'Fehler bei sollte baum is null'
	set @rc = -1
	return -1
end	

-- KEIN waere darf unerfüllt bleiben (Bei Markierung nur in Netz-Knoten)
-- Wenn für ein Löcher.Baum kein Zweig in dem Netz der Ringe gefunden wird ...
if 
(
	select COUNT(*)
	from Löcher 
	where AnglerGuid = @AnglerGuid
	and ILOs = 2
	and fit >= 2
	and BaumGuid is null
	and NetzGuid not in
		(
		select NetzGuid 
		from Ringe
		Where CodeGuid = @CodeGuid
		and KnotenGuid in
			(
			select KnotenGuid
			from Löcher
			where AnglerGuid = @AnglerGuid
			and ILOs = 2
			and Ringe.KnotenGuid = Löcher.KnotenGuid
			and Ringe.OLIs >= Löcher.fit
			)
		)
) > 0

Begin
	--print 'Fehler bei waere baum is null'
	set @rc = -1
	return -1
end	




if @rc=0
	-- Print 'Code ' + convert(nvarchar,@CodeGuid) + ' beisst Angler ' + convert(nvarchar,@AnglerGuid)



return @rc





SET NOCOUNT OFF