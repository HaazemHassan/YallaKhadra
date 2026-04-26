namespace YallaKhadra.Core.Features.Users.Commands.Responses {
    public class ToggleUserLockResponse {
        public int UserId { get; set; }
        public bool IsLocked { get; set; }
    }
}
