public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context,
        IUserRepository users,
        IInitiativeRepository initiatives,
        IFundraisingRepository fundraisings,
        IDonateRepository donations)
    {
        _context = context;
        Users = users;
        Initiatives = initiatives;
        Fundraisings = fundraisings;
        Donations = donations;
    }

    public IUserRepository Users { get; }
    public IInitiativeRepository Initiatives { get; }
    public IFundraisingRepository Fundraisings { get; }
    public IDonateRepository Donations { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
