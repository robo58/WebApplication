using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication.Models
{
    public partial class PI10Context : DbContext
    {
        public PI10Context()
        {
        }

        public PI10Context(DbContextOptions<PI10Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Boje> Boje { get; set; }
        public virtual DbSet<Certifikati> Certifikati { get; set; }
        public virtual DbSet<DodatnaOprema> DodatnaOprema { get; set; }
        public virtual DbSet<Kategorije> Kategorije { get; set; }
        public virtual DbSet<Klijenti> Klijenti { get; set; }
        public virtual DbSet<Mjenjaci> Mjenjaci { get; set; }
        public virtual DbSet<Modeli> Modeli { get; set; }
        public virtual DbSet<Odjeli> Odjeli { get; set; }
        public virtual DbSet<Osobe> Osobe { get; set; }
        public virtual DbSet<Ponuda> Ponuda { get; set; }
        public virtual DbSet<PonudaVozac> PonudaVozac { get; set; }
        public virtual DbSet<PonudaVozilo> PonudaVozilo { get; set; }
        public virtual DbSet<Profili> Profili { get; set; }
        public virtual DbSet<Proizvodjaci> Proizvodjaci { get; set; }
        public virtual DbSet<Slike> Slike { get; set; }
        public virtual DbSet<SlikeVozila> SlikeVozila { get; set; }
        public virtual DbSet<Specifikacije> Specifikacije { get; set; }
        public virtual DbSet<Tvrtke> Tvrtke { get; set; }
        public virtual DbSet<Usluge> Usluge { get; set; }
        public virtual DbSet<Vozila> Vozila { get; set; }
        public virtual DbSet<VrsteGoriva> VrsteGoriva { get; set; }
        public virtual DbSet<Zahtjev> Zahtjev { get; set; }
        public virtual DbSet<Zaposlenici> Zaposlenici { get; set; }
        public virtual DbSet<ZaposleniciCertifikati> ZaposleniciCertifikati { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=rppp.fer.hr,3000;Database=PI-10;User Id=pi10;Password=M-A-N-G.O");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Boje>(entity =>
            {
                entity.HasKey(x => x.IdBoje)
                    .HasName("PK__boje__DAD8CDB485A69935");

                entity.ToTable("boje");

                entity.Property(e => e.IdBoje)
                    .HasColumnName("id_boje")
                    .ValueGeneratedNever();

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Certifikati>(entity =>
            {
                entity.HasKey(x => x.IdCertifikata)
                    .HasName("PK__certifik__CC4F424D8EF4C127");

                entity.ToTable("certifikati");

                entity.Property(e => e.IdCertifikata)
                    .HasColumnName("id_certifikata")
                    .ValueGeneratedNever();

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DodatnaOprema>(entity =>
            {
                entity.HasKey(x => x.IdDodatneOpreme)
                    .HasName("PK__dodatna___2B2D8F63A44682FE");

                entity.ToTable("dodatna_oprema");

                entity.Property(e => e.IdDodatneOpreme)
                    .HasColumnName("id_dodatne_opreme")
                    .ValueGeneratedNever();

                entity.Property(e => e.Klima).HasColumnName("klima");

                entity.Property(e => e.KozniSicevi).HasColumnName("kozni_sicevi");

                entity.Property(e => e.Siber).HasColumnName("siber");
            });

            modelBuilder.Entity<Kategorije>(entity =>
            {
                entity.HasKey(x => x.IdKategorije)
                    .HasName("PK__kategori__A086CBE6E4C5315C");

                entity.ToTable("kategorije");

                entity.Property(e => e.IdKategorije)
                    .HasColumnName("id_kategorije")
                    .ValueGeneratedNever();

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Klijenti>(entity =>
            {
                entity.HasKey(x => x.IdKlijenta)
                    .HasName("PK__klijenti__8C829559DC3B94CF");

                entity.ToTable("klijenti");

                entity.Property(e => e.IdKlijenta)
                    .HasColumnName("id_klijenta")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdOsobe).HasColumnName("id_osobe");

                entity.Property(e => e.IdTvrtke).HasColumnName("id_tvrtke");

                entity.HasOne(d => d.IdOsobeNavigation)
                    .WithMany(p => p.Klijenti)
                    .HasForeignKey(x => x.IdOsobe)
                    .HasConstraintName("FK__klijenti__id_oso__3C69FB99");

                entity.HasOne(d => d.IdTvrtkeNavigation)
                    .WithMany(p => p.Klijenti)
                    .HasForeignKey(x => x.IdTvrtke)
                    .HasConstraintName("FK__klijenti__id_tvr__52593CB8");
            });

            modelBuilder.Entity<Mjenjaci>(entity =>
            {
                entity.HasKey(x => x.IdMjenjaca)
                    .HasName("PK__mjenjaci__BFC87F1CE4A2442E");

                entity.ToTable("mjenjaci");

                entity.Property(e => e.IdMjenjaca)
                    .HasColumnName("id_mjenjaca")
                    .ValueGeneratedNever();

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Modeli>(entity =>
            {
                entity.HasKey(x => x.IdModela)
                    .HasName("PK__modeli__B3BFCFE3214C1AC6");

                entity.ToTable("modeli");

                entity.Property(e => e.IdModela)
                    .HasColumnName("id_modela")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdSpecifikacija).HasColumnName("id_specifikacija");

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdSpecifikacijaNavigation)
                    .WithMany(p => p.Modeli)
                    .HasForeignKey(x => x.IdSpecifikacija)
                    .HasConstraintName("FK__modeli__id_speci__4222D4EF");
            });

            modelBuilder.Entity<Odjeli>(entity =>
            {
                entity.HasKey(x => x.IdOdjela)
                    .HasName("PK__odjeli__AC670D526E13565D");

                entity.ToTable("odjeli");

                entity.Property(e => e.IdOdjela)
                    .HasColumnName("id_odjela")
                    .ValueGeneratedNever();

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Osobe>(entity =>
            {
                entity.HasKey(x => x.IdOsobe)
                    .HasName("PK__osobe__D10DBF755469F76F");

                entity.ToTable("osobe");

                entity.Property(e => e.IdOsobe)
                    .HasColumnName("id_osobe")
                    .ValueGeneratedNever();

                entity.Property(e => e.DatumRodjenja)
                    .HasColumnName("datum_rodjenja")
                    .HasColumnType("date");

                entity.Property(e => e.Ime)
                    .HasColumnName("ime")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Prezime)
                    .HasColumnName("prezime")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ponuda>(entity =>
            {
                entity.HasKey(x => x.IdPonude)
                    .HasName("PK__ponuda__76299DD788D33406");

                entity.ToTable("ponuda");

                entity.Property(e => e.IdPonude)
                    .HasColumnName("id_ponude")
                    .ValueGeneratedNever();

                entity.Property(e => e.DodatniPopustPostotak).HasColumnName("dodatni_popust_postotak");

                entity.Property(e => e.IdZahtjeva).HasColumnName("id_zahtjeva");

                entity.Property(e => e.PopustKolicinaPostotak).HasColumnName("popust_kolicina_postotak");

                entity.HasOne(d => d.IdZahtjevaNavigation)
                    .WithMany(p => p.Ponuda)
                    .HasForeignKey(x => x.IdZahtjeva)
                    .HasConstraintName("FK__ponuda__id_zahtj__4E88ABD4");
            });

            modelBuilder.Entity<PonudaVozac>(entity =>
            {
                entity.HasKey(x => x.IdPv)
                    .HasName("PK__ponuda_v__0148A34A5E511BA4");

                entity.ToTable("ponuda_vozac");

                entity.Property(e => e.IdPv)
                    .HasColumnName("id_pv")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdPonude).HasColumnName("id_ponude");

                entity.Property(e => e.IdVozaca).HasColumnName("id_vozaca");

                entity.HasOne(d => d.IdPonudeNavigation)
                    .WithMany(p => p.PonudaVozac)
                    .HasForeignKey(x => x.IdPonude)
                    .HasConstraintName("FK__ponuda_vo__id_po__49C3F6B7");

                entity.HasOne(d => d.IdVozacaNavigation)
                    .WithMany(p => p.PonudaVozac)
                    .HasForeignKey(x => x.IdVozaca)
                    .HasConstraintName("FK__ponuda_vo__id_vo__4BAC3F29");
            });

            modelBuilder.Entity<PonudaVozilo>(entity =>
            {
                entity.HasKey(x => x.IdPvozilo)
                    .HasName("PK__ponuda_v__0B5F9A98A1E05912");

                entity.ToTable("ponuda_vozilo");

                entity.Property(e => e.IdPvozilo)
                    .HasColumnName("id_pvozilo")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdPonude).HasColumnName("id_ponude");

                entity.Property(e => e.IdVozila).HasColumnName("id_vozila");

                entity.HasOne(d => d.IdPonudeNavigation)
                    .WithMany(p => p.PonudaVozilo)
                    .HasForeignKey(x => x.IdPonude)
                    .HasConstraintName("FK__ponuda_vo__id_po__4AB81AF0");

                entity.HasOne(d => d.IdVozilaNavigation)
                    .WithMany(p => p.PonudaVozilo)
                    .HasForeignKey(x => x.IdVozila)
                    .HasConstraintName("FK__ponuda_vo__id_vo__4CA06362");
            });

            modelBuilder.Entity<Profili>(entity =>
            {
                entity.HasKey(x => x.IdProfila)
                    .HasName("PK__profili__0981A572ACE5569C");

                entity.ToTable("profili");

                entity.Property(e => e.IdProfila)
                    .HasColumnName("id_profila")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdZaposlenika).HasColumnName("id_zaposlenika");

                entity.HasOne(d => d.IdZaposlenikaNavigation)
                    .WithMany(p => p.Profili)
                    .HasForeignKey(x => x.IdZaposlenika)
                    .HasConstraintName("FK__profili__id_zapo__3F466844");
            });

            modelBuilder.Entity<Proizvodjaci>(entity =>
            {
                entity.HasKey(x => x.IdProizvodjaca)
                    .HasName("PK__proizvod__06B284B0219FEE0D");

                entity.ToTable("proizvodjaci");

                entity.Property(e => e.IdProizvodjaca)
                    .HasColumnName("id_proizvodjaca")
                    .ValueGeneratedNever();

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Slike>(entity =>
            {
                entity.HasKey(x => x.IdSlike)
                    .HasName("PK__slike__FF613FFEF2255A92");

                entity.ToTable("slike");

                entity.Property(e => e.IdSlike)
                    .HasColumnName("id_slike")
                    .ValueGeneratedNever();

                entity.Property(e => e.Lokacija)
                    .HasColumnName("lokacija")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SlikeVozila>(entity =>
            {
                entity.HasKey(x => x.IdSv)
                    .HasName("PK__slike_vo__014858E9B2CB527B");

                entity.ToTable("slike_vozila");

                entity.Property(e => e.IdSv)
                    .HasColumnName("id_sv")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdSlike).HasColumnName("id_slike");

                entity.Property(e => e.IdVozila).HasColumnName("id_vozila");

                entity.HasOne(d => d.IdSlikeNavigation)
                    .WithMany(p => p.SlikeVozila)
                    .HasForeignKey(x => x.IdSlike)
                    .HasConstraintName("FK__slike_voz__id_sl__5070F446");

                entity.HasOne(d => d.IdVozilaNavigation)
                    .WithMany(p => p.SlikeVozila)
                    .HasForeignKey(x => x.IdVozila)
                    .HasConstraintName("FK__slike_voz__id_vo__5165187F");
            });

            modelBuilder.Entity<Specifikacije>(entity =>
            {
                entity.HasKey(x => x.IdSpecifikacija)
                    .HasName("PK__specifik__0931200E56E054F7");

                entity.ToTable("specifikacije");

                entity.Property(e => e.IdSpecifikacija)
                    .HasColumnName("id_specifikacija")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdBoje).HasColumnName("id_boje");

                entity.Property(e => e.IdDodatneOpreme).HasColumnName("id_dodatne_opreme");

                entity.Property(e => e.IdMjenjaca).HasColumnName("id_mjenjaca");

                entity.Property(e => e.IdVrsteGoriva).HasColumnName("id_vrste_goriva");

                entity.Property(e => e.KonjskeSnage).HasColumnName("konjske_snage");

                entity.Property(e => e.Potrosnja).HasColumnName("potrosnja");

                entity.Property(e => e.VelicinaTankaULitrima).HasColumnName("velicina_tanka_u_litrima");

                entity.HasOne(d => d.IdBojeNavigation)
                    .WithMany(p => p.Specifikacije)
                    .HasForeignKey(x => x.IdBoje)
                    .HasConstraintName("FK__specifika__id_bo__440B1D61");

                entity.HasOne(d => d.IdDodatneOpremeNavigation)
                    .WithMany(p => p.Specifikacije)
                    .HasForeignKey(x => x.IdDodatneOpreme)
                    .HasConstraintName("FK__specifika__id_do__44FF419A");

                entity.HasOne(d => d.IdMjenjacaNavigation)
                    .WithMany(p => p.Specifikacije)
                    .HasForeignKey(x => x.IdMjenjaca)
                    .HasConstraintName("FK__specifika__id_mj__4316F928");

                entity.HasOne(d => d.IdVrsteGorivaNavigation)
                    .WithMany(p => p.Specifikacije)
                    .HasForeignKey(x => x.IdVrsteGoriva)
                    .HasConstraintName("FK__specifika__id_vr__48CFD27E");
            });

            modelBuilder.Entity<Tvrtke>(entity =>
            {
                entity.HasKey(x => x.IdTvrtke)
                    .HasName("PK__tvrtke__260D51ABEA507663");

                entity.ToTable("tvrtke");

                entity.Property(e => e.IdTvrtke)
                    .HasColumnName("id_tvrtke")
                    .ValueGeneratedNever();

                entity.Property(e => e.DogovoreniPopustPostotak).HasColumnName("dogovoreni_popust_postotak");

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.VrijemeSuradnjeGodine).HasColumnName("vrijeme_suradnje_godine");
            });

            modelBuilder.Entity<Usluge>(entity =>
            {
                entity.HasKey(x => x.IdUsluge)
                    .HasName("PK__usluge__9FD22D675869A3A2");

                entity.ToTable("usluge");

                entity.Property(e => e.IdUsluge)
                    .HasColumnName("id_usluge")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdKategorije).HasColumnName("id_kategorije");

                entity.Property(e => e.NazivUsluge)
                    .HasColumnName("naziv_usluge")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdKategorijeNavigation)
                    .WithMany(p => p.Usluge)
                    .HasForeignKey(x => x.IdKategorije)
                    .HasConstraintName("FK__usluge__id_kateg__403A8C7D");
            });

            modelBuilder.Entity<Vozila>(entity =>
            {
                entity.HasKey(x => x.IdVozila)
                    .HasName("PK__vozila__899BBC8CBC16EA2E");

                entity.ToTable("vozila");

                entity.Property(e => e.IdVozila)
                    .HasColumnName("id_vozila")
                    .ValueGeneratedNever();

                entity.Property(e => e.Cijena).HasColumnName("cijena");

                entity.Property(e => e.Dostupno).HasColumnName("dostupno");

                entity.Property(e => e.IdModela).HasColumnName("id_modela");

                entity.Property(e => e.IdProizvodjaca).HasColumnName("id_proizvodjaca");

                entity.HasOne(d => d.IdModelaNavigation)
                    .WithMany(p => p.Vozila)
                    .HasForeignKey(x => x.IdModela)
                    .HasConstraintName("FK__vozila__id_model__412EB0B6");

                entity.HasOne(d => d.IdProizvodjacaNavigation)
                    .WithMany(p => p.Vozila)
                    .HasForeignKey(x => x.IdProizvodjaca)
                    .HasConstraintName("FK__vozila__id_proiz__45F365D3");
            });

            modelBuilder.Entity<VrsteGoriva>(entity =>
            {
                entity.HasKey(x => x.IdVrsteGoriva)
                    .HasName("PK__vrste_go__A0C0C22749FAB4D8");

                entity.ToTable("vrste_goriva");

                entity.Property(e => e.IdVrsteGoriva)
                    .HasColumnName("id_vrste_goriva")
                    .ValueGeneratedNever();

                entity.Property(e => e.Naziv)
                    .HasColumnName("naziv")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Zahtjev>(entity =>
            {
                entity.HasKey(x => x.IdZahtjeva)
                    .HasName("PK__zahtjev__657E646F80D3C4F0");

                entity.ToTable("zahtjev");

                entity.Property(e => e.IdZahtjeva)
                    .HasColumnName("id_zahtjeva")
                    .ValueGeneratedNever();

                entity.Property(e => e.BrojVozila).HasColumnName("broj_vozila");

                entity.Property(e => e.DatumDo)
                    .HasColumnName("datum_do")
                    .HasColumnType("date");

                entity.Property(e => e.DatumOd)
                    .HasColumnName("datum_od")
                    .HasColumnType("date");

                entity.Property(e => e.IdKlijenta).HasColumnName("id_klijenta");

                entity.Property(e => e.IdUsluge).HasColumnName("id_usluge");

                entity.Property(e => e.RutaKilometri).HasColumnName("ruta_kilometri");

                entity.HasOne(d => d.IdKlijentaNavigation)
                    .WithMany(p => p.Zahtjev)
                    .HasForeignKey(x => x.IdKlijenta)
                    .HasConstraintName("FK__zahtjev__id_klij__4D94879B");

                entity.HasOne(d => d.IdUslugeNavigation)
                    .WithMany(p => p.Zahtjev)
                    .HasForeignKey(x => x.IdUsluge)
                    .HasConstraintName("FK__zahtjev__id_uslu__4F7CD00D");
            });

            modelBuilder.Entity<Zaposlenici>(entity =>
            {
                entity.HasKey(x => x.IdZaposlenika)
                    .HasName("PK__zaposlen__FBAF864B035D4EF8");

                entity.ToTable("zaposlenici");

                entity.Property(e => e.IdZaposlenika)
                    .HasColumnName("id_zaposlenika")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdOdjela).HasColumnName("id_odjela");

                entity.Property(e => e.IdOsobe).HasColumnName("id_osobe");

                entity.Property(e => e.RadniStaz).HasColumnName("radni_staz");

                entity.HasOne(d => d.IdOdjelaNavigation)
                    .WithMany(p => p.Zaposlenici)
                    .HasForeignKey(x => x.IdOdjela)
                    .HasConstraintName("FK__zaposleni__id_od__3E52440B");

                entity.HasOne(d => d.IdOsobeNavigation)
                    .WithMany(p => p.Zaposlenici)
                    .HasForeignKey(x => x.IdOsobe)
                    .HasConstraintName("FK__zaposleni__id_os__3D5E1FD2");
            });

            modelBuilder.Entity<ZaposleniciCertifikati>(entity =>
            {
                entity.HasKey(x => x.IdZc)
                    .HasName("PK__zaposlen__01481031A2DB8977");

                entity.ToTable("zaposlenici_certifikati");

                entity.Property(e => e.IdZc)
                    .HasColumnName("id_zc")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdCertifikata).HasColumnName("id_certifikata");

                entity.Property(e => e.IdZaposlenika).HasColumnName("id_zaposlenika");

                entity.HasOne(d => d.IdCertifikataNavigation)
                    .WithMany(p => p.ZaposleniciCertifikati)
                    .HasForeignKey(x => x.IdCertifikata)
                    .HasConstraintName("FK__zaposleni__id_ce__47DBAE45");

                entity.HasOne(d => d.IdZaposlenikaNavigation)
                    .WithMany(p => p.ZaposleniciCertifikati)
                    .HasForeignKey(x => x.IdZaposlenika)
                    .HasConstraintName("FK__zaposleni__id_za__46E78A0C");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
