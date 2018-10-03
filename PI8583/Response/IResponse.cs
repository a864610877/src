using Oxite;

namespace PI8583
{
    public interface IResponse
    {
        byte[] GetData();
        byte[] GetError();
        int Result { set; get; }
    }
}