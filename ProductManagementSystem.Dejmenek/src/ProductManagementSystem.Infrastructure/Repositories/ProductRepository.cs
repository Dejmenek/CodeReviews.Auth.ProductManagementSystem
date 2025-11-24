using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Infrastructure.Database;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Infrastructure.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Paged<Product>> GetProducts(
        int page,
        string? search,
        PageSize productsPerPage,
        SortingProductColumn sortingProductColumn = SortingProductColumn.Name,
        SortingDirection sortingOrder = SortingDirection.Ascending)
    {
        IQueryable<Product> productsQuery = _context.Products;

        if (!string.IsNullOrEmpty(search))
            productsQuery = productsQuery.Where(u => u.Name!.Contains(search));

        productsQuery = sortingProductColumn switch
        {
            SortingProductColumn.Name => sortingOrder == SortingDirection.Ascending
                ? productsQuery.OrderBy(p => p.Name)
                : productsQuery.OrderByDescending(p => p.Name),

            SortingProductColumn.Price => sortingOrder == SortingDirection.Ascending
                ? productsQuery.OrderBy(p => p.Price)
                : productsQuery.OrderByDescending(p => p.Price),

            SortingProductColumn.DateAdded => sortingOrder == SortingDirection.Ascending
                ? productsQuery.OrderBy(p => p.DateAdded)
                : productsQuery.OrderByDescending(p => p.DateAdded),

            _ => sortingOrder == SortingDirection.Ascending
                ? productsQuery.OrderBy(p => p.Id)
                : productsQuery.OrderByDescending(p => p.Id)
        };

        var totalProducts = await productsQuery.CountAsync();

        var products = await productsQuery
            .Skip((page - 1) * (int)productsPerPage)
            .Take((int)productsPerPage)
            .ToListAsync();

        return new Paged<Product>
        {
            Items = products,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalProducts / (double)(int)productsPerPage),
            PageSize = productsPerPage,
            TotalCount = totalProducts
        };
    }

    public async Task<int> GetProductsCount()
    {
        return await _context.Products.CountAsync();
    }

    public async Task RemoveProducts(List<int> productIds)
    {
        await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ExecuteDeleteAsync();
    }

    public async Task RemoveSingleProduct(int productId)
    {
        await _context.Products
            .Where(p => p.Id == productId)
            .ExecuteDeleteAsync();
    }

    public async Task AddLaptop(Laptop laptop)
    {
        await _context.Laptops.AddAsync(laptop);
        await _context.SaveChangesAsync();
    }

    public async Task AddDesktop(Desktop desktop)
    {
        await _context.Desktops.AddAsync(desktop);
        await _context.SaveChangesAsync();
    }

    public async Task<Product?> GetProductById(int productId)
    {
        return await _context.Products.FindAsync(productId);
    }

    public async Task UpdateProduct(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}
