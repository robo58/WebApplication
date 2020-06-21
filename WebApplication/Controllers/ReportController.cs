using WebApplication.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using WebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
  [Authorize]
  public class ReportController : Controller
  {
    private readonly PI10Context ctx;

    public ReportController(PI10Context ctx)
    {
      this.ctx = ctx;
    }

    public IActionResult Index()
    {
      return View();
    }

    public async Task<IActionResult> Zahtjevi()
    {
      string naslov = "Popis zahtjeva";
      var zahtjevi = await ctx.Zahtjev
                            .AsNoTracking()
                            .OrderBy(d => d.IdZahtjeva)
                            .Select(d=> new ZahtjevViewModel
                            {
                                IdZahtjeva = d.IdZahtjeva,
                                ImePrezime = d.IdKlijentaNavigation.FirstName + " " + d.IdKlijentaNavigation.LastName,
                                NazivUsluge = d.IdUslugeNavigation.NazivUsluge,
                                DatumOd = d.DatumOd,
                                DatumDo = d.DatumDo,
                                BrojVozila = d.BrojVozila
                            })
                            .ToListAsync();
      PdfReport report = CreateReport(naslov);
      #region Podnožje i zaglavlje
      report.PagesFooter(footer =>
      {
        footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
      })
      .PagesHeader(header =>
      {
        header.CacheHeader(cache: true); // It's a default setting to improve the performance.
        header.DefaultHeader(defaultHeader =>
        {
          defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
          defaultHeader.Message(naslov);
        });
      });
      #endregion
      #region Postavljanje izvora podataka i stupaca
      report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(zahtjevi));

      report.MainTableColumns(columns =>
      {
        columns.AddColumn(column =>
        {
          column.IsRowNumber(true);
          column.CellsHorizontalAlignment(HorizontalAlignment.Right);
          column.IsVisible(true);
          column.Order(0);
          column.Width(1);
          column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName(nameof(Zahtjev.IdZahtjeva));
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(1);
          column.Width(2);
          column.HeaderCell("Id zahtjeva");
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<ZahtjevViewModel>(x => x.ImePrezime);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(2);
          column.Width(3);
          column.HeaderCell("Klijent", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<ZahtjevViewModel>(x => x.NazivUsluge);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(3);
          column.Width(1);
          column.HeaderCell("Naziv usluge", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<ZahtjevViewModel>(x => x.DatumOd);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(4);
          column.Width(1);
          column.HeaderCell("Datum od", horizontalAlignment: HorizontalAlignment.Center);
        });
        columns.AddColumn(column =>
        {
          column.PropertyName<ZahtjevViewModel>(x => x.DatumDo);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(4);
          column.Width(1);
          column.HeaderCell("Datum do", horizontalAlignment: HorizontalAlignment.Center);
        });
      });

      #endregion      
      byte[] pdf = report.GenerateAsByteArray();

      if (pdf != null)
      {
        Response.Headers.Add("content-disposition", "inline; filename=zahtjevi.pdf");
        return File(pdf, "application/pdf");
        //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
      }
      else
        return NotFound();
    }


//     #region Master-detail header
//     public class MasterDetailsHeaders : IPageHeader
//     {
//       private string naslov;
//       public MasterDetailsHeaders(string naslov)
//       {
//         this.naslov = naslov;
//       }
//       public IPdfFont PdfRptFont { set; get; }

//       public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
//       {
//         var idDokumenta = newGroupInfo.GetSafeStringValueOf(nameof(StavkaDenorm.IdDokumenta));
//         var urlDokumenta = newGroupInfo.GetSafeStringValueOf(nameof(StavkaDenorm.UrlDokumenta));        
//         var nazPartnera = newGroupInfo.GetSafeStringValueOf(nameof(StavkaDenorm.NazPartnera));
//         var datDokumenta = (DateTime) newGroupInfo.GetValueOf(nameof(StavkaDenorm.DatDokumenta));
//         var iznosDokumenta = (decimal) newGroupInfo.GetValueOf(nameof(StavkaDenorm.IznosDokumenta));        

//         var table = new PdfGrid(relativeWidths: new[] { 2f, 5f, 2f, 3f }) { WidthPercentage = 100 };

//         table.AddSimpleRow(
//             (cellData, cellProperties) =>
//             {
//               cellData.Value = "Id dokumenta:";
//               cellProperties.PdfFont = PdfRptFont;
//               cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
//               cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
//             },
//             (cellData, cellProperties) =>
//             {
//               cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
//               var cellTemplate = new HyperlinkField(BaseColor.Black, false)
//               {
//                 TextPropertyName = nameof(StavkaDenorm.IdDokumenta),
//                 NavigationUrlPropertyName = nameof(StavkaDenorm.UrlDokumenta),
//                 BasicProperties = new CellBasicProperties
//                 {
//                   HorizontalAlignment = HorizontalAlignment.Left,
//                   PdfFontStyle = DocumentFontStyle.Bold,
//                   PdfFont = PdfRptFont
//                 }
//               };
                         
//               cellData.CellTemplate = cellTemplate;              
//               cellProperties.PdfFont = PdfRptFont;
//               cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
//             },            
//             (cellData, cellProperties) =>
//             {
//               cellData.Value = "Datum dokumenta:";
//               cellProperties.PdfFont = PdfRptFont;
//               cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
//               cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
//             },
//             (cellData, cellProperties) =>
//             {
//               cellData.Value = datDokumenta;
//               cellProperties.PdfFont = PdfRptFont;
//               cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
//               cellProperties.DisplayFormatFormula = obj => ((DateTime)obj).ToString("dd.MM.yyyy");
//             });

//         table.AddSimpleRow(
//             (cellData, cellProperties) =>
//             {
//               cellData.Value = "Partner:";
//               cellProperties.PdfFont = PdfRptFont;
//               cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
//               cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
//             },
//             (cellData, cellProperties) =>
//             {
//               cellData.Value = nazPartnera;
//               cellProperties.PdfFont = PdfRptFont;
//               cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
//             },
//             (cellData, cellProperties) =>
//             {
//               cellData.Value = "Iznos dokumenta:";
//               cellProperties.PdfFont = PdfRptFont;
//               cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
//               cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
//             },
//             (cellData, cellProperties) =>
//             {
//               cellData.Value = iznosDokumenta;
//               cellProperties.DisplayFormatFormula = obj => ((decimal)obj).ToString("C2");                                                    
//               cellProperties.PdfFont = PdfRptFont;
//               cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
//             });        
//         return table.AddBorderToTable(borderColor: BaseColor.LightGray, spacingBefore: 5f);
//       }

//       public PdfGrid RenderingReportHeader(Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
//       {
//         var table = new PdfGrid(numColumns: 1) { WidthPercentage = 100 };
//         table.AddSimpleRow(
//            (cellData, cellProperties) =>
//            {
//              cellData.Value = naslov;
//              cellProperties.PdfFont = PdfRptFont;
//              cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
//              cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
//            });
//         return table.AddBorderToTable();
//       }
//     }
//     #endregion

    #region Private methods
    private PdfReport CreateReport(string naslov)
    {
      var pdf = new PdfReport();

      pdf.DocumentPreferences(doc =>
      {
        doc.Orientation(PageOrientation.Portrait);
        doc.PageSize(PdfPageSize.A4);
        doc.DocumentMetadata(new DocumentMetadata
        {
          Author = "PI-10",
          Application = "CarServiceApp",
          Title = naslov
        });
        doc.Compression(new CompressionSettings
        {
          EnableCompression = true,
          EnableFullCompression = true
        });
      })
      .MainTableTemplate(template =>
      {
        template.BasicTemplate(BasicTemplate.ProfessionalTemplate);
      })                
        .DefaultFonts(fonts =>
        {
            fonts.Path(Path.Combine("wwwroot", "fonts", "verdana.ttf"),
            Path.Combine("wwwroot", "fonts", "tahoma.ttf"));
            fonts.Size(9);
            fonts.Color(System.Drawing.Color.Black);
        })
      .MainTablePreferences(table =>
      {
        table.ColumnsWidthsType(TableColumnWidthType.Relative);
        //table.NumberOfDataRowsPerPage(20);
        table.GroupsPreferences(new GroupsPreferences
        {
          GroupType = GroupType.HideGroupingColumns,
          RepeatHeaderRowPerGroup = true,
          ShowOneGroupPerPage = true,
          SpacingBeforeAllGroupsSummary = 5f,
          NewGroupAvailableSpacingThreshold = 150,
          SpacingAfterAllGroupsSummary = 5f
        });
        table.SpacingAfter(4f);
      });

      return pdf;
    }
    #endregion
  }
}