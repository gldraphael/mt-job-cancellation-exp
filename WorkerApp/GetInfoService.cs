using Bogus;
using CommonLib.MessageContracts;

namespace WorkerApp
{
    public class GetInfoService
    {
        private readonly Faker<GetInfoResponse> faker;

        public GetInfoService()
        {
            faker = new Faker<GetInfoResponse>()
                .RuleFor(i => i.Name, f => f.Person.FullName)
                .RuleFor(i => i.CountryCode, f => f.Address.CountryCode())
                .RuleFor(i => i.Email, f => f.Person.Email)
                .RuleFor(i => i.Phone, f => f.Person.Phone);
        }

        public GetInfoResponse GetInfo(int id) =>
            faker.Generate() with { Id = id };
    }
}
