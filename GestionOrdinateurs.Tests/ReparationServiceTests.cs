using FluentAssertions;
using GestionOrdinateurs.API;

namespace GestionOrdinateurs.Tests;

public class ReparationServiceTests
{
    [Fact]
    public async Task SimulerReparation_CreeUneReparationTerminee()
    {
        var context = TestHelper.CreerContexteAvecDonnees();
        var service = new ReparationService(context, new JournalService());
        var ordinateur = context.Ordinateurs.First(o => o.Id == 1);

        await service.SimulerReparationAsync(ordinateur);

        var reparations = context.Reparations.ToList();
        reparations.Should().HaveCount(1);
        reparations[0].OrdinateurId.Should().Be(1);
        reparations[0].Statut.Should().Be("Terminée");
    }
}