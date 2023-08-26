namespace ItemService.EventProcessor
{
    public interface IProcessoEvento
    {
        void Processa(string mensagem);
    }
}