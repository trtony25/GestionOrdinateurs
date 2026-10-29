using FluentAssertions;
using GestionOrdinateurs.API;
using GestionOrdinateurs.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace GestionOrdinateurs.Tests;

public class OrdinateursControllerTests
{
    private OrdinateursController CreerController()
    {
        var service = new OrdinateurService(TestHelper.CreerContexteAvecDonnees(), new JournalService());
        return new OrdinateursController(service, NullLogger<OrdinateursController>.Instance);
    }

    [Fact]
    public async Task GetComputerById_Inexistant_Retourne404()
    {
        var controller = CreerController();
        (await controller.GetComputerById(9999)).Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetComputerById_Existant_Retourne200()
    {
        var controller = CreerController();
        (await controller.GetComputerById(1)).Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Create_NouvelOrdinateur_Retourne201()
    {
        var controller = CreerController();
        (await controller.Create(TestHelper.NouvelOrdinateur(100))).Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Create_IdDejaUtilise_Retourne409()
    {
        var controller = CreerController();
        (await controller.Create(TestHelper.NouvelOrdinateur(1))).Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Create_IdNul_Retourne400()
    {
        var controller = CreerController();
        (await controller.Create(TestHelper.NouvelOrdinateur(0))).Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_IdUrlDifferentDeLObjet_Retourne400()
    {
        var controller = CreerController();
        (await controller.Update(1, TestHelper.NouvelOrdinateur(2))).Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_OrdinateurInexistant_Retourne404()
    {
        var controller = CreerController();
        (await controller.Update(9999, TestHelper.NouvelOrdinateur(9999))).Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_OrdinateurExistant_Retourne200()
    {
        var controller = CreerController();
        (await controller.Update(1, TestHelper.NouvelOrdinateur(1))).Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_OrdinateurExistant_Retourne204PuisSuivant404()
    {
        var controller = CreerController();

        (await controller.Delete(1)).Should().BeOfType<NoContentResult>();
        (await controller.Delete(1)).Should().BeOfType<NotFoundResult>();
    }
}