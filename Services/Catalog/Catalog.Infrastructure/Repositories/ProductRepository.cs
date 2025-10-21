
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypesRepository
    {
        private readonly ICatalogContext _context;
        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        #region IProductRepository Members
        public async Task<Product> GetProduct(string id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;
            if(!string.IsNullOrEmpty(catalogSpecParams.Search))
            {
                // Decode URL-encoded characters
                string decodedSearch = System.Net.WebUtility.UrlDecode(catalogSpecParams.Search);
                filter &= builder.Regex(p => p.Name,
                    new MongoDB.Bson.BsonRegularExpression(Regex.Escape(decodedSearch), "i"));
            }

            if(!string.IsNullOrEmpty(catalogSpecParams.BrandId))
            {
                filter &= builder.Eq(p => p.Brands.Id, catalogSpecParams.BrandId);
            }

            if(!string.IsNullOrEmpty(catalogSpecParams.TypeId))
            {
                filter &= builder.Eq(p => p.Types.Id, catalogSpecParams.TypeId);
            }
            var totalItems = await _context.Products.CountDocumentsAsync(filter);
            var data = await DataFilter(catalogSpecParams, filter);
            //var data = await _context.Products.Find(filter).Skip((catalogSpecParams.PageIndex -1) * catalogSpecParams.PageSize)
            //        .Limit(catalogSpecParams.PageSize).ToListAsync();

            return new Pagination<Product>(
                catalogSpecParams.PageIndex,
                catalogSpecParams.PageSize,
                (int)totalItems,
                data
            );
        }

        public async Task<IEnumerable<Product>> GetProductsByBrand(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return Enumerable.Empty<Product>();

            // Decode URL-encoded characters
            string decodedBrandName = System.Net.WebUtility.UrlDecode(brandName);

            // Use regex for case-insensitive search
            var filter = Builders<Product>.Filter.Regex(p => p.Brands.Name,
                new MongoDB.Bson.BsonRegularExpression(Regex.Escape(decodedBrandName), "i"));

            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Enumerable.Empty<Product>();
            // Decode the URL-encoded string
            string decodedName = System.Net.WebUtility.UrlDecode(name);
            var filter = Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(Regex.Escape(decodedName), "i"));
            return await _context.Products.Find(filter).ToListAsync();
            //return await _context.Products.Find(p => p.Name.ToLower() == name.ToLower()).ToListAsync();
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var deletedProduct = await _context.Products.DeleteOneAsync(p => p.Id == id);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }


        public async Task<bool> UpdateProduct(Product product)
        {
            var updatedProduct = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }

        #endregion

        #region IBrandRepository Members
        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
        {
            return await _context.Brands.Find(p => true).ToListAsync();
        }
        #endregion

        #region ITypesRepository Members
        public async Task<IEnumerable<ProductType>> GetAllTypes()
        {
            return await _context.Types.Find(p => true).ToListAsync();
        }

        #endregion

        #region private Methods
        private async Task<IReadOnlyList<Product>> DataFilter(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortDefn = Builders<Product>.Sort.Ascending("Name"); //default sort
            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                switch (catalogSpecParams.Sort)
                {
                    case "priceAsc":
                        sortDefn = Builders<Product>.Sort.Ascending(p => p.Price);
                        break;
                    case "priceDesc":
                        sortDefn = Builders<Product>.Sort.Descending(p => p.Price);
                        break;
                    default:
                        sortDefn = Builders<Product>.Sort.Ascending(p => p.Name);
                        break;
                }
            }
            return await _context.Products.Find(filter)
                    .Sort(sortDefn)
                    .Skip((catalogSpecParams.PageIndex - 1) * catalogSpecParams.PageSize)
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync();
        }

        #endregion
    }
}
