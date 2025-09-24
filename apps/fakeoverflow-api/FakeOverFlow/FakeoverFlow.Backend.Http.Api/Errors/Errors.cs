using FakeoverFlow.Backend.Abstraction.Errors;

namespace FakeoverFlow.Backend.Http.Api.Errors;

public partial class Errors
{
    public static readonly Error UnknownError = new Error("UNKNOWN_ERROR",ErrorType.Unknown ,"An unknown error has occurred."); 
}