namespace UsefulExtensions.SmsActivators.Types
{
    public enum SetStatusEnum
    {
        ReportSmsSent = 1,
        RequestOneMore = 3,
        EndActivation = 6,
        CancelActivation = 8
    }

    public enum SetStatusResult
    {
        AccessReady,
        AccessReadyGet,
        AccessActivation,
        AccessCancel
    }

    public enum StatusEnum
    {
        StatusWaitCode,
        StatusWaitRetry,
        StatusCancel,
        StatusOk
    }
}