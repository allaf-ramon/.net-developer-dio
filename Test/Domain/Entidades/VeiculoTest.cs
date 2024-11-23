using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class VeiculoTest
    {
        [TestMethod]
        public void DeveCriarVeiculoComSucesso()
        {
            // Arrange
            var veiculo = new Veiculo
            {
                Nome = "Gol",
                Marca = "Volkswagen",
                Ano = 2020
            };

            // Assert
            Assert.IsNotNull(veiculo);
            Assert.AreEqual("Gol", veiculo.Nome);
            Assert.AreEqual("Volkswagen", veiculo.Marca);
            Assert.AreEqual(2020, veiculo.Ano);
        }

        [TestMethod]
        public void DeveDefinirEObterPropriedadesCorretamente()
        {
            // Arrange
            var veiculo = new Veiculo();

            // Act
            veiculo.Nome = "Civic";
            veiculo.Marca = "Honda";
            veiculo.Ano = 2021;

            // Assert
            Assert.AreEqual("Civic", veiculo.Nome);
            Assert.AreEqual("Honda", veiculo.Marca);
            Assert.AreEqual(2021, veiculo.Ano);
        }

        [TestMethod]
        public void DeveInicializarIdComValorPadrao()
        {
            // Arrange
            var veiculo = new Veiculo();

            // Assert
            Assert.AreEqual(0, veiculo.Id);
        }
    }
}
