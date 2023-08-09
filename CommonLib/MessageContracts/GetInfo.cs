namespace CommonLib.MessageContracts
{
    public sealed record GetInfo
    {
        public int Id { get; init; }
    }

    public sealed record GetInfoResponse
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public string? CountryCode { get; init; }
    }
}
