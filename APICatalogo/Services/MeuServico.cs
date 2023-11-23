namespace APICatalogo.Services
{
    public class MeuServico : IMeuServico
    {
        public string Saudacao(string nome)
        {
            return $"Ben Vindo, {nome} \n\n{DateTime.Now}";
        }
    }
}
