using FluentAssertions;
using GestionOrdinateurs.API;

namespace GestionOrdinateurs.Tests;

public class OrdinateurServiceTests
{
    private OrdinateurService CreerService() =>
        new OrdinateurService(TestHelper.CreerContexteAvecDonnees(), new JournalService());

    [Fact]
    public async Task GetAll_RetourneTousLesOrdinateurs()
    {
        var service = CreerService();
        (await service.GetAllAsync()).Should().HaveCount(4);
    }

    [Fact]
    public async Task GetById_IdInexistant_RetourneNull()
    {
        var service = CreerService();
        (await service.GetByIdAsync(9999)).Should().BeNull();
    }

    [Fact]
    public async Task Add_NouvelOrdinateur_EstAjoute()
    {
        var service = CreerService();

        (await service.AddAsync(TestHelper.NouvelOrdinateur(100))).Should().BeTrue();
        (await service.GetAllAsync()).Should().HaveCount(5);
    }

    [Fact]
    public async Task Add_IdDejaUtilise_RetourneFalse()
    {
        var service = CreerService();
        (await service.AddAsync(TestHelper.NouvelOrdinateur(1))).Should().BeFalse();
    }

    [Fact]
    public async Task Update_OrdinateurExistant_ModifieLesChamps()
    {
        var service = CreerService();
        var maj = TestHelper.NouvelOrdinateur(1);
        maj.Nom = "PC-MODIFIE";
        maj.Prix = 999;

        (await service.UpdateAsync(1, maj)).Should().BeTrue();

        var resultat = await service.GetByIdAsync(1);
        resultat!.Nom.Should().Be("PC-MODIFIE");
        resultat.Prix.Should().Be(999);
    }

    [Fact]
    public async Task Update_IdInexistant_RetourneFalse()
    {
        var service = CreerService();
        (await service.UpdateAsync(9999, TestHelper.NouvelOrdinateur(9999))).Should().BeFalse();
    }

    [Fact]
    public async Task Delete_OrdinateurExistant_EstSupprime()
    {
        var service = CreerService();

        (await service.DeleteAsync(1)).Should().BeTrue();
        (await service.GetByIdAsync(1)).Should().BeNull();
        (await service.DeleteAsync(1)).Should().BeFalse();
    }

    [Fact]
    public async Task GetDisponibles_RetourneSeulementLesOrdinateursEnStock()
    {
        var service = CreerService();
        var resultat = await service.GetDisponiblesAsync();

        resultat.Should().HaveCount(3);
        resultat.Should().OnlyContain(o => o.Stock > 0);
    }

    [Fact]
    public async Task GetByMarque_RetourneLesOrdinateursDeLaMarque()
    {
        var service = CreerService();
        var resultat = await service.GetByMarqueAsync("HP");

        resultat.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetParPrixDecroissant_RetourneDuPlusCherAuMoinsCher()
    {
        var service = CreerService();
        var resultat = await service.GetParPrixDecroissantAsync();

        resultat.Select(o => o.Prix).Should().BeInDescendingOrder();
        resultat.First().Prix.Should().Be(3000);
    }
}