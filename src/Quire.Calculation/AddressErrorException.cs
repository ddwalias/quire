using System;
using System.Runtime.Serialization;

namespace Quire.Calculation;

public partial class AddressErrorException : Exception
{
    public AddressErrorException()
    {
    }

    public AddressErrorException(string message)
        : base(message)
    {
    }

    public AddressErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected AddressErrorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
