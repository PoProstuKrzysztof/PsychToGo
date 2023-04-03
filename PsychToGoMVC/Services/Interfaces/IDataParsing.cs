namespace PsychToGoMVC.Services.Interfaces;

public interface IDataParsing<T>
{
    public T DeserializeType(HttpResponseMessage response);
}