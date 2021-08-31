using Microsoft.EntityFrameworkCore.Migrations;

namespace APICatalogo.Migrations
{
    public partial class Populadb : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Categorias(Nome, ImageURL) Values('Bebidas', 'http://www.macoratti.net/imagens/1.jpg')");
            mb.Sql("INSERT INTO Categorias(Nome, ImageURL) Values('Lanches', 'http://www.macoratti.net/imagens/2.jpg')");
            mb.Sql("INSERT INTO Categorias(Nome, ImageURL) Values('Sobremesas', 'http://www.macoratti.net/imagens/3.jpg')");

            mb.Sql(@"INSERT INTO Produtos(Nome, Descricao, Preco, ImageURL, Estoque, DataCadastro, CategoriaId) Values('Coca-Cola Diet', 'Refrigerande de Cola',
            5.45, 'http://www.macoratti.net/imagens/coca.jpg', 50, now(), (SELECT  CategoriaId FROM Categorias WHERE Nome='Bebidas'))");

            mb.Sql(@"INSERT INTO Produtos(Nome, Descricao, Preco, ImageURL, Estoque, DataCadastro, CategoriaId) Values('Lanche de Atum', 'Lanche de atum com maionese',
            8.50, 'http://www.macoratti.net/imagens/atum.jpg', 10, now(), (SELECT  CategoriaId FROM Categorias WHERE Nome='Lanches'))");

            mb.Sql(@"INSERT INTO Produtos(Nome, Descricao, Preco, ImageURL, Estoque, DataCadastro, CategoriaId) Values('Pudim 100 g', 'Pudim de leite condensado 100g',
            6.75, 'http://www.macoratti.net/imagens/pudim.jpg', 20, now(), (SELECT  CategoriaId FROM Categorias WHERE Nome='Sobremesas'))");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM CATEGORIAS");
            mb.Sql("DELETE FROM PRODUTOS");
        }
    }
}
