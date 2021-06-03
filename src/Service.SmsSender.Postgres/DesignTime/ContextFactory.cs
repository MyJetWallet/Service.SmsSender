using MyJetWallet.Sdk.Postgres;

namespace Service.SmsSender.Postgres.DesignTime
{
    public class ContextFactory : MyDesignTimeContextFactory<SmsSenderDbContext>
    {
        public ContextFactory() : base(options => new SmsSenderDbContext(options))
        {
        }
    }
}