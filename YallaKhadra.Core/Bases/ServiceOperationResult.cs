using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Bases {
    public class ServiceOperationResult {
        public ServiceOperationStatus Status { get; private set; }
        public string? ErrorMessage { get; private set; }

        protected ServiceOperationResult(ServiceOperationStatus status, string? errorMessage = null) {
            Status = status;
            ErrorMessage = errorMessage;
        }

        public static ServiceOperationResult Success()
            => new ServiceOperationResult(ServiceOperationStatus.Succeeded, null);

        public static ServiceOperationResult Failure(ServiceOperationStatus status, string message)
            => new ServiceOperationResult(status, message);

    }
    public class ServiceOperationResult<T> : ServiceOperationResult {

        public T? Data { get; private set; }

        private ServiceOperationResult(ServiceOperationStatus status, string? errorMessage = null, T? data = default) : base(status, errorMessage) {
            Data = data;
        }

        public static ServiceOperationResult<T?> Success(T? data)
            => new ServiceOperationResult<T?>(ServiceOperationStatus.Succeeded, null, data);

        public static new ServiceOperationResult<T?> Failure(ServiceOperationStatus status, string message)
            => new ServiceOperationResult<T?>(status, message, default);

    }
}