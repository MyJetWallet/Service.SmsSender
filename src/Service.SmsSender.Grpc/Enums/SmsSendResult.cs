using System.Diagnostics.CodeAnalysis;

namespace Service.SmsSender.Grpc.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SmsSendResult
    {
        OK,
        FAILED,
        TEMPLATE_NOT_FOUND
    }
}
