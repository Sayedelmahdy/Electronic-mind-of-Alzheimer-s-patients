using DAL.Context;
using DAL.EntityTest;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;


public class BaseRepositoryTests
{


    public BaseRepositoryTests()
    {

    }
    private DbContextOptions<DBContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }
    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "1" };

            await repository.AddAsync(entity);

            var result = await context.Set<TestEntity>().FindAsync(entity.Id);
            Assert.NotNull(result);
        }
    }

    [Fact]
    public void Add_ShouldAddEntity()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "2" };

            repository.Add(entity);

            var result = context.Set<TestEntity>().Find(entity.Id);
            Assert.NotNull(result);
        }
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenEntityExists()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "3" };

            await repository.AddAsync(entity);
            var result = await repository.AnyAsync(e => e.Id == entity.Id);

            Assert.True(result);
        }
    }

    [Fact]
    public void Any_ShouldReturnTrue_WhenEntityExists()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "4" };

            repository.Add(entity);
            var result = repository.Any(e => e.Id == entity.Id);

            Assert.True(result);
        }
    }

    [Fact]
    public void Attach_ShouldAttachEntity()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "5" };

            repository.Attach(entity);

            var attachedEntity = context.Entry(entity);
            Assert.Equal(EntityState.Unchanged, attachedEntity.State);
        }
    }

    [Fact]
    public void Count_ShouldReturnCorrectCount()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "6" };

            repository.Add(entity);
            var count = repository.Count();

            Assert.Equal(1, count);
        }
    }

    [Fact]
    public void Count_WithExpression_ShouldReturnCorrectCount()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "7" };

            repository.Add(entity);
            var count = repository.Count(e => e.Id == entity.Id);

            Assert.Equal(1, count);
        }
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "8" };

            await repository.AddAsync(entity);
            var count = await repository.CountAsync();

            Assert.Equal(1, count);
        }
    }

    [Fact]
    public async Task CountAsync_WithExpression_ShouldReturnCorrectCount()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "9" };

            await repository.AddAsync(entity);
            var count = await repository.CountAsync(e => e.Id == entity.Id);

            Assert.Equal(1, count);
        }
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "10" };

            await repository.AddAsync(entity);
            await repository.DeleteAsync(entity);
            var result = await context.Set<TestEntity>().FindAsync(entity.Id);

            Assert.Null(result);
        }
    }

    [Fact]
    public void Delete_ShouldRemoveEntity()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "11" };

            repository.Add(entity);
            repository.Delete(entity);
            var result = context.Set<TestEntity>().Find(entity.Id);

            Assert.Null(result);
        }
    }

    [Fact]
    public void Entry_ShouldSetEntityState()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "12" };

            repository.Entry(entity);

            var entry = context.Entry(entity);
            Assert.Equal(EntityState.Unchanged, entry.State);
        }
    }

    [Fact]
    public async Task FindAsync_ShouldReturnEntity_WhenEntityExists()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "13" };

            await repository.AddAsync(entity);
            var result = await repository.FindAsync(e => e.Id == entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }
    }

    [Fact]
    public void Find_ShouldReturnEntity_WhenEntityExists()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "14" };

            repository.Add(entity);
            var result = repository.Find(e => e.Id == entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity1 = new TestEntity { Id = "15" };
            var entity2 = new TestEntity { Id = "16" };

            await repository.AddAsync(entity1);
            await repository.AddAsync(entity2);
            var result = await repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public void GetAll_ShouldReturnAllEntities()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity1 = new TestEntity { Id = "17" };
            var entity2 = new TestEntity { Id = "18" };

            repository.Add(entity1);
            repository.Add(entity2);
            var result = repository.GetAll();

            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "19" };

            await repository.AddAsync(entity);
            var result = await repository.GetByIdAsync(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }
    }

    [Fact]
    public void GetById_ShouldReturnEntity_WhenEntityExists()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "20" };

            repository.Add(entity);
            var result = repository.GetById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "21", Name = "Original" };

            await repository.AddAsync(entity);
            entity.Name = "Updated";
            await repository.UpdateAsync(entity);

            var updatedEntity = await context.Set<TestEntity>().FindAsync(entity.Id);
            Assert.Equal("Updated", updatedEntity.Name);
        }
    }

    [Fact]
    public void Include_ShouldIncludeProperties()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity = new TestEntity { Id = "22" };

            repository.Add(entity);
            var query = repository.Include(e => e.RelatedEntity);
            var result = query.FirstOrDefault();

            Assert.NotNull(result);
        }
    }

    [Fact]
    public async Task WhereAsync_ShouldReturnFilteredEntities()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity1 = new TestEntity { Id = "23", Name = "Test1" };
            var entity2 = new TestEntity { Id = "24", Name = "Test2" };

            await repository.AddAsync(entity1);
            await repository.AddAsync(entity2);
            var result = await repository.WhereAsync(e => e.Name.Contains("Test"));

            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public void Where_ShouldReturnFilteredEntities()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity1 = new TestEntity { Id = "25", Name = "Test1" };
            var entity2 = new TestEntity { Id = "26", Name = "Test2" };

            repository.Add(entity1);
            repository.Add(entity2);
            var result = repository.Where(e => e.Name.Contains("Test"));

            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public void Select_ShouldReturnFilteredQueryable()
    {
        var dbContextOptions = CreateNewContextOptions();
        using (var context = new DBContext(dbContextOptions))
        {
            var repository = new BaseRepository<TestEntity>(context);
            var entity1 = new TestEntity { Id = "29", Name = "Test1" };
            var entity2 = new TestEntity { Id = "30", Name = "Test2" };

            repository.Add(entity1);
            repository.Add(entity2);
            var result = repository.Select(e => e.Name.Contains("Test"));

            Assert.Equal(2, result.Count());
        }
    }


}

