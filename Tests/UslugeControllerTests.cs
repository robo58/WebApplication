using WebApplication.Models;
using WebApplication.ViewModels;
using System;
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebApplication;
using Moq;

namespace Tests
{
    public class UslugeControllerTests
  {   
    IOptionsSnapshot<AppSettings> options;
    public UslugeControllerTests()
    {
      var mockOptions = new Mock<IOptionsSnapshot<AppSettings>>();
      var appSettings = new AppSettings
      {
        PageSize = 10 
      };
      mockOptions.SetupGet(options => options.Value).Returns(appSettings);
      options = mockOptions.Object;
    }
   
    [Trait("UnitTest", "UslugeController")]    
    [Fact]
    public void AkoNemaUslugaPreusmjeriNaCreate()
    {
      var mockLogger = new Mock<ILogger<UslugeController>>();
      
      var dbOptions = new DbContextOptionsBuilder<PI10Context>()
                .UseInMemoryDatabase(databaseName: "PI10Memory1")
                .Options;

      using (var context = new PI10Context(dbOptions))
      {
        var controller = new UslugeController(context, options, mockLogger.Object);
        var tempDataMock = new Mock<ITempDataDictionary>();                
        controller.TempData = tempDataMock.Object;

        // Act
        var result = controller.Index();

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Create", redirectToActionResult.ActionName);

        mockLogger.Verify(l => l.Log(LogLevel.Information,
                                    It.IsAny<EventId>(),
                                    It.IsAny<object>(),
                                    It.IsAny<Exception>(),
                                    (Func<object, Exception, string>)It.IsAny<object>())
                                 , Times.Once());
      }
    }


    [Trait("UnitTest", "UslugeController")]
    [Fact]
    public void VratiIspravanBrojUsluga()
    {
      // Arrange      
      var mockLogger = new Mock<ILogger<UslugeController>>();

      var dbOptions = new DbContextOptionsBuilder<PI10Context>()
                .UseInMemoryDatabase(databaseName: "PI10Memory2")
                .Options;

      using (var context = new PI10Context(dbOptions))
      {
        for (int i = 0; i < 50; i++)
        {
          context.Add(new Usluge
          {
            IdUsluge = i,
            NazivUsluge = "naz"
          });
        }
        context.SaveChanges();
        var controller = new UslugeController(context, options, mockLogger.Object);
        var tempDataMock = new Mock<ITempDataDictionary>();
        controller.TempData = tempDataMock.Object;

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        UslugeViewModel model = Assert.IsType<UslugeViewModel>(viewResult.Model);
        Assert.Equal(options.Value.PageSize, model.Usluge.Count());        
      }
    }
  }
}