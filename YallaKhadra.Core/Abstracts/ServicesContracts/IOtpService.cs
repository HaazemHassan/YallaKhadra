namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IOtpService {
        string Generate(int length = 6);
    }
}
