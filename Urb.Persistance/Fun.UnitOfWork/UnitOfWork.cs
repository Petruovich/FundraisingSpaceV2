using Fun.Application.Fun.IRepositories;
using Urb.Application.Urb.IRepositories;
using Urb.Application.Urb.IUnitOfWork;

public class UnitOfWork : IUnitOfWOrk
{
    private readonly MainDataContext _context;

    public UnitOfWork(MainDataContext context,
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
    public void BeginTransaction()
    {
        _context.Database.BeginTransaction();
    }
    public int Commit()
    {
        _context.Database.CommitTransaction();
        return 1;
    }
    public void Rollback()
    {
        _context.Database.RollbackTransaction();
    }
}
