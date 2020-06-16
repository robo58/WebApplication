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

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Boje> Boje { get; set; }
        public virtual DbSet<DodatnaOprema> DodatnaOprema { get; set; }
        public virtual DbSet<Kategorije> Kategorije { get; set; }
        public virtual DbSet<Mjenjaci> Mjenjaci { get; set; }
        public virtual DbSet<Modeli> Modeli { get; set; }
        public virtual DbSet<Odjeli> Odjeli { get; set; }
        public virtual DbSet<Ponuda> Ponuda { get; set; }
        public virtual DbSet<PonudaVozac> PonudaVozac { get; set; }
        public virtual DbSet<PonudaVozilo> PonudaVozilo { get; set; }
        public virtual DbSet<Proizvodjaci> Proizvodjaci { get; set; }
        public virtual DbSet<Slike> Slike { get; set; }
        public virtual DbSet<Specifikacije> Specifikacije { get; set; }
        public virtual DbSet<Usluge> Usluge { get; set; }
        public virtual DbSet<Vozila> Vozila { get; set; }
        public virtual DbSet<VrsteGoriva> VrsteGoriva { get; set; }
        public virtual DbSet<Zahtjev> Zahtjev { get; set; }
        public virtual DbSet<Zaposlenici> Zaposlenici { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=rppp.fer.hr,3000;Database=PI-10;User Id=pi10;Password=M-A-N-G.O;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Boje>(entity =>
            {
                entity.HasKey(e => e.IdBoje)
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

            modelBuilder.Entity<DodatnaOprema>(entity =>
            {
                entity.HasKey(e => e.IdDodatneOpreme)
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
                entity.HasKey(e => e.IdKategorije)
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

            modelBuilder.Entity<Mjenjaci>(entity =>
            {
                entity.HasKey(e => e.IdMjenjaca)
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
                entity.HasKey(e => e.IdModela)
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

                entity.Property(e => e.Tip)
                    .HasColumnName("tip")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdSpecifikacijaNavigation)
                    .WithMany(p => p.Modeli)
                    .HasForeignKey(d => d.IdSpecifikacija)
                    .HasConstraintName("FK__modeli__id_speci__4222D4EF");
            });

            modelBuilder.Entity<Odjeli>(entity =>
            {
                entity.HasKey(e => e.IdOdjela)
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

            modelBuilder.Entity<Ponuda>(entity =>
            {
                entity.HasKey(e => e.IdPonude)
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
                    .HasForeignKey(d => d.IdZahtjeva)
                    .HasConstraintName("FK__ponuda__id_zahtj__4E88ABD4");
            });

            modelBuilder.Entity<PonudaVozac>(entity =>
            {
                entity.HasKey(e => e.IdPv)
                    .HasName("PK__ponuda_v__0148A34A5E511BA4");

                entity.ToTable("ponuda_vozac");

                entity.Property(e => e.IdPv)
                    .HasColumnName("id_pv")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdPonude).HasColumnName("id_ponude");

                entity.Property(e => e.IdVozaca).HasColumnName("id_vozaca");

                entity.HasOne(d => d.IdPonudeNavigation)
                    .WithMany(p => p.PonudaVozac)
                    .HasForeignKey(d => d.IdPonude)
                    .HasConstraintName("FK__ponuda_vo__id_po__49C3F6B7");

                entity.HasOne(d => d.IdVozacaNavigation)
                    .WithMany(p => p.PonudaVozac)
                    .HasForeignKey(d => d.IdVozaca)
                    .HasConstraintName("FK__ponuda_vo__id_vo__4BAC3F29");
            });

            modelBuilder.Entity<PonudaVozilo>(entity =>
            {
                entity.HasKey(e => e.IdPvozilo)
                    .HasName("PK__ponuda_v__0B5F9A98A1E05912");

                entity.ToTable("ponuda_vozilo");

                entity.Property(e => e.IdPvozilo)
                    .HasColumnName("id_pvozilo")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdPonude).HasColumnName("id_ponude");

                entity.Property(e => e.IdVozila).HasColumnName("id_vozila");

                entity.HasOne(d => d.IdPonudeNavigation)
                    .WithMany(p => p.PonudaVozilo)
                    .HasForeignKey(d => d.IdPonude)
                    .HasConstraintName("FK__ponuda_vo__id_po__4AB81AF0");

                entity.HasOne(d => d.IdVozilaNavigation)
                    .WithMany(p => p.PonudaVozilo)
                    .HasForeignKey(d => d.IdVozila)
                    .HasConstraintName("FK__ponuda_vo__id_vo__4CA06362");
            });

            modelBuilder.Entity<Proizvodjaci>(entity =>
            {
                entity.HasKey(e => e.IdProizvodjaca)
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
                entity.HasKey(e => e.IdSlike)
                    .HasName("PK__slike__FF613FFEF2255A92");

                entity.ToTable("slike");

                entity.Property(e => e.IdSlike)
                    .HasColumnName("id_slike")
                    .ValueGeneratedNever();

                entity.Property(e => e.SlikaBinary).HasColumnName("slika_binary");
            });

            modelBuilder.Entity<Specifikacije>(entity =>
            {
                entity.HasKey(e => e.IdSpecifikacija)
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
                    .HasForeignKey(d => d.IdBoje)
                    .HasConstraintName("FK__specifika__id_bo__440B1D61");

                entity.HasOne(d => d.IdDodatneOpremeNavigation)
                    .WithMany(p => p.Specifikacije)
                    .HasForeignKey(d => d.IdDodatneOpreme)
                    .HasConstraintName("FK__specifika__id_do__44FF419A");

                entity.HasOne(d => d.IdMjenjacaNavigation)
                    .WithMany(p => p.Specifikacije)
                    .HasForeignKey(d => d.IdMjenjaca)
                    .HasConstraintName("FK__specifika__id_mj__4316F928");

                entity.HasOne(d => d.IdVrsteGorivaNavigation)
                    .WithMany(p => p.Specifikacije)
                    .HasForeignKey(d => d.IdVrsteGoriva)
                    .HasConstraintName("FK__specifika__id_vr__48CFD27E");
            });

            modelBuilder.Entity<Usluge>(entity =>
            {
                entity.HasKey(e => e.IdUsluge)
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
                    .HasForeignKey(d => d.IdKategorije)
                    .HasConstraintName("FK__usluge__id_kateg__403A8C7D");
            });

            modelBuilder.Entity<Vozila>(entity =>
            {
                entity.HasKey(e => e.IdVozila)
                    .HasName("PK__vozila__899BBC8CBC16EA2E");

                entity.ToTable("vozila");

                entity.HasIndex(e => e.IdVozila)
                    .HasName("vozila_id_vozila_index");

                entity.Property(e => e.IdVozila)
                    .HasColumnName("id_vozila")
                    .ValueGeneratedNever();

                entity.Property(e => e.Cijena).HasColumnName("cijena");

                entity.Property(e => e.Dostupno).HasColumnName("dostupno");

                entity.Property(e => e.IdModela).HasColumnName("id_modela");

                entity.Property(e => e.IdProizvodjaca).HasColumnName("id_proizvodjaca");

                entity.Property(e => e.IdSlike).HasColumnName("id_slike");

                entity.HasOne(d => d.IdModelaNavigation)
                    .WithMany(p => p.Vozila)
                    .HasForeignKey(d => d.IdModela)
                    .HasConstraintName("FK__vozila__id_model__412EB0B6");

                entity.HasOne(d => d.IdProizvodjacaNavigation)
                    .WithMany(p => p.Vozila)
                    .HasForeignKey(d => d.IdProizvodjaca)
                    .HasConstraintName("FK__vozila__id_proiz__45F365D3");

                entity.HasOne(d => d.IdSlikeNavigation)
                    .WithMany(p => p.Vozila)
                    .HasForeignKey(d => d.IdSlike)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("vozila_slike_id_slike_fk");
            });

            modelBuilder.Entity<VrsteGoriva>(entity =>
            {
                entity.HasKey(e => e.IdVrsteGoriva)
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
                entity.HasKey(e => e.IdZahtjeva)
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
                    .HasForeignKey(d => d.IdKlijenta)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("zahtjev_AspNetUsers_Id_fk");

                entity.HasOne(d => d.IdUslugeNavigation)
                    .WithMany(p => p.Zahtjev)
                    .HasForeignKey(d => d.IdUsluge)
                    .HasConstraintName("FK__zahtjev__id_uslu__4F7CD00D");
            });

            modelBuilder.Entity<Zaposlenici>(entity =>
            {
                entity.HasKey(e => e.IdZaposlenika)
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
                    .HasForeignKey(d => d.IdOdjela)
                    .HasConstraintName("FK__zaposleni__id_od__3E52440B");

                entity.HasOne(d => d.IdOsobeNavigation)
                    .WithMany(p => p.Zaposlenici)
                    .HasForeignKey(d => d.IdOsobe)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("zaposlenici_AspNetUsers_Id_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
