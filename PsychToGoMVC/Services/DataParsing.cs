using Newtonsoft.Json;
using PsychToGoMVC.Services.Interfaces;

namespace PsychToGoMVC.Services;

public class DataParsing<T> : IDataParsing<T>
{
    public T DeserializeType(HttpResponseMessage? response)
    {
        try
        {
            string data = response.Content.ReadAsStringAsync().Result;
            T modelToReturn = JsonConvert.DeserializeObject<T>( data );

            return modelToReturn;
        }
        catch (Exception)
        {
            throw new InvalidOperationException( "Deserialization failed" );
        }
    }
}