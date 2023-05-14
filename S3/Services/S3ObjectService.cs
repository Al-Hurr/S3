using S3.ApplicationDbContext;
using S3.Models;

namespace S3.Services
{
    public class S3ObjectService
    {
        private readonly AppDbContext _dbContext;

        public S3ObjectService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void  Create(S3Object entity)
        {
            _dbContext.Set<S3Object>().Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(S3Object entity)
        {
            _dbContext.Update(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<S3Object> GetAll()
        {
            return _dbContext.Set<S3Object>().AsQueryable();
        }

        public S3Object GetByName(string name)
        {
            return _dbContext.Set<S3Object>().FirstOrDefault(x => x.Name == name);
        }
    }
}