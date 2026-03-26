using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Models;

namespace OLI_it.Web.Data;

public partial class OliItDbContext : DbContext
{
    public OliItDbContext()
    {
    }

    public OliItDbContext(DbContextOptions<OliItDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Angler> Anglers { get; set; }

    public virtual DbSet<AnglerPostIt> AnglerPostIts { get; set; }

    public virtual DbSet<Baum> Baums { get; set; }

    public virtual DbSet<CheckLöcher> CheckLöchers { get; set; }

    public virtual DbSet<CheckRinge> CheckRinges { get; set; }

    public virtual DbSet<CheckZweigWeiter> CheckZweigWeiters { get; set; }

    public virtual DbSet<Code> Codes { get; set; }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<Extra> Extras { get; set; }

    public virtual DbSet<FristAbgelaufen> FristAbgelaufens { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<Inbox> Inboxes { get; set; }

    public virtual DbSet<JournalAngler> JournalAnglers { get; set; }

    public virtual DbSet<JournalPostIt> JournalPostIts { get; set; }

    public virtual DbSet<JournalStamm> JournalStamms { get; set; }

    public virtual DbSet<JournalToll> JournalTolls { get; set; }

    public virtual DbSet<JournalTopLab> JournalTopLabs { get; set; }

    public virtual DbSet<Knoten> Knotens { get; set; }

    public virtual DbSet<Löcher> Löchers { get; set; }

    public virtual DbSet<Metalog> Metalogs { get; set; }

    public virtual DbSet<Netz> Netzs { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<PostIt> PostIts { get; set; }

    public virtual DbSet<PostItAngler> PostItAnglers { get; set; }

    public virtual DbSet<PostItAnzahlen> PostItAnzahlens { get; set; }

    public virtual DbSet<PostItCode> PostItCodes { get; set; }

    public virtual DbSet<PostItKonto> PostItKontos { get; set; }

    public virtual DbSet<PostItStamm> PostItStamms { get; set; }

    public virtual DbSet<PostItTopLab> PostItTopLabs { get; set; }

    public virtual DbSet<Provision> Provisions { get; set; }

    public virtual DbSet<Q> Qs { get; set; }

    public virtual DbSet<Ringe> Ringes { get; set; }

    public virtual DbSet<ServiceLog> ServiceLogs { get; set; }

    public virtual DbSet<ShortCut> ShortCuts { get; set; }

    public virtual DbSet<Spiegel> Spiegels { get; set; }

    public virtual DbSet<Stamm> Stamms { get; set; }

    public virtual DbSet<StammAngler> StammAnglers { get; set; }

    public virtual DbSet<StammDurchToll> StammDurchTolls { get; set; }

    public virtual DbSet<StammInbox> StammInboxes { get; set; }

    public virtual DbSet<StammKonto> StammKontos { get; set; }

    public virtual DbSet<StammNews> StammNews { get; set; }

    public virtual DbSet<StammPostIt> StammPostIts { get; set; }

    public virtual DbSet<StammPostItTopLabTolli> StammPostItTopLabTollis { get; set; }

    public virtual DbSet<StammTopLab> StammTopLabs { get; set; }

    public virtual DbSet<Models.String> Strings { get; set; }

    public virtual DbSet<TblTuerLog> TblTuerLogs { get; set; }

    public virtual DbSet<Tolli> Tollis { get; set; }

    public virtual DbSet<TopLab> TopLabs { get; set; }

    public virtual DbSet<TopLabTopLab> TopLabTopLabs { get; set; }

    public virtual DbSet<TuerLog> TuerLogs { get; set; }

    public virtual DbSet<UnionJournale> UnionJournales { get; set; }

    public virtual DbSet<Wurzeln> Wurzelns { get; set; }

    public virtual DbSet<Zweig> Zweigs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration is handled via dependency injection in Program.cs
        // Connection string is read from user secrets or appsettings.json
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Angler>(entity =>
        {
            entity.HasKey(e => e.AnglerGuid).HasName("PK_AnglerGuid");

            entity.ToTable("Angler", "oli");

            entity.Property(e => e.AnglerGuid).HasDefaultValueSql("(newid())", "DF_Angler_AnglerGuid");
            entity.Property(e => e.Angler1)
                .HasMaxLength(50)
                .HasColumnName("Angler");
            entity.Property(e => e.Beschreibung).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Gescannt).HasColumnName("gescannt");
            entity.Property(e => e.Versionsnummer).HasMaxLength(10);

            entity.HasOne(d => d.Stamm).WithMany(p => p.Anglers)
                .HasForeignKey(d => d.StammGuid)
                .HasConstraintName("FK_Angler_Stamm");
        });

        modelBuilder.Entity<AnglerPostIt>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("AnglerPostIt", "oli");

            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Gelesen)
                .HasColumnType("datetime")
                .HasColumnName("gelesen");
            entity.Property(e => e.KooK).HasColumnType("money");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.Typ)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<Baum>(entity =>
        {
            entity.HasKey(e => e.BaumGuid);

            entity.ToTable("Baum", "oli");

            entity.Property(e => e.BaumGuid).HasDefaultValueSql("(newid())", "DF_Baum_BaumGuid");
            entity.Property(e => e.Baum1)
                .HasMaxLength(255)
                .HasColumnName("Baum");
            entity.Property(e => e.Beschreibung).HasMaxLength(255);
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.EnDescription)
                .HasMaxLength(255)
                .HasColumnName("en_description");
            entity.Property(e => e.EnName)
                .HasMaxLength(255)
                .HasColumnName("en_name");
            entity.Property(e => e.Owner)
                .HasMaxLength(255)
                .HasColumnName("owner");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
        });

        modelBuilder.Entity<CheckLöcher>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("check_Löcher", "oli");

            entity.Property(e => e.Baum).HasMaxLength(255);
            entity.Property(e => e.Knoten).HasMaxLength(255);
            entity.Property(e => e.Netz).HasMaxLength(255);
            entity.Property(e => e.Zweig).HasMaxLength(255);
        });

        modelBuilder.Entity<CheckRinge>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("check_Ringe", "oli");

            entity.Property(e => e.Baum).HasMaxLength(255);
            entity.Property(e => e.Knoten).HasMaxLength(255);
            entity.Property(e => e.Netz).HasMaxLength(255);
            entity.Property(e => e.Zweig).HasMaxLength(255);
        });

        modelBuilder.Entity<CheckZweigWeiter>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("check_Zweig_weiter", "oli");

            entity.Property(e => e.Baum).HasMaxLength(255);
            entity.Property(e => e.Netz).HasMaxLength(255);
            entity.Property(e => e.WeiterBaumGuid).HasColumnName("weiterBaumGuid");
            entity.Property(e => e.WeiterNetzGuid).HasColumnName("weiterNetzGuid");
        });

        modelBuilder.Entity<Code>(entity =>
        {
            entity.HasKey(e => e.CodeGuid).HasName("PK_CodeGuid");

            entity.ToTable("Code", "oli");

            entity.HasIndex(e => e.PostItGuid, "Idx_PostItGuid");

            entity.Property(e => e.CodeGuid).HasDefaultValueSql("(newid())", "DF__Code__rowguid");
            entity.Property(e => e.Gescannt).HasColumnName("gescannt");
            entity.Property(e => e.Kommentar).HasMaxLength(250);
            entity.Property(e => e.Versionsnummer)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.PostIt).WithMany(p => p.Codes)
                .HasForeignKey(d => d.PostItGuid)
                .HasConstraintName("FK_Code_PostIt");
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.ToTable("counter", "oli");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Host)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("host");
            entity.Property(e => e.Ip)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ip");
            entity.Property(e => e.OliUser)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Ref)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ref");
            entity.Property(e => e.Site)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("site");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("url");
            entity.Property(e => e.Zeit)
                .HasDefaultValueSql("(getdate())", "DF_counter_zeit")
                .HasColumnType("datetime")
                .HasColumnName("zeit");
        });

        modelBuilder.Entity<Extra>(entity =>
        {
            entity.HasKey(e => e.ExtrasGuid);

            entity.ToTable("Extras", "oli");

            entity.Property(e => e.ExtrasGuid).HasDefaultValueSql("(newid())", "DF__Extras__rowguid");
            entity.Property(e => e.Descend)
                .HasDefaultValue(true, "DF_Extras_descend")
                .HasColumnName("descend");
            entity.Property(e => e.Freakmode).HasColumnName("freakmode");
            entity.Property(e => e.Fristablauf)
                .HasDefaultValue(true, "DF_Extras_fristablauf")
                .HasColumnName("fristablauf");
            entity.Property(e => e.Gutschrift)
                .HasDefaultValue(true, "DF_Extras_gutschrift")
                .HasColumnName("gutschrift");
            entity.Property(e => e.Hilfe)
                .HasDefaultValue(true, "DF_Extras_hilfe")
                .HasColumnName("hilfe");
            entity.Property(e => e.HtmlMail).HasDefaultValue(true, "DF_Extras_HtmlMail");
            entity.Property(e => e.NewP).HasColumnName("newP");
            entity.Property(e => e.NewT)
                .HasDefaultValue(true, "DF_Extras_newT")
                .HasColumnName("newT");
            entity.Property(e => e.Newsletter)
                .HasDefaultValue(true, "DF_Extras_newsletter")
                .HasColumnName("newsletter");
            entity.Property(e => e.Showclosed).HasColumnName("showclosed");
            entity.Property(e => e.Sort)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("datum", "DF_Extras_sort")
                .HasColumnName("sort");
            entity.Property(e => e.Tipps)
                .HasDefaultValue(true, "DF_Extras_tipps")
                .HasColumnName("tipps");
            entity.Property(e => e.TxtOnly).HasDefaultValue(true, "DF_Extras_TxtOnly");
            entity.Property(e => e.Werbefrei).HasColumnName("werbefrei");
            entity.Property(e => e.ZeilenZahl).HasDefaultValue(5, "DF_Extras_ZeilenZahl");

            entity.HasOne(d => d.Stamm).WithMany(p => p.Extras)
                .HasForeignKey(d => d.StammGuid)
                .HasConstraintName("FK_Extras_Stamm");
        });

        modelBuilder.Entity<FristAbgelaufen>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("FristAbgelaufen", "oli");

            entity.Property(e => e.Bezahlt)
                .HasColumnType("money")
                .HasColumnName("bezahlt");
            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.EMail)
                .HasMaxLength(50)
                .HasColumnName("eMail");
            entity.Property(e => e.Frist).HasColumnType("datetime");
            entity.Property(e => e.Gemailt).HasColumnName("gemailt");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Stamm).HasMaxLength(50);
            entity.Property(e => e.Titel).HasMaxLength(255);
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.HistoryGuid);

            entity.ToTable("History", "oli");

            entity.Property(e => e.HistoryGuid).ValueGeneratedNever();
            entity.Property(e => e.BoundKooK).HasColumnType("money");
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.FlowKooK).HasColumnType("money");
            entity.Property(e => e.Provision).HasColumnType("money");
            entity.Property(e => e.SummePostItKonto).HasColumnType("money");
            entity.Property(e => e.SummeStammKonto).HasColumnType("money");
        });

        modelBuilder.Entity<Inbox>(entity =>
        {
            entity.HasKey(e => e.InboxGuid);

            entity.ToTable("Inbox", "oli");

            entity.Property(e => e.InboxGuid).HasDefaultValueSql("(newid())", "DF__INBOX__rowguid");
            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(getdate())", "DF_INBOX_Datum")
                .HasColumnType("datetime");
            entity.Property(e => e.Gelesen)
                .HasColumnType("datetime")
                .HasColumnName("gelesen");
            entity.Property(e => e.Gemailt)
                .HasColumnType("datetime")
                .HasColumnName("gemailt");
            entity.Property(e => e.Gesehen)
                .HasColumnType("datetime")
                .HasColumnName("gesehen");

            entity.HasOne(d => d.Stamm).WithMany(p => p.Inboxes)
                .HasForeignKey(d => d.StammGuid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Inbox_Stamm");

            entity.HasOne(d => d.TopLab).WithMany(p => p.Inboxes)
                .HasForeignKey(d => d.TopLabGuid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Inbox_TopLab");
        });

        modelBuilder.Entity<JournalAngler>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("JournalAngler", "oli");

            entity.Property(e => e.Datei)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Titel).HasMaxLength(50);
            entity.Property(e => e.Wert).HasMaxLength(255);
            entity.Property(e => e.Zeichen)
                .HasMaxLength(1)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JournalPostIt>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("JournalPostIt", "oli");

            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.Wert)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Zeichen)
                .HasMaxLength(1)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JournalStamm>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("JournalStamm", "oli");

            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Titel).HasMaxLength(50);
            entity.Property(e => e.Wert).HasMaxLength(255);
            entity.Property(e => e.Zeichen)
                .HasMaxLength(1)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JournalToll>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("JournalToll", "oli");

            entity.Property(e => e.Datei)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Datum)
                .HasColumnType("datetime")
                .HasColumnName("datum");
            entity.Property(e => e.Titel).HasMaxLength(31);
            entity.Property(e => e.Wert).HasMaxLength(255);
            entity.Property(e => e.Zeichen)
                .HasMaxLength(1)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JournalTopLab>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("JournalTopLab", "oli");

            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.Wert).HasMaxLength(3000);
            entity.Property(e => e.Zeichen)
                .HasMaxLength(1)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Knoten>(entity =>
        {
            entity.HasKey(e => e.KnotenGuid);

            entity.ToTable("Knoten", "oli");

            entity.Property(e => e.KnotenGuid).HasDefaultValueSql("(newid())", "DF_Knoten_KnotenGuid");
            entity.Property(e => e.Beschreibung).HasMaxLength(3000);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.EnDescription)
                .HasMaxLength(255)
                .HasColumnName("en_description");
            entity.Property(e => e.EnName)
                .HasMaxLength(255)
                .HasColumnName("en_name");
            entity.Property(e => e.Knoten1)
                .HasMaxLength(255)
                .HasColumnName("Knoten");
            entity.Property(e => e.Owner)
                .HasMaxLength(255)
                .HasColumnName("owner");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
            entity.Property(e => e.VgbIlos).HasColumnName("VgbILOs");
            entity.Property(e => e.VgbOlis).HasColumnName("VgbOLIs");
            entity.Property(e => e.WeiterBaumGuid).HasColumnName("weiterBaumGuid");
            entity.Property(e => e.WeiterNetzGuid).HasColumnName("weiterNetzGuid");

            entity.HasOne(d => d.Netz).WithMany(p => p.Knotens)
                .HasForeignKey(d => d.NetzGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Knoten_Netz");
        });

        modelBuilder.Entity<Löcher>(entity =>
        {
            entity.HasKey(e => e.LochGuid);

            entity.ToTable("Löcher", "oli");

            entity.HasIndex(e => new { e.AnglerGuid, e.NetzGuid, e.KnotenGuid, e.BaumGuid, e.ZweigGuid, e.Ilos, e.Fit }, "Idx_Löcher_AnglerGuid");

            entity.Property(e => e.LochGuid).HasDefaultValueSql("(newid())", "DF_Löcher_LochGuid");
            entity.Property(e => e.Ilos).HasColumnName("ILOs");

            entity.HasOne(d => d.Angler).WithMany(p => p.Löchers)
                .HasForeignKey(d => d.AnglerGuid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Löcher_Angler");
        });

        modelBuilder.Entity<Metalog>(entity =>
        {
            entity.ToTable("metalog", "oli");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Host)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("host");
            entity.Property(e => e.Ip)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ip");
            entity.Property(e => e.OliUser)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Ref)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ref");
            entity.Property(e => e.Site)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("site");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("url");
            entity.Property(e => e.Zeit)
                .HasDefaultValueSql("(getdate())", "DF_metalog_zeit")
                .HasColumnType("datetime")
                .HasColumnName("zeit");
        });

        modelBuilder.Entity<Netz>(entity =>
        {
            entity.HasKey(e => e.NetzGuid);

            entity.ToTable("Netz", "oli");

            entity.Property(e => e.NetzGuid).HasDefaultValueSql("(newid())", "DF_Netz_NetzGuid");
            entity.Property(e => e.Beschreibung).HasMaxLength(255);
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.EnDescription)
                .HasMaxLength(255)
                .HasColumnName("en_description");
            entity.Property(e => e.EnName)
                .HasMaxLength(255)
                .HasColumnName("en_name");
            entity.Property(e => e.Netz1)
                .HasMaxLength(255)
                .HasColumnName("Netz");
            entity.Property(e => e.Owner)
                .HasMaxLength(255)
                .HasColumnName("owner");
            entity.Property(e => e.Rdf).HasColumnName("RDF");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsGuid);

            entity.ToTable("News", "oli");

            entity.Property(e => e.NewsGuid).HasDefaultValueSql("(newid())", "DF_News_NewsGuid");
            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(getdate())", "DF_News_Datum")
                .HasColumnType("datetime");
            entity.Property(e => e.Gelesen)
                .HasColumnType("datetime")
                .HasColumnName("gelesen");
            entity.Property(e => e.Gemailt)
                .HasColumnType("datetime")
                .HasColumnName("gemailt");
            entity.Property(e => e.Gesehen)
                .HasColumnType("datetime")
                .HasColumnName("gesehen");

            entity.HasOne(d => d.Angler).WithMany(p => p.News)
                .HasForeignKey(d => d.AnglerGuid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_News_Angler");

            entity.HasOne(d => d.Code).WithMany(p => p.News)
                .HasForeignKey(d => d.CodeGuid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_News_Code");
        });

        modelBuilder.Entity<PostIt>(entity =>
        {
            entity.HasKey(e => e.PostItGuid).HasName("PK_PostItGuid");

            entity.ToTable("PostIt", "oli");

            entity.Property(e => e.PostItGuid).HasDefaultValueSql("(newid())", "DF_PostIt_rowguid");
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(getdate())", "DF_PostIt_Datum")
                .HasColumnType("datetime");
            entity.Property(e => e.KooK).HasColumnType("money");
            entity.Property(e => e.PostIt1)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("PostIt");
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.Typ)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<PostItAngler>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("PostItAngler", "oli");

            entity.Property(e => e.Angler).HasMaxLength(50);
            entity.Property(e => e.Beschreibung).HasMaxLength(255);
            entity.Property(e => e.Pdatei)
                .HasMaxLength(255)
                .HasColumnName("PDatei");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Sdatei).HasMaxLength(255);
            entity.Property(e => e.Stamm).HasMaxLength(50);
            entity.Property(e => e.Titel).HasMaxLength(255);
        });

        modelBuilder.Entity<PostItAnzahlen>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("PostIt_Anzahlen", "oli");

            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.KooK).HasColumnType("money");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.Typ)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<PostItCode>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("PostItCode", "oli");

            entity.Property(e => e.Gescannt).HasColumnName("gescannt");
            entity.Property(e => e.Kommentar).HasMaxLength(250);
            entity.Property(e => e.Versionsnummer)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<PostItKonto>(entity =>
        {
            entity.ToTable("PostItKonto", "oli");

            entity.Property(e => e.PostItKontoId).HasColumnName("PostItKontoID");
            entity.Property(e => e.Betrag).HasColumnType("money");
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Kommentar)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PostItStamm>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("PostItStamm", "oli");

            entity.Property(e => e.Bezahlt)
                .HasColumnType("money")
                .HasColumnName("bezahlt");
            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Frist).HasColumnType("datetime");
            entity.Property(e => e.KooK).HasColumnType("money");
            entity.Property(e => e.Stamm).HasMaxLength(50);
        });

        modelBuilder.Entity<PostItTopLab>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("PostItTopLab", "oli");

            entity.Property(e => e.Datum)
                .HasColumnType("datetime")
                .HasColumnName("datum");
            entity.Property(e => e.Lohn).HasColumnType("money");
            entity.Property(e => e.Pdatei)
                .HasMaxLength(255)
                .HasColumnName("pdatei");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Purl)
                .HasMaxLength(255)
                .HasColumnName("PURL");
            entity.Property(e => e.Sdatei)
                .HasMaxLength(255)
                .HasColumnName("sdatei");
            entity.Property(e => e.Stamm).HasMaxLength(50);
            entity.Property(e => e.Tdatei)
                .HasMaxLength(255)
                .HasColumnName("tdatei");
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.TopLab).HasMaxLength(3000);
            entity.Property(e => e.Turl)
                .HasMaxLength(255)
                .HasColumnName("TURL");
        });

        modelBuilder.Entity<Provision>(entity =>
        {
            entity.HasKey(e => e.ProvisionGuid);

            entity.ToTable("Provision", "oli");

            entity.Property(e => e.ProvisionGuid).HasDefaultValueSql("(newid())", "DF_Provision_ProvisionGuid");
            entity.Property(e => e.Betrag).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(getdate())", "DF_Provision_Datum")
                .HasColumnType("datetime");

            entity.HasOne(d => d.PostIt).WithMany(p => p.Provisions)
                .HasForeignKey(d => d.PostItGuid)
                .HasConstraintName("FK_Provision_PostIt");

            entity.HasOne(d => d.Stamm).WithMany(p => p.Provisions)
                .HasForeignKey(d => d.StammGuid)
                .HasConstraintName("FK_Provision_Stamm");
        });

        modelBuilder.Entity<Q>(entity =>
        {
            entity.ToTable("Q", "oli");

            entity.Property(e => e.Qid).HasColumnName("QID");
            entity.Property(e => e.A)
                .HasMaxLength(50)
                .HasDefaultValue("angleR", "DF_Q_A");
            entity.Property(e => e.AL)
                .HasMaxLength(50)
                .HasColumnName("A_L");
            entity.Property(e => e.AX)
                .HasMaxLength(50)
                .HasColumnName("A_X");
            entity.Property(e => e.B)
                .HasMaxLength(50)
                .HasDefaultValue("baum", "DF_Q_B");
            entity.Property(e => e.C)
                .HasMaxLength(50)
                .HasDefaultValue("code", "DF_Q_C");
            entity.Property(e => e.CR)
                .HasMaxLength(50)
                .HasColumnName("C_R");
            entity.Property(e => e.CX)
                .HasMaxLength(50)
                .HasColumnName("C_X");
            entity.Property(e => e.F)
                .HasMaxLength(50)
                .HasDefaultValue("fit", "DF_Q_f")
                .HasColumnName("f");
            entity.Property(e => e.G)
                .HasMaxLength(50)
                .HasDefaultValue("get", "DF_Q_g")
                .HasColumnName("g");
            entity.Property(e => e.I)
                .HasMaxLength(50)
                .HasDefaultValue("ilos", "DF_Q_I");
            entity.Property(e => e.K)
                .HasMaxLength(50)
                .HasDefaultValue("knoten", "DF_Q_K");
            entity.Property(e => e.L)
                .HasMaxLength(50)
                .HasDefaultValue("löcher", "DF_Q_L");
            entity.Property(e => e.N)
                .HasMaxLength(50)
                .HasDefaultValue("netz", "DF_Q_N");
            entity.Property(e => e.O)
                .HasMaxLength(50)
                .HasDefaultValue("olis", "DF_Q_O");
            entity.Property(e => e.P)
                .HasMaxLength(50)
                .HasDefaultValue("postit", "DF_Q_P");
            entity.Property(e => e.PC)
                .HasMaxLength(50)
                .HasColumnName("P_C");
            entity.Property(e => e.PS)
                .HasMaxLength(50)
                .HasColumnName("P_S");
            entity.Property(e => e.PT)
                .HasMaxLength(50)
                .HasColumnName("P_T");
            entity.Property(e => e.PX)
                .HasMaxLength(50)
                .HasColumnName("P_X");
            entity.Property(e => e.Q1)
                .HasMaxLength(50)
                .HasColumnName("Q");
            entity.Property(e => e.R)
                .HasMaxLength(50)
                .HasDefaultValue("ringe", "DF_Q_R");
            entity.Property(e => e.S)
                .HasMaxLength(50)
                .HasDefaultValue("stamm", "DF_Q_S");
            entity.Property(e => e.SA)
                .HasMaxLength(50)
                .HasColumnName("S_A");
            entity.Property(e => e.SP)
                .HasMaxLength(50)
                .HasDefaultValue("", "DF_Q_S_P")
                .HasColumnName("S_P");
            entity.Property(e => e.ST)
                .HasMaxLength(50)
                .HasColumnName("S_T");
            entity.Property(e => e.T)
                .HasMaxLength(50)
                .HasDefaultValue("toplab", "DF_Q_T");
            entity.Property(e => e.W)
                .HasMaxLength(50)
                .HasDefaultValue("wurzel", "DF_Q_w")
                .HasColumnName("w");
            entity.Property(e => e.X)
                .HasMaxLength(50)
                .HasDefaultValue("x", "DF_Q_x")
                .HasColumnName("x");
            entity.Property(e => e.XT)
                .HasMaxLength(50)
                .HasColumnName("X_T");
            entity.Property(e => e.Z)
                .HasMaxLength(50)
                .HasDefaultValue("zweig", "DF_Q_Z");
        });

        modelBuilder.Entity<Ringe>(entity =>
        {
            entity.HasKey(e => e.RingGuid);

            entity.ToTable("Ringe", "oli");

            entity.HasIndex(e => new { e.CodeGuid, e.NetzGuid, e.KnotenGuid, e.BaumGuid, e.ZweigGuid, e.Olis, e.Get }, "Idx_Ringe_CodeGuid");

            entity.Property(e => e.RingGuid).HasDefaultValueSql("(newid())", "DF_Ringe_RingGuid");
            entity.Property(e => e.Olis).HasColumnName("OLIs");

            entity.HasOne(d => d.Code).WithMany(p => p.Ringes)
                .HasForeignKey(d => d.CodeGuid)
                .HasConstraintName("FK_Ringe_Code");
        });

        modelBuilder.Entity<ServiceLog>(entity =>
        {
            entity.ToTable("ServiceLog", "oli");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Datum)
                .HasColumnType("datetime")
                .HasColumnName("datum");
            entity.Property(e => e.Emailid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("emailid");
            entity.Property(e => e.Ip)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ip");
            entity.Property(e => e.Kommentar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("kommentar");
            entity.Property(e => e.Service)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("service");
            entity.Property(e => e.Stamm)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("stamm");
            entity.Property(e => e.Target)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("target");
        });

        modelBuilder.Entity<ShortCut>(entity =>
        {
            entity.HasKey(e => e.ShortCutsGuid);

            entity.ToTable("ShortCuts", "oli");

            entity.Property(e => e.ShortCutsGuid).HasDefaultValueSql("(newid())", "DF_ShortCuts_ShortCutsGuid");
            entity.Property(e => e.Auto).HasColumnName("auto");
            entity.Property(e => e.ShortCut1)
                .HasMaxLength(50)
                .HasColumnName("ShortCut");

            entity.HasOne(d => d.Stamm).WithMany(p => p.ShortCuts)
                .HasForeignKey(d => d.StammGuid)
                .HasConstraintName("FK_ShortCuts_Stamm");
        });

        modelBuilder.Entity<Spiegel>(entity =>
        {
            entity.HasKey(e => new { e.CodeGuid, e.AnglerGuid }).HasName("PK_CodeGuid_AnglerGuid");

            entity.ToTable("Spiegel", "oli");

            entity.HasIndex(e => e.AnglerGuid, "Idx_Spiegel_Angler");

            entity.Property(e => e.Gelesen)
                .HasColumnType("datetime")
                .HasColumnName("gelesen");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Zeit)
                .HasDefaultValueSql("(getdate())", "DF__Spiegel_Zeit")
                .HasColumnType("datetime")
                .HasColumnName("zeit");

            entity.HasOne(d => d.Angler).WithMany(p => p.Spiegels)
                .HasForeignKey(d => d.AnglerGuid)
                .HasConstraintName("FK_Spiegel_Angler");

            entity.HasOne(d => d.Code).WithMany(p => p.Spiegels)
                .HasForeignKey(d => d.CodeGuid)
                .HasConstraintName("FK_Spiegel_Code");
        });

        modelBuilder.Entity<Stamm>(entity =>
        {
            entity.HasKey(e => e.StammGuid).HasName("PK_StammGuid");

            entity.ToTable("Stamm", "oli");

            entity.HasIndex(e => e.Stamm1, "IX_Stamm").IsUnique();

            entity.Property(e => e.StammGuid).HasDefaultValueSql("(newid())", "DF_Stamm_rowguid");
            entity.Property(e => e.Bank).HasMaxLength(50);
            entity.Property(e => e.Beschreibung).HasMaxLength(255);
            entity.Property(e => e.Blz)
                .HasMaxLength(50)
                .HasColumnName("BLZ");
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(getdate())", "DF_Stamm_Datum")
                .HasColumnType("datetime");
            entity.Property(e => e.EMail)
                .HasMaxLength(50)
                .HasColumnName("eMail");
            entity.Property(e => e.GebDate)
                .HasColumnType("datetime")
                .HasColumnName("gebDate");
            entity.Property(e => e.KooK).HasColumnType("money");
            entity.Property(e => e.Kto).HasMaxLength(50);
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Stamm1)
                .HasMaxLength(50)
                .HasColumnName("Stamm");
            entity.Property(e => e.Tel).HasMaxLength(50);
            entity.Property(e => e.Unterschrift).HasMaxLength(50);
            entity.Property(e => e.Versionsnummer).HasMaxLength(10);
            entity.Property(e => e.ZuQid)
                .HasDefaultValue(1, "DF_Stamm_zuQID")
                .HasColumnName("zuQID");
        });

        modelBuilder.Entity<StammAngler>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StammAngler", "oli");

            entity.Property(e => e.Angler).HasMaxLength(50);
            entity.Property(e => e.Beschreibung).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.SumT).HasColumnType("money");
        });

        modelBuilder.Entity<StammDurchToll>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StammDurchToll", "oli");

            entity.Property(e => e.Stamm).HasMaxLength(50);
        });

        modelBuilder.Entity<StammInbox>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StammInbox", "oli");

            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Gelesen)
                .HasColumnType("datetime")
                .HasColumnName("gelesen");
            entity.Property(e => e.Gesehen)
                .HasColumnType("datetime")
                .HasColumnName("gesehen");
            entity.Property(e => e.Pdatei)
                .HasMaxLength(255)
                .HasColumnName("PDatei");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Ptitel)
                .HasMaxLength(255)
                .HasColumnName("PTitel");
            entity.Property(e => e.Tdatei)
                .HasMaxLength(255)
                .HasColumnName("TDatei");
            entity.Property(e => e.Tdatum)
                .HasColumnType("datetime")
                .HasColumnName("TDatum");
            entity.Property(e => e.TopLab).HasMaxLength(3000);
            entity.Property(e => e.Ttitel)
                .HasMaxLength(255)
                .HasColumnName("TTitel");
        });

        modelBuilder.Entity<StammKonto>(entity =>
        {
            entity.ToTable("StammKonto", "oli");

            entity.Property(e => e.StammKontoId).HasColumnName("StammKontoID");
            entity.Property(e => e.Betrag).HasColumnType("money");
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Kommentar)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StammNews>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StammNews", "oli");

            entity.Property(e => e.Angler).HasMaxLength(50);
            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Gelesen)
                .HasColumnType("datetime")
                .HasColumnName("gelesen");
            entity.Property(e => e.Gesehen)
                .HasColumnType("datetime")
                .HasColumnName("gesehen");
            entity.Property(e => e.KooK).HasColumnType("money");
            entity.Property(e => e.Pdatum)
                .HasColumnType("datetime")
                .HasColumnName("PDatum");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Titel).HasMaxLength(255);
        });

        modelBuilder.Entity<StammPostIt>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StammPostIt", "oli");

            entity.Property(e => e.Bezahlt)
                .HasColumnType("money")
                .HasColumnName("bezahlt");
            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Frist).HasColumnType("datetime");
            entity.Property(e => e.Gemailt).HasColumnName("gemailt");
            entity.Property(e => e.KooK).HasColumnType("money");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.Typ)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("URL");
            entity.Property(e => e.Wclosed).HasColumnName("wclosed");
        });

        modelBuilder.Entity<StammPostItTopLabTolli>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StammPostItTopLabTollis", "oli");

            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Stamm).HasMaxLength(50);
            entity.Property(e => e.TopLab).HasMaxLength(3000);
            entity.Property(e => e.Tstamm)
                .HasMaxLength(50)
                .HasColumnName("TStamm");
            entity.Property(e => e.TstammGuid).HasColumnName("TStammGuid");
        });

        modelBuilder.Entity<StammTopLab>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StammTopLab", "oli");

            entity.Property(e => e.KooK).HasColumnType("money");
            entity.Property(e => e.Lohn).HasColumnType("money");
            entity.Property(e => e.Pdatei).HasMaxLength(255);
            entity.Property(e => e.Pdatum).HasColumnType("datetime");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Ptitel)
                .HasMaxLength(255)
                .HasColumnName("PTitel");
            entity.Property(e => e.Purl)
                .HasMaxLength(255)
                .HasColumnName("PURL");
            entity.Property(e => e.Tdatei).HasMaxLength(255);
            entity.Property(e => e.Tdatum).HasColumnType("datetime");
            entity.Property(e => e.TopLab).HasMaxLength(3000);
            entity.Property(e => e.Ttitel)
                .HasMaxLength(255)
                .HasColumnName("TTitel");
            entity.Property(e => e.Turl)
                .HasMaxLength(255)
                .HasColumnName("TURL");
        });

        modelBuilder.Entity<Models.String>(entity =>
        {
            entity.HasKey(e => e.StringsGuid);

            entity.ToTable("Strings", "oli");

            entity.Property(e => e.StringsGuid).HasDefaultValueSql("(newid())", "DF_Strings_StringsGuid");

            entity.HasOne(d => d.ShortCuts).WithMany(p => p.Strings)
                .HasForeignKey(d => d.ShortCutsGuid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Strings_ShortCuts");
        });

        modelBuilder.Entity<TblTuerLog>(entity =>
        {
            entity.HasKey(e => e.TuerLogId);

            entity.ToTable("tblTuerLog", "oli");

            entity.Property(e => e.TuerLogId).HasColumnName("TuerLogID");
            entity.Property(e => e.Aguid).HasColumnName("aguid");
            entity.Property(e => e.Cguid).HasColumnName("cguid");
            entity.Property(e => e.Datum)
                .HasColumnType("datetime")
                .HasColumnName("datum");
            entity.Property(e => e.Eglsguid).HasColumnName("eglsguid");
            entity.Property(e => e.Host)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("host");
            entity.Property(e => e.Ip)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ip");
            entity.Property(e => e.Kommentar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("kommentar");
            entity.Property(e => e.Pguid).HasColumnName("pguid");
            entity.Property(e => e.Sguid).HasColumnName("sguid");
            entity.Property(e => e.Tguid).HasColumnName("tguid");
        });

        modelBuilder.Entity<Tolli>(entity =>
        {
            entity.HasKey(e => new { e.StammGuid, e.TopLabGuid });

            entity.ToTable("Tollis", "oli");

            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(getdate())", "DF_Tollis_datum")
                .HasColumnType("datetime")
                .HasColumnName("datum");
            entity.Property(e => e.TollText).HasMaxLength(255);

            entity.HasOne(d => d.Stamm).WithMany(p => p.Tollis)
                .HasForeignKey(d => d.StammGuid)
                .HasConstraintName("FK_Tollis_Stamm");

            entity.HasOne(d => d.TopLab).WithMany(p => p.Tollis)
                .HasForeignKey(d => d.TopLabGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tollis_TopLab");
        });

        modelBuilder.Entity<TopLab>(entity =>
        {
            entity.HasKey(e => e.TopLabGuid).HasName("PK_TopLabGuid");

            entity.ToTable("TopLab", "oli");

            entity.HasIndex(e => e.PostItGuid, "Idx_TopLab_PostItGuid");

            entity.HasIndex(e => e.StammGuid, "Idx_TopLab_StammGuid");

            entity.HasIndex(e => e.TopTopLabGuid, "Idx_TopTopLabGuid");

            entity.Property(e => e.TopLabGuid).HasDefaultValueSql("(newid())", "DF__TopLab__rowguid");
            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(getdate())", "DF_TopLab_datum")
                .HasColumnType("datetime");
            entity.Property(e => e.Lohn).HasColumnType("money");
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.TopLab1)
                .HasMaxLength(3000)
                .HasColumnName("TopLab");
            entity.Property(e => e.Typ)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("URL");

            entity.HasOne(d => d.PostIt).WithMany(p => p.TopLabs)
                .HasForeignKey(d => d.PostItGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TopLab_PostIt");

            entity.HasOne(d => d.Stamm).WithMany(p => p.TopLabs)
                .HasForeignKey(d => d.StammGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TopLab_Stamm");

            entity.HasOne(d => d.TopTopLab).WithMany(p => p.InverseTopTopLab)
                .HasForeignKey(d => d.TopTopLabGuid)
                .HasConstraintName("FK_TopLab_TopLab");
        });

        modelBuilder.Entity<TopLabTopLab>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TopLabTopLab", "oli");

            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Lohn).HasColumnType("money");
            entity.Property(e => e.Pdatei)
                .HasMaxLength(255)
                .HasColumnName("pdatei");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Purl)
                .HasMaxLength(255)
                .HasColumnName("PURL");
            entity.Property(e => e.Sdatei)
                .HasMaxLength(255)
                .HasColumnName("sdatei");
            entity.Property(e => e.Stamm).HasMaxLength(50);
            entity.Property(e => e.Tdatei)
                .HasMaxLength(255)
                .HasColumnName("tdatei");
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.TopLab).HasMaxLength(3000);
            entity.Property(e => e.Turl)
                .HasMaxLength(255)
                .HasColumnName("TURL");
        });

        modelBuilder.Entity<TuerLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TuerLog", "oli");

            entity.Property(e => e.Angler).HasMaxLength(50);
            entity.Property(e => e.Datum)
                .HasColumnType("datetime")
                .HasColumnName("datum");
            entity.Property(e => e.EglStamm)
                .HasMaxLength(50)
                .HasColumnName("eglStamm");
            entity.Property(e => e.Host)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("host");
            entity.Property(e => e.Ip)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ip");
            entity.Property(e => e.Kommentar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("kommentar");
            entity.Property(e => e.PostIt)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Stamm).HasMaxLength(50);
            entity.Property(e => e.TopLab).HasMaxLength(3000);
        });

        modelBuilder.Entity<UnionJournale>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UnionJournale", "oli");

            entity.Property(e => e.Datei).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Titel).HasMaxLength(255);
            entity.Property(e => e.Wert).HasMaxLength(3000);
            entity.Property(e => e.Zeichen)
                .HasMaxLength(1)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Wurzeln>(entity =>
        {
            entity.HasKey(e => new { e.StammGuid, e.PostItGuid }).HasName("PK_StammGuid_PostItGuid");

            entity.ToTable("Wurzeln", "oli");

            entity.HasIndex(e => e.PostItGuid, "Idx_Wurzeln_PostItGuid");

            entity.HasIndex(e => new { e.PostItGuid, e.Closed }, "Idx_Wurzeln_PostItGuid_closed");

            entity.Property(e => e.Bezahlt)
                .HasColumnType("money")
                .HasColumnName("bezahlt");
            entity.Property(e => e.Closed).HasColumnName("closed");
            entity.Property(e => e.Frist).HasColumnType("datetime");
            entity.Property(e => e.Gemailt).HasColumnName("gemailt");
            entity.Property(e => e.StammZust).HasDefaultValue(1, "DF_Wurzeln_StammZust");

            entity.HasOne(d => d.PostIt).WithMany(p => p.Wurzelns)
                .HasForeignKey(d => d.PostItGuid)
                .HasConstraintName("FK_Wurzeln_PostIt");

            entity.HasOne(d => d.Stamm).WithMany(p => p.Wurzelns)
                .HasForeignKey(d => d.StammGuid)
                .HasConstraintName("FK_Wurzeln_Stamm");
        });

        modelBuilder.Entity<Zweig>(entity =>
        {
            entity.HasKey(e => e.ZweigGuid);

            entity.ToTable("Zweig", "oli");

            entity.Property(e => e.ZweigGuid).HasDefaultValueSql("(newid())", "DF_Zweig_ZweigGuid");
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.EnName)
                .HasMaxLength(255)
                .HasColumnName("en_name");
            entity.Property(e => e.Owner)
                .HasMaxLength(255)
                .HasColumnName("owner");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
            entity.Property(e => e.WeiterBaumGuid).HasColumnName("weiterBaumGuid");
            entity.Property(e => e.WeiterNetzGuid).HasColumnName("weiterNetzGuid");
            entity.Property(e => e.Zweig1)
                .HasMaxLength(255)
                .HasColumnName("Zweig");

            entity.HasOne(d => d.Baum).WithMany(p => p.Zweigs)
                .HasForeignKey(d => d.BaumGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zweig_Baum");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
